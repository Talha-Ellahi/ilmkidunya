using HtmlAgilityPack;
using IKDFrontEnd.Interfaces;
using IKDFrontEnd.Models;
using IKDFrontEnd.Services;
using IKDFrontEnd.ViewModels;
using IKDFrontEnd.ViewModels.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Globalization;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Net.WebRequestMethods;
using System.Globalization;

namespace IKDFrontEnd.Controllers
{
    [Route("jobs")] // Base route prefix: /tutors

    public class JobsController : Controller
    {
        private readonly DbikdContext _context;
        private readonly BannerService _bannerService;
        private readonly CmsRepository _cmsRepo;
        private readonly ITezMateService _tezMateService;
        private readonly IConfiguration _config;
        private readonly IServiceProvider _serviceProvider;

        public JobsController(DbikdContext context, BannerService bannerService, CmsRepository cmsRepo, ITezMateService tezMateService, IConfiguration config, IServiceProvider serviceProvider)
        {
            _context = context;
            _bannerService = bannerService;
            _cmsRepo = cmsRepo;
            _tezMateService = tezMateService;
            _config = config;
            _serviceProvider = serviceProvider;
        }



        [HttpGet("")]
        public async Task<IActionResult> Home()
        {
            // === Banners ===
            ViewBag.Banners = await _bannerService.GetBannersAsync();

            // === CMS Data ===
            var cmsData = await _cmsRepo.GetByUrlAsync("https://www.ilmkidunya.com/jobs");
            ViewBag.CmsData = cmsData;

            // === Get all job data in single query ===
            var jobData = await GetJobDataAsync();

            // === Cities With Images ===
            var citiesWithImages = await GetCitiesWithImagesAsync();

            // === Combine Into ViewModel ===
            var model = new JobListingPageViewModel
            {
                GroupedJobAds = jobData.GroupedAds,
                TopCompanies = jobData.TopCompanies,
                CitiesWithImages = citiesWithImages
            };

            return View(model);
        }

        private async Task<(List<GroupedJobAdViewModel> GroupedAds, List<CompanyJobViewModel> TopCompanies)> GetJobDataAsync()
        {
            // Using explicit join since there's no navigation property
            var result = await (
                from job in _context.Tbljobadslatests
                join company in _context.Companies on job.CompanyId equals company.Id
                where job.IsActive && job.CompanyId != null
                orderby job.Dated descending
                select new
                {
                    job.Id,
                    job.Name,
                    job.CompanyId,
                    job.Dated,
                    job.NoofJobs,
                    job.LastDate,
                    job.Url,
                    CompanyName = company.Name,
                    CompanyUrl = company.Url,
                    CompanyLogo = company.Logo
                })
                .Take(50)
                .ToListAsync();

            if (!result.Any())
                return (new List<GroupedJobAdViewModel>(), new List<CompanyJobViewModel>());

            // === Process grouped job ads ===
            var grouped = result
                .GroupBy(j => new { j.CompanyId, j.Dated })
                .Select(g =>
                {
                    var first = g.First();
                    var items = g.ToList();

                    return new GroupedJobAdViewModel
                    {
                        Dated = g.Key.Dated,
                        CompanyName = first.CompanyName ?? "Unknown",
                        CompanyUrl = first.CompanyUrl ?? "#",
                        JobTitles = items.Select(x => x.Name ?? "Untitled").ToList(),
                        JobCounts = items.Where(x => x.NoofJobs.HasValue && x.NoofJobs > 0)
                                        .Select(x => x.NoofJobs.Value)
                                        .ToList(),
                        JobAdIds = items.Select(x => x.Id).ToList(),
                        LastDate = items.Max(x => x.LastDate),
                        DetailUrl = items.FirstOrDefault(x => !string.IsNullOrEmpty(x.Url))?.Url
                    };
                })
                .OrderByDescending(g => g.Dated)
                .Take(10)
                .ToList();

            // === Process top companies ===
            var topCompanies = result
                .GroupBy(j => j.CompanyId)
                .Select(g =>
                {
                    var first = g.OrderByDescending(x => x.Dated).First();
                    return new CompanyJobViewModel
                    {
                        CompanyName = first.CompanyName ?? "Unknown",
                        LogoUrl = string.IsNullOrEmpty(first.CompanyLogo)
                            ? "https://images.ishallwin.com/jobs/company-logo.webp"
                            : $"https://cdn.ilmkidunya.com/images/CompanyLogo/{first.CompanyLogo}",
                        CompanyUrl = !string.IsNullOrEmpty(first.CompanyUrl) ? first.CompanyUrl : "#"
                    };
                })
                .Take(10)
                .ToList();

            return (grouped, topCompanies);
        }

        private async Task<List<CityViewModel>> GetCitiesWithImagesAsync()
        {
            return await _context.TblDefCities
                .AsNoTracking()
                .Where(c => c.IsImageAvailable == true && c.ImageName != null)
                .OrderBy(c => c.SortOrder)
                .Select(c => new CityViewModel
                {
                    CityName = c.CityName ?? "Unknown",
                    ImageUrl = $"https://images.ishallwin.com/jobs/{c.ImageName.Replace(".png", ".webp")}",
                    Url = !string.IsNullOrEmpty(c.Url) ? c.Url : "#"
                })
                .Take(10)
                .ToListAsync();
        }





        [Route("city-wise")]
        public async Task<IActionResult> CityWise()
        {

            ViewBag.Banners = await _bannerService.GetBannersAsync();
            //var cmsData = await _context.TblCms
            //   .Where(c => c.Url == "https://www.ilmkidunya.com/jobs/city-wise")
            //   .FirstOrDefaultAsync();
            var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/jobs/city-wise");
            ViewBag.CmsData = cmsData;

            // Step 1: Fetch all active jobs with non-null city IDs
            var jobCityStrings = await _context.Tbljobadslatests
                .Where(j => j.IsActive && j.JobAdsCitiesIds != null)
                .Select(j => j.JobAdsCitiesIds)
                .ToListAsync();

            // Step 2: Split and parse city IDs to int
            var cityIdList = jobCityStrings
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .SelectMany(s => s.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(idStr => int.TryParse(idStr, out var id) ? (int?)id : null)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .Distinct()
                .ToList();

            // Step 3: Fetch city names and generate URLs
            var cityList = await _context.TblDefCities
               .Where(c => cityIdList.Contains(c.CityId) && c.IsActive == true)
               .OrderBy(c => c.CityName)
               .Select(c => new CityJobViewModel
               {
                   CityId = c.CityId,
                   CityName = c.CityName ?? "Unknown",
                   Url = "/jobs-in-" + (c.CityName ?? "unknown").ToLower().Replace(" ", "-")
               })
               .ToListAsync(); // ✅ stays async


            return View(cityList);
        }





        [Route("industry/govt-service")]
        public async Task<IActionResult> GovtDepartments()
        {

            ViewBag.Banners = await _bannerService.GetBannersAsync();
            //var cmsData = await _context.TblCms
            //   .Where(c => c.Url == "https://www.ilmkidunya.com/jobs/industry/govt-service")
            //   .FirstOrDefaultAsync();
            var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/jobs/industry/govt-service");
            ViewBag.CmsData = cmsData;

            var today = DateTime.Today;

            // Step 1: Get all active jobs that are not expired
            //&& j.LastDate >= today
            var activeJobCompanyIds = await _context.Tbljobadslatests

                .Where(j => j.IsActive && j.CompanyId != null)

                .Select(j => j.CompanyId.Value)

                .Distinct()

                .ToListAsync();

            // Step 2: Get active, govt companies that have at least one valid job

            var companies = await _context.Companies

                .Where(c => activeJobCompanyIds.Contains(c.Id) && c.IsActive == true && c.IsGovt == true)

                .Select(c => new CompanyListItemViewModel

                {

                    Id = c.Id,

                    CompanyName = c.Name,

                    CompanyUrl = c.Url,

                })

                .OrderBy(c => c.CompanyName)

                .ToListAsync();

            return View(companies);
        }




        [Route("company-wise-jobs")]
        public async Task<IActionResult> CompanyWise()
        {

            ViewBag.Banners = await _bannerService.GetBannersAsync();
            //var cmsData = await _context.TblCms
            //   .Where(c => c.Url == "https://www.ilmkidunya.com/jobs/company-wise-jobs")
            //   .FirstOrDefaultAsync();
            var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/jobs/company-wise-jobs");
            ViewBag.CmsData = cmsData;

            var today = DateTime.Today;

            // Step 1: Get all active jobs that are not expired

            var activeJobCompanyIds = await _context.Tbljobadslatests

                .Where(j => j.IsActive && j.CompanyId != null)

                .Select(j => j.CompanyId.Value)

                .Distinct()

                .ToListAsync();

            // Step 2: Get active, non-govt companies that have at least one valid job

            var companies = await _context.Companies

                .Where(c => activeJobCompanyIds.Contains(c.Id) && c.IsActive == true && !c.IsGovt == true)

                .Select(c => new CompanyListItemViewModel

                {

                    Id = c.Id,

                    CompanyName = c.Name,

                    CompanyUrl = c.Url

                })

                .OrderBy(c => c.CompanyName)

                .ToListAsync();

            return View(companies);


        }


        [Route("industry")]
        public async Task<IActionResult> Industry()
        {
            ViewBag.Banners = await _bannerService.GetBannersAsync();
            //var cmsData = await _context.TblCms
            //   .Where(c => c.Url == "https://www.ilmkidunya.com/jobs/industry")
            //   .FirstOrDefaultAsync();
            var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/jobs/industry");
            ViewBag.CmsData = cmsData;

            var industries = await _context.JobTypes
                .Where(j => j.IsActive == true && j.Id != 45 && j.Id != 63)
                .OrderBy(j => j.SortOrder)
                .Select(j => new IndustryViewModel
                {
                    Url = j.Url,
                    Name = j.Name,
                    ImageName = j.ImageName ?? "default.webp" // Fallback if image missing
                })
                .ToListAsync();

            return View(industries);
        }


        [Route("professions")]
        public async Task<IActionResult> Profession()
        {
            ViewBag.Banners = await _bannerService.GetBannersAsync();
            //var cmsData = await _context.TblCms
            //   .Where(c => c.Url == "https://www.ilmkidunya.com/jobs/professions")
            //   .FirstOrDefaultAsync();
            var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/jobs/professions");
            ViewBag.CmsData = cmsData;

            var professionGroups = await _context.JobTypes
                .Where(jt => jt.IsActive == true && jt.Id != 8 && jt.Id != 45)
                .OrderBy(jt => jt.SortOrder)
                .Select(jt => new ProfessionGroupViewModel
                {
                    JobTypeName = jt.Name,
                    SubCategories = _context.SubJobCategories
                        .Where(sjc => sjc.JobTypeId == jt.Id && sjc.IsActive == true)
                        .OrderBy(sjc => sjc.SortOrder)
                        .Select(sjc => sjc.Name)
                        .ToList()
                })
                .ToListAsync();

            return View(professionGroups);
        }


        //[Route("{CompanyName}-jobdetail-{id:long}")]
        //[Route("jobs-in-{rest}-{id:long}")]
        //[Route("{jobTitle}-jobs-in-{CompanyName}-{id:long}")]
        [Route("{jobTitle}-jobs-in-{CompanyName}-{id:long}.aspx")]
        [Route("{slug}-{id:long}")]
        public async Task<IActionResult> JobDetail(long id)
        {
            // === Banners ===
            ViewBag.Banners = await _bannerService.GetBannersAsync();



            // === Main Job Fetch ===
            var mainJob = await _context.Tbljobadslatests
                .AsNoTracking()
                .FirstOrDefaultAsync(j => j.Id == id);

            JobAd mainJobAd = null;

            if (mainJob == null)
            {
                mainJobAd = await _context.JobAds
                    .AsNoTracking()
                    .FirstOrDefaultAsync(j => j.Id == id);
            }

            if (mainJob == null && mainJobAd == null)
            {
                // Return safe placeholder instead of NotFound
                return Content("N/A");
            }

            // Pick the source object (either from Tbljobadslatests or JobAds)
            var jobSource = (object)mainJob ?? mainJobAd;

            // Extract fields (works for both types if they have the same property names)
            var companyId = (mainJob != null ? mainJob.CompanyId : mainJobAd.CompanyId);
            var dated = (mainJob != null ? mainJob.Dated : mainJobAd.Dated);
            // Extract Year and Month from Dated
            string year = dated?.Year.ToString() ?? "0000";
            string month = dated?.Month.ToString("D2") ?? "00";

            var company = await _context.Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == companyId);

            var jobType = company?.IsGovt == true ? "Government" : "Private";
            var jobStatus = (mainJob != null ? mainJob.JobTimeStatus : mainJobAd.JobTimeStatus) == 1 ? "Full Time" : "Part Time";

            int.TryParse((mainJob != null ? mainJob.JobAdsCitiesIds : mainJobAd.JobAdsCitiesIds)
                ?.Split(',')
                .FirstOrDefault(), out int cityId);

            var city = await _context.TblDefCities
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CityId == cityId);

            // === Same Company Jobs on Same Date ===
            var sameCompanyJobs = await _context.Tbljobadslatests
                .AsNoTracking()
                .Where(j => j.CompanyId == companyId && j.Dated == dated)
                .ToListAsync();

            // === Prepare JobDetailViewModel ===

            var qualificationId = mainJob?.QualificationId ?? mainJobAd?.QualificationId ?? 0;
            var jobScaleId = mainJob?.JobScaleId ?? mainJobAd?.JobScaleId ?? 0;

            // Preload qualification & job-scale names used by sameCompanyJobs and main job
            var qualificationIds = sameCompanyJobs
                .Where(j => j.QualificationId.HasValue)
                .Select(j => j.QualificationId!.Value)
                .Concat(new[] { qualificationId })
                .Where(id => id > 0)
                .Distinct()
                .ToList();

            var jobScaleIds = sameCompanyJobs
                .Where(j => j.JobScaleId.HasValue)
                .Select(j => j.JobScaleId!.Value)
                .Concat(new[] { jobScaleId })
                .Where(id => id > 0)
                .Distinct()
                .ToList();

            var qualificationsMap = qualificationIds.Any()
                ? await _context.Qualifications.Where(q => qualificationIds.Contains(q.Id)).AsNoTracking().ToDictionaryAsync(q => q.Id, q => q.Name)
                : new Dictionary<int, string>();

            var jobScalesMap = jobScaleIds.Any()
                ? await _context.TblJobScales.Where(s => jobScaleIds.Contains(s.Id)).AsNoTracking().ToDictionaryAsync(s => s.Id, s => s.Name)
                : new Dictionary<int, string>();

            var positions = sameCompanyJobs.Select(j =>
            {
                var qName = j.QualificationId.HasValue && qualificationsMap.TryGetValue(j.QualificationId.Value, out var qn) ? qn : string.Empty;
                var jsName = j.JobScaleId.HasValue && jobScalesMap.TryGetValue(j.JobScaleId.Value, out var jsn) ? jsn : string.Empty;

                return new JobPositionViewModel
                {
                    Title = j.Name ?? string.Empty,
                    NoOfJobs = j.NoofJobs,
                    Qualification = qName,
                    JobScale = jsName,
                    AgeLimit = j.AgeLimit,
                    IsMale = j.IsMale == 1,
                    Experience = j.Experience
                };
            }).ToList();

            // Related jobs (safe company access)
            var relatedJobs = new List<RelatedJobViewModel>();
            if (companyId != null)
            {
                relatedJobs = await _context.Tbljobadslatests
                    .AsNoTracking()
                    .Where(r => r.CompanyId == companyId && r.Id != id)
                    .OrderByDescending(r => r.Dated)
                    .Take(5)
                    .Select(r => new RelatedJobViewModel
                    {
                        PostId = r.Id,
                        PostedDate = r.Dated,
                        PostName = r.Name,
                        NoOfJobs = r.NoofJobs,
                        LastDate = r.LastDate,
                        CompanyName = company != null ? (company.Name != null ? company.Name : "") : "",
                        CompanyUrl = company != null ? (company.Url != null ? company.Url : "") : "",
                    })
                    .ToListAsync();
            }

            var job = new JobDetailViewModel
            {
                PostName = mainJob?.Name ?? mainJobAd?.Name ?? string.Empty,
                NoOfJobs = mainJob?.NoofJobs ?? mainJobAd?.NoofJobs,
                QualificationName = qualificationId > 0
                    ? (qualificationsMap.TryGetValue(qualificationId, out var qn) ? qn : await _context.Qualifications.Where(q => q.Id == qualificationId).Select(q => q.Name).FirstOrDefaultAsync() ?? string.Empty)
                    : string.Empty,
                Detail = mainJob?.Detail ?? mainJobAd?.Detail ?? string.Empty,
                JobScaleName = jobScaleId > 0
                    ? (jobScalesMap.TryGetValue(jobScaleId, out var jsn) ? jsn : await _context.TblJobScales.Where(s => s.Id == jobScaleId).Select(s => s.Name).FirstOrDefaultAsync() ?? string.Empty)
                    : string.Empty,
                IsMale = ((mainJob?.IsMale ?? mainJobAd?.IsMale) == 1),
                AgeLimit = mainJob?.AgeLimit ?? mainJobAd?.AgeLimit,
                Experience = mainJob?.Experience ?? mainJobAd?.Experience,
                LastDate = mainJob?.LastDate ?? mainJobAd?.LastDate,
                CompanyName = company?.Name,
                CompanyUrl = company?.Url,
                Dated = dated,
                ImageName = !string.IsNullOrEmpty(mainJob?.ImageName)
                    ? $"https://jobs.ilmkidunya.com/jobs/Images/{year}/{month}/Large/" + mainJob.ImageName
                    : (!string.IsNullOrEmpty(mainJobAd?.ImageName) ? $"https://jobs.ilmkidunya.com/jobs/Images/{year}/{month}/Large/" + mainJobAd.ImageName : null),
                JobPositions = positions,
                RelatedJobs = relatedJobs
            };

            // === CMS Meta Data ===
            var jobTitles = string.Join(", ", job.JobPositions.Select(p => p.Title));
            var totalJobs = job.JobPositions.Sum(p => p.NoOfJobs ?? 0);

            var cmsData = new TblCm
            {
                MetaTitle = !string.IsNullOrWhiteSpace(mainJob != null ? mainJob.MetaTitle : mainJobAd.MetaTitle)
                ? (mainJob != null ? mainJob.MetaTitle : mainJobAd.MetaTitle)
                : $"{job.CompanyName} Jobs 2026 {jobTitles} Vacancies",

                MetaDesc = !string.IsNullOrEmpty(mainJob?.MetaDesc)
                            ? mainJob.MetaDesc
                            : (!string.IsNullOrEmpty(mainJobAd?.MetaDesc)
                                ? mainJobAd.MetaDesc
                                : $"{job.CompanyName} has announced {job.NoOfJobs} {jobTitles} Jobs 2026 for {(job.IsMale ?? true ? "Male" : "Female")} candidates. Apply now and check eligibility, qualification details, and application process."),


                MetaKeys = mainJob != null ? mainJob.MetaKeyword : mainJobAd.MetaKeyword,
                Heading = $"Latest Jobs in {job.CompanyName} 2026",
                Desc1 = mainJob != null ? mainJob.Detail : mainJobAd.Detail,
                Desc2 = $@"<ul>
              <li><span>Total Vacancies:</span> {totalJobs}</li>
              <li><span>Job Ad Date:</span> {dated:dd-MMM-yyyy}</li>
              <li><span>Last Date to Apply:</span> {job.LastDate:dd-MMM-yyyy}</li>
              <li><span>Job Type:</span> {jobType}</li>
              <li><span>Location:</span> {city?.CityName}</li>
              <li><span>Job Status:</span> {jobStatus}</li>
              </ul>"
            };

            ViewBag.CmsData = cmsData;
            ViewBag.CompanyId = companyId;

            return View(job);
        }


        [Route("load-more-jobs")]
        public async Task<IActionResult> LoadMoreJobs(
            int skip = 0,
            int take = 20,
            int? cityId = null,
            string? profession = null,
            int? companyId = null)
        {
            var query = _context.Tbljobadslatests
                .Where(j => j.IsActive && j.CompanyId != null);

            if (cityId.HasValue)
            {
                query = query.Where(j => j.JobAdsCitiesIds != null &&
                                         j.JobAdsCitiesIds.Contains(cityId.Value.ToString()));
            }

            if (companyId.HasValue)
            {
                query = query.Where(j => j.CompanyId == companyId.Value);
            }

            // ✅ Filter by profession (same logic as JobsBySubCategory)
            if (!string.IsNullOrEmpty(profession))
            {
                var allSubCats = _context.SubJobCategories.AsNoTracking().ToList();
                var matchSlug = Slugify(profession);
                var subCat = allSubCats
                    .FirstOrDefault(s => Slugify(s.Name) == matchSlug);

                if (subCat != null)
                {
                    int targetJobTypeId = subCat.JobTypeId;

                    query = query.Where(j => j.JobAdsTypesIds != null &&
                        (
                            j.JobAdsTypesIds == targetJobTypeId.ToString() ||
                            j.JobAdsTypesIds.StartsWith(targetJobTypeId + "{#-!-#}") ||
                            j.JobAdsTypesIds.EndsWith("{#-!-#}" + targetJobTypeId) ||
                            j.JobAdsTypesIds.Contains("{#-!-#}" + targetJobTypeId + "{#-!-#}")
                        ));

                    // refine with title similarity (same as JobsBySubCategory)
                    var ads = query.Take(500).AsNoTracking().ToList();
                    query = ads
                        .Where(j => IsNameSimilar(subCat.Name, j.Name ?? string.Empty, 0.6))
                        .AsQueryable();
                }
                else
                {
                    // search directly by profession name in Name/MetaKeyword
                    string searchName = profession.Replace("-", " ").Trim();
                    searchName = char.ToUpper(searchName[0]) + searchName.Substring(1);

                    query = query.Where(j => j.Name.Contains(searchName) ||
                                             (j.MetaKeyword != null && j.MetaKeyword.Contains(searchName)));
                }
            }

            // ✅ Fetch more than needed for grouping
            var jobAds = query
                .AsNoTracking()               // move this up
                .OrderByDescending(j => j.Dated)
                .Take(500)                    // enough for grouping
                .ToList();


            var companyIds = jobAds
                .Where(j => j.CompanyId.HasValue)
                .Select(j => j.CompanyId.Value)
                .Distinct()
                .ToList();

            var companiesDict = await _context.Companies
                .Where(c => companyIds.Contains(c.Id))
                .ToDictionaryAsync(c => c.Id, c => c);

            var grouped = jobAds
                .GroupBy(j => new { j.CompanyId, j.Dated })
                .Select(g =>
                {
                    var companyIdValue = g.Key.CompanyId ?? 0;
                    var companyName = companiesDict.ContainsKey(companyIdValue) ? companiesDict[companyIdValue].Name : "Unknown";
                    var companyUrl = companiesDict.ContainsKey(companyIdValue) ? companiesDict[companyIdValue].Url : "Unknown";

                    return new GroupedJobAdViewModel
                    {
                        Dated = g.Key.Dated,
                        CompanyName = companyName,
                        CompanyUrl = companyUrl,
                        JobTitles = g.Select(x => x?.Name ?? "Untitled").ToList(),
                        JobCounts = g.Where(x => x?.NoofJobs.HasValue == true && x.NoofJobs > 0)
                                     .Select(x => x.NoofJobs ?? 0)
                                     .ToList(),
                        JobAdIds = g.Select(x => x?.Id ?? 0).ToList(),
                        LastDate = g.Max(x => x.LastDate) ?? DateTime.MinValue,
                        DetailUrl = g.Select(x => x?.Url ?? "#").FirstOrDefault() ?? "#"
                    };
                })
                .OrderByDescending(g => g.Dated)
                .Skip(skip)
                .Take(take)
                .ToList();

            var model = new JobListingPageViewModel
            {
                GroupedJobAds = grouped
            };

            return PartialView("_JobAdListPartial", model);
        }



        [Route("city-wise/jobs-in-{slug}")]// New route for fetching jobs by city ID
        public async Task<IActionResult> CityJobs(string slug)
        {
            // === Banners ===
            ViewBag.Banners = await _bannerService.GetBannersAsync();
            //var cmsData = await _context.TblCms
            //    .FirstOrDefaultAsync(c => c.Url == $"https://www.ilmkidunya.com/jobs/city-wise/jobs-in-{slug}");
            var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/jobs/city-wise/jobs-in-{slug}");
            ViewBag.CmsData = cmsData;

            // === Resolve city by slug ===
            var city = await _context.TblDefCities
                .AsNoTracking()
                .FirstOrDefaultAsync(c =>
                    c.CityName != null &&
                    c.CityName.ToLower().Replace(" ", "-") == slug);

            if (city == null)
            {
                return NotFound();
            }

            ViewBag.CityId = city.CityId;

            //ViewBag.CmsData = new TblCm
            //{
            //    MetaTitle = $"Latest Jobs in {city?.CityName ?? "Unknown City"} 2026",
            //    MetaDesc = $"Find the most recent job openings in {city?.CityName ?? "your city"}.",
            //    Heading = $"Jobs in {city?.CityName ?? "Unknown City"}",
            //    Desc1 = $"Explore the latest career opportunities in {city?.CityName ?? "this city"} across various industries.",
            //    Url = $"https://www.ilmkidunya.com/jobs/city/{city.CityId}" // Example URL for CMS
            //};

            //ViewBag.CityId = cityId; // Pass cityId to view for potential use in partials/JS

            // === Get job IDs that belong to this city ===
            // Assuming JobAdsCitiesIds is a comma-separated string of city IDs
            var cityIdStr = city.CityId.ToString();

            var jobAds = await _context.Tbljobadslatests
                .Where(j => j.IsActive && j.JobAdsCitiesIds != null &&
                    (
                        j.JobAdsCitiesIds == cityIdStr || // exact match
                        j.JobAdsCitiesIds.StartsWith(cityIdStr + ",") ||
                        j.JobAdsCitiesIds.EndsWith("," + cityIdStr) ||
                        j.JobAdsCitiesIds.Contains("," + cityIdStr + ",")
                    )
                )
                .OrderByDescending(j => j.Dated)
                .Take(100)
                .ToListAsync();

            // === Fetch company details for grouping ===
            var companyIds = jobAds
                .Where(j => j.CompanyId.HasValue)
                .Select(j => j.CompanyId ?? 0)
                .Distinct()
                .ToList();

            var companiesDict = await _context.Companies
                .Where(c => companyIds.Contains(c.Id))
                .ToDictionaryAsync(c => c.Id, c => c);

            // === Grouped result ===
            var grouped = jobAds
                .GroupBy(j => new { j.CompanyId, j.Dated }) // Group by company and date
                .Select(g =>
                {
                    var companyId = g.Key?.CompanyId ?? 0;
                    var companyName = companiesDict.ContainsKey(companyId) ? companiesDict[companyId].Name : "Unknown";
                    var companyUrl = companiesDict.ContainsKey(companyId) ? companiesDict[companyId].Url : "#";

                    return new GroupedJobAdViewModel
                    {
                        Dated = g.Key?.Dated ?? DateTime.MinValue,
                        CompanyName = companyName ?? "Unknown Company",
                        CompanyUrl = companyUrl ?? "#",
                        JobTitles = g.Where(x => x != null)
                                   .Select(x => x?.Name ?? "Untitled")
                                   .ToList(),
                        JobCounts = g.Where(x => x != null && x.NoofJobs.HasValue && x.NoofJobs > 0)
                                   .Select(x => x.NoofJobs ?? 0)
                                   .ToList(),
                        JobAdIds = g.Where(x => x != null)
                                  .Select(x => x?.Id ?? 0)
                                  .ToList(),
                        LastDate = g.Where(x => x?.LastDate != null)
                                  .Max(x => x.LastDate) ?? DateTime.MinValue,
                        DetailUrl = g.Where(x => x != null)
                                   .Select(x => x?.Url ?? "#")
                                   .FirstOrDefault() ?? "#"

                    };
                })
                .OrderByDescending(g => g.Dated)
                .ToList();

            var model = new JobListingPageViewModel
            {
                GroupedJobAds = grouped
            };

            return View(model); // Reusing the Home view as it expects JobListingPageViewModel
        }





        [Route("industry/{slug}")]
        public async Task<IActionResult> JobsByCategory(string slug)
        {
            ViewBag.Banners = await _bannerService.GetBannersAsync();

            TblCmsDto cmsData = new TblCmsDto();


            cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/jobs/industry/{slug}");


            var jobTypes = await _context.JobTypes
                .AsNoTracking()
                .ToListAsync();


            var jobType = jobTypes
                .FirstOrDefault(jt => Slugify(jt.Url) == Slugify(slug));

            if (jobType == null)
            {
                return NotFound();
            }
            if (cmsData == null)
            {
                cmsData = new TblCmsDto
                {
                    MetaTitle = jobType.MetaTitle,
                    MetaDesc = jobType.MetaDesc,
                    MetaKeys = jobType.MetaKeyword,
                    Heading = string.IsNullOrWhiteSpace(jobType.Heading) ? jobType.Name + " Jobs 2026" : jobType.Heading,
                    Desc1 = jobType.Detail,
                    Desc2 = jobType.Detail2
                };
            }




            ViewBag.CmsData = cmsData;
            int jobTypeId = jobType.Id;

            var jobAds = await _context.Tbljobadslatests
                .Where(j => j.IsActive && j.JobAdsTypesIds != null &&
                    (
                        j.JobAdsTypesIds == jobTypeId.ToString() ||
                        j.JobAdsTypesIds.StartsWith(jobTypeId + "{#-!-#}") ||
                        j.JobAdsTypesIds.EndsWith("{#-!-#}" + jobTypeId) ||
                        j.JobAdsTypesIds.Contains("{#-!-#}" + jobTypeId + "{#-!-#}")
                    )
                )
                .OrderByDescending(j => j.Dated)
                .Take(100)
                .ToListAsync();

            var companyIds = jobAds
                .Where(j => j.CompanyId.HasValue)
                .Select(j => j.CompanyId.Value)
                .Distinct()
                .ToList();

            var companiesDict = await _context.Companies
                .Where(c => companyIds.Contains(c.Id))
                .ToDictionaryAsync(c => c.Id, c => c);

            var grouped = jobAds
                .GroupBy(j => new { j.CompanyId, j.Dated })
                .Select(g =>
                {
                    var companyId = g.Key.CompanyId.Value;
                    var companyName = companiesDict.ContainsKey(companyId)
                        ? companiesDict[companyId].Name
                        : "Unknown";
                    var companyUrl = companiesDict.ContainsKey(companyId)
                                  ? companiesDict[companyId].Url
                                  : "Unknown";
                    return new GroupedJobAdViewModel
                    {
                        Dated = g.Key.Dated,
                        CompanyName = companyName,
                        CompanyUrl = companyUrl,
                        JobTitles = g.Select(x => x.Name ?? "Untitled").ToList(),
                        JobCounts = g.Where(x => x.NoofJobs.HasValue && x.NoofJobs > 0)
                                     .Select(x => x.NoofJobs.Value)
                                     .ToList(),
                        JobAdIds = g.Select(x => x.Id).ToList(),
                        LastDate = g.Max(x => x.LastDate),
                        DetailUrl = g.Select(x => x.Url).FirstOrDefault()
                    };
                })
                .OrderByDescending(g => g.Dated)
                .Take(20)
                .ToList();

            var model = new JobListingPageViewModel
            {
                GroupedJobAds = grouped
            };

            return View(model);
        }


        [Route("professions/{slug}")]
        public async Task<IActionResult> JobsBySubCategory(string slug)
        {
            ViewBag.Banners = await _bannerService.GetBannersAsync();

            var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/jobs/professions/{slug}");


            if (slug.EndsWith("-jobs", StringComparison.OrdinalIgnoreCase))
            {
                slug = slug[..^5]; // Remove "-jobs" suffix
            }

            var matchSlug = Slugify(slug);

            // 1. Fetch all subcategories (no tracking for performance)
            var allSubCats = await _context.SubJobCategories
                .AsNoTracking()
                .ToListAsync();

            var subCat = allSubCats
                .FirstOrDefault(s => Slugify(s.Name) == matchSlug);

            List<Tbljobadslatest> ads;

            if (subCat != null)
            {
                // 2. Try to get CMS data for matched subcategory


                int targetJobTypeId = subCat.JobTypeId;

                // 3. Filter job ads by JobTypeId (stored as string in JobAdsTypesIds)
                ads = await _context.Tbljobadslatests
                    .Where(j => j.IsActive && j.JobAdsTypesIds != null &&
                        (
                            j.JobAdsTypesIds == targetJobTypeId.ToString() ||
                            j.JobAdsTypesIds.StartsWith(targetJobTypeId + "{#-!-#}") ||
                            j.JobAdsTypesIds.EndsWith("{#-!-#}" + targetJobTypeId) ||
                            j.JobAdsTypesIds.Contains("{#-!-#}" + targetJobTypeId + "{#-!-#}")
                        ))
                    .AsNoTracking()
                    .ToListAsync();

                // 4. Further refine by title similarity
                ads = ads
                    .Where(j => IsNameSimilar(subCat.Name, j.Name ?? string.Empty, 0.6))
                    .ToList();
            }
            else
            {
                // 5. No subcategory found → search directly by name/title
                string searchName = slug.Replace("-", " ").Trim();

                ads = await _context.Tbljobadslatests
                    .Where(j => j.IsActive &&
                        (j.Name.Contains(searchName) ||
                         (j.MetaKeyword != null && j.MetaKeyword.Contains(searchName))))
                    .AsNoTracking()
                    .ToListAsync();
            }

            // 6. Prepare CMS metadata if not found in DB

            if (cmsData == null)
            {
                string searchName = slug.Replace("-", " ").Trim();
                string capitalized = char.ToUpper(searchName[0]) + searchName.Substring(1);

                var CmsData = new TblCm
                {
                    MetaTitle = $"Latest {capitalized} jobs in Pakistan.",
                    MetaDesc = $"List of open vacancies for {capitalized} jobs in Pakistan. {capitalized} jobs in Lahore. Best {capitalized} jobs in Karachi. Jobs for {capitalized}s in Islamabad.",
                    MetaKeys = $"{capitalized} jobs in Pakistan, {capitalized} jobs in Lahore, {capitalized} jobs in Islamabad, {capitalized} jobs in Karachi, list of {capitalized} jobs, best {capitalized} jobs",
                    Heading = $"{capitalized} Jobs in Pakistan",
                    Desc1 = ""

                };

                ViewBag.CmsData = CmsData;
            }
            else if (cmsData.Desc1 == null)
            {
                cmsData.Desc1 = "";
                ViewBag.CmsData = cmsData;
            }
            else
            {
                ViewBag.CmsData = cmsData;
            }
            if (ads == null || ads.Count() == 0 || !ads.Any())
            {
                return NotFound();
            }
            // 7. Group job ads and return to view
            var grouped = BuildGroupedJobListings(ads);
            var model = new JobListingPageViewModel { GroupedJobAds = grouped };

            ViewBag.Profession = slug;

            return View(model);
        }


        [Route("{companyUrl}-jobs-in-{cityName:alpha}")]
        public async Task<IActionResult> CompanyCityJobs(string companyUrl, string cityName)
        {
            // === Banners ===
            ViewBag.Banners = await _bannerService.GetBannersAsync();

            // === CMS Data ===
            //var cmsData = await _context.TblCms
            //    .Where(c => c.Url == $"https://www.ilmkidunya.com/jobs/{companyUrl}-jobs-in-{cityName}")
            //    .FirstOrDefaultAsync();
            var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/jobs/{companyUrl}-jobs-in-{cityName}");
            if (cmsData == null)
            {
                cmsData = new TblCmsDto
                {
                    MetaTitle = $"Jobs in {companyUrl} at {cityName}",
                    MetaDesc = $"Find the latest job openings in {cityName} for {companyUrl}.",
                    MetaKeys = $"{companyUrl}, {cityName}, jobs, career opportunities",
                    Heading = $"Latest Jobs in {companyUrl} at {cityName}",
                    Desc1 = "",
                    Desc2 = ""
                };
            }


            ViewBag.CmsData = cmsData;

            // === Get Company Info ===
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Url == companyUrl);

            if (company == null)
            {
                return NotFound("Company not found.");
            }

            // === Get City Info ===
            var city = await _context.TblDefCities
                .FirstOrDefaultAsync(c => c.CityName.Replace(" ", "-").ToLower() == cityName.ToLower());

            if (city == null)
            {
                return NotFound("City not found.");
            }


            // === Filter Active Job Ads by Company + City ===
            var activeJobAds = await _context.Tbljobadslatests
                .Where(j => j.IsActive &&
                            j.CompanyId == company.Id &&
                            !string.IsNullOrEmpty(j.JobAdsCities) &&
                            j.JobAdsCities.ToLower().Contains(city.CityName.ToLower()))
                .OrderByDescending(j => j.Dated)
                .Take(100)
                .ToListAsync();


            var grouped = activeJobAds
                .GroupBy(j => new { j.CompanyId, j.Dated })
                .Select(g => new GroupedJobAdViewModel
                {
                    Dated = g.Key.Dated,
                    CompanyName = company.Name,
                    JobTitles = g.Select(x => x.Name ?? "Untitled").ToList(),
                    JobCounts = g.Where(x => x.NoofJobs.HasValue && x.NoofJobs > 0)
                                 .Select(x => x.NoofJobs.Value)
                                 .ToList(),
                    CompanyUrl = company.Url,
                    JobAdIds = g.Select(x => x.Id).ToList(),
                    LastDate = g.Max(x => x.LastDate),
                    DetailUrl = g.Select(x => x.Url).FirstOrDefault()
                })
                .OrderByDescending(g => g.Dated)
                .Take(20)
                .ToList();




            // === Combine Into ViewModel ===
            var model = new JobListingPageViewModel
            {
                GroupedJobAds = grouped
            };

            return View(model);
        }



        //[Route("{slug}")]
        //public async Task<IActionResult> CompanyJobs(string slug)
        //{
        //    // === Banners & CMS ===
        //    ViewBag.Banners = await _bannerService.GetBannersAsync();


        //    TblCmsDto cmsData = new TblCmsDto();




        //    // === Resolve company by slug ===
        //    var company = await _context.Companies
        //        .AsNoTracking()
        //        .FirstOrDefaultAsync(c =>
        //            c.Url != null &&
        //            c.Url == slug.ToLower());


        //    if (company == null)
        //    {
        //        return NotFound();
        //    }


        //    cmsData.MetaTitle = company.MetaTitle;
        //    cmsData.MetaDesc = company.MetaDesc;
        //    cmsData.MetaKeys = company.MetaKeyword;
        //    cmsData.Heading = string.IsNullOrWhiteSpace(company.Heading) ? company.Name + " Jobs 2026" : company.Heading;
        //    cmsData.Desc1 = company.ShortDetail;
        //    cmsData.Desc2 = company.ShortDesc;
        //    cmsData.Desc3 = company.Description;


        //    ViewBag.CmsData = cmsData;

        //    ViewBag.CompanyId = company.Id;
        //    // === Fetch job ads for that company ===
        //    var jobAds = await _context.Tbljobadslatests
        //        .Where(j => j.IsActive && j.CompanyId == company.Id)
        //        .OrderByDescending(j => j.Dated)
        //        .Take(100) // or use pagination
        //        .ToListAsync();

        //    var grouped = jobAds
        //        .GroupBy(j => j.Dated)
        //        .Select(g => new GroupedJobAdViewModel
        //        {
        //            Dated = g.Key,
        //            CompanyName = company.Name,
        //            CompanyUrl = company.Url,
        //            JobTitles = g.Select(x => x.Name ?? "Untitled").ToList(),
        //            JobCounts = g.Where(x => x.NoofJobs.HasValue && x.NoofJobs > 0)
        //                         .Select(x => x.NoofJobs.Value)
        //                         .ToList(),
        //            JobAdIds = g.Select(x => x.Id).ToList(),
        //            LastDate = g.Max(x => x.LastDate),
        //            DetailUrl = g.Select(x => x.Url).FirstOrDefault()
        //        })
        //        .OrderByDescending(g => g.Dated)
        //        .ToList();

        //    var model = new JobListingPageViewModel
        //    {
        //        GroupedJobAds = grouped
        //    };

        //    return View(model);
        //}

        [Route("{slug}")]
        public async Task<IActionResult> CompanyJobs(
            string slug,
            [FromServices] IBackgroundTaskQueue backgroundQueue
        )
        {
            // === Banners ===
            ViewBag.Banners = await _bannerService.GetBannersAsync();

            TblCmsDto cmsData = new TblCmsDto();

            // === Resolve company ===
            var company = await _context.Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(c =>
                    c.Url != null &&
                    c.Url == slug.ToLower());

            if (company == null)
            {
                string redirectUrl = $"/jobs/professions/{slug}";
                return Redirect(redirectUrl);
            }

            // === Prepare CMS data (OLD CONTENT) ===
            cmsData.MetaTitle = company != null
                ? $"{company.Name} Jobs 2026 – Latest {(company.IsGovt.GetValueOrDefault() ? "Govt" : "Private")}. Vacancies & Apply Online"
                : "Jobs 2026 – Latest Vacancies & Apply Online";



            cmsData.MetaDesc = company.MetaDesc;
            cmsData.MetaKeys = company.MetaKeyword;
            cmsData.Heading = company.Name + " Jobs 2026";



            var html = company.ShortDetail;
            var doc = new HtmlDocument();

            if (html == null)
            {
                html = "";
            }

            doc.LoadHtml(html);



            // Select all <strong> and <b> inside <h2> and <h3>
            var nodes = doc.DocumentNode.SelectNodes("//h2//strong | //h2//b | //h3//strong | //h3//b");

            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    node.ParentNode.ReplaceChild(
                        doc.CreateTextNode(node.InnerText),
                        node
                    );
                }
            }

            cmsData.Desc1 = doc.DocumentNode.InnerHtml;


            cmsData.Desc2 = company.ShortDesc;
            cmsData.Desc3 = company.Description;

            ViewBag.CmsData = cmsData;
            ViewBag.CompanyId = company.Id;

            // ===========================================================
            //      CHECK IF CONTENT IS OLDER THAN 1 MONTH
            //      IF YES → RUN TEZMATE DYNAMIC CMS IN BACKGROUND
            // ===========================================================

            bool isOldContent = company.Dated < DateTime.Now.AddMonths(-1);



            if (isOldContent)
            {
                backgroundQueue.QueueBackgroundWorkItem(async (ct) =>
                {
                    using var scope = _serviceProvider.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<DbikdContext>();

                    try
                    {
                        // Create description fields list for batch update
                        var descFields = new List<(string Name, string CurrentValue, string Prompt, int MaxLength)>
            {
("Desc1", company.ShortDetail ?? "",
    $@"Create HTML content for {company.Name}'s careers page. You MUST follow these EXACT commands in order:

COMMAND 1: Write this EXACT heading: <h2>{company.Name}: Career Opportunities for 2026</h2>
Then write 2 paragraphs about {company.Name}'s overview and 2026 career plans.

COMMAND 2: Write this EXACT heading: <h2>Anticipated Job Openings in 2026</h2>
Write 1 paragraph introducing expected openings.
Then create an unordered list with 4 items. Each item MUST start with <strong> and include the word 'jobs' or 'roles'.

COMMAND 3: Write this EXACT heading: <h2>Application Process and Key Requirements</h2>
Write 1 paragraph about the hiring process.
Then create an unordered list with 4 items. Each item MUST start with <strong>.

COMMAND 4: Write this EXACT heading: <h2>Why Build a Career at {company.Name}?</h2>
Write 1 paragraph about career benefits.
Then create an unordered list with 4 items. Each item MUST start with <strong>.

COMMAND 5: Write this EXACT heading: <h2>Frequently Asked Questions (FAQs)</h2>

Then write these EXACT 6 questions with answers:
<h3>When will the official job advertisement for 2026 be released?</h3>
<p>[Answer specific to {company.Name}]</p>

<h3>Can I submit my application before the official announcement?</h3>
<p>[Answer specific to {company.Name}]</p>

<h3>What is the primary method of application submission?</h3>
<p>[Answer specific to {company.Name}]</p>

<h3>Are there opportunities for fresh graduates?</h3>
<p>[Answer specific to {company.Name}]</p>

<h3>Does {company.Name} offer positions for non-teaching or support staff?</h3>
<p>[Answer specific to {company.Name}]</p>

<h3>How can I prepare for the interview process?</h3>
<p>[Answer specific to {company.Name}]</p>

COMMAND 6: Write 1 closing paragraph directing candidates to check the official website.

STRICT FORMATTING RULES:
1. Use ONLY: h2, h3, p, ul, li, strong tags
2. h1, h4, h5, h6 tags are not allowed
3. Each section MUST be separated by a blank line
4. Each list item MUST use the format: <li><strong>Category:</strong> Description</li>
5. Total approximately 5000 characters
6. Must include bullet points and FAQ's
7. Content must be complete this is on the priority

FAILURE TO FOLLOW THESE COMMANDS EXACTLY WILL RESULT IN INVALID CONTENT.", 20000),


                ("Desc2", company.ShortDesc ?? "",
                    $"FIELD CONTEXT: This is the short summary description for the {company.Name} jobs page. INSTRUCTIONS: Generate approximately 100 characters only. Use a single paragraph only. Keep it concise and clear. Briefly summarize {company.Name} careers, jobs, and employment opportunities. No extra sentences or formatting.", 200),

                ("Desc3", company.Description ?? "",
                    $"FIELD CONTEXT: This is the supporting description field for the {company.Name} jobs page. INSTRUCTIONS: Generate approximately 500 characters only. Use a single paragraph only. Provide additional details about working at {company.Name}, including professional environment, career development, global opportunities, and employee-focused culture. Keep the tone professional and engaging.", 500)
            };

                        // Send all description fields in one go
                        var multiHtmlRequest = new TezMateMultiHtmlRequest
                        {
                            loginCode = _config["TezMate:LoginCode"],
                            pageUrl = $"/jobs/{slug}",
                            percentage = 10,
                            maxLength = 20000,

                            additionalPrompt = $@"
IMPORTANT: Follow all instructions exactly. Content must be SEO-optimized for job searches. Include relevant keywords such as {company.Name} jobs, careers, employment, vacancies, hiring, and job opportunities. Content must be professional, accurate, up-to-date, unique, and written in natural, engaging English. Avoid plagiarism, keyword stuffing, and misleading information.",

                            htmlFields = descFields.Select(f => new TezMateHtmlField
                            {
                                name = f.Name,
                                type = "html",
                                currentValue = f.CurrentValue,
                                additionalPrompt = f.Prompt   // ✅ field-specific prompt
                            }).ToList(),

                            generateMeta = false
                        };


                        var multiResult = await _tezMateService.GetUpdatedHtmlContentsAsync(multiHtmlRequest);

                        if (multiResult?.Success == true && multiResult.Result != null)
                        {
                            var dbCompany = await db.Companies
                                .FirstOrDefaultAsync(x => x.Id == company.Id, ct);

                            if (dbCompany != null)
                            {
                                bool hasUpdates = false;

                                foreach (var updatedField in multiResult.Result)
                                {
                                    var updatedContent = updatedField.UpdatedContent?.ToString()?.Trim() ?? "";
                                    var fieldInfo = descFields.FirstOrDefault(f => f.Name == updatedField.Name);



                                    // Apply specific processing for Desc1
                                    if (updatedField.Name == "Desc1")
                                    {
                                        var updatedHtml = updatedContent;
                                        var updatedDoc = new HtmlDocument();
                                        updatedDoc.LoadHtml(updatedHtml);

                                        // Remove <strong> and <b> tags inside <h2> and <h3>
                                        var strongNodes = updatedDoc.DocumentNode
                                            .SelectNodes("//h2//strong | //h2//b | //h3//strong | //h3//b");

                                        if (strongNodes != null)
                                        {
                                            foreach (var node in strongNodes)
                                            {
                                                var text = node.InnerText;

                                                // Check previous sibling
                                                if (node.PreviousSibling != null &&
                                                    node.PreviousSibling.NodeType == HtmlNodeType.Text &&
                                                    !node.PreviousSibling.InnerText.EndsWith(" "))
                                                {
                                                    text = " " + text;
                                                }

                                                // Check next sibling
                                                if (node.NextSibling != null &&
                                                    node.NextSibling.NodeType == HtmlNodeType.Text &&
                                                    !node.NextSibling.InnerText.StartsWith(" "))
                                                {
                                                    text = text + " ";
                                                }

                                                node.ParentNode.ReplaceChild(
                                                    updatedDoc.CreateTextNode(text),
                                                    node
                                                );
                                            }
                                        }

                                        // Remove h1 tags if any
                                        var h1Nodes = updatedDoc.DocumentNode.SelectNodes("//h1");
                                        if (h1Nodes != null)
                                        {
                                            foreach (var node in h1Nodes)
                                            {
                                                var h2Node = HtmlNode.CreateNode($"<h2>{node.InnerHtml}</h2>");
                                                node.ParentNode.ReplaceChild(h2Node, node);
                                            }
                                        }

                                        // Convert h4 and h5 tags to h2
                                        var h4Nodes = updatedDoc.DocumentNode.SelectNodes("//h4");
                                        if (h4Nodes != null)
                                        {
                                            foreach (var node in h4Nodes)
                                            {
                                                var h2Node = HtmlNode.CreateNode($"<h2>{node.InnerHtml}</h2>");
                                                node.ParentNode.ReplaceChild(h2Node, node);
                                            }
                                        }

                                        var h5Nodes = updatedDoc.DocumentNode.SelectNodes("//h5");
                                        if (h5Nodes != null)
                                        {
                                            foreach (var node in h5Nodes)
                                            {
                                                var h2Node = HtmlNode.CreateNode($"<h2>{node.InnerHtml}</h2>");
                                                node.ParentNode.ReplaceChild(h2Node, node);
                                            }
                                        }

                                        updatedContent = updatedDoc.DocumentNode.InnerHtml;
                                    }

                                    // Apply specific processing for Desc2
                                    if (updatedField.Name == "Desc3")
                                    {
                                        var updatedHtml = updatedContent;

                                        // Only wrap in p tag if content is not empty and doesn't already start with a p tag
                                        if (!string.IsNullOrWhiteSpace(updatedHtml) &&
                                            !updatedHtml.TrimStart().StartsWith("<p", StringComparison.OrdinalIgnoreCase))
                                        {
                                            var updatedDoc = new HtmlDocument();
                                            updatedDoc.LoadHtml(updatedHtml);

                                            // Create a p element and add the content as its inner HTML
                                            var pNode = HtmlNode.CreateNode($"<p>{updatedHtml}</p>");
                                            updatedDoc.DocumentNode.RemoveAllChildren();
                                            updatedDoc.DocumentNode.AppendChild(pNode);

                                            updatedContent = updatedDoc.DocumentNode.InnerHtml;
                                        }
                                    }

                                    // Update the database field
                                    switch (updatedField.Name)
                                    {
                                        case "Desc1":
                                            if (dbCompany.ShortDetail != updatedContent)
                                            {
                                                dbCompany.ShortDetail = updatedContent;
                                                hasUpdates = true;
                                            }
                                            break;

                                        case "Desc2":
                                            if (dbCompany.ShortDesc != updatedContent)
                                            {
                                                dbCompany.ShortDesc = updatedContent;
                                                hasUpdates = true;
                                            }
                                            break;

                                        case "Desc3":
                                            if (dbCompany.Description != updatedContent)
                                            {
                                                dbCompany.Description = updatedContent;
                                                hasUpdates = true;
                                            }
                                            break;
                                    }
                                }

                                if (hasUpdates)
                                {
                                    dbCompany.Dated = DateTime.Now;
                                    await db.SaveChangesAsync(ct);
                                    Console.WriteLine($"Updated description fields for company: {company.Name}");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Failed to update description fields: {multiResult?.ErrorMessage}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("TezMate background error: " + ex.Message);
                    }
                });
            }


            // === Load JOB ADS (this stays same) ===
            var jobAds = await _context.Tbljobadslatests
                .Where(j => j.IsActive && j.CompanyId == company.Id)
                .OrderByDescending(j => j.Dated)
                .Take(100)
                .ToListAsync();

            var grouped = jobAds
                .GroupBy(j => j.Dated)
                .Select(g => new GroupedJobAdViewModel
                {
                    Dated = g.Key,
                    CompanyName = company.Name,
                    CompanyUrl = company.Url,
                    JobTitles = g.Select(x => x.Name ?? "Untitled").ToList(),
                    JobCounts = g.Where(x => x.NoofJobs.HasValue && x.NoofJobs > 0)
                                 .Select(x => x.NoofJobs.Value)
                                 .ToList(),
                    JobAdIds = g.Select(x => x.Id).ToList(),
                    LastDate = g.Max(x => x.LastDate),
                    DetailUrl = g.Select(x => x.Url).FirstOrDefault()
                })
                .OrderByDescending(g => g.Dated)
                .ToList();

            return View(new JobListingPageViewModel
            {
                GroupedJobAds = grouped
            });
        }




        private List<string> GetLimitedJobTitles(IEnumerable<string> jobTitles, int maxLength = 59)
        {
            var result = new List<string>();
            int currentLength = 0;

            foreach (var title in jobTitles)
            {
                int additionalLength = (result.Count > 0 ? 1 : 0) + title.Length; // space adds 1 char
                if (currentLength + additionalLength <= maxLength)
                {
                    if (result.Count > 0)
                        currentLength += 1; // for space
                    result.Add(title);
                    currentLength += title.Length;
                }
                else
                {
                    result.Add("...");
                    break;
                }
            }

            return result;
        }


        private string Slugify(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "unknown";

            // Convert to lowercase, remove special characters, then replace spaces with hyphens
            return System.Text.RegularExpressions.Regex
                .Replace(input.ToLower(), @"[^a-z0-9\s-]", "")
                .Replace(" ", "-");
        }

        private bool IsNameSimilar(string a, string b, double threshold = 0.6)
        {
            if (string.IsNullOrWhiteSpace(a) || string.IsNullOrWhiteSpace(b))
                return false;

            a = a.ToLowerInvariant();
            b = b.ToLowerInvariant();

            // simple containment check
            if (b.Contains(a) || a.Contains(b))
                return true;

            // approximate match via common char ratio
            var common = a.Intersect(b).Distinct().Count();
            var total = Math.Max(a.Length, b.Length);
            return ((double)common / total) >= threshold;
        }

        private List<GroupedJobAdViewModel> BuildGroupedJobListings(List<Tbljobadslatest> ads)
        {
            var companies = ads
                .Where(j => j.CompanyId.HasValue)
                .Select(j => j.CompanyId.Value)
                .Distinct()
                .ToList();

            // Get company name and URL together
            var compDict = _context.Companies
                .Where(c => companies.Contains(c.Id))
                .ToDictionary(c => c.Id, c => new { c.Name, c.Url });

            var grouped = ads
                .Where(j => j.CompanyId.HasValue)
                .GroupBy(j => new { j.CompanyId, j.Dated })
                .Select(g =>
                {
                    var cid = g.Key.CompanyId.Value;
                    return new GroupedJobAdViewModel
                    {
                        Dated = g.Key.Dated,
                        CompanyName = compDict.ContainsKey(cid) ? compDict[cid].Name : "Unknown",
                        CompanyUrl = compDict.ContainsKey(cid) ? compDict[cid].Url : "#",
                        JobTitles = g.Select(x => x.Name ?? "Untitled").ToList(),
                        JobCounts = g.Where(x => x.NoofJobs > 0).Select(x => x.NoofJobs.Value).ToList(),
                        JobAdIds = g.Select(x => x.Id).ToList(),
                        LastDate = g.Max(x => x.LastDate),
                        DetailUrl = g.Select(x => x.Url).FirstOrDefault()
                    };
                })
                .OrderByDescending(g => g.Dated)
                .Take(20)
                .ToList();

            return grouped;
        }









        // Individual hardcoded routes
        [Route("nawai-waqt-newspaper-jobs-in-lahore.aspx")]
        public IActionResult NawaiWaqtLahoreRedirect()
        {
            string redirectUrl = $"/jobs/nawai-waqt-newspaper-jobs-in-lahore";
            return Redirect(redirectUrl);
        }

        [Route("popular-jobs/doctors-jobs-in-pakistan.aspx")]
        public IActionResult DoctorsJobsPakistanRedirect()
        {
            string redirectUrl = $"/jobs/professions/doctors";
            return Redirect(redirectUrl);
        }

        [Route("latest-jobs-in-faisalabad.aspx")]
        public IActionResult FaisalabadJobsRedirect()
        {
            string redirectUrl = $"/jobs/city-wise/jobs-in-faisalabad";
            return Redirect(redirectUrl);
        }

        [Route("teachers-jobs-in-army-public-school-faisal-karachipakistan")]
        public async Task<IActionResult> ArmyPublicSchoolTeachersRedirect()
        {
            var teacherJob = await _context.Tbljobadslatests
                .FirstOrDefaultAsync(j =>
                    j.Name != null &&
                    j.Name.ToLower().Contains("army public school") &&
                    j.Name.ToLower().Contains("teacher"));

            if (teacherJob != null)
            {
                // Find the correct slug for the job detail
                var company = await _context.Companies
                    .FirstOrDefaultAsync(c => c.Id == teacherJob.CompanyId);

                if (company != null && !string.IsNullOrEmpty(company.Url))
                {
                    // Extract job title from the job name for the slug
                    var jobTitle = teacherJob.Name?.ToLower().Replace(" ", "-") ?? "job";
                    string redirectUrl = $"/jobs/{jobTitle}-jobs-in-{company.Url}-{teacherJob.Id}.aspx";
                    return Redirect(redirectUrl);
                }
                else
                {
                    // Fallback to generic job detail
                    string redirectUrl = $"/jobs/{teacherJob.Id}";
                    return Redirect(redirectUrl);
                }
            }

            string professionRedirectUrl = $"/jobs/professions/teachers";
            return Redirect(professionRedirectUrl);
        }

        [Route("jobs-in-nawai-waqt-newspaper.aspx")]
        public IActionResult NawaiWaqtJobsRedirect()
        {
            string redirectUrl = $"/jobs/nawai-waqt-newspaper";
            return Redirect(redirectUrl);
        }

        [Route("industry/consultancy-services-jobs-in-manshera")]
        public IActionResult ConsultancyServicesMansheraRedirect()
        {
            string redirectUrl = $"/jobs/industry/consultancy-services";
            return Redirect(redirectUrl);
        }

        [Route("material-engineer-jobs-public-sector-organization.aspx")]
        public IActionResult MaterialEngineerPublicSectorRedirect()
        {
            string redirectUrl = $"/jobs/professions/material-engineer";
            return Redirect(redirectUrl);
        }

        [Route("material-engineer-jobs-frontier-works-organization-fwo")]
        public IActionResult MaterialEngineerFWORedirect()
        {
            string redirectUrl = $"/jobs/frontier-works-organization-fwo";
            return Redirect(redirectUrl);
        }

        [Route("medical-technician-jobs-attock")]
        public IActionResult MedicalTechnicianAttockRedirect()
        {
            string redirectUrl = $"/jobs/professions/medical-technician";
            return Redirect(redirectUrl);
        }

    }
}
