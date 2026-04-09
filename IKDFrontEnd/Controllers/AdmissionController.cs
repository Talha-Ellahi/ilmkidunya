using IKDFrontEnd.DBCollege;
using IKDFrontEnd.Models;
using IKDFrontEnd.Services;
using IKDFrontEnd.ViewModels.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Security.Policy;
using System.Text.Json;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;

namespace IKDFrontEnd.Controllers
{
    public class AdmissionController : Controller
    {
        private readonly DbikdContext _context;
        private readonly BannerService _bannerService;
        private readonly CmsRepository _cmsRepo;
        private readonly IDistributedCache _distributedCache;
		private readonly DbCollegeContext _contextCollege;
		public AdmissionController(
			DbikdContext context,
			BannerService bannerService,
			CmsRepository cmsRepo,
			IDistributedCache distributedCache,
			DbCollegeContext contextCollege)  // Added distributed cache parameter
		{
			_context = context;
			_bannerService = bannerService;
			_cmsRepo = cmsRepo;
			_distributedCache = distributedCache;
			_contextCollege = contextCollege;
		}

		[Route("admissions/")]
        public async Task<IActionResult> Home(int page = 1, int pageSize = 30)
        {
            var skip = (page - 1) * pageSize;

            // Get banners (unchanged)
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            string cacheKey = $"admissions_home_page_{page}_{pageSize}";
            HomePageViewModel viewModel = null;

            // Try to get from Redis cache
            try
            {
                var cachedData = await _distributedCache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cachedData))
                {
                    viewModel = JsonSerializer.Deserialize<HomePageViewModel>(cachedData);

                    // Optional: add debug info
                    // ViewBag.CacheSource = "Redis";
                }
            }
            catch
            {
                // If Redis fails, just continue to database
            }

            // If not in cache, get from database
            if (viewModel == null)
            {
                var cities = await _contextCollege.TblDefCities.OrderBy(c => c.CityName).ToListAsync();
                var courseLevels = await _contextCollege.TblXcourseLevels.OrderBy(c => c.SortOrder).ToListAsync();
                ViewBag.AdCount = await _contextCollege.TblAdmissions.CountAsync();

                var admissions = await (
                    from a in _contextCollege.TblAdmissions
                    join co in _contextCollege.TblColleges on a.CollegeId equals co.Id
                    join ci in _contextCollege.TblDefCities on co.CityId equals ci.CityId
                    orderby a.Dated descending
                    select new
                    {
                        a.Id,
                        a.AdmissionTitle,
                        a.NoticeImageThumb,
                        a.Dated,
                        a.LastDate,
                        a.Url,
                        CollegeName = co.Name,
                        CollageLogo = co.Logo,
                        CollageUrl = co.Url,
                        CityName = ci.CityName
                    }
                )
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

                var result = admissions.Select(a => new CityWiseAdmissionViewModel
                {
                    AdmissionId = a.Id,
                    AdmissionTitle = a.AdmissionTitle,
                    AdmissionLogo = GetAdmissionLogoPath(a.NoticeImageThumb, a.Dated),
                    Dated = a.Dated,
                    LastDate = a.LastDate,
                    CollegeName = a.CollegeName,
                    CollageLogo = a.CollageLogo,
                    CityName = a.CityName,
                    CollageUrl = a.CollageUrl,
                    Url = a.Url
                }).ToList();

                viewModel = new HomePageViewModel
                {
                    Admissions = result,
                    SearchResults = null,
                    Cities = cities,
                    CourseLevels = courseLevels
                };

                // Save to Redis cache
                try
                {
                    await _distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(viewModel), new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) // Cache for 1 hour
                    });

                    // Optional: add debug info
                    // ViewBag.CacheSource = "Database";
                }
                catch
                {
                    // If Redis fails, just continue
                }
            }

            // Handle AJAX request (not cached)
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(viewModel.Admissions);
            }

            // Get CMS data (not cached as it might change frequently)
            var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/admissions/");
            ViewBag.CmsData = cmsData;

            return View("Home", viewModel);
        }


        [Route("admissions/search")]
        public async Task<IActionResult> HomeSearch(string levelUrl, string cityUrl, int page = 1, int pageSize = 10)
        {


            var skip = (page - 1) * pageSize;

            var educationLevel = await _contextCollege.TblXcourseLevels
                .Where(l => l.Url == levelUrl.Replace(".", ""))
                .FirstOrDefaultAsync();
            var admissions = await (
                from a in _contextCollege.TblAdmissions
                join ac in _contextCollege.TblAdmissionCourses on a.Id equals ac.NoticeId
                join c in _contextCollege.Courses on ac.CourseId equals c.Id
                join co in _contextCollege.TblColleges on c.CollegeId equals co.Id
                join ci in _contextCollege.TblDefCities on co.CityId equals ci.CityId
                where
                    (educationLevel == null || c.EducationLevelId == educationLevel.Id) &&
                    (string.IsNullOrEmpty(cityUrl) || cityUrl == "select-city" || ci.Url == cityUrl)
                group new { a, co, ci } by new
                {
                    a.Id,
                    a.AdmissionTitle,
                    a.Url,
                    a.NoticeImageThumb,
                    a.Dated,
                    a.LastDate,
                    CollegeName = co.Name,
                    CollageLogo = co.Logo,
                    CityName = ci.CityName
                } into g
                orderby g.Key.Dated descending
                select new
                {
                    g.Key.Id,
                    g.Key.AdmissionTitle,
                    g.Key.Url,
                    g.Key.NoticeImageThumb,
                    g.Key.Dated,
                    g.Key.LastDate,
                    g.Key.CollegeName,
                    g.Key.CollageLogo,
                    g.Key.CityName
                }
            )
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();


            var admissionIds = admissions.Select(x => x.Id).Distinct().ToList();




            var result = admissions.Select(a => new CityWiseAdmissionViewModel
            {
                AdmissionId = a.Id,
                AdmissionTitle = a.AdmissionTitle,
                Url = a.Url,
                AdmissionLogo = a.NoticeImageThumb,
                Dated = a.Dated,
                LastDate = a.LastDate,
                CollegeName = a.CollegeName,
                CollageLogo = a.CollageLogo,
                CityName = a.CityName,

            }).ToList();
            ViewBag.CityUrl = cityUrl;
            ViewBag.LevelUrl = levelUrl;
            var cities = await _contextCollege.TblDefCities.OrderBy(c => c.CityName).ToListAsync();
            var courseLevels = await _contextCollege.TblXcourseLevels.OrderBy(c => c.SortOrder).ToListAsync();

            var viewModel = new HomePageViewModel
            {

                SearchResults = result,
                Cities = cities,
                CourseLevels = courseLevels,
                Admissions = result


            };
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_AdmissionHomeSearch", viewModel);
            }

            return View("Home", viewModel);
        }

        [Route("/admissions/latest-admissions-in-top-colleges-universities-of-pakistan")]
        public async Task<IActionResult> Collegesforlatestadmissions(int page = 1, int pageSize = 50)
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;
            var colleges = await (
                                     from a in _contextCollege.TblAdmissions
                                     join ac in _contextCollege.TblAdmissionCourses on a.Id equals ac.NoticeId
                                     join c in _contextCollege.Courses on ac.CourseId equals c.Id
                                     join co in _contextCollege.TblColleges on c.CollegeId equals co.Id
                                     group new { a, co } by new
                                     {
                                         co.Id,
                                         co.Name,
                                         co.Logo,
                                         co.Url
                                     } into g
                                     orderby g.Max(x => x.a.Dated) descending
                                     select new
                                     {
                                         CollegeId = g.Key.Id,
                                         CollegeName = g.Key.Name,
                                         CollageLogo = g.Key.Logo,
                                         CollegeUrl = g.Key.Url,
                                         LatestAdmissionDate = g.Max(x => x.a.Dated)
                                     }
                                 )
                                 .Skip((page - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync();



            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(colleges);
            }
            //var cmsData = await _context.TblCms
            //                              .Where(c => c.Url.Contains("/admissions/latest-admissions-in-top-colleges-universities-of-pakistan"))
            //                              .FirstOrDefaultAsync();
            var cmsData = await _cmsRepo.GetByUrlAsync($"/admissions/latest-admissions-in-top-colleges-universities-of-pakistan");
            ViewBag.CmsData = cmsData;
            return View(colleges);
        }

        [Route("admissions/city-wise-admission.aspx")]
        public async Task<IActionResult> CityWiseAdmissions()
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            var cities = await (
                                 from city in _contextCollege.TblDefCities
                                 join college in _contextCollege.TblColleges on city.CityId equals college.CityId
                                 join admission in _contextCollege.TblAdmissions on college.Id equals admission.CollegeId
                                 group city by new { city.CityId, city.CityName } into g
                                 orderby g.Key.CityName
                                 select new DBCollege.TblDefCity
                                 {
                                     CityId = g.Key.CityId,
                                     CityName = g.Key.CityName
                                 }
                             ).ToListAsync();

            if (cities == null)
            {
                return NotFound();
            }
            var cmsData = await _cmsRepo.GetByUrlAsync($"/admissions/city-wise-admission.aspx");
            ViewBag.CmsData = cmsData;

            return View(cities);
        }

        [Route("/admissions/admissions-in-{cityName}.aspx")]
        public async Task<IActionResult> CityWiseAdmissionsDetail(string cityName, int page = 1, int pageSize = 10)
        {
            var skip = (page - 1) * pageSize;
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;
            var totalAdmissionsCount = await (
                                               from a in _contextCollege.TblAdmissions
                                               join ci in _contextCollege.TblDefCities on a.CityId equals ci.CityId
                                               where ci.CityName == cityName
                                               select a.Id
                                           ).CountAsync();
            ViewBag.TotalAdmissions = totalAdmissionsCount;
            ViewBag.CityName = cityName;

            var admissions = await (
                                    from a in _contextCollege.TblAdmissions
                                    join co in _contextCollege.TblColleges on a.CollegeId equals co.Id
                                    join ci in _contextCollege.TblDefCities on co.CityId equals ci.CityId
                                    where ci.CityName == cityName
                                    orderby a.Dated descending
                                    select new
                                    {
                                        a.Id,
                                        a.AdmissionTitle,
                                        a.Url,
                                        a.NoticeImageThumb,
                                        a.Dated,
                                        a.LastDate,
                                        CollegeName = co.Name,
                                        CollageLogo = co.Logo,
                                        CityName = ci.CityName
                                    }
                                )
                                .Skip(skip)
                                .Take(pageSize)
                                .ToListAsync();

            if (admissions == null)
            {
                return NotFound();
            }
            var admissionIds = admissions.Select(x => x.Id).ToList();


            var result = admissions.Select(a => new CityWiseAdmissionViewModel
            {
                AdmissionId = a.Id,
                AdmissionTitle = a.AdmissionTitle,
                Url = a.Url,
                AdmissionLogo = GetAdmissionLogoPath(a.NoticeImageThumb, a.Dated),
                Dated = a.Dated,
                LastDate = a.LastDate,
                CollegeName = a.CollegeName,
                CollageLogo = a.CollageLogo,
                CityName = a.CityName,

            }).ToList();
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(result);
            }

            //var cmsData = await _context.TblCms
            //   .Where(c => c.Url.Contains($"/admissions/admissions-in-{cityName}.aspx"))
            //   .FirstOrDefaultAsync();
            var cmsData = await _cmsRepo.GetByUrlAsync($"/admissions/admissions-in-{cityName}.aspx");
            ViewBag.CmsData = cmsData;
            return View(result);
        }

        [Route("/admissions/level-wise-admissions.aspx")]
        public async Task<IActionResult> LevelWiseAdmissions()
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;
            //var cmsData = await _context.TblCms
            //                            .Where(c => c.Url.Contains("/admissions/level-wise-admissions.aspx"))
            //                            .FirstOrDefaultAsync();
            var cmsData = await _cmsRepo.GetByUrlAsync($"/admissions/level-wise-admissions.aspx");
            if (cmsData == null)
            {
                return NotFound();
            }
            ViewBag.CmsData = cmsData;
            return View("LevelWiseAdmissions");
        }




        [Route("/admissions/{levelUrl}-admissions-in-{cityUrl}-{id:int}.{aspx?}")]

        public async Task<IActionResult> LevelWiseAdmissionsDetailWithSection(
    string levelUrl,
    string cityUrl,
    int page = 1,
    int pageSize = 10,
    int id = 0)
        {
            var skip = (page - 1) * pageSize;

            if (levelUrl.ToLower() == "phd")
                levelUrl = "ph-d";
            else if (levelUrl.ToLower() == "mphil")
                levelUrl = "m-phil";

            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;



            // ✅ Case 2: Education level null but id != 0 → fetch admission directly by Id
            if (id > 0)
            {
                var admission = await (
                    from a in _contextCollege.TblAdmissions
                    join co in _contextCollege.TblColleges on a.CollegeId equals co.Id
                    join ci in _contextCollege.TblDefCities on co.CityId equals ci.CityId
                    where a.Id == id
                    select new CityWiseAdmissionViewModel
                    {
                        AdmissionId = a.Id,
                        AdmissionTitle = a.AdmissionTitle,
                        Url = a.Url,
                        AdmissionLogo = string.IsNullOrEmpty(a.NoticeImageThumb) || a.Dated == null
                            ? "/images/no-image.png"
                            : $"{a.Dated.Value.Year}/{a.Dated.Value.Month}/thumb/{a.NoticeImageThumb}",
                        Dated = a.Dated,
                        LastDate = a.LastDate,
                        CollegeName = co.Name,
                        CollageLogo = co.Logo,
                        CityName = ci.CityName,
                    }
                ).FirstOrDefaultAsync();

                if (admission == null)
                    return NotFound();


                //            var cmsData = await _context.TblCms
                //.Where(c => c.Url.Contains($"/admissions/{levelUrl}-admissions-in-{cityUrl}-{id}.aspx"))
                //.FirstOrDefaultAsync();

                var cmsData = await _cmsRepo.GetByUrlAsync($"/admissions/{levelUrl}-admissions-in-{cityUrl}-{id}.aspx");

                if (cmsData == null)
                {
                    cmsData = new TblCmsDto
                    {
                        Id = 0, // since it doesn’t exist in DB
                        SectionId = null,
                        PageName = $"{levelUrl} Admissions in {cityUrl}",
                        Url = $"/admissions/{levelUrl}-admissions-in-{cityUrl}-{id}.aspx",
                        Heading = $"{levelUrl.ToUpper()} Admissions in {cityUrl}",
                        Desc1 = $"Find the latest {levelUrl} admissions in {cityUrl}.",
                        Desc2 = null,
                        Desc3 = null,
                        MetaTitle = $"{levelUrl} Admissions {cityUrl} - Admission Details",
                        MetaDesc = $"Get information about {levelUrl} admissions available in {cityUrl}. Stay updated with the latest admission notices.",
                        MetaKeys = $"{levelUrl}, admissions, {cityUrl}, universities, colleges",
                        Image = "/images/default-admissions-banner.jpg",
                        UserId = null,
                        Date = DateTime.UtcNow
                    };
                }

                ViewBag.CmsData = cmsData;




                return View("LevelWiseAdmissionsDetail", new List<CityWiseAdmissionViewModel> { admission });
            }
            else
            {
                return NotFound();
            }
        }

        [Route("/admissions/{levelUrl}-admissions-in-{cityUrl:regex(^[[a-zA-Z-]]+$)}.aspx")]

        public async Task<IActionResult> LevelWiseAdmissionsDetailWithCity(string levelUrl, string cityUrl, int page = 1, int pageSize = 10, int id = 0)
        {


            var skip = (page - 1) * pageSize;
            if (levelUrl.ToLower() == "phd")
                levelUrl = "ph-d";
            else if (levelUrl.ToLower() == "mphil")
                levelUrl = "m-phil";

            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            var educationLevel = await _contextCollege.TblXcourseLevels
                .Where(l => l.Url.Contains(levelUrl))
                .FirstOrDefaultAsync();

            if (educationLevel == null)
            {
                return await CategoryWiseAdmissionsDetailWithCity(levelUrl, cityUrl);
            }


            var admissions = await (
                                    from a in _contextCollege.TblAdmissions
                                    join ac in _contextCollege.TblAdmissionCourses on a.Id equals ac.NoticeId
                                    join c in _contextCollege.Courses on ac.CourseId equals c.Id
                                    join co in _contextCollege.TblColleges on c.CollegeId equals co.Id
                                    join ci in _contextCollege.TblDefCities on co.CityId equals ci.CityId
                                    where c.EducationLevelId == educationLevel.Id && ci.Url == cityUrl
                                    group new { a, co, ci } by new
                                    {
                                        a.Id,
                                        a.AdmissionTitle,
                                        a.Url,
                                        a.NoticeImageThumb,
                                        a.Dated,
                                        a.LastDate,
                                        CollegeName = co.Name,
                                        CollageLogo = co.Logo,
                                        CityName = ci.CityName
                                    } into g
                                    orderby g.Key.Dated descending
                                    select new
                                    {
                                        g.Key.Id,
                                        g.Key.AdmissionTitle,
                                        g.Key.Url,
                                        g.Key.NoticeImageThumb,
                                        g.Key.Dated,
                                        g.Key.LastDate,
                                        g.Key.CollegeName,
                                        g.Key.CollageLogo,
                                        g.Key.CityName
                                    }
                                )
                                .Skip(skip)
                                .Take(pageSize)
                                .ToListAsync();
            if (admissions == null)
            {
                return NotFound();
            }
            var admissionIds = admissions.Select(x => x.Id).ToList();




            var result = admissions.Select(a => new CityWiseAdmissionViewModel
            {
                AdmissionId = a.Id,
                AdmissionTitle = a.AdmissionTitle,
                Url = a.Url,
                AdmissionLogo = string.IsNullOrEmpty(a.NoticeImageThumb) || a.Dated == null
                    ? "/images/no-image.png"
                    : $"{a.Dated.Value.Year}/{a.Dated.Value.Month.ToString()}/thumb/{a.NoticeImageThumb}",
                Dated = a.Dated,
                LastDate = a.LastDate,
                CollegeName = a.CollegeName,
                CollageLogo = a.CollageLogo,
                CityName = a.CityName,

            }).ToList();
            ViewBag.Citylist = await (
                                         from a in _contextCollege.TblAdmissions
                                         join ac in _contextCollege.TblAdmissionCourses on a.Id equals ac.NoticeId
                                         join c in _contextCollege.Courses on ac.CourseId equals c.Id
                                         join co in _contextCollege.TblColleges on c.CollegeId equals co.Id
                                         join ci in _contextCollege.TblDefCities on co.CityId equals ci.CityId
                                         where c.EducationLevelId == educationLevel.Id
                                         select new
                                         {
                                             ci.CityId,
                                             ci.CityName
                                         }
                                     )
                                     .Distinct()
                                     .OrderBy(c => c.CityName)
                                     .ToListAsync();

            ViewBag.levelUrl = levelUrl;
            ViewBag.CityUrl = cityUrl;
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(result);
            }


            //var cmsData = await _context.TblCms
            //      .Where(c => c.Url.Contains($"/admissions/{levelUrl}-admissions-in-{cityUrl}.aspx"))
            //      .FirstOrDefaultAsync();
            var cmsData = await _cmsRepo.GetByUrlAsync($"/admissions/{levelUrl}-admissions-in-{cityUrl}.aspx");
            ViewBag.CmsData = cmsData;
            return View("LevelWiseAdmissionsDetailWithCity", result);
        }

        [Route("/admissions/admissions-inter-{cityUrl}.aspx")]
        public async Task<IActionResult> LevelWiseAdmissionsDetailWithCityInter(string cityUrl, int page = 1, int pageSize = 10)
        {


            var skip = (page - 1) * pageSize;

            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            var educationLevel = await _contextCollege.TblXcourseLevels
                .Where(l => l.Id == 3)
                .FirstOrDefaultAsync();
            var levelUrl = educationLevel.Url;
            if (educationLevel == null)
            {
                return await CategoryWiseAdmissionsDetailWithCity(levelUrl, cityUrl);
            }


            var admissions = await (
                                    from a in _contextCollege.TblAdmissions
                                    join ac in _contextCollege.TblAdmissionCourses on a.Id equals ac.NoticeId
                                    join c in _contextCollege.Courses on ac.CourseId equals c.Id
                                    join co in _contextCollege.TblColleges on c.CollegeId equals co.Id
                                    join ci in _contextCollege.TblDefCities on co.CityId equals ci.CityId
                                    where c.EducationLevelId == educationLevel.Id && ci.Url == cityUrl
                                    group new { a, co, ci } by new
                                    {
                                        a.Id,
                                        a.AdmissionTitle,
                                        a.Url,
                                        a.NoticeImageThumb,
                                        a.Dated,
                                        a.LastDate,
                                        CollegeName = co.Name,
                                        CollageLogo = co.Logo,
                                        CityName = ci.CityName
                                    } into g
                                    orderby g.Key.Dated descending
                                    select new
                                    {
                                        g.Key.Id,
                                        g.Key.AdmissionTitle,
                                        g.Key.Url,
                                        g.Key.NoticeImageThumb,
                                        g.Key.Dated,
                                        g.Key.LastDate,
                                        g.Key.CollegeName,
                                        g.Key.CollageLogo,
                                        g.Key.CityName
                                    }
                                )
                                .Skip(skip)
                                .Take(pageSize)
                                .ToListAsync();

            if (admissions == null)
            {
                return NotFound();
            }
            var admissionIds = admissions.Select(x => x.Id).ToList();



            var result = admissions.Select(a => new CityWiseAdmissionViewModel
            {
                AdmissionId = a.Id,
                AdmissionTitle = a.AdmissionTitle,
                Url = a.Url,
                AdmissionLogo = a.NoticeImageThumb,
                Dated = a.Dated,
                LastDate = a.LastDate,
                CollegeName = a.CollegeName,
                CollageLogo = a.CollageLogo,
                CityName = a.CityName,

            }).ToList();
            ViewBag.Citylist = await _contextCollege.TblDefCities.OrderBy(c => c.CityName).ToListAsync();
            ViewBag.levelUrl = levelUrl;
            ViewBag.CityUrl = cityUrl;
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(result);
            }


            //var cmsData = await _context.TblCms
            //      .Where(c => c.Url.Contains($"/admissions/admissions-inter-{cityUrl}.aspx"))
            //      .FirstOrDefaultAsync();
            var cmsData = await _cmsRepo.GetByUrlAsync($"/admissions/admissions-inter-{cityUrl}.aspx");
            ViewBag.CmsData = cmsData;
            return View("LevelWiseAdmissionsDetailWithCity", result);
        }

        [Route("/admissions/categories-wise-admissions.aspx")]
        public async Task<IActionResult> CategoryWiseAdmissions()
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;
            var categories = await (
                                        from c in _contextCollege.CourseCategories
                                        orderby c.SortOrder
                                        select new CourseCategoryViewModel
                                        {
                                            Id = c.Id,
                                            Name = c.Name,
                                            Url = c.Url,
                                            Image = c.Image,
                                            TotalAdmission = _contextCollege.CourseCategoryJoins.Count(cc => cc.CategoryId == c.Id)
                                        }
                                    )
                                    .ToListAsync();
            //var cmsData = await _context.TblCms
            //   .Where(c => c.Url.Contains($"/admissions/categories-wise-admissions.aspx"))
            //   .FirstOrDefaultAsync();
            var cmsData = await _cmsRepo.GetByUrlAsync($"/admissions/categories-wise-admissions.aspx");
            ViewBag.CmsData = cmsData;
            return View("CategoryWiseAdmissions", categories);
        }

        [Route("/admissions/{catUrl}-admissions")]
        public async Task<IActionResult> CategoryWiseAdmissionsDetail(string catUrl, int page = 1, int pageSize = 10)
        {
            var skip = (page - 1) * pageSize;

            ViewBag.Banners = await _bannerService.GetBannersAsync();

            var category = await _contextCollege.CourseCategories
                .FirstOrDefaultAsync(c => c.Url == catUrl);

            if (category == null)
                return NotFound();

            // Step 1: Get distinct admission IDs (fast and light)
            var distinctAdmissionIds = await (
                from a in _contextCollege.TblAdmissions
                join ac in _context.TblAdmissionCourses on a.Id equals ac.NoticeId
                join c in _contextCollege.Courses on ac.CourseId equals c.Id
                join ccj in _contextCollege.CourseCategoryJoins on c.Id equals ccj.CourseId
                where ccj.CategoryId == category.Id
                select a.Id)
                .Distinct()
                .OrderByDescending(id => id)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            if (!distinctAdmissionIds.Any())
                return View("CategoryWiseAdmissionsDetail", new List<CityWiseAdmissionViewModel>());

            // Step 2: Fetch only the required admissions
            var paginatedAdmissions = await (
                from a in _contextCollege.TblAdmissions
                join ac in _contextCollege.TblAdmissionCourses on a.Id equals ac.NoticeId
                join c in _contextCollege.Courses on ac.CourseId equals c.Id
                join ccj in _contextCollege.CourseCategoryJoins on c.Id equals ccj.CourseId
                join co in _contextCollege.TblColleges on a.CollegeId equals co.Id
                join ci in _contextCollege.TblDefCities on co.CityId equals ci.CityId
                where distinctAdmissionIds.Contains(a.Id) && ccj.CategoryId == category.Id
                select new CityWiseAdmissionViewModel
                {
                    AdmissionId = a.Id,
                    AdmissionTitle = a.AdmissionTitle,
                    Url = a.Url,
                    AdmissionLogo = string.IsNullOrEmpty(a.NoticeImageThumb) || a.Dated == null
                    ? "/images/no-image.png"
                    : $"{a.Dated.Value.Year}/{a.Dated.Value.Month.ToString()}/thumb/{a.NoticeImageThumb}",

                    Dated = a.Dated,
                    LastDate = a.LastDate,
                    CollegeName = co.Name,
                    CollageLogo = co.Logo,
                    CityName = ci.CityName
                })
                .Distinct()
                .OrderByDescending(x => x.Dated)
                .ToListAsync();

            // Step 3: Get City List
            ViewBag.Citylist = await (
                from a in _contextCollege.TblAdmissions
                join ac in _contextCollege.TblAdmissionCourses on a.Id equals ac.NoticeId
                join c in _contextCollege.Courses on ac.CourseId equals c.Id
                join ccj in _contextCollege.CourseCategoryJoins on c.Id equals ccj.CourseId
                join co in _contextCollege.TblColleges on a.CollegeId equals co.Id
                join ci in _contextCollege.TblDefCities on co.CityId equals ci.CityId
                where ccj.CategoryId == category.Id
                select new { ci.CityId, ci.CityName })
                .Distinct()
                .OrderBy(c => c.CityName)
                .ToListAsync();

            ViewBag.CategoryUrl = catUrl;

            // For AJAX "Load More"
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(paginatedAdmissions);
            }

            //ViewBag.CmsData = await _context.TblCms
            //    .FirstOrDefaultAsync(c => c.Url == $"https://www.ilmkidunya.com/admissions/{catUrl}-admissions");
            ViewBag.CmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/admissions/{catUrl}-admissions");
            return View("CategoryWiseAdmissionsDetail", paginatedAdmissions);
        }



        [Route("/admissions/testing/{catUrl}-admissions-in-{cityUrl}")]
        public async Task<IActionResult> CategoryWiseAdmissionsDetailWithCity(string catUrl, string cityUrl, int page = 1, int pageSize = 10)
        {
            var skip = (page - 1) * pageSize;

            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            var category = await _contextCollege.CourseCategories
                .Where(c => c.Url == catUrl)
                .FirstOrDefaultAsync();

            if (category == null)
            {
                if (catUrl == "engineering" && cityUrl == "wah-cantt")
                {
                    return Redirect("/admissions/engineering-technology-admissions-in-Wah%20Cantt");
                }
                var redirectUrl = $"/{catUrl}/admissions-in-{cityUrl}";
                return Redirect(redirectUrl);
            }

            var admissions = await (
                                      from a in _contextCollege.TblAdmissions
                                      join ac in _contextCollege.TblAdmissionCourses on a.Id equals ac.NoticeId
                                      join c in _contextCollege.Courses on ac.CourseId equals c.Id
                                      join ccj in _contextCollege.CourseCategoryJoins on c.Id equals ccj.CourseId
                                      join co in _contextCollege.TblColleges on c.CollegeId equals co.Id
                                      join ci in _contextCollege.TblDefCities on co.CityId equals ci.CityId
                                      where ccj.CategoryId == category.Id && ci.Url == cityUrl
                                      group new { a, co, ci } by new
                                      {
                                          a.Id,
                                          a.AdmissionTitle,
                                          a.Url,
                                          a.NoticeImageThumb,
                                          a.Dated,
                                          a.LastDate,
                                          CollegeName = co.Name,
                                          CollageLogo = co.Logo,
                                          CityName = ci.CityName
                                      } into g
                                      orderby g.Key.Dated descending
                                      select new
                                      {
                                          g.Key.Id,
                                          g.Key.AdmissionTitle,
                                          g.Key.Url,
                                          g.Key.NoticeImageThumb,
                                          g.Key.Dated,
                                          g.Key.LastDate,
                                          g.Key.CollegeName,
                                          g.Key.CollageLogo,
                                          g.Key.CityName
                                      }
                                  )
                                  .Skip(skip)
                                  .Take(pageSize)
                                  .ToListAsync();

            if (admissions == null)
            {
                return NotFound();
            }
            var admissionIds = admissions.Select(x => x.Id).ToList();



            var result = admissions.Select(a => new CityWiseAdmissionViewModel
            {
                AdmissionId = a.Id,
                AdmissionTitle = a.AdmissionTitle,
                Url = a.Url,
                AdmissionLogo = string.IsNullOrEmpty(a.NoticeImageThumb) || a.Dated == null
                    ? "/images/no-image.png"
                    : $"{a.Dated.Value.Year}/{a.Dated.Value.Month.ToString()}/thumb/{a.NoticeImageThumb}",
                Dated = a.Dated,
                LastDate = a.LastDate,
                CollegeName = a.CollegeName,
                CollageLogo = a.CollageLogo,
                CityName = a.CityName,

            }).ToList();

            ViewBag.Citylist = await (
                                        from a in _contextCollege.TblAdmissions
                                        join ac in _contextCollege.TblAdmissionCourses on a.Id equals ac.NoticeId
                                        join c in _contextCollege.Courses on ac.CourseId equals c.Id
                                        join ccj in _contextCollege.CourseCategoryJoins on c.Id equals ccj.CourseId
                                        join co in _contextCollege.TblColleges on c.CollegeId equals co.Id
                                        join ci in _contextCollege.TblDefCities on co.CityId equals ci.CityId
                                        where ccj.CategoryId == category.Id
                                        select new
                                        {
                                            ci.CityId,
                                            ci.CityName
                                        }
                                    )
                                    .Distinct()
                                    .OrderBy(c => c.CityName)
                                    .ToListAsync();

            ViewBag.CategoryUrl = catUrl;
            ViewBag.CityUrl = cityUrl;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(result);
            }

            //var cmsData = await _context.TblCms
            //    .Where(c => c.Url.Contains($"/admissions/{catUrl}-admissions-in-{cityUrl}"))
            //    .FirstOrDefaultAsync();
            var cmsData = await _cmsRepo.GetByUrlAsync($"/admissions/{catUrl}-admissions-in-{cityUrl}");
            ViewBag.CmsData = cmsData;
            return View("CategoryWiseAdmissionsDetailWithCity", result);
        }


        public async Task<IActionResult> AdmissionsView(string adUrl)
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            // Step 1: Fetch main admission record
            var admission = await (
                from a in _contextCollege.TblAdmissions
                join co in _contextCollege.TblColleges on a.CollegeId equals co.Id
                join ci in _contextCollege.TblDefCities on co.CityId equals ci.CityId
                where a.Url == adUrl.Replace(".aspx", "")
                select new AdmissionDetailViewModel
                {
                    Id = a.Id,
                    AdmissionTitle = a.AdmissionTitle,
                    Url = a.Url,
                    Detail = a.Details,
                    NoticeImageThumb = a.NoticeImageThumb,
                    NoticeImageLarge = a.NoticeImageLarge,
                    Dated = a.Dated,
                    LastDate = a.LastDate,
                    CollegeName = co.Name,
                    CollageLogo = co.Logo,
                    ColUrl = co.Url,
                    CityName = ci.CityName,
                    MetaTitle = a.MetaTitle,
                    MetaDesc = a.MetaDesc,
                    MetaKeys = a.MetaKeywords
                }
            ).FirstOrDefaultAsync();

            if (admission == null)
            {
                return await AutoContentAdmissionPages(adUrl);
            }

            // ✅ Step 2: Fetch all courses related to this admission
            var courseNames = await (
                from ac in _contextCollege.TblAdmissionCourses
                join c in _contextCollege.Courses on ac.CourseId equals c.Id
                where ac.NoticeId == admission.Id
                select c.Name
            ).Distinct().ToListAsync();

            // ✅ Step 3: Join course names into a readable string (e.g., "BS, MS & PhD programs")
            string coursesText = "";
            if (courseNames != null && courseNames.Count > 0)
            {
                if (courseNames.Count == 1)
                    coursesText = courseNames.First();
                else if (courseNames.Count == 2)
                    coursesText = string.Join(" & ", courseNames);
                else
                    coursesText = string.Join(", ", courseNames.Take(courseNames.Count - 1)) + " & " + courseNames.Last();
            }

            // ✅ Step 4: Generate MetaTitle and MetaDesc if null or empty
            admission.MetaTitle = !string.IsNullOrWhiteSpace(admission.MetaTitle)
                ? admission.MetaTitle
                : $"{admission.CollegeName} {admission.CityName} Admission 2025 | Apply Now";

            admission.MetaDesc = !string.IsNullOrWhiteSpace(admission.MetaDesc)
                ? admission.MetaDesc
                : $"{admission.CollegeName} {admission.CityName} Admission 2025 is open for {coursesText}. Apply online before the last date. Check eligibility, fee structure & more.";

            return View("AdmissionsView", admission);
        }


        [Route("/admissions/most-popular-admissions-pakistan.aspx")]
        public async Task<IActionResult> PopularAdmisions(int page = 1, int pageSize = 20)
        {
            var skip = (page - 1) * pageSize;
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;
            var fcolleges = await _contextCollege.TblColleges
                                  .Where(C => C.IsFeatured == true)
                                  .ToListAsync();
            ViewBag.Featured = fcolleges;

            var admissions = await (
                                    from a in _contextCollege.TblAdmissions
                                    join co in _contextCollege.TblColleges on a.CollegeId equals co.Id
                                    join ci in _contextCollege.TblDefCities on co.CityId equals ci.CityId
                                    orderby a.Dated descending
                                    select new
                                    {
                                        a.Id,
                                        a.AdmissionTitle,
                                        a.Url,
                                        a.NoticeImageThumb,
                                        a.Dated,
                                        a.LastDate,
                                        CollegeName = co.Name,
                                        CollageLogo = co.Logo,
                                        CityName = ci.CityName
                                    }
                                )
                                .Skip(skip)
                                .Take(pageSize)
                                .ToListAsync();
            var admissionIds = admissions.Select(x => x.Id).ToList();


            var result = admissions.Select(a => new CityWiseAdmissionViewModel
            {
                AdmissionId = a.Id,
                AdmissionTitle = a.AdmissionTitle,
                Url = a.Url,
                AdmissionLogo = GetAdmissionLogoPath(a.NoticeImageThumb, a.Dated),
                Dated = a.Dated,
                LastDate = a.LastDate,
                CollegeName = a.CollegeName,
                CollageLogo = a.CollageLogo,
                CityName = a.CityName,

            }).ToList();
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(result);
            }

            //var cmsData = await _context.TblCms
            //   .Where(c => c.Url.Contains($"/admissions/admissions-in-.aspx"))
            //   .FirstOrDefaultAsync();

            var cmsData = await _cmsRepo.GetByUrlAsync($"/admissions/most-popular-admissions-pakistan.aspx");
            ViewBag.CmsData = cmsData;
            return View(result);
        }


        [Route("admissions-guide/{**urlSlug}")]
        public async Task<IActionResult> AutoContentAdmissionPages(string urlSlug)
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            // Step 1: Get full path from request
            var fullPath = HttpContext.Request.Path.Value?.TrimStart('/'); // e.g. "admissions/abc.aspx" or "admissions-guide/xyz.aspx"

            // Step 2: Query with full match
            //var content = await _context.TblCms
            //    .Where(c => c.Url == $"https://www.ilmkidunya.com/{fullPath}")
            //    .FirstOrDefaultAsync();
            var content = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/{fullPath}");
            if (content == null)
            {
                //content = await _context.TblCms
                // .Where(c => c.Url.Contains(fullPath.Replace("admissions/admission-guide-", "").Replace("-inter-part-1", "")))
                // .FirstOrDefaultAsync();

                content = await _cmsRepo.GetByUrlAsync(fullPath.Replace("admissions/admission-guide-", "").Replace("-inter-part-1", ""));

            }
            if (content == null)
                return NotFound(); // Optional: show custom 404

            return View("AutoContentAdmissionPages", content);
        }

        [Route("admissions/{levelUrl}")]
        public async Task<IActionResult> LevelWiseAdmissionsDetail(string levelUrl, int page = 1, int pageSize = 10)
        {
            var match = Regex.Match(levelUrl, @"-(\d+)(?:\.aspx)?$", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return await AdmissionsView(levelUrl);
            }

            var skip = (page - 1) * pageSize;

            if (levelUrl.ToLower() == "phd")
                levelUrl = "ph-d";
            else if (levelUrl.ToLower() == "mphil")
                levelUrl = "m-phil";

            if (levelUrl.Contains("-admissions-in-"))
            {
                var parts = levelUrl.Split("-admissions-in-", StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    return await CategoryWiseAdmissionsDetailWithCity(parts[0], parts[1]);
                }
            }

            var educationLevel = await _context.TblXcourseLevels
                .FirstOrDefaultAsync(l => l.Url == levelUrl);

            if (educationLevel == null)
            {
                return await AutoContentAdmissionPages(levelUrl);
            }

            ViewBag.Banners = await _bannerService.GetBannersAsync();

            // Step 1: Get filtered admissions (distinct by AdmissionId)
            var admissionQuery = (
                from a in _contextCollege.TblAdmissions
                join ac in _contextCollege.TblAdmissionCourses on a.Id equals ac.NoticeId
                join c in _contextCollege.Courses on ac.CourseId equals c.Id
                join co in _contextCollege.TblColleges on a.CollegeId equals co.Id
                join ci in _contextCollege.TblDefCities on co.CityId equals ci.CityId
                where c.EducationLevelId == educationLevel.Id
                select new
                {
                    Admission = a,
                    College = co,
                    City = ci
                });

            var distinctAdmissions = await admissionQuery
                .GroupBy(x => x.Admission.Id)
                .OrderByDescending(g => g.First().Admission.Dated)
                .Skip(skip)
                .Take(pageSize)
                .Select(g => new CityWiseAdmissionViewModel
                {
                    AdmissionId = g.First().Admission.Id,
                    AdmissionTitle = g.First().Admission.AdmissionTitle,
                    Url = g.First().Admission.Url,
                    AdmissionLogo = g.First().Admission.NoticeImageThumb,



                    Dated = g.First().Admission.Dated,
                    AdmissionLogoYear = g.First().Admission.Dated.HasValue ? g.First().Admission.Dated.Value.Year : (int?)null,
                    AdmissionLogoMonth = g.First().Admission.Dated.HasValue ? g.First().Admission.Dated.Value.Month : (int?)null,


                    LastDate = g.First().Admission.LastDate,
                    CollegeName = g.First().College.Name,
                    CollageLogo = g.First().College.Logo,
                    CityName = g.First().City.CityName
                })
                .ToListAsync();

            // Step 2: City list for filter display
            ViewBag.Citylist = await admissionQuery
                .Select(x => new { x.City.CityId, x.City.CityName })
                .Distinct()
                .OrderBy(x => x.CityName)
                .ToListAsync();

            ViewBag.levelUrl = levelUrl;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(distinctAdmissions);
            }

            //var cmsData = await _context.TblCms
            //    .Where(c => c.Url.Contains($"/admissions/{levelUrl.Replace("-", "")}"))
            //    .FirstOrDefaultAsync();
            var cmsData = await _cmsRepo.GetByUrlAsync($"/admissions/{levelUrl.Replace("-", "")}");
            ViewBag.CmsData = cmsData;

            return View("LevelWiseAdmissionsDetail", distinctAdmissions);
        }


        //[Route("/{catUrl}/admissions-in-{cityUrl}")]
        //public async Task<IActionResult> CategoryAdmissionsInCity(string catUrl, string cityUrl, int page = 1, int pageSize = 10)
        //{
        //    var skip = (page - 1) * pageSize;

        //    // Load banners
        //    var banners = await _bannerService.GetBannersAsync();
        //    ViewBag.Banners = banners;

        //    // Find category by Url
        //    var category = await _context.CourseCategories
        //        .FirstOrDefaultAsync(c => c.Url == catUrl);

        //    if (category == null)
        //        return NotFound();

        //    // Query admissions for given category + city
        //    var admissions = await (
        //        from a in _context.TblAdmissions
        //        join ac in _context.TblAdmissionCourses on a.Id equals ac.NoticeId
        //        join c in _context.Courses on ac.CourseId equals c.Id
        //        join ccj in _context.CourseCategoryJoins on c.Id equals ccj.CourseId
        //        join co in _context.TblColleges on c.CollegeId equals co.Id
        //        join ci in _context.TblDefCities on co.CityId equals ci.CityId
        //        where ccj.CategoryId == category.Id && ci.Url == cityUrl
        //        group new { a, co, ci } by new
        //        {
        //            a.Id,
        //            a.AdmissionTitle,
        //            a.Url,
        //            a.NoticeImageThumb,
        //            a.Dated,
        //            a.LastDate,
        //            CollegeName = co.Name,
        //            CollageLogo = co.Logo,
        //            CityName = ci.CityName
        //        } into g
        //        orderby g.Key.Dated descending
        //        select new
        //        {
        //            g.Key.Id,
        //            g.Key.AdmissionTitle,
        //            g.Key.Url,
        //            g.Key.NoticeImageThumb,
        //            g.Key.Dated,
        //            g.Key.LastDate,
        //            g.Key.CollegeName,
        //            g.Key.CollageLogo,
        //            g.Key.CityName
        //        }
        //    )
        //    .Skip(skip)
        //    .Take(pageSize)
        //    .ToListAsync();

        //    // Map to ViewModel
        //    var result = admissions.Select(a => new CityWiseAdmissionViewModel
        //    {
        //        AdmissionId = a.Id,
        //        AdmissionTitle = a.AdmissionTitle,
        //        Url = a.Url,
        //        AdmissionLogo = string.IsNullOrEmpty(a.NoticeImageThumb) || a.Dated == null
        //            ? "/images/no-image.png"
        //            : $"{a.Dated.Value.Year}/{a.Dated.Value.Month}/thumb/{a.NoticeImageThumb}",
        //        Dated = a.Dated,
        //        LastDate = a.LastDate,
        //        CollegeName = a.CollegeName,
        //        CollageLogo = a.CollageLogo,
        //        CityName = a.CityName
        //    }).ToList();

        //    // City list for filters
        //    ViewBag.Citylist = await (
        //        from a in _context.TblAdmissions
        //        join ac in _context.TblAdmissionCourses on a.Id equals ac.NoticeId
        //        join c in _context.Courses on ac.CourseId equals c.Id
        //        join ccj in _context.CourseCategoryJoins on c.Id equals ccj.CourseId
        //        join co in _context.TblColleges on c.CollegeId equals co.Id
        //        join ci in _context.TblDefCities on co.CityId equals ci.CityId
        //        where ccj.CategoryId == category.Id
        //        select new { ci.CityId, ci.CityName }
        //    )
        //    .Distinct()
        //    .OrderBy(c => c.CityName)
        //    .ToListAsync();

        //    ViewBag.CategoryUrl = catUrl;
        //    ViewBag.CityUrl = cityUrl;

        //    // Ajax request (Load More)
        //    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        //    {
        //        return Json(result);
        //    }

        //    // CMS content
        //    var cmsData = await _context.TblCms
        //        .FirstOrDefaultAsync(c => c.Url.Contains($"/admissions/{catUrl}-admissions-in-{cityUrl}.aspx"));
        //    ViewBag.CmsData = cmsData;

        //    return View("CategoryWiseAdmissionsDetailWithCity", result);
        //}



        private string GetAdmissionLogoPath(string fileName, DateTime? dated)
        {
            if (string.IsNullOrEmpty(fileName) || dated == null)
                return "/images/no-image.png"; // fallback image

            var year = dated.Value.Year;
            var month = dated.Value.Month.ToString(); // e.g. 03 for March

            return $"{year}/{month}/thumb/{fileName}";
        }

    }
}
