using Dapper;
using IKDFrontEnd.DBCollege;

//using IKDFrontEnd.DBCollege;
using IKDFrontEnd.Models;
using IKDFrontEnd.Services;
using IKDFrontEnd.ViewModels;
using IKDFrontEnd.ViewModels.Common;
using Microsoft.AspNetCore.Components.Sections;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Data;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime;
using System.Security.Policy;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace IKDFrontEnd.Controllers
{
    public class CollegesController : Controller
    {
		//private readonly DbikdContext _context;
		private readonly DbCollegeContext _context;
		private readonly BannerService _bannerService;
        private readonly ILogger<CollegesController> _logger;
        private readonly CmsRepository _cmsRepo;
        private readonly IDistributedCache _distributedCache;

		public CollegesController(
			DbCollegeContext context,
            BannerService bannerService,
			ILogger<CollegesController> logger,
			CmsRepository cmsRepo,
			IDistributedCache distributedCache)  // Added distributed cache parameter
		{
            _context = context;
            _bannerService = bannerService;
			_logger = logger;
			_cmsRepo = cmsRepo;
			_distributedCache = distributedCache;
			//_contextCollege = contextCollege;
		}

		[Route("/colleges/")]
        [Route("/colleges/index.html")]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Home()
        {
            string cacheKey = "colleges_home_page_data";

            // Try to get from Redis cache
            try
            {
                var cachedData = await _distributedCache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cachedData))
                {
                    var cachedDataObject = JsonSerializer.Deserialize<CollegesHomeCacheData>(cachedData);
                    if (cachedDataObject != null)
                    {
                        ViewBag.Banners = cachedDataObject.Banners;
                        ViewBag.Cities = cachedDataObject.Cities;
                        ViewBag.Levels = cachedDataObject.Levels;
                        ViewBag.Categories = cachedDataObject.Categories;
                        ViewBag.FeaturedColleges = cachedDataObject.FeaturedColleges;

                        // Optional: add debug info
                        // ViewBag.CacheSource = "Redis";

                        return View("Home");
                    }
                }
            }
            catch
            {
                // If Redis fails, just continue to database
            }

            // If not in cache, get from database
            var banners = await _bannerService.GetBannersAsync();
            var cities = await _context.TblDefCities.ToListAsync();
            var categories = await _context.CourseCategories.OrderBy(c => c.SortOrder).AsNoTracking().ToListAsync();
            var levels = await _context.TblXcourseLevels.OrderBy(c => c.SortOrder).AsNoTracking().ToListAsync();
            var featuredColleges = await _context.TblColleges
                                                 .Where(c => c.IsFeatured.HasValue && c.IsFeatured.Value)
                                                 .OrderBy(c => c.SortOrder == null).ThenBy(c => c.SortOrder)
                                                 .ToListAsync();

            ViewBag.Banners = banners;
            ViewBag.Cities = cities;
            ViewBag.Levels = levels;
            ViewBag.Categories = categories;
            ViewBag.FeaturedColleges = featuredColleges;

            // Save to Redis cache
            try
            {
                var cacheData = new CollegesHomeCacheData
                {
                    Banners = banners,
                    Cities = cities,
                    Levels = levels,
                    Categories = categories,
                    FeaturedColleges = featuredColleges
                };

                await _distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(cacheData), new DistributedCacheEntryOptions
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

            return View("Home");
        }


        // Helper class for caching
        public class CollegesHomeCacheData
        {
            public List<Banner> Banners { get; set; }
            public List<DBCollege.TblDefCity> Cities { get; set; }
            public List<DBCollege.TblXcourseLevel> Levels { get; set; }
            public List<DBCollege.CourseCategory> Categories { get; set; }
            public List<DBCollege.TblCollege> FeaturedColleges { get; set; }
        }



        [Route("colleges/{collegeUrl:regex(^((?!universities-in-)(?!colleges-in-)).*$)}.aspx")]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> CollegeHome(string collegeUrl)
        {
            if (string.IsNullOrWhiteSpace(collegeUrl))
                return NotFound();

            // Handle special pages first
            if (collegeUrl.Contains("-reviews"))
            {
                var result = await CollegeReviews(collegeUrl) as ViewResult;
                if (result != null)
                    return View("CollegeReviews", result.Model);
                return NotFound();
            }

            if (collegeUrl.Contains("fee-structure"))
            {
                var result = await CollegeFee(collegeUrl) as ViewResult;
                if (result != null)
                    return View("CollegeFee", result.Model);
                return NotFound();
            }

            if (collegeUrl.Contains("-admission"))
            {
                var result = await CollegeAmissions(collegeUrl) as ViewResult;
                if (result != null)
                    return View("CollegeAmissions", result.Model);
                return NotFound();
            }

            if (collegeUrl.Contains("-merit-list"))
            {
                var result = await CollegeMeritLists(collegeUrl) as ViewResult;
                if (result != null)
                    return View("CollegeMeritLists", result.Model);
                return NotFound();
            }

            // Remove unwanted suffix
            collegeUrl = collegeUrl.Replace("-courses", "");

            // ✅ Fetch all data from Stored Procedure using Dapper
            using var connection = new SqlConnection(
                _context.Database.GetConnectionString());

            using var multi = await connection.QueryMultipleAsync(
                "sp_GetCollegeHomeData",
                new { CollegeUrl = collegeUrl },
                commandType: CommandType.StoredProcedure);

            var college = await multi.ReadFirstOrDefaultAsync<CollegeCityDto>();
            if (college == null)
            {
                if (string.IsNullOrWhiteSpace(collegeUrl))
                    return NotFound();
                return RedirectToAction("LevelWiseCollegesDetails", new { levelUrl = collegeUrl });
            }

            var reviews = (await multi.ReadAsync<DBCollege.TblCollegereview>()).ToList();
            var distinctStudyLevels = (await multi.ReadAsync<string>()).ToList();
            var latestAdmission = await multi.ReadFirstOrDefaultAsync<DBCollege.TblAdmission>();
            var allCourses = (await multi.ReadAsync<DBCollege.Course>()).ToList();

            // Calculate ratings
            var (overallRating, reviewScore) = CalculateOverallRanking(reviews, college.GoogleRanking, college.Fbranking);

            var baseUrl = collegeUrl
                 .Replace("-fee-structure", "")
                 .Replace("-admission", "")
                 .Replace("-merit-lists", "");

            ViewBag.CollegeBaseUrl = baseUrl;

            // CMS data (as before)
            var cmsData = await _cmsRepo.GetListByUrlAsync($"/colleges/{baseUrl}");
            ViewBag.ShowFeeStructure = cmsData.Any(c => c.Url.Contains(baseUrl + "-fee-structure.aspx"));
            ViewBag.ShowMeritList = cmsData.Any(c => c.Url.Contains(baseUrl + "-merit-lists.aspx"));
            ViewBag.ShowAdmissions = cmsData.Any(c => c.Url.Contains(baseUrl + "-admission.aspx"));

            // Prepare final view model
            var viewModel = new CollegesViewModel
            {
                CollegeId = college.Id,
                CollegeName = college.Name ?? "Not Available",
                CollegeShortName = college.ShortName ?? college.Name ?? "Not Available",
                Url = college.Url ?? "Not Available",
                CollegeImage = college.Logo ?? "Not Available",
                EstablishedYear = college.EstablishedYear?.ToString() ?? "Not Available",
                StudentsCount = college.StudentsCount?.ToString() ?? "Not Available",
                Recognition = college.AffiliationStatus ?? "Not Available",
                ContactNumber = college.ContactNumber ?? "Not Available",
                Address = college.Address ?? "Not Available",
                Website = college.Website ?? "Not Available",
                IsGovt = college.IsGovt,
                AffiliationStatus = college.AffiliationStatus ?? "Not Available",
                MetaTitle = college.MetaTitle ?? "Not Available",
                MetaDesc = college.MetaDesc ?? "Not Available",
                MetaKeyWords = college.MetaKeyWords ?? "Not Available",
                InstituteDetails = college.InstituteDetails ?? "Not Available",
                AdministrationDetails = college.AdministrationDetails ?? "Not Available",
                InstituteOtherDetails = college.InstituteOtherDetails ?? "Not Available",
                FacebokkRanking = college.Fbranking,
                GoogleRanking = college.GoogleRanking,
                City = college.CityName ?? "Not Available",
                Email = college.Email ?? "Not Available",
                IlmkiDuniyaRanking = overallRating.ToString("0.0"),
                OveraAlRanking = reviewScore.ToString("0.0"),
                Latitude = college.Latitude,
                Longitude = college.Longitude,
                Zoomlevel = college.Zoomlevel,
                Reviews = reviews ?? new List<DBCollege.TblCollegereview>(),
                TotalCourseCount = allCourses?.Count() ?? 0,
                DistinctStudyLevels = distinctStudyLevels ?? new List<string>(),
                AllCourses = allCourses ?? new List<DBCollege.Course>(),
                Addmission = latestAdmission,
                AddmissionLogo = latestAdmission != null
                    ? GetAdmissionLogoPathThumb(latestAdmission.NoticeImageThumb, latestAdmission.Dated)
                    : null
            };

            // Other ViewBag data
            ViewBag.Levels = await _context.TblXcourseLevels.AsNoTracking().ToListAsync();
            ViewBag.Cities = await _context.TblDefCities.AsNoTracking().ToListAsync();
            ViewBag.Fields = await _context.CourseCategories.AsNoTracking().ToListAsync();
            ViewBag.Banners = await _bannerService.GetBannersAsync();

            return View("CollegeHome", viewModel);
        }






        [HttpGet]
        [Route("colleges/{course}-colleges-in-{cityUrl}")]
        [Route("/colleges/{course}-colleges-universities-in-{cityUrl}")]
        public async Task<IActionResult> GetInstitutions(
        string course,
        string cityUrl,
        int page = 1,
        int pageSize = 10)
        {
            string? filterCategory = null;
            string? filterValue = null;

            switch (course.ToLower())
            {
                case "engineering":
                    filterCategory = "category";
                    filterValue = "engineering";
                    break;

                case "fsc-pre-medical":
                    filterCategory = "level";
                    filterValue = "fsc pre medical";
                    break;

                case "mphil-english":
                    filterCategory = "level";
                    filterValue = "mphil";
                    break;

                case "ielts":
                    filterCategory = "category";
                    filterValue = "ielts";
                    break;

                case "govt":
                    filterCategory = "government";
                    break;

                case "private":
                    filterCategory = "private";
                    break;

                case "bs-zoology":
                    filterCategory = "level";
                    filterValue = "bs zoology";
                    break;
                case "mdcat":
                    filterCategory = "level";
                    filterValue = "mdcat";
                    break;
                case "bs-economics":
                    filterCategory = "level";
                    filterValue = "bs economics";
                    break;
                case "msc":
                    filterCategory = "level";
                    filterValue = "msc";
                    break;
                default:

                    return NotFound(); // Unknown course filter
            }

            return await GetFilteredInstitutionsByCity(cityUrl, filterCategory, filterValue, page, pageSize);
        }


        [HttpGet]
        [Route("colleges/col/search")]
        public async Task<IActionResult> SearchColleges(string keyword, int? cityId, int? categoryId, int? levelId, int page = 1, int pageSize = 10)
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;
            var categories = await _context.CourseCategories.OrderBy(c => c.SortOrder).AsNoTracking().ToListAsync();
            ViewBag.Categories = categories;
            var query = _context.TblColleges.Where(c => c.IsActive == true);

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(c => c.Name.Contains(keyword) || c.ShortName.Contains(keyword));

            if (cityId.HasValue)
                query = query.Where(c => c.CityId == cityId.Value);
            if (categoryId.HasValue)
            {
                var courseIdsInCategory = await _context.CourseCategoryJoins
                    .Where(j => j.CategoryId == categoryId.Value)
                    .Select(j => j.CourseId)
                    .Distinct()
                    .ToListAsync();

                var collegeIdsFromCategory = await _context.Courses
                    .Where(c => courseIdsInCategory.Contains(c.Id) && c.CollegeId.HasValue)
                    .Select(c => c.CollegeId.Value)
                    .Distinct()
                    .ToListAsync();

                query = query.Where(c => collegeIdsFromCategory.Contains(c.Id));
            }


            if (levelId.HasValue)
            {
                var collegeIdsWithLevel = await _context.Courses
                    .Where(c => c.EducationLevelId == levelId.Value && c.CollegeId.HasValue)
                    .Select(c => c.CollegeId.Value)
                    .Distinct()
                    .ToListAsync();

                query = query.Where(c => collegeIdsWithLevel.Contains(c.Id));
            }


            ViewBag.TotalColleges = await query.CountAsync();

            query = query.OrderByDescending(c => c.Views);
            var pagedColleges = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var collegeIds = pagedColleges.Select(c => c.Id).ToList();

            var courseCounts = await _context.Courses
                .Where(c => c.IsActive == true && c.CollegeId.HasValue && collegeIds.Contains(c.CollegeId.Value))
                .GroupBy(c => c.CollegeId)
                .Select(g => new { CollegeId = g.Key.Value, Count = g.Count() })
                .ToListAsync();

            var collegeRatings = await GetCollegeRatingsAsync(collegeIds);
            var results = pagedColleges.Select(c => new CollegeSearchResult
            {
                Name = c.Name,
                Logo = c.Logo,
                Url = c.Url,
                Address = c.Address,
                ContactNumber = c.ContactNumber,
                IsGovt = c.IsGovt,
                Qsranking = c.Qsranking,
                GoogleRanking = c.GoogleRanking,
                Rating = collegeRatings.FirstOrDefault(x => x.CollegeId == c.Id)?.AvgRating ?? 0,

                TotalCourses = courseCounts.FirstOrDefault(x => x.CollegeId == c.Id)?.Count ?? 0
            })
            .ToList();
            ViewBag.SerialStart = (page - 1) * pageSize + 1;
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_SearchResultTable", results);
            var viewModel = new SearchViewModel
            {
                Keyword = keyword,
                CityId = cityId,
                CategoryId = categoryId,
                LevelId = levelId,
                Results = results
            };

            return View(viewModel);
        }






        private (decimal overallRating, decimal reviewScore) CalculateOverallRanking(List<DBCollege.TblCollegereview> reviews, decimal? googleRanking, decimal? facebookRanking)
        {
            // Initialize variables
            decimal reviewScore = 0;
            List<decimal> ratings = new List<decimal>();

            // Check if reviews exist and calculate reviewScore
            if (reviews.Any())
            {
                // Filter and convert ratings to decimals
                ratings = reviews
                            .Where(r => decimal.TryParse(r.Rating, out _)) // Filter out invalid ratings
                            .Select(r => Convert.ToDecimal(r.Rating))
                            .ToList();

                if (ratings.Any())
                {
                    reviewScore = ratings.Average(); // Average rating from reviews
                }
            }

            // Handling Google and Facebook Rankings
            decimal googleRank = googleRanking.HasValue ? googleRanking.Value : 0;
            decimal facebookRank = facebookRanking.HasValue ? facebookRanking.Value : 0;

            // Calculate IKDOverallRating
            decimal totalReviews = reviews.Count;
            decimal sumOfRatings = ratings.Sum();
            decimal maxSumOfRatings = totalReviews * 5; // Assuming each user can rate once (max rating = 5)

            // Prevent division by zero if maxSumOfRatings is zero
            decimal ikdOverallRating = 0;
            if (maxSumOfRatings != 0)
            {
                ikdOverallRating = Math.Round((sumOfRatings / maxSumOfRatings) * 5, 1);
            }

            // Calculate Overall Ranking
            decimal overallRating;
            if (googleRank == 0.0m && facebookRank == 0.0m)
            {
                overallRating = ikdOverallRating; // Use IKDOverallRating if no Google or Facebook ranking
            }
            else if (googleRank == 0.0m && facebookRank != 0.0m)
            {
                overallRating = Math.Round((facebookRank + ikdOverallRating) / 2, 1);
            }
            else if (facebookRank == 0.0m && googleRank != 0.0m)
            {
                overallRating = Math.Round((googleRank + ikdOverallRating) / 2, 1);
            }
            else
            {
                overallRating = Math.Round((googleRank + facebookRank + ikdOverallRating) / 3, 1);
            }

            return (overallRating, reviewScore);
        }

        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> CollegeFee(string collegeUrl)
        {

            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            var collegeName = collegeUrl.Replace("-fee-structure", "");
            var college = await _context.TblColleges
                .Where(c => c.Url.Contains(collegeName))
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (college == null)
            {
                return NotFound();
            }


            var courses = await _context.Courses
                        .Where(xc => xc.CollegeId == college.Id)
                        .Join(_context.TblXcourseLevels,
                        xc => xc.EducationLevelId,
                        cl => cl.Id,
                        (xc, cl) => new { xc, cl })
                        .Join(_context.TblColleges,
                      temp => temp.xc.CollegeId,
                      ci => ci.Id,
                      (temp, ci) => new
                      {
                          temp.xc.Id,
                          temp.xc.Name,
                          temp.xc.Duration,
                          temp.xc.Fee,
                          CourseLevel = temp.cl.Name,
                          CollegeName = ci.Name
                      })
                     .ToListAsync();
            if (courses == null)
            {
                return NotFound();
            }
            var groupedCourses = courses
                .GroupBy(c => c.CourseLevel)
                .Select(g => new CourseGroupedData
                {
                    CourseLevel = g.Key,
                    Courses = g.Select(c => new DBCollege.Course
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Duration = c.Duration,
                        Fee = c.Fee
                    }).ToList()
                })
                .ToList();
            var baseUrl = collegeUrl
            .Replace("-fee-structure", "")
            .Replace("-admission", "")
            .Replace("-merit-lists", "");
            ViewBag.CollegeBaseUrl = baseUrl;
            //var cmsData = await _context.TblCms
            //   .Where(c => c.Url.Contains(collegeUrl + ".aspx"))
            //   .FirstOrDefaultAsync();
            var cmsData = await _cmsRepo.GetByUrlAsync($"/colleges/{collegeUrl}.aspx");
            ViewBag.CmsData = cmsData;
            //var cmsDataa = await _context.TblCms
            //                 .Where(c => c.Url.Contains(baseUrl))
            //                 .ToListAsync();
            var cmsDataa = await _cmsRepo.GetListByUrlAsync($"/colleges/{baseUrl}");


            ViewBag.ShowFeeStructure = cmsDataa.Any(c => c.Url.Contains(baseUrl + "-fee-structure.aspx"));
            ViewBag.ShowMeritList = cmsDataa.Any(c => c.Url.Contains(baseUrl + "-merit-lists.aspx"));
            ViewBag.ShowAdmissions = cmsDataa.Any(c => c.Url.Contains(baseUrl + "-admission.aspx"));
            ViewBag.Levels = await _context.TblXcourseLevels.AsNoTracking().ToListAsync();
            ViewBag.Cities = await _context.TblDefCities.AsNoTracking().ToListAsync();
            ViewBag.Fields = await _context.CourseCategories.AsNoTracking().ToListAsync();

            var viewModel = new CollegesViewModel
            {
                CollegeId = college.Id,
                CollegeName = college.Name,
                CollegeImage = college.Logo,
                CollegeShortName = college.ShortName,
                GroupedCourses = groupedCourses
            };

            return View(("CollegeFee"), viewModel);
        }

        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> CollegeAmissions(string collegeUrl)
        {
            if (string.IsNullOrWhiteSpace(collegeUrl))
                return NotFound("Invalid college URL.");

            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners ?? new List<Banner>();

            var collegeName = collegeUrl.Replace("-admission", "", StringComparison.OrdinalIgnoreCase);

            var college = await _context.TblColleges
                .Where(c => c.Url.Contains(collegeName))
                .Select(c => new DBCollege.TblCollege
                {
                    Id = c.Id,
                    Name = c.Name ?? "",
                    Logo = c.Logo ?? "",
                    Address = c.Address ?? "",
                    ContactNumber = c.ContactNumber ?? ""
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (college == null)
                return NotFound("College not found.");

            var admissions = await (from a in _context.TblAdmissions
                                    join ac in _context.TblAdmissionCourses on a.Id equals ac.NoticeId
                                    join c in _context.Courses on ac.CourseId equals c.Id
                                    where a.CollegeId == college.Id
                                    select new
                                    {
                                        a.Id,
                                        a.Dated,
                                        a.LastDate,
                                        a.NoticeImageLarge,
                                        c.Name,
                                        a.AdmissionTitle,
                                        a.NoticeImageThumb
                                    })
                                    .OrderByDescending(x => x.Dated)
                                    .AsNoTracking()
                                    .ToListAsync();

            if (admissions == null || !admissions.Any())
                return NotFound();

            var groupedAdmissions = admissions
                .GroupBy(x => x.Id)
                .Select(g => new CourseGroupedData
                {
                    AdmissionDate = g.Select(x => x.Dated).FirstOrDefault(),
                    LastDate = g.Select(x => x.LastDate).FirstOrDefault() == new DateTime(1800, 1, 21)
                                ? (DateTime?)null
                                : g.Select(x => x.LastDate).FirstOrDefault(),
                    AdmissionImage = g.Select(x => x.NoticeImageLarge).FirstOrDefault() ?? "",
                    Courses = g.Select(x => new DBCollege.Course
                    {
                        Name = string.IsNullOrWhiteSpace(x.Name) ? "" : x.Name,
                        CreatedDate = DateTime.Now
                    })
                    .OrderByDescending(c => c.CreatedDate)
                    .Take(5)
                    .ToList(),
                    AdmissionTitle = string.IsNullOrWhiteSpace(g.Select(x => x.AdmissionTitle).FirstOrDefault())
                                     ? ""
                                     : g.Select(x => x.AdmissionTitle).FirstOrDefault(),
                    AddmissionLogoList = g
                        .Select(x => GetAdmissionLogoPath(x.NoticeImageThumb, x.Dated))
                        .Where(p => !string.IsNullOrEmpty(p))
                        .Distinct()
                        .DefaultIfEmpty("")
                        .ToList()
                })
                .OrderByDescending(x => x.AdmissionDate)
                .Take(20)
                .ToList();

            var viewModel = new CollegesViewModel
            {
                CollegeId = college.Id,
                CollegeName = college.Name,
                CollegeImage = college.Logo,
                Address = college.Address,
                ContactNumber = college.ContactNumber,
                Addmissions = groupedAdmissions.Select(ad => new DBCollege.TblAdmission
                {
                    Dated = ad.AdmissionDate ?? DateTime.Now,
                    LastDate = ad.LastDate,
                    NoticeImageLarge = ad.AdmissionImage ?? "",
                    AdmissionTitle = ad.AdmissionTitle ?? ""
                }).ToList(),
                GroupedCourses = groupedAdmissions
            };

            var baseUrl = collegeUrl
                .Replace("-fee-structure", "", StringComparison.OrdinalIgnoreCase)
                .Replace("-admission", "", StringComparison.OrdinalIgnoreCase)
                .Replace("-merit-lists", "", StringComparison.OrdinalIgnoreCase);

            ViewBag.CollegeBaseUrl = baseUrl;

            var cmsData = await _cmsRepo.GetByUrlAsync($"/colleges/{collegeUrl}.aspx");
            ViewBag.CmsData = cmsData;

            var cmsDataa = await _cmsRepo.GetListByUrlAsync($"/colleges/{baseUrl}");
            ViewBag.ShowFeeStructure = cmsDataa.Any(c => c.Url.Contains(baseUrl + "-fee-structure.aspx"));
            ViewBag.ShowMeritList = cmsDataa.Any(c => c.Url.Contains(baseUrl + "-merit-lists.aspx"));
            ViewBag.ShowAdmissions = cmsDataa.Any(c => c.Url.Contains(baseUrl + "-admission.aspx"));
            ViewBag.Levels = await _context.TblXcourseLevels.AsNoTracking().ToListAsync();
            ViewBag.Cities = await _context.TblDefCities.AsNoTracking().ToListAsync();
            ViewBag.Fields = await _context.CourseCategories.AsNoTracking().ToListAsync();

            return View("CollegeAmissions", viewModel);
        }


        [HttpGet]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> CollegeMeritLists(string collegeUrl)
        {



            // Get banners (if this is used in the view, keep it)
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            // Get the college by URL
            var collegeName = collegeUrl.Replace("-merit-lists", "");
            var college = await _context.TblColleges
                .Where(c => c.Url.Contains(collegeName))
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (college == null)
            {
                return NotFound("College not found");
            }

            // Single query to get all required data
            var query = await (
                from cl in _context.TblXcourseLevels
                orderby cl.SortOrder
                join c in _context.Courses on cl.Id equals c.EducationLevelId
                where c.CollegeId == college.Id
                join ml in _context.TblMeritLists on c.Id equals ml.CourseId
                join mlt in _context.TblMeritListTypes on ml.MeritListTypeId equals mlt.Id
                select new
                {
                    CourseLevel = cl,
                    Course = c,
                    MeritList = ml,
                    MeritListType = mlt
                })
                .ToListAsync();

            var courseLevelData = query
                .GroupBy(x => x.CourseLevel)
                .Select(g => new CourseLevelData
                {
                    CourseLevelId = g.Key.Id,
                    CourseLevelName = g.Key.Name,
                    Courses = g.GroupBy(x => x.Course)
                        .Select(cg => new CourseDetails
                        {
                            Id = cg.Key.Id,
                            Name = cg.Key.Name,
                            CourseLevelId = g.Key.Id,
                            MeritLists = cg.Select(x => new MeritListDetails
                            {
                                AddedDate = x.MeritList.AddedDate,
                                MeritListTypeName = x.MeritListType.MeritListTypeName,
                                MeritListName = x.MeritList.MeritListName,
                                FileName = x.MeritList.FileName,
                                CourseName = cg.Key.Name
                            }).ToList()
                        }).ToList()
                }).ToList();

            if (courseLevelData == null)
            {
                return NotFound();
            }
            var latestMeritLists = query
                .OrderByDescending(x => x.MeritList.AddedDate)
                .Take(10)
                .Select(x => new MeritListResult
                {
                    AddedDate = x.MeritList.AddedDate,
                    MeritListName = x.MeritList.MeritListName ?? string.Empty,
                    FileName = x.MeritList.FileName,
                    CourseName = x.Course.Name,
                    MeritListTypeName = x.MeritListType.MeritListTypeName
                })
                .ToList();

            var viewModel = new CollegesViewModel
            {
                CollegeId = college.Id,
                College = college,
                CourseLevels = courseLevelData,
                MeritList = latestMeritLists,
            };
            var baseUrl = collegeUrl
             .Replace("-fee-structure", "")
             .Replace("-admission", "")
             .Replace("-merit-lists", "");
            ViewBag.CollegeBaseUrl = baseUrl;

            var cmsData = await _cmsRepo.GetByUrlAsync($"/colleges/{collegeUrl}.aspx");
            ViewBag.CmsData = cmsData;
            var cmsDataa = await _cmsRepo.GetListByUrlAsync($"/colleges/{baseUrl}");
            // Check if there are any items in cmsDataa with matching URLs
            ViewBag.ShowFeeStructure = cmsDataa.Any(c => c.Url.Contains(baseUrl + "-fee-structure.aspx"));
            ViewBag.ShowMeritList = cmsDataa.Any(c => c.Url.Contains(baseUrl + "-merit-lists.aspx"));
            ViewBag.ShowAdmissions = cmsDataa.Any(c => c.Url.Contains(baseUrl + "-admission.aspx"));
            ViewBag.Levels = await _context.TblXcourseLevels.AsNoTracking().ToListAsync();
            ViewBag.Cities = await _context.TblDefCities.AsNoTracking().ToListAsync();
            ViewBag.Fields = await _context.CourseCategories.AsNoTracking().ToListAsync();

            return View("CollegeMeritLists", viewModel);
        }

        [Route("colleges/city-wise-colleges.aspx")]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> CityWiseColleges()
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            var cities = await _context.TblDefCities.OrderBy(c => c.CityName).ToListAsync();

            var collegeCount = await _context.TblColleges.CountAsync(c => c.TypeOfInstituteId == 5);
            var universityCount = await _context.TblColleges.CountAsync(c => c.TypeOfInstituteId == 1);
            var featuredColleges = await _context.TblColleges
                .Where(c => c.IsFeatured.HasValue && c.IsFeatured.Value)
                .ToListAsync();

            ViewBag.College = collegeCount;
            ViewBag.Universities = universityCount;
            ViewBag.Cities = cities;
            ViewBag.FeaturedC = featuredColleges;

            return View("CityWiseColleges");
        }




        [Route("/colleges/level-wise-colleges.aspx")]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> LevelWiseColleges(string? url = "")
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;
            var levelData = await _context.TblXcourseLevels
                                            .Where(l => new[] { 3, 6, 7, 8, 9 }.Contains(l.Id))
                                            .OrderBy(l => l.SortOrder)
                                            .ToListAsync();
            if (levelData == null)
            {

                return NotFound();
            }
            var cmsData = await _cmsRepo.GetByUrlAsync($"/colleges/level-wise-colleges.aspx");
            ViewBag.CmsData = cmsData;

            return View(("LevelWiseColleges"), levelData);
        }



        private async Task<CityInstitutionsViewModel> GetCollegesByCityAsync(int levelId, string cityName, int page, int pageSize = 10)
        {
            var cities = await _context.TblDefCities.ToListAsync();
            ViewBag.Cities = cities;

            var cityQuery = cities.AsQueryable()
                .Select(c => new { c.CityId, c.CityName });

            if (!string.IsNullOrEmpty(cityName) && cityName != "All Cities")
            {
                cityQuery = cityQuery.Where(c => c.CityName.ToLower() == cityName.ToLower());
            }

            var city = cityQuery.FirstOrDefault();

            // First: Get all college IDs offering this level's courses
            var collegeIdsWithLevel = await _context.Courses
                .Where(c => c.EducationLevelId == levelId && c.IsActive == true)
                .Select(c => c.CollegeId)
                .Distinct()
                .ToListAsync();

            var colleges = _context.TblColleges
                .Where(c => collegeIdsWithLevel.Contains(c.Id) && c.IsActive == true);
            colleges = colleges.OrderByDescending(c => c.Views);
            if (city != null && cityName != "All Cities")
            {
                colleges = colleges.Where(c => c.CityId == city.CityId);
            }
            ViewBag.CollegeSerialStart = (page - 1) * pageSize + 1;
            var pagedColleges = await colleges
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var pagedCollegeIds = pagedColleges.Select(c => c.Id).ToList();


            var courses = await _context.Courses
                .Where(c => pagedCollegeIds.Contains(c.CollegeId ?? 0) && c.EducationLevelId == levelId && c.IsActive == true)
                .ToListAsync();
            var collegeRatings = await GetCollegeRatingsAsync(pagedCollegeIds);
            var collegeWithCourseCount = pagedColleges.Select(college => new CollegeWithCourseCountViewModel
            {
                College = college,
                CourseCount = courses.Count(c => c.CollegeId == college.Id),
                AvgRating = collegeRatings.FirstOrDefault(r => r.CollegeId == (college.Id))?.AvgRating ?? 0
            }).ToList();

            return new CityInstitutionsViewModel
            {
                Colleges = collegeWithCourseCount,
                CityName = cityName ?? "All Cities"
            };
        }


        [Route("/colleges/college-category")]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> CategoryWiseColleges()
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;
            var categoryData = await _context.CourseCategories.OrderBy(c => c.SortOrder).ToListAsync();

            //ViewBag.CmsData = await _context.TblCms
            //                        .Where(c => c.Url.Contains("/colleges/college-category"))
            //                        .FirstOrDefaultAsync();
            var cmsData = await _cmsRepo.GetByUrlAsync($"/colleges/college-category");
            ViewBag.CmsData = cmsData;
            return View(("CategoryWiseColleges"), categoryData);
        }

        [Route("/colleges/category/{catUrl}")]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> CategoryWiseCollegesDetail(string catUrl, string cityName, int page, int pageSize = 1)
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;
            var categoryData = await _context.CourseCategories
                                         .Where(c => c.Url == catUrl)
                                         .FirstOrDefaultAsync();
            if (categoryData == null)
            {
                return NotFound();
            }
            ViewBag.CatUrl = catUrl;
            ViewBag.CatName = categoryData?.Name;

            var viewModel = await GetCollegesByCityAsyncc(categoryData.Id, cityName, page);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_CityInstitutionsTable", viewModel);
            }
            //ViewBag.CmsData = await _context.TblCms
            //    .Where(c => c.Url.Contains($"/colleges/category/{catUrl}"))
            //    .FirstOrDefaultAsync();
            var cmsData = await _cmsRepo.GetByUrlAsync($"/colleges/category/{catUrl}");
            ViewBag.CmsData = cmsData;

            return View(("CategoryWiseCollegesDetail"), viewModel);
        }


        private async Task<CityInstitutionsViewModel> GetCollegesByCityAsyncc(int catId, string cityName, int page, int pageSize = 10)
        {
            var cities = await _context.TblDefCities.ToListAsync();
            ViewBag.Cities = cities;

            var cityQuery = cities.AsQueryable()
                .Select(c => new { c.CityId, c.CityName });

            if (!string.IsNullOrEmpty(cityName) && cityName != "All Cities")
            {
                cityQuery = cityQuery.Where(c => c.CityName.ToLower() == cityName.ToLower());
            }

            var city = cityQuery.FirstOrDefault();

            // Step 1: Get course IDs under this category
            var courseIdsInCategory = await _context.CourseCategoryJoins
                .Where(j => j.CategoryId == catId)
                .Select(j => j.CourseId)
                .Distinct()
                .ToListAsync();

            // Step 2: Get college IDs from courses
            var collegeIds = await _context.Courses
                .Where(c => courseIdsInCategory.Contains(c.Id) && c.IsActive == true)
                .Select(c => c.CollegeId)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .Distinct()
                .ToListAsync();
            var colleges = await _context.TblColleges
                .Where(c => c.IsActive == true && collegeIds.Contains(c.Id))
                .ToListAsync();

            ViewBag.TotalUniversities = colleges.Count(c => c.TypeOfInstituteId == 1);
            ViewBag.TotalColleges = colleges.Count(c => c.TypeOfInstituteId == 5);
            ViewBag.IsPrivate = colleges.Count(c => c.IsGovt == "Private");
            ViewBag.IsPublic = colleges.Count(c => c.IsGovt == "Public");



            if (city != null && cityName != "All Cities")
            {
                colleges = colleges.Where(c => c.CityId == city.CityId).ToList();
            }

            // Prevent negative or zero page numbers
            if (page < 1)
                page = 1;

            var pagedColleges = colleges
                                  .OrderByDescending(c => c.Views ?? 0)
                                  .Skip((page - 1) * pageSize)
                                  .Take(pageSize)
                                  .ToList();


            var pagedCollegeIds = pagedColleges.Select(c => c.Id).ToList();

            var courses = await _context.Courses
                .Where(c => pagedCollegeIds.Contains(c.CollegeId ?? 0) && courseIdsInCategory.Contains(c.Id))
                .ToListAsync();
            var collegeRatings = await GetCollegeRatingsAsync(pagedCollegeIds);
            var collegeWithCourseCount = pagedColleges.Select(college => new CollegeWithCourseCountViewModel
            {
                College = college,
                CourseCount = courses.Count(c => c.CollegeId == college.Id),
                AvgRating = collegeRatings.FirstOrDefault(r => r.CollegeId == (college.Id))?.AvgRating ?? 0
            }).ToList();
            ViewBag.CollegeSerialStart = (page - 1) * pageSize + 1;
            return new CityInstitutionsViewModel
            {
                Colleges = collegeWithCourseCount,
                CityName = cityName ?? "All Cities"
            };
        }

        [Route("/colleges/featured-listing-of-colleges.aspx")]
        public async Task<IActionResult> FeaturedColleges(int page = 1, int pageSize = 10)
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;
            var fcolleges = await _context.TblColleges
                                 .Where(C => C.IsFeatured == true)
                                 .OrderBy(c => c.SortOrder)
                                 .Skip((page - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync();
            if (fcolleges == null)
            {
                return NotFound();
            }
            //ViewBag.CmsData = await _context.TblCms
            //                       .Where(c => c.Url.Contains("/colleges/featured-listing-of-colleges.aspx"))
            //                       .FirstOrDefaultAsync();

            var cmsData = await _cmsRepo.GetByUrlAsync($"/colleges/featured-listing-of-colleges.aspx");
            ViewBag.CmsData = cmsData;
            return View(("FeaturedColleges"), fcolleges);
        }


        [Route("submitform/colleges")]
        public async Task<IActionResult> SubmitSuggestion(SuggestionViewModel model)
        {
            string extractedSource = "Guide"; // default
            int? educationLevelId = null;
            int? categoryId = null;

            if (!string.IsNullOrEmpty(model.PageUrl))
            {
                try
                {
                    var uri = new Uri(model.PageUrl);
                    var firstSegment = uri.AbsolutePath.Trim('/').Split('/')[0].ToLower();

                    if (firstSegment == "courses")
                        extractedSource = "Courses";
                    else if (firstSegment == "colleges")
                        extractedSource = "Colleges";
                    else
                    {
                        extractedSource = "Guide";
                        var uri2 = new Uri(model.PageUrl);
                        string cleanUrl = uri2.AbsolutePath.TrimEnd('/').ToLower() + "/";



                        var guide = await _context.TblGuidesDefinations
                            .FirstOrDefaultAsync(g => g.GuideMainUrl.Contains(cleanUrl));
                        ;

                        if (guide != null)
                        {
                            educationLevelId = guide.EducationLevelId;

                            if (!string.IsNullOrEmpty(guide.CategoryIds))
                            {
                                categoryId = int.TryParse(guide.CategoryIds.Split(',')[0], out var catId) ? catId : (int?)null;
                            }
                        }
                    }
                }
                catch
                {
                    extractedSource = "Guide";
                }
            }

            if (ModelState.IsValid)
            {
                var suggestion = new DBCollege.TblCourseinquiry
                {
                    CollegeId = model.CollegeId,
                    Name = model.Name,
                    MobileNo = model.Mobile,
                    CityId = model.CityId,

                    // Agar guides page hai to table se lene, warna model se
                    EducationLevel = educationLevelId ?? model.EducationLevel,
                    CategoryId = categoryId ?? model.FieldId,

                    Url = model.PageUrl,
                    ExamDescription = model.Description,
                    Inquirydate = DateOnly.FromDateTime(DateTime.Now),
                    IsAgreed = model.AgreeToTerms,
                    CurrentDegree = model.CurrentDegree,
                    Isactive = true,
                    PhoneVerified = false,
                    InquiryType = 1,
                    Marks = "80%",
                    TestTaken = false,
                    DivisionId = null,
                    Attempts = 0,
                    DateofBirth = null,
                    CallMe = true,
                    Source = extractedSource,
                    GuideId = null,
                    Status = 1,
                    CourseName = "no value",
                    Email = "Email"
                };

                try
                {
                    _context.TblCourseinquiries.Add(suggestion);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Form submitted successfully!" });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return Json(new { success = false, message = ex.Message });
                }
            }

            return Json(new { success = false, message = "Error occurred while submitting the form." });
        }



        [Route("/colleges/{LevelUrl}-institutes-{CatUrl}")]
        public async Task<IActionResult> CategoryAndLevelWiseColleges(string LevelUrl, string CatUrl, int page, int pageSize = 1)
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;
            var cat = await _context.CourseCategories.
                Where(c => c.Url == CatUrl).FirstOrDefaultAsync();
            var level = await _context.TblXcourseLevels.
                Where(c => c.Url == LevelUrl).FirstOrDefaultAsync();
            var topCourses = await (from c in _context.Courses
                                    join ccj in _context.CourseCategoryJoins on c.Id equals ccj.CourseId
                                    join cc in _context.CourseCategories on ccj.CategoryId equals cc.Id
                                    join l in _context.TblXcourseLevels on c.EducationLevelId equals l.Id
                                    where l.Id == level.Id && cc.Id == cat.Id
                                    select new
                                    {
                                        c.Id,
                                        c.Name,
                                    })
                         .Take(12)
                         .ToListAsync();
            var topCourseList = topCourses.Select(c => new DBCollege.Course
            {
                Id = c.Id,
                Name = c.Name,
            }).ToList();
            if (topCourseList == null)
            {
                return NotFound();
            }

            var viewModel = await GetCCollegesByCityAsync(cat.Id, level.Id, page);
            viewModel.TopCourses = topCourseList;
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("partialview", viewModel);
            }

            ViewBag.CatUrl = CatUrl;
            ViewBag.LevelUrl = LevelUrl;
            ViewBag.CatName = cat?.Name;
            ViewBag.LevelName = level?.Name;
            //ViewBag.CmsData = await _context.TblCms
            //                        .Where(c => c.Url.Contains($"/colleges/{LevelUrl}-institutes-{CatUrl}"))
            //                        .FirstOrDefaultAsync();
            var cmsData = await _cmsRepo.GetByUrlAsync($"/colleges/{LevelUrl}-institutes-{CatUrl}");
            ViewBag.CmsData = cmsData;

            return View(("CategoryAndLevelWiseColleges"), viewModel);
        }


        private async Task<CityInstitutionsViewModel> GetCCollegesByCityAsync(int catId, int levelId, int page, int pageSize = 10, string cityName = "All Cities")
        {
            var cities = await _context.TblDefCities.ToListAsync();
            ViewBag.Cities = cities;

            var cityQuery = cities.AsQueryable()
                .Select(c => new { c.CityId, c.CityName });

            if (!string.IsNullOrEmpty(cityName) && cityName != "All Cities")
            {
                cityQuery = cityQuery.Where(c => c.CityName.ToLower() == cityName.ToLower());
            }

            var city = cityQuery.FirstOrDefault();

            var courseIdsInCategoryAndLevel = await _context.CourseCategoryJoins
                .Where(j => j.CategoryId == catId)
                .Join(_context.Courses,
                    j => j.CourseId,
                    c => c.Id,
                    (j, c) => new { j, c })
                .Where(joined => joined.c.EducationLevelId == levelId)
                .Select(joined => joined.j.CourseId)
                .Distinct()
                .ToListAsync();

            var collegeIds = await _context.Courses
                .Where(c => courseIdsInCategoryAndLevel.Contains(c.Id) && c.IsActive == true)
                .Select(c => c.CollegeId)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .Distinct()
                .ToListAsync();

            var colleges = await _context.TblColleges
                .Where(c => c.IsActive == true && collegeIds.Contains(c.Id))
                .OrderByDescending(c => c.Views)
                .ToListAsync();

            if (city != null && cityName != "All Cities")
            {
                colleges = colleges.Where(c => c.CityId == city.CityId).ToList();
            }


            if (page < 1)
                page = 1;

            // Apply pagination
            var pagedColleges = colleges
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var pagedCollegeIds = pagedColleges.Select(c => c.Id).ToList();

            var courses = await _context.Courses
                .Where(c => pagedCollegeIds.Contains(c.CollegeId ?? 0) && courseIdsInCategoryAndLevel.Contains(c.Id))
                .ToListAsync();
            var collegeRatings = await GetCollegeRatingsAsync(pagedCollegeIds);
            var collegeWithCourseNames = pagedColleges.Select(college => new CollegeWithCourseNamesViewModel
            {
                College = college,
                CourseNames = courses
                    .Where(c => c.CollegeId == college.Id)
                    .Select(c => c.Name)
                    .ToList(),
                AvgRating = collegeRatings.FirstOrDefault(r => r.CollegeId == (college.Id))?.AvgRating ?? 0
            }).ToList();
            ViewBag.CollegeSerialStart = (page - 1) * pageSize + 1;
            return new CityInstitutionsViewModel
            {
                CollegesWithCourseNames = collegeWithCourseNames,
                CityName = cityName ?? "All Cities"
            };
        }



        [Route("colleges/reviews/{colUrl}")]
        public async Task<IActionResult> CollegeReviews(string colUrl, int page = 1, int pageSize = 10)
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;
            var college = await _context.TblColleges
                               .Where(tc => tc.Url == colUrl.Replace("-reviews", ""))
                               .Select(tc => new { tc.Id, tc.Name })
                               .FirstOrDefaultAsync();
            var reviews = await _context.TblCollegereviews
                                 .Where(tr => tr.InstId == college.Id)
                                 .OrderByDescending(tr => tr.Id)
                                 .Select(tr => new ReviewViewModel
                                 {
                                     ReviewId = tr.Id,
                                     ReviewName = tr.Name,
                                     ReviewRating = tr.Rating,
                                     ReviewComment = tr.Comment,
                                     ReviewDate = tr.Date,
                                     ReviewerEmail = tr.Email,
                                     Afford = tr.Afford,
                                     Academics = tr.Academics,
                                     JobPlacement = tr.JobPlacement,
                                     Facilities = tr.Facilities,
                                     ReAdmission = tr.ReAdmission
                                 })
                                 .Skip((page - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync();
            if (reviews == null)
            {
                return NotFound();
            }
            var viewModel = new CollegeReviewsViewModel
            {
                Reviews = reviews,
                CollegeUrl = colUrl,
                CollegeName = college?.Name,
                CollegeId = college?.Id

            };
            ViewBag.Levels = await _context.TblXcourseLevels.AsNoTracking().ToListAsync();
            ViewBag.Cities = await _context.TblDefCities.AsNoTracking().ToListAsync();
            ViewBag.Fields = await _context.CourseCategories.AsNoTracking().ToListAsync();

            return View("CollegeReviews", viewModel);
        }



        [HttpPost]
        public async Task<IActionResult> SubmitReview([FromForm] DBCollege.TblCollegereview model)
        {
            if (ModelState.IsValid)
            {

                var review = new DBCollege.TblCollegereview
                {
                    Name = model.Name,
                    Comment = model.Comment,
                    Academics = model.Academics,
                    Facilities = model.Facilities,
                    Afford = model.Afford,
                    JobPlacement = model.JobPlacement,
                    Rating = (((model.Afford ?? 0) + (model.Academics ?? 0) + (model.JobPlacement ?? 0) + (model.Facilities ?? 0)) / 4.0).ToString("0.0"),
                    InstId = model.InstId,
                    Date = DateTime.Now
                };


                _context.TblCollegereviews.Add(review);
                await _context.SaveChangesAsync();


                return Json(new { success = true, message = "Review submitted successfully!" });
            }

            return Json(new { success = false, message = "There was an error submitting the review." });
        }


        private async Task<List<CollegeRating>> GetCollegeRatingsAsync(List<int> collegeIds)
        {
            var reviews = await _context.TblCollegereviews
                                .Where(tr => collegeIds.Contains(tr.InstId ?? 0) && !string.IsNullOrEmpty(tr.Rating))
                                .ToListAsync();

            var collegeRatings = reviews
                .GroupBy(tr => tr.InstId)
                .Select(g => new CollegeRating
                {
                    CollegeId = g.Key ?? 0,
                    AvgRating = g.Where(r => !string.IsNullOrEmpty(r.Rating) && double.TryParse(r.Rating, out var val) && val > 0)
                                 .Select(r => double.Parse(r.Rating))
                                 .DefaultIfEmpty(0)
                                 .Average()
                })
                .ToList();

            return collegeRatings;
        }

        [Route("/colleges/testing/{urlSlug}")]
        public async Task<IActionResult> GetCustomPages(string urlSlug, int page = 1, int pageSize = 10)
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            var sectionContent = await (from sc in _context.SectionContentImports
                                        join st in _context.SectionTypeImports
                                        on sc.ContentId equals st.Id
                                        where st.Url == urlSlug + ".aspx"
                                        select new
                                        {
                                            st.Id,
                                            st.Name,
                                            sc.CollegeCategoryId,
                                            sc.CityId,
                                            sc.CollegeTypeId,
                                            sc.GenderId,
                                            sc.Keywords,
                                            st.Url,
                                            sc.ContentId,
                                            sc.Detail,
                                            sc.Heading,
                                            sc.MetaTitle,
                                            sc.MetaKeyword,
                                            sc.MetaDesc,
                                        })
                                       .AsNoTracking()
                                       .FirstOrDefaultAsync();

            if (sectionContent == null)
            {
                return NotFound();
            }

            IQueryable<DBCollege.TblCollege> collegesQuery = _context.TblColleges.OrderByDescending(c => c.Views);

            bool hasCategory = sectionContent.CollegeCategoryId.HasValue && sectionContent.CollegeCategoryId.Value > 0;
            bool hasCity = sectionContent.CityId.HasValue && sectionContent.CityId.Value > 0;
            bool hasType = !string.IsNullOrEmpty(sectionContent.CollegeTypeId) && sectionContent.CollegeTypeId != "-1";
            bool hasGender = !string.IsNullOrEmpty(sectionContent.GenderId) && sectionContent.GenderId != "-1";

            if (hasCategory)
                collegesQuery = collegesQuery.Where(c => c.TypeOfInstituteId == sectionContent.CollegeCategoryId.Value);

            if (hasCity)
                collegesQuery = collegesQuery.Where(c => c.CityId == sectionContent.CityId.Value);

            if (hasType)
                collegesQuery = collegesQuery.Where(c => c.IsGovt == sectionContent.CollegeTypeId);

            if (hasGender)
                collegesQuery = collegesQuery.Where(c => c.GenderType == sectionContent.GenderId);





            var sectionKeywords = string.IsNullOrEmpty(sectionContent.Keywords)
                ? new List<string>()
                : sectionContent.Keywords.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(k => k.Trim().ToLower())
                    .ToList();


            if (sectionKeywords.Any())
            {
                collegesQuery = collegesQuery.Where(college =>
                        _context.Courses.Any(course =>
                        course.CollegeId == college.Id &&
                        course.Name != null &&
                        sectionKeywords.Any(kw => course.Name.ToLower().Contains(kw))
                    )
                );
            }

            var totalCount = await collegesQuery.CountAsync();
            var collegesList = await collegesQuery.Select(c => new CustomPagesCollegeViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Address = c.Address,
                ContactNumber = c.ContactNumber,
                Sector = c.IsGovt,
                PopularityScore = c.Qsranking ?? 0,
                LogoUrl = c.Logo,
                Url = c.Url
            })
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            ViewBag.TotalCount = totalCount;
            ViewBag.CollegeSerialStart = (page - 1) * pageSize + 1;
            ViewBag.UrlSlug = urlSlug;

            var cmsData = await _cmsRepo.GetByUrlAsync(
                $"https://www.ilmkidunya.com/colleges/{urlSlug}.aspx"
            );

            if (cmsData == null)
            {
                cmsData = new TblCmsDto
                {
                    Desc1 = sectionContent.Detail,
                    MetaDesc = sectionContent.MetaDesc,
                    Heading = sectionContent.Heading,
                    MetaKeys = sectionContent.MetaKeyword,
                    MetaTitle = sectionContent.MetaTitle
                };
            }

            ViewBag.CmsData = cmsData;

            return View(("GetCustomPages"), collegesList);
        }



        [HttpGet]
        [Route("colleges/filter/{cityUrl}")]
        public async Task<IActionResult> GetFilteredInstitutionsByCity(
    string cityUrl,
    string filterType = null,       // "government", "category", "level"
    string filterValue = null,      // Category URL (e.g., "engineering") or Level URL (e.g., "mphil")
    int page = 1,
    int pageSize = 10)
        {
            try
            {
                var banners = await _bannerService.GetBannersAsync();
                ViewBag.Banners = banners;

                // Find the city by URL or name
                var city = await _context.TblDefCities
                    .Where(c => c.Url == cityUrl || c.CityName.ToLower() == cityUrl.ToLower())
                    .FirstOrDefaultAsync();

                if (city == null)
                {
                    return NotFound("City not found");
                }

                // Start with base query for active institutions in the specified city
                IQueryable<DBCollege.TblCollege> institutionsQuery = _context.TblColleges
                    .Where(c => c.IsActive == true && c.CityId == city.CityId)
                    .OrderByDescending(c => c.Views);

                string filterDescription = "";

                // Apply filters based on filterType
                switch (filterType?.ToLower())
                {
                    case "government":
                        institutionsQuery = institutionsQuery.Where(c => c.IsGovt == "Public");
                        filterDescription = "Government";
                        break;

                    case "private":
                        institutionsQuery = institutionsQuery.Where(c => c.IsGovt == "Private");
                        filterDescription = "Private";
                        break;

                    case "category":
                        if (!string.IsNullOrEmpty(filterValue))
                        {
                            var category = await _context.CourseCategories
                                .Where(cc => cc.Url == filterValue)
                                .FirstOrDefaultAsync();

                            if (category != null)
                            {
                                // Get course IDs in this category
                                var courseIdsInCategory = await _context.CourseCategoryJoins
                                    .Where(j => j.CategoryId == category.Id)
                                    .Select(j => j.CourseId)
                                    .Distinct()
                                    .ToListAsync();

                                // Get college IDs that offer these courses
                                var collegeIdsFromCategory = await _context.Courses
                                    .Where(c => courseIdsInCategory.Contains(c.Id) && c.CollegeId.HasValue && c.IsActive == true)
                                    .Select(c => c.CollegeId.Value)
                                    .Distinct()
                                    .ToListAsync();

                                institutionsQuery = institutionsQuery.Where(c => collegeIdsFromCategory.Contains(c.Id));
                                filterDescription = category.Name;
                            }
                        }
                        break;

                    case "level":
                        if (!string.IsNullOrEmpty(filterValue))
                        {
                            // Handle special cases for level URLs
                            string levelUrl = filterValue.ToLower();
                            if (levelUrl == "phd") levelUrl = "ph-d";
                            else if (levelUrl == "mphil") levelUrl = "m-phil";

                            var level = await _context.TblXcourseLevels
                                .Where(l => l.Url == levelUrl)
                                .FirstOrDefaultAsync();

                            if (level != null)
                            {
                                // Get college IDs that offer courses at this level
                                var collegeIdsWithLevel = await _context.Courses
                                    .Where(c => c.EducationLevelId == level.Id && c.CollegeId.HasValue && c.IsActive == true)
                                    .Select(c => c.CollegeId.Value)
                                    .Distinct()
                                    .ToListAsync();

                                institutionsQuery = institutionsQuery.Where(c => collegeIdsWithLevel.Contains(c.Id));
                                filterDescription = level.Name;
                            }
                        }
                        break;

                    case "combined": // For category + level combination (like "engineering" + "bachelor")
                        if (!string.IsNullOrEmpty(filterValue))
                        {
                            var filterParts = filterValue.Split('-');
                            if (filterParts.Length == 2)
                            {
                                string categoryUrl = filterParts[0];
                                string levelUrl = filterParts[1];

                                // Handle special cases
                                if (levelUrl == "phd") levelUrl = "ph-d";
                                else if (levelUrl == "mphil") levelUrl = "m-phil";

                                var category = await _context.CourseCategories
                                    .Where(cc => cc.Url == categoryUrl)
                                    .FirstOrDefaultAsync();

                                var level = await _context.TblXcourseLevels
                                    .Where(l => l.Url == levelUrl)
                                    .FirstOrDefaultAsync();

                                if (category != null && level != null)
                                {
                                    // Get course IDs that match both category and level
                                    var courseIdsInCategoryAndLevel = await _context.CourseCategoryJoins
                                        .Where(j => j.CategoryId == category.Id)
                                        .Join(_context.Courses,
                                            j => j.CourseId,
                                            c => c.Id,
                                            (j, c) => new { j, c })
                                        .Where(joined => joined.c.EducationLevelId == level.Id && joined.c.IsActive == true)
                                        .Select(joined => joined.j.CourseId)
                                        .Distinct()
                                        .ToListAsync();

                                    var collegeIds = await _context.Courses
                                        .Where(c => courseIdsInCategoryAndLevel.Contains(c.Id) && c.CollegeId.HasValue)
                                        .Select(c => c.CollegeId.Value)
                                        .Distinct()
                                        .ToListAsync();

                                    institutionsQuery = institutionsQuery.Where(c => collegeIds.Contains(c.Id));
                                    filterDescription = $"{category.Name} - {level.Name}";
                                }
                            }
                        }
                        break;

                    default:
                        // No filter applied - show all institutions
                        filterDescription = "All";
                        break;
                }

                // Get filtered institutions
                var filteredInstitutions = await institutionsQuery.ToListAsync();

                // Separate colleges and universities
                var colleges = filteredInstitutions.Where(c => c.TypeOfInstituteId == 5).ToList();
                var universities = filteredInstitutions.Where(c => c.TypeOfInstituteId == 1).ToList();

                // Get total counts
                int totalColleges = colleges.Count;
                int totalUniversities = universities.Count;

                // Apply pagination
                var pagedColleges = colleges
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var pagedUniversities = universities
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Get IDs for course counting and ratings
                var pagedCollegeIds = pagedColleges.Select(c => c.Id).ToList();
                var pagedUniversityIds = pagedUniversities.Select(u => u.Id).ToList();
                var allPagedIds = pagedCollegeIds.Concat(pagedUniversityIds).ToList();

                // Get course counts
                var courseCounts = await _context.Courses
                    .Where(c => c.IsActive == true && c.CollegeId.HasValue && allPagedIds.Contains(c.CollegeId.Value))
                    .GroupBy(c => c.CollegeId)
                    .Select(g => new { CollegeId = g.Key.Value, Count = g.Count() })
                    .ToListAsync();

                // Get ratings
                var collegeRatings = await GetCollegeRatingsAsync(allPagedIds);

                // Map colleges with details
                var collegesWithDetails = pagedColleges.Select(college => new CollegeWithCourseCountViewModel
                {
                    College = college,
                    CourseCount = courseCounts.FirstOrDefault(x => x.CollegeId == college.Id)?.Count ?? 0,
                    AvgRating = collegeRatings.FirstOrDefault(r => r.CollegeId == (college.Id))?.AvgRating ?? 0
                }).ToList();

                // Map universities with details
                var universitiesWithDetails = pagedUniversities.Select(university => new CollegeWithCourseCountViewModel
                {
                    College = university,
                    CourseCount = courseCounts.FirstOrDefault(x => x.CollegeId == university.Id)?.Count ?? 0,
                    AvgRating = collegeRatings.FirstOrDefault(r => r.CollegeId == (university.Id))?.AvgRating ?? 0
                }).ToList();

                // Create city info
                var cityInfo = new
                {
                    Name = city.CityName,
                    TotalColleges = totalColleges,
                    TotalUniversities = totalUniversities,
                    TotalInstitutions = totalColleges + totalUniversities,
                    FilterType = filterType ?? "all",
                    FilterValue = filterValue ?? "all",
                    FilterDescription = filterDescription,
                    CityImage = city.ImageName,
                };

                // Create view model
                var viewModel = new CityInstitutionsViewModel
                {
                    CityInfo = cityInfo,
                    Colleges = collegesWithDetails,
                    Universities = universitiesWithDetails,
                    CityName = city.CityName
                };

                // Set ViewBag properties
                ViewBag.CityUrl = cityUrl;
                ViewBag.CityName = city.CityName;
                ViewBag.FilterType = filterType;
                ViewBag.FilterValue = filterValue;
                ViewBag.FilterDescription = filterDescription;
                ViewBag.TotalColleges = totalColleges;
                ViewBag.TotalUniversities = totalUniversities;
                ViewBag.CollegeSerialStart = (page - 1) * pageSize + 1;
                ViewBag.UniversitySerialStart = (page - 1) * pageSize + 1;

                // Additional ViewBag data
                ViewBag.Levels = await _context.TblXcourseLevels.AsNoTracking().ToListAsync();
                ViewBag.Cities = await _context.TblDefCities.AsNoTracking().ToListAsync();
                ViewBag.Fields = await _context.CourseCategories.AsNoTracking().ToListAsync();

                // CMS data
                //var cmsData = await _context.TblCms
                //    .Where(c => c.Url.Contains($"/colleges/filter/{cityUrl}") ||
                //                c.Url.Contains($"{filterType}-{filterValue}-{cityUrl}") ||
                //                c.Url.Contains($"https://www.ilmkidunya.com/colleges/{filterValue.ToLower().Replace(" ", "-")}-colleges-in-{cityUrl}"))

                var cmsData = await _cmsRepo.GetListByUrlAsync($"/colleges/filter/{cityUrl}");

                if (cmsData == null)
                {
                    cmsData = await _cmsRepo.GetListByUrlAsync($"{filterType}-{filterValue}-{cityUrl}");

                    if (cmsData == null)
                    {
                        cmsData = await _cmsRepo.GetListByUrlAsync($"https://www.ilmkidunya.com/colleges/{filterValue.ToLower().Replace(" ", "-")}-colleges-in-{cityUrl}");
                    }
                }


                if (cmsData == null)
                {
                    var CmsData = new Models.TblCm
                    {
                        PageName = "Colleges Information",
                        Url = $"/colleges/filter/{cityUrl}",
                        Heading = $"{filterValue} Colleges in {cityUrl}",
                        Desc1 = $"Explore the list of {filterValue} colleges located in {cityUrl}.",
                        Desc2 = "Find admission details, courses, and other information.",
                        Desc3 = "Updated content to help you select the best college.",
                        MetaTitle = $"{filterValue} Colleges in {cityUrl}",
                        MetaDesc = $"Find all {filterValue} colleges located in {cityUrl} with admission details.",
                        MetaKeys = $"{filterValue}, Colleges, {cityUrl}, Admissions",
                        Image = "/images/default-college.jpg",
                        Date = DateTime.Now
                    };
                    ViewBag.CmsData = CmsData;
                }
                else
                {
                    ViewBag.CmsData = cmsData;
                }



                // Handle AJAX requests
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("_FilteredInstitutionsTable", viewModel);
                }

                return View("LevelWiseCollegesDetails", viewModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving institutions.");
            }
        }




        // Helper method to create specific route methods for common filters
        [HttpGet]
        [Route("colleges/government-institutions/{cityUrl}")]
        public async Task<IActionResult> GetGovernmentInstitutions(string cityUrl, int page = 1, int pageSize = 10)
        {
            return await GetFilteredInstitutionsByCity(cityUrl, "government", null, page, pageSize);
        }


        [Route("colleges/private-institutions/{cityUrl}")]
        public async Task<IActionResult> GetPrivateInstitutions(string cityUrl, int page = 1, int pageSize = 10)
        {
            return await GetFilteredInstitutionsByCity(cityUrl, "private", null, page, pageSize);
        }


        [Route("colleges/engineering-colleges/{cityUrl}")]
        public async Task<IActionResult> GetEngineeringColleges(string cityUrl, int page = 1, int pageSize = 10)
        {
            return await GetFilteredInstitutionsByCity(cityUrl, "category", "engineering", page, pageSize);
        }

        [Route("colleges/{collegeUrl}/{courseUrl}.aspx")]
        public async Task<IActionResult> CollegeCourseDetail(string collegeUrl, string courseUrl)
        {
            try
            {
                ViewBag.Banners = await _bannerService.GetBannersAsync();

                string normalizedCourseName = courseUrl.Replace("-", " ").ToLower();

                // Get College
                var college = await _context.TblColleges
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Url == collegeUrl && c.IsActive == true);

                if (college == null)
                    return NotFound("College not found");

                // Get Course for this college
                var course = await _context.Courses
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.IsActive == true
                                            && c.CollegeId == college.Id
                                            && c.Name.ToLower().Contains(normalizedCourseName));

                if (course == null)
                    return NotFound("Course not found");

                // Get all course-related College IDs
                var courseCollegeIds = await _context.Courses
                    .Where(c => c.IsActive == true && c.Name.ToLower().Contains(normalizedCourseName))
                    .Select(c => c.CollegeId)
                    .ToListAsync();

                // Get related colleges
                var relatedColleges = await _context.TblColleges
                    .Where(c => c.IsActive == true && courseCollegeIds.Contains(c.Id) && c.Id != college.Id)
                    .Select(c => new RelatedColleges
                    {
                        College = c,
                        Courses = _context.Courses
                            .Where(course => course.CollegeId == c.Id && course.IsActive == true)
                            .Select(course => new CourseInfo
                            {
                                Name = course.Name ?? "N/A",
                                TotalFee = course.TotalFee ?? "N/A",
                                Duration = course.Duration ?? "N/A"
                            }).ToList()
                    }).ToListAsync();

                // Get Admission Info
                var admissionCourseIds = await _context.TblAdmissionCourses
                    .Where(ac => ac.CourseId == course.Id)
                    .Select(ac => ac.NoticeId)
                    .ToListAsync();

                var admission = await _context.TblAdmissions
                    .Where(a => admissionCourseIds.Contains(a.Id))
                    .OrderByDescending(a => a.Dated)
                    .FirstOrDefaultAsync();

                // Merit Lists
                var meritLists = await (
                    from ml in _context.TblMeritLists
                    join mlt in _context.TblMeritListTypes on ml.MeritListTypeId equals mlt.Id
                    join c in _context.Courses on ml.CourseId equals c.Id
                    where ml.CourseId == course.Id && ml.CollegeId == college.Id
                    orderby ml.AddedDate descending
                    select new MeritListViewModel
                    {
                        CourseName = c.Name,
                        MeritListName = ml.MeritListName,
                        MeritListType = mlt.MeritListTypeName,
                        AddedDate = ml.AddedDate,
                        FileName = ml.FileName
                    }
                ).Take(5).ToListAsync();

                // College Reviews & Ratings
                var collegeReviews = await _context.TblCollegereviews
                    .Where(r => r.InstId == college.Id)
                    .ToListAsync();

                var (overallRating, reviewScore) = CalculateOverallRanking(
                    collegeReviews, college.GoogleRanking, college.Fbranking);

                // View Model
                var viewModel = new CollegeCourseDetailViewModel
                {
                    College = new CollegeBasicInfo
                    {
                        Id = college.Id,
                        Name = college.Name,
                        Url = college.Url,
                        Logo = college.Logo,
                        Address = college.Address,
                        ContactNumber = college.ContactNumber,
                        Website = college.Website,
                        Email = college.Email,
                        OverallRating = overallRating,
                        ReviewScore = reviewScore,
                        IsGovt = college.IsGovt
                    },
                    Course = new CourseDetailInfo
                    {
                        Id = course.Id,
                        Name = course.Name,
                        Duration = course.Duration,
                        Fee = course.Fee,
                        TotalFees = course.TotalFee
                    },
                    AdmissionInfo = admission,
                    AdmissionLogo = GetAdmissionLogoPathThumb(admission?.NoticeImageLarge, admission?.Dated),
                    MeritLists = meritLists,
                    RelatedColleges = relatedColleges
                };

                // ViewBag Data
                var baseUrl = $"{collegeUrl}/{courseUrl}"
                    .Replace("-fee-structure", "")
                    .Replace("-admission", "")
                    .Replace("-merit-lists", "");

                ViewBag.CollegeBaseUrl = baseUrl;

                //var cmsData = await _context.TblCms
                //    .Where(c => c.Url.Contains(baseUrl))
                //    .ToListAsync();

                var cmsData = await _cmsRepo.GetListByUrlAsync($"/colleges/{baseUrl}");

                ViewBag.ShowFeeStructure = cmsData.Any(c => c.Url.Contains(baseUrl + "-fee-structure.aspx"));
                ViewBag.ShowMeritList = cmsData.Any(c => c.Url.Contains(baseUrl + "-merit-lists.aspx"));
                ViewBag.ShowAdmissions = cmsData.Any(c => c.Url.Contains(baseUrl + "-admission.aspx"));

                ViewBag.Levels = await _context.TblXcourseLevels.AsNoTracking().ToListAsync();
                ViewBag.Cities = await _context.TblDefCities.AsNoTracking().ToListAsync();
                ViewBag.Fields = await _context.CourseCategories.AsNoTracking().ToListAsync();
                if (cmsData == null || !cmsData.Any())
                {
                    cmsData = new List<TblCmsDto>
                    {
                        new TblCmsDto
                        {
                            PageName = college.Name ?? "N/A",
                            Heading = $"Details for {course.Name ?? "N/A"} at {college.Name ?? "N/A"}",
                            MetaTitle = $"{college.Name ?? "N/A"} - {course.Name ?? "N/A"}",
                            MetaDesc = $"Learn about {course.Name ?? "N/A"} at {college.Name ?? "N/A"}.",
                            MetaKeys = $"{college.Name}, {course.Name}, education, admissions"
                        }
                    };
                }

                ViewBag.CmsData = cmsData.FirstOrDefault();
                return View("CollegeCourseDetail", viewModel);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving course details.");
            }
        }




        //[HttpGet]
        //[Route("colleges/{collegeUrl}/{courseUrl}.aspx")]
        //public async Task<IActionResult> CollegeCourseDetail(string collegeUrl, string courseUrl)
        //{
        //    try
        //    {
        //        ViewBag.Banners = await _bannerService.GetBannersAsync();

        //        string normalizedCourseName = courseUrl.Replace("-", " ").ToLower();

        //        using var conn = _context.Database.GetDbConnection();
        //        await conn.OpenAsync();

        //        using var cmd = conn.CreateCommand();
        //        cmd.CommandText = "dbo.sp_GetCollegeCourseDetail";
        //        cmd.CommandType = System.Data.CommandType.StoredProcedure;

        //        var paramCollege = cmd.CreateParameter();
        //        paramCollege.ParameterName = "@CollegeUrl";
        //        paramCollege.Value = collegeUrl;
        //        cmd.Parameters.Add(paramCollege);

        //        var paramCourse = cmd.CreateParameter();
        //        paramCourse.ParameterName = "@CourseName";
        //        paramCourse.Value = normalizedCourseName;
        //        cmd.Parameters.Add(paramCourse);

        //        using var reader = await cmd.ExecuteReaderAsync();

        //        // 1️⃣ College Info
        //        CollegeBasicInfo college = null;
        //        if (await reader.ReadAsync())
        //        {
        //            college = new CollegeBasicInfo
        //            {
        //                Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //                Name = reader.GetString(reader.GetOrdinal("Name")),
        //                Url = reader.GetString(reader.GetOrdinal("Url")),
        //                Logo = reader["Logo"] as string,
        //                Address = reader["Address"] as string,
        //                ContactNumber = reader["ContactNumber"] as string,
        //                Website = reader["Website"] as string,
        //                Email = reader["Email"] as string,
        //                IsGovt = reader.GetString(reader.GetOrdinal("IsGovt")),
        //                Fbranking = reader["Fbranking"] as int?,
        //                GoogleRanking = reader["GoogleRanking"] as int?
        //            };
        //        }

        //        if (!await reader.NextResultAsync())
        //            return NotFound("Course data not found.");

        //        // 2️⃣ Course Info
        //        CourseDetailInfo course = null;
        //        if (await reader.ReadAsync())
        //        {
        //            course = new CourseDetailInfo
        //            {
        //                Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //                Name = reader.GetString(reader.GetOrdinal("Name")),
        //                Duration = reader["Duration"] as string,
        //                Fee = reader["Fee"] as string,
        //                TotalFees = reader["TotalFee"] as string
        //            };
        //        }

        //        if (!await reader.NextResultAsync())
        //            return NotFound("Related colleges data not found.");

        //        // 3️⃣ Related Colleges
        //        var relatedColleges = new List<RelatedColleges>();
        //        while (await reader.ReadAsync())
        //        {
        //            var relCollegeId = reader.GetInt32(reader.GetOrdinal("CollegeId"));
        //            var existing = relatedColleges.FirstOrDefault(r => r.College.Id == relCollegeId);
        //            if (existing == null)
        //            {
        //                existing = new RelatedColleges
        //                {
        //                    College = new TblCollege
        //                    {
        //                        Id = relCollegeId,
        //                        Name = reader.GetString(reader.GetOrdinal("CollegeName"))
        //                    },
        //                    Courses = new List<CourseInfo>()
        //                };

        //                relatedColleges.Add(existing);
        //            }

        //            existing.Courses.Add(new CourseInfo
        //            {
        //                Name = reader["CourseName"] as string ?? "N/A",
        //                TotalFee = reader["TotalFee"] as string ?? "N/A",
        //                Duration = reader["Duration"] as string ?? "N/A"
        //            });
        //        }

        //        if (!await reader.NextResultAsync())
        //            return NotFound("Admission data not found.");

        //        // 4️⃣ Admission Info
        //        TblAdmission admission = null;
        //        if (await reader.ReadAsync())
        //        {
        //            admission = new TblAdmission
        //            {
        //                Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //                AdmissionTitle = reader["AdmissionTitle"] as string,
        //                CollegeId = reader.GetInt32(reader.GetOrdinal("CollegeId")),
        //                Details = reader["Details"] as string,
        //                Dated = reader["Dated"] as DateTime?,
        //                NoticeImageLarge = reader["NoticeImageLarge"] as string,
        //                NoticeImageThumb = reader["NoticeImageThumb"] as string,
        //                LastDate = reader["LastDate"] as DateTime?,
        //                Url = reader["Url"] as string
        //            };
        //        }

        //        if (!await reader.NextResultAsync())
        //            return NotFound("Merit list data not found.");

        //        // 5️⃣ Merit Lists
        //        var meritLists = new List<MeritListViewModel>();
        //        while (await reader.ReadAsync())
        //        {
        //            meritLists.Add(new MeritListViewModel
        //            {
        //                CourseName = reader["CourseName"] as string,
        //                MeritListName = reader["MeritListName"] as string,
        //                MeritListType = reader["MeritListType"] as string,
        //                AddedDate = reader["AddedDate"] as DateTime?,
        //                FileName = reader["FileName"] as string
        //            });
        //        }

        //        if (!await reader.NextResultAsync())
        //            return NotFound("College reviews not found.");

        //        // 6️⃣ College Reviews
        //        var collegeReviews = new List<TblCollegereview>();
        //        while (await reader.ReadAsync())
        //        {
        //            collegeReviews.Add(new TblCollegereview
        //            {
        //                Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //                InstId = reader.GetInt32(reader.GetOrdinal("InstId")),
        //                Name = reader["Name"] as string,
        //                Email = reader["Email"] as string,
        //                Rating = reader["Rating"] as string,
        //                Comment = reader["Comment"] as string,
        //                Date = reader["Date"] as DateTime?
        //            });
        //        }

        //        // Calculate ratings
        //        var (overallRating, reviewScore) = CalculateOverallRanking(collegeReviews, college.GoogleRanking, college.Fbranking);
        //        college.OverallRating = overallRating;
        //        college.ReviewScore = reviewScore;

        //        // Build ViewModel
        //        var viewModel = new CollegeCourseDetailViewModel
        //        {
        //            College = college,
        //            Course = course,
        //            AdmissionInfo = admission,
        //            AdmissionLogo = GetAdmissionLogoPath(admission?.NoticeImageLarge, admission?.Dated),
        //            MeritLists = meritLists,
        //            RelatedColleges = relatedColleges
        //        };

        //        // ViewBag and CMS
        //        var baseUrl = $"{collegeUrl}/{courseUrl}"
        //            .Replace("-fee-structure", "")
        //            .Replace("-admission", "")
        //            .Replace("-merit-lists", "");

        //        ViewBag.CollegeBaseUrl = baseUrl;

        //        var cmsData = await _cmsRepo.GetListByUrlAsync($"/colleges/{baseUrl}");
        //        ViewBag.ShowFeeStructure = cmsData.Any(c => c.Url.Contains(baseUrl + "-fee-structure.aspx"));
        //        ViewBag.ShowMeritList = cmsData.Any(c => c.Url.Contains(baseUrl + "-merit-lists.aspx"));
        //        ViewBag.ShowAdmissions = cmsData.Any(c => c.Url.Contains(baseUrl + "-admission.aspx"));

        //        ViewBag.Levels = await _context.TblXcourseLevels.AsNoTracking().ToListAsync();
        //        ViewBag.Cities = await _context.TblDefCities.AsNoTracking().ToListAsync();
        //        ViewBag.Fields = await _context.CourseCategories.AsNoTracking().ToListAsync();

        //        ViewBag.CmsData = (cmsData == null || !cmsData.Any())
        //            ? new TblCmsDto
        //            {
        //                PageName = college.Name ?? "N/A",
        //                Heading = $"Details for {course.Name ?? "N/A"} at {college.Name ?? "N/A"}",
        //                MetaTitle = $"{college.Name ?? "N/A"} - {course.Name ?? "N/A"}",
        //                MetaDesc = $"Learn about {course.Name ?? "N/A"} at {college.Name ?? "N/A"}.",
        //                MetaKeys = $"{college.Name}, {course.Name}, education, admissions"
        //            }
        //            : cmsData.FirstOrDefault();

        //        return View("CollegeCourseDetail", viewModel);
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500, "An error occurred while retrieving course details.");
        //    }
        //}


        [Route("/{college}/colleges-universities-in-{cityName:alpha}")]
        [Route("colleges/colleges-in-{cityName:regex(^[[a-zA-Z]]+$)}")]
        [Route("colleges/colleges-in-{cityName:regex(^[[a-zA-Z\\-]]+$)}.aspx")]
        public async Task<IActionResult> CityWiseCollegesDetail(string cityName, int page = 1)
        {
            //cityName = cityName.Replace("colleges-in-", "").Replace(".aspx", "");
            const int pageSize = 10;
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            var city = await _context.TblDefCities
                .Where(c => c.Url == cityName)
                .FirstOrDefaultAsync();

            if (city == null)
            {
                return NotFound("City Not Found");
                //return RedirectToAction("CollegeHome", cityName);
            }

            // All active colleges/universities in city without pagination (for counts)
            var allInstitutes = await _context.TblColleges
                .Where(c => c.IsActive == true && c.CityId == city.CityId)
                .OrderByDescending(c => c.Views)
                .ToListAsync();

            // Totals for all active institutes in city
            int totalColleges = allInstitutes.Count(c => c.TypeOfInstituteId == 5);
            int totalUniversities = allInstitutes.Count(c => c.TypeOfInstituteId == 1);


            var allInstituteIds = allInstitutes.Select(i => i.Id).ToList();
            var allCourses = await _context.Courses
                .Where(c => c.CollegeId != null)
                .Join(_context.TblColleges
                    .Where(col => col.CityId == city.CityId),
                    cr => cr.CollegeId,
                    cl => cl.Id,
                    (cr, cl) => cr)
                .ToListAsync();



            var courseCountsByInstitute = allCourses
                .GroupBy(c => c.CollegeId)
                .ToDictionary(g => g.Key.Value, g => g.Count());

            var collegeRatings = await GetCollegeRatingsAsync(allInstituteIds);

            // Pagination applied only on colleges & universities separately
            var pagedColleges = allInstitutes
                .Where(c => c.TypeOfInstituteId == 5)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var pagedUniversities = allInstitutes
                .Where(c => c.TypeOfInstituteId == 1)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Map paginated colleges with course count and rating
            var collegeWithCourseCount = pagedColleges.Select(college => new CollegeWithCourseCountViewModel
            {
                College = college,
                CourseCount = courseCountsByInstitute.ContainsKey(college.Id) ? courseCountsByInstitute[college.Id] : 0,
                AvgRating = collegeRatings.FirstOrDefault(r => r.CollegeId == (college.Id))?.AvgRating ?? 0
            }).ToList();

            // Map paginated universities with course count and rating
            var universityWithCourseCount = pagedUniversities.Select(university => new CollegeWithCourseCountViewModel
            {
                College = university,
                CourseCount = courseCountsByInstitute.ContainsKey(university.Id) ? courseCountsByInstitute[university.Id] : 0,
                AvgRating = collegeRatings.FirstOrDefault(r => r.CollegeId == (university.Id))?.AvgRating ?? 0
            }).ToList();

            var cityInfo = new
            {
                TotalUniversities = totalUniversities,
                TotalColleges = totalColleges,
                GovernmentInstitutions = allInstitutes.Count(c => c.IsGovt == "Public"),
                PrivateInstitutions = allInstitutes.Count(c => c.IsGovt == "Private"),
                InstitutionsForIntermediate = allCourses.Count(c => c.EducationLevelId == 3),
                InstitutionsForBachelor = allCourses.Count(c => c.EducationLevelId == 6),
                InstitutionsForMasters = allCourses.Count(c => c.EducationLevelId == 7),
                InstitutionsForDS = allCourses.Count(c => c.EducationLevelId == 13),
                Name = city.CityName,
                CityImage = city.ImageName,
            };

            //ViewBag.CmsData = await _context.TblCms
            //    .Where(c => c.Url.Contains($"/colleges/colleges-in-{cityName}.aspx"))
            //    .FirstOrDefaultAsync();
            var cmsData = await _cmsRepo.GetByUrlAsync($"/colleges/colleges-in-{cityName}.aspx");
            ViewBag.CmsData = cmsData;

            ViewBag.UniversitySerialStart = (page - 1) * pageSize + 1;
            ViewBag.CollegeSerialStart = (page - 1) * pageSize + 1;


            var viewModel = new CityInstitutionsViewModel
            {
                CityInfo = cityInfo,
                Colleges = collegeWithCourseCount,
                Universities = universityWithCourseCount,
            };

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return View("CityWiseCollegesDetail", viewModel);
            }

            return View(viewModel);
        }



        private string GetAdmissionLogoPath(string fileName, DateTime? dated)
        {
            if (string.IsNullOrEmpty(fileName) || dated == null)
                return "/images/no-image.png"; // fallback image

            var year = dated.Value.Year;
            var month = dated.Value.Month.ToString(); // e.g. 03 for March

            return $"{year}/{month}/thumb/{fileName}";
        }
		private string GetAdmissionLogoPathThumb(string fileName, DateTime? dated)
		{
			if (string.IsNullOrEmpty(fileName) || dated == null)
				return "/images/no-image.png"; // fallback image

			var year = dated.Value.Year;
			var month = dated.Value.Month.ToString(); // e.g. 03 for March

			return $"{year}/{month}/large/{fileName}";
		}

		[Route("colleges/{levelUrl:regex(^[[a-zA-Z0-9\\-]]+$)}")]
        public async Task<IActionResult> LevelWiseCollegesDetails(string levelUrl, string cityName = "All Cities", int page = 1, string viewType = "desktop")
        {


            ViewBag.Banners = await _bannerService.GetBannersAsync();
            // Normalize special cases
            if (levelUrl.ToLower() == "phd")
                levelUrl = "ph-d";
            else if (levelUrl.ToLower() == "mphil")
                levelUrl = "m-phil";
            var level = await _context.TblXcourseLevels
                .Where(l => l.Url == levelUrl)
                .FirstOrDefaultAsync();

            if (level == null)
            {
                var redirectSlug = levelUrl;

                return await GetCustomPages(redirectSlug);
            }

            ViewBag.LevelUrl = levelUrl;
            ViewBag.LevelName = level.Name;

            var viewModel = await GetCollegesByCityAsync(level.Id, cityName, page);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                if (viewType == "mobile")
                {
                    return PartialView("_MobileInstitutionsList", viewModel);
                }
                else
                {
                    return PartialView("_CityInstitutionsTable", viewModel);
                }
            }
            //ViewBag.CmsData = await _context.TblCms
            //                        .Where(c => c.Url == $"https://www.ilmkidunya.com/colleges/{levelUrl.Replace("-", "")}")
            //                        .FirstOrDefaultAsync();
            var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/colleges/{levelUrl.Replace("-", "")}");
            ViewBag.CmsData = cmsData;

            return View(viewModel);
        }

        //[HttpGet]
        //[Route("colleges/ielts-preparation/{cityUrl}")]
        //public async Task<IActionResult> GetIELTSInstitutions(string cityUrl, int page = 1, int pageSize = 10)
        //{
        //    return await GetFilteredInstitutionsByCity(cityUrl, "category", "ielts", page, pageSize);
        //}

        //[HttpGet]
        //[Route("colleges/engineering-bachelor/{cityUrl}")]
        //public async Task<IActionResult> GetEngineeringBachelorInstitutions(string cityUrl, int page = 1, int pageSize = 10)
        //{
        //    return await GetFilteredInstitutionsByCity(cityUrl, "combined", "engineering-bachelor", page, pageSize);
        //}

        //[HttpGet("colleges/{university}/{program}.aspx")]

    }
}
