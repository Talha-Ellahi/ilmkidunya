using IKDFrontEnd.Models;
using IKDFrontEnd.Services;
using IKDFrontEnd.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace IKDFrontEnd.Controllers
{
    public class HomeController : Controller
    {
        private readonly DbikdContext _context;
        private readonly BannerService _bannerService;
        private readonly ILogger<HomeController> _logger;
        private readonly CmsRepository _cmsRepo;

        private readonly IMemoryCache _memoryCache;

        public HomeController(
            DbikdContext context,
            BannerService bannerService,
            ILogger<HomeController> logger,
            CmsRepository cmsRepo,
            IMemoryCache memoryCache)   // 👈 change here
        {
            _context = context;
            _bannerService = bannerService;
            _logger = logger;
            _cmsRepo = cmsRepo;
            _memoryCache = memoryCache; // 👈 assign
        }



        [OutputCache(Duration = 60)] // Optional – can keep or remove
        public async Task<IActionResult> Index()
        {
            string cacheKey = "home_page_data";

            var banneres = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banneres;

            // Try to get from memory cache
            if (_memoryCache.TryGetValue(cacheKey, out HomePageViewModel3 cachedModel))
            {
                ViewBag.HideHeaderLowerBanner = true;
                return View("Index", cachedModel);
            }

            try
            {
                // 2️⃣ If not cached → run DB logic (your existing code)
                var model = new HomePageViewModel3();
                using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "dbikduser.GetHomePageDataNew3";
                        command.CommandType = CommandType.StoredProcedure;

                        var stopwatch = new Stopwatch();
                        stopwatch.Start();
                        using (var reader = await command.ExecuteReaderAsync())
                        {


                            // 1. Sliders
                            model.Sliders = new List<SliderViewModel>();
                            while (await reader.ReadAsync())
                            {
                                var url = reader["Url"]?.ToString();

                                bool isBanner = (reader["Isbanned"] != DBNull.Value && Convert.ToBoolean(reader["Isbanned"]));

                                model.Sliders.Add(new SliderViewModel
                                {
                                    Url = !string.IsNullOrEmpty(url) && isBanner
                                            ? Convert.ToBase64String(Encoding.UTF8.GetBytes(url))
                                            : url,
                                    Image = reader["Image"].ToString()
                                });
                            }

                            // 2. Latest News
                            await reader.NextResultAsync();
                            model.LatestNews = new List<NewsViewModel>();
                            while (await reader.ReadAsync())
                            {
                                model.LatestNews.Add(new NewsViewModel
                                {
                                    RewriteUrl = reader["Rewrite_Url"].ToString(),
                                    PictureThumbnail = reader["Picture_Thumbnail"].ToString(),
                                    MainHeading = reader["Main_Heading"].ToString(),
                                    Dated = reader["Dated"] != DBNull.Value
                                                ? Convert.ToDateTime(reader["Dated"])
                                                : (DateTime?)null
                                });
                            }

                            // 2.1 News Slider (ShowInSlider = 1)
                            await reader.NextResultAsync();
                            model.SliderNews = new List<NewsViewModel>();
                            while (await reader.ReadAsync())
                            {
                                model.SliderNews.Add(new NewsViewModel
                                {
                                    RewriteUrl = reader["Rewrite_Url"].ToString(),
                                    Picture_1 = reader["Picture_1"].ToString(),
                                    Dated = reader["Dated"] != DBNull.Value
                                                        ? Convert.ToDateTime(reader["Dated"])
                                                        : (DateTime?)null
                                });
                            }

                            // 3. Articles
                            await reader.NextResultAsync();
                            model.Articles = new List<ArticleViewModel>();
                            while (await reader.ReadAsync())
                            {
                                model.Articles.Add(new ArticleViewModel
                                {
                                    RewriteUrl = reader["Rewrite_Url"].ToString(),
                                    PictureThumbnail = reader["Picture_Thumbnail"].ToString(),
                                    Title = reader["Title"].ToString(),
                                    Dated = reader["Dated"] != DBNull.Value
                                                ? Convert.ToDateTime(reader["Dated"])
                                                : (DateTime?)null
                                });
                            }

                            // 4. Admissions
                            await reader.NextResultAsync();
                            model.Admissions = new List<AdmissionViewModel>();
                            while (await reader.ReadAsync())
                            {
                                model.Admissions.Add(new AdmissionViewModel
                                {
                                    Title = reader["Title"].ToString(),
                                    Url = reader["Url"].ToString(),
                                    Image = reader["Image"].ToString(),
                                    AdmissionOpenDate = reader["AdmissionOpenDate"] != DBNull.Value
                                                            ? Convert.ToDateTime(reader["AdmissionOpenDate"])
                                                            : (DateTime?)null,
                                    AppliedDate = reader["AppliedDate"] != DBNull.Value
                                                            ? Convert.ToDateTime(reader["AppliedDate"])
                                                            : (DateTime?)null
                                });
                            }

                            // 5. Cities
                            await reader.NextResultAsync();
                            var cities = new List<TblDefCity>();
                            while (await reader.ReadAsync())
                            {
                                cities.Add(new TblDefCity
                                {
                                    CityId = (int)reader["City_Id"],
                                    CityName = reader["City_Name"].ToString()
                                });
                            }

                            // 6. Categories
                            await reader.NextResultAsync();
                            var categories = new List<CourseCategory>();
                            while (await reader.ReadAsync())
                            {
                                categories.Add(new CourseCategory
                                {
                                    Id = (int)reader["Id"],
                                    Name = reader["Name"].ToString()
                                });
                            }

                            // 7. Levels
                            await reader.NextResultAsync();
                            var levels = new List<TblXcourseLevel>();
                            while (await reader.ReadAsync())
                            {
                                levels.Add(new TblXcourseLevel
                                {
                                    Id = (int)reader["Id"],
                                    Name = reader["Name"].ToString(),
                                    SortOrder = (int)reader["SortOrder"]
                                });
                            }

                            model.Courses = new CoursesViewModels
                            {
                                Cities = cities,
                                Categories = categories,
                                Levels = levels
                            };

                            // 8. WebStorySliders
                            await reader.NextResultAsync();
                            model.WebStorySliders = new List<SliderHome>();
                            while (await reader.ReadAsync())
                            {
                                model.WebStorySliders.Add(new SliderHome
                                {
                                    ID = (int)reader["Id"],
                                    Slidertitle = reader["Slidertitle"].ToString(),
                                    Categoryname = reader["Categoryname"].ToString(),
                                    Image = reader["Image"].ToString(),
                                    Date = reader["Date"] != DBNull.Value
                                                ? Convert.ToDateTime(reader["Date"])
                                                : (DateTime?)null
                                });
                            }

                            // 9. Featured Colleges
                            await reader.NextResultAsync();
                            model.FeaturedColleges = new List<FeaturedCollegeViewModel>();
                            while (await reader.ReadAsync())
                            {
                                model.FeaturedColleges.Add(new FeaturedCollegeViewModel
                                {
                                    Name = reader["Name"].ToString(),
                                    Url = reader["Url"].ToString(),
                                    Logo = reader["Logo"].ToString(),
                                    SortOrder = (int)reader["Sort_Order"]
                                });
                            }

                            // 10. Jobs
                            await reader.NextResultAsync();
                            model.Jobs = new List<GroupedJobAdViewModel>();
                            while (await reader.ReadAsync())
                            {
                                model.Jobs.Add(new GroupedJobAdViewModel
                                {
                                    Dated = reader["Dated"] != DBNull.Value
                                            ? Convert.ToDateTime(reader["Dated"])
                                            : DateTime.MinValue,

                                    LastDate = reader["LastDate"] != DBNull.Value
                                                ? Convert.ToDateTime(reader["LastDate"])
                                                : (DateTime?)null,
                                    JobCounts = new List<int> { Convert.ToInt32(reader["JobCount"]) },
                                    DetailUrl = reader["DetailUrl"].ToString(),
                                    CompanyName = reader["CompanyName"].ToString(),
                                    CompanyImage = reader["CompanyImage"].ToString()
                                });
                            }

                            // 11. Banners
                            await reader.NextResultAsync();
                            var banners = new List<Banner>();
                            while (await reader.ReadAsync())
                            {
                                var url = reader["Url"]?.ToString();
                                var advertiser = reader["Advertiser"]?.ToString();

                                banners.Add(new Banner
                                {
                                    Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                                    Url = !string.IsNullOrEmpty(url) && !string.Equals(advertiser, "no", StringComparison.OrdinalIgnoreCase)
                                            ? Convert.ToBase64String(Encoding.UTF8.GetBytes(url))
                                            : url,  // ? agar Advertiser "no" hai to original URL hi rahega
                                    Image = reader["Image"]?.ToString(),
                                    Advertiser = advertiser,
                                    SortOrder = reader["SortOrder"] != DBNull.Value ? Convert.ToInt32(reader["SortOrder"]) : 0
                                });
                            }

                            // 12 Home Links
                            var homeLinks = _context.SectionTypeImports
                                .Where(s => s.SectionId == 195)
                                .Take(4)
                                .ToList();

                            model.HomeLinks = homeLinks;

                            ViewBag.Banners = banners;
                            stopwatch.Stop();
                            var dbTime = stopwatch.ElapsedMilliseconds;
                            ViewBag.DbTime = dbTime;

                        }
                    }
                }

                var cacheOptions = new MemoryCacheEntryOptions()
                          .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _memoryCache.Set(cacheKey, model, cacheOptions);

                ViewBag.HideHeaderLowerBanner = true;
                return View("Index", model);
            }
            catch (Exception ex)
            {
                return Content("ERROR:\n" + ex.Message + "\n\nSTACK:\n" + ex.StackTrace);
            }
        }

        [HttpGet]
        public async Task<IActionResult> LoadCategoryGuides()
        {
            var categories = await GetCategoryCoursesAsync();
            return PartialView("_CategoryGuidesPartial", categories);
        }

        [Route("home/testing-page")]
        public async Task<IActionResult> testingpage()
        {
            ViewBag.Banners = await _bannerService.GetBannersAsync();
            var categories = await GetCategoryCoursesAsync();
            return PartialView("testingpage", categories);
        }

        private async Task<List<CategoryCoursesViewModel>> GetCategoryCoursesAsync()
        {
            // Step 1: Get latest 10 categories based on guide activity
            var categoryIdsRaw = await _context.TblGuidesDefinations
                .OrderByDescending(g => g.Id)
                .Select(g => g.CategoryIds)
                .ToListAsync();

            // Step 2: Convert comma-separated CategoryIds to distinct int list
            var latestCategoryIds = categoryIdsRaw
                .AsEnumerable()
                .SelectMany(ids => (ids ?? "")
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(idStr => int.TryParse(idStr.Trim(), out var parsed) ? parsed : 0))
                .Where(id => id > 0)
                .Distinct()
                .Take(10)
                .ToList();

            // Step 3: Fetch categories
            var latestCategories = await _context.CourseCategories
                .Where(c => latestCategoryIds.Contains(c.Id))
                .ToListAsync();

            // Step 4: Get active levels ordered by SortOrder
            var levels = await _context.TblXcourseLevels
                .Where(l => l.IsActive == true)
                .OrderBy(l => l.SortOrder)
                .ToListAsync();

            // Step 5: Build level order list with numeric awareness
            var activeLevelIds = levels
                .OrderBy(l =>
                {
                    // detect numeric part in level name
                    var digits = new string((l.Name ?? "").Where(char.IsDigit).ToArray());
                    if (int.TryParse(digits, out int num))
                        return num; // 9, 10, 11 etc.
                    return int.MaxValue;
                })
                .ThenBy(l => l.SortOrder ?? int.MaxValue)
                .Select(l => l.Id)
                .ToList();

            var result = new List<CategoryCoursesViewModel>();

            foreach (var category in latestCategories)
            {
                // get all guides for this category
                var guidesData = await _context.TblGuidesDefinations
                    .Where(g =>
                        activeLevelIds.Contains(g.EducationLevelId) &&
                        ("," + g.CategoryIds + ",").Contains("," + category.Id + ","))
                    .Select(g => new
                    {
                        g.EducationLevelId,
                        g.Abrevation,
                        g.GuideMainUrl,
                        g.Tags // 👈 sometimes contains '9th', '10th' etc.
                    })
                    .ToListAsync();

                // Now sort guides properly
                var orderedGuides = guidesData
                    .OrderBy(g =>
                    {
                        // 1️⃣ first by level SortOrder / numeric
                        var levelIndex = activeLevelIds.IndexOf(g.EducationLevelId);

                        // 2️⃣ then by numeric inside same level (if tags or name contain 9th, 10th etc.)
                        var digits = new string((g.Abrevation ?? g.Tags ?? "").Where(char.IsDigit).ToArray());
                        int numericOrder = 9999;
                        if (int.TryParse(digits, out int n))
                            numericOrder = n;

                        return (levelIndex * 1000) + numericOrder; // combined key
                    })
                    .Select(g => new GuideViewModel
                    {
                        GuideName = g.Abrevation,
                        GuideMainUrl = g.GuideMainUrl
                    })
                    .Take(40)
                    .ToList();

                result.Add(new CategoryCoursesViewModel
                {
                    CategoryName = category.Name ?? "Unnamed",
                    Guides = orderedGuides
                });
            }


            return result;
        }





        public async Task<IActionResult> HomeBanner(int page = 1)
        {
            const int pageSize = 50;

            ViewBag.Banners = await _bannerService.GetBannersAsync();

            var newsList = await _context.TblMainNews
                .AsNoTracking()
                .Where(n => n.Approve == true)
                .OrderByDescending(n => n.Dated)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(n => new TblMainNews
                {
                    NewsId = n.NewsId,
                    MainHeading = n.MainHeading,
                    RewriteUrl = n.RewriteUrl,
                    Dated = n.Dated,
                    PictureThumbnail = string.IsNullOrEmpty(n.PictureThumbnail)
                        ? "https://images.ishallwin.com/news/default-thumb.webp"
                        : $"https://cdn.ilmkidunya.com/news/newsImages/{n.Dated.Value.Year}/{n.Dated.Value.Month}/thumb/{n.PictureThumbnail}"
                })
                .ToListAsync();

            var newsModel = new NewsPageViewModel
            {
                LatestNews = newsList,
                MetaTitle = "Latest News - IKD",
                MetaDescription = "Get the latest educational news, updates, and announcements.",
                MetaKeywords = "news, education, results, announcements",
                MetaImage = "https://images.ishallwin.com/news/default-news-image.jpg",

            };


            return View("HomeBanner", newsModel);
        }
        public async Task<IActionResult> Homesearchcourses()
        {
            var citiesWithCourses = await (
                  from ci in _context.TblDefCities
                  join co in _context.TblColleges on ci.CityId equals co.CityId
                  join c in _context.Courses on co.Id equals c.CollegeId
                  where c.IsActive == true
                  select ci
              ).Distinct().ToListAsync();

            var categories = await _context.CourseCategories
                .OrderBy(c => c.Name)
                .ToListAsync();

            var levels = await _context.TblXcourseLevels
                .OrderBy(l => l.SortOrder)
                .ToListAsync();

            var courseSearch = new CoursesViewModels
            {
                Cities = citiesWithCourses,
                Categories = categories,
                Levels = levels
            };

            var model = new HomePageViewModel3
            {
                Courses = courseSearch
            };

            return PartialView("_Homesearchcourses", model);
        }





        [HttpGet("Home/NavbarPartial")]
        public IActionResult NavbarPartial()
        {
            return PartialView("_LoginNavbar");
        }




        [HttpGet("contactus")]
        public async Task<IActionResult> ContactUs()
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;
            ViewBag.CmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/contactus/");

            return View();
        }

        [HttpPost]
        public IActionResult SubmitContact(string name, string email, string No, string subject, string message)
        {
            var contact = new TblContactMessage
            {
                Name = name,
                Email = string.IsNullOrWhiteSpace(email) ? " " : email,
                Subject = subject,
                Message = message,
                PhoneNo = No,
                CreatedAt = DateTime.Now
            };

            _context.TblContactMessages.Add(contact);
            _context.SaveChanges();

            return Json(new { success = true, message = "Your message has been sent successfully!" });
        }




        [HttpGet("privacy-policy")]
        public async Task<IActionResult> Privacypolicy()
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            return View();
        }



        [HttpGet("data-deletion")]
        public async Task<IActionResult> Datadeletion()
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            return View();
        }

        [HttpGet("privacy")]
        public async Task<IActionResult> Privacy()
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;
            ViewBag.CmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/privacy");

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost]
        [Route("savepage-feedback")]
        public IActionResult SaveFeedback([FromBody] TblPageFeedback model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Comments))
            {
                return BadRequest("Feedback cannot be empty.");
            }


            model.Date = DateTime.Now;

            _context.TblPageFeedbacks.Add(model);
            _context.SaveChanges();

            return Ok(new { success = true, message = "Feedback saved successfully" });
        }
    }


}
