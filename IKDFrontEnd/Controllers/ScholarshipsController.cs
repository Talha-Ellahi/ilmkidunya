using IKDFrontEnd.Models;
using IKDFrontEnd.Services;
using IKDFrontEnd.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Policy;

namespace IKDFrontEnd.Controllers
{
    public class ScholarshipsController : Controller
    {
        private readonly DbikdContext _context;
        private readonly BannerService _bannerService;
        private readonly CmsRepository _cmsRepo;

        public ScholarshipsController(DbikdContext context, BannerService bannerService , CmsRepository cmsRepo)
        {
            _context = context;
            _bannerService = bannerService;
            _cmsRepo = cmsRepo;
        }



        [HttpGet]
        [Route("scholarships")]
        public async Task<IActionResult> Index()
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;
            var model = new HomeScholarshipViewModel
            {
                StudyLevels = _context.TblSchStudyLevels
                                  .Where(s => s.IsActive == true)
                                  .OrderBy(s => s.SortOrder)
                                  .ToList(),

         
                Countries = _context.TblSches
                                    .Where(s => s.IsActive == true)
                                    .Join(_context.TblPlaceOfStudies,
                                          s => s.CountryId,
                                          c => c.Id,
                                          (s, c) => c)
                                    .Distinct()
                                    .OrderBy(c => c.Name)
                                    .ToList(),

                LatestScholarships = _context.TblSches
                                         .Where(s => s.IsActive == true)
                                         .OrderByDescending(s => s.Id)
                                         .Take(4)
                                         .ToList()
            };


            return View(model);
        }



        [HttpGet]
        [Route("scholarship/search")]
        public async Task<IActionResult> SearchScholarships(string countryName, string studyLevelName, int page = 1, int pageSize = 10)
        {
            var query = _context.TblSches
                .Include(s => s.TblSchStudyLevelChildren)
                .Include(s => s.TblSchFieldsofStudyChildren)
                .Where(s => s.IsActive == true)
                .AsQueryable();

            if (!string.IsNullOrEmpty(countryName))
            {
                var matchingCountryIds = _context.TblPlaceOfStudies
                    .Where(c => c.Name.Contains(countryName))
                    .Select(c => c.Id)
                    .ToList();

                query = query.Where(s => s.CountryId.HasValue && matchingCountryIds.Contains(s.CountryId.Value));
            }

            if (!string.IsNullOrEmpty(studyLevelName))
            {
                var matchingLevelIds = _context.TblSchStudyLevels
                    .Where(l => l.SchStudyLevelName.Contains(studyLevelName))
                    .Select(l => l.Id)
                    .ToList();

                query = query.Where(s => s.TblSchStudyLevelChildren
                    .Any(c => matchingLevelIds.Contains(c.SchStudyLevelId)));
            }

            var model = await query
                .OrderByDescending(s => s.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new ScholarshipApiViewModel
                {
                    SchName = s.SchName,
                    Url = s.Url,
                    Deadline = s.Deadline,
                    Image = s.SchImage,
                    StudyLevels = s.TblSchStudyLevelChildren
                        .Select(sl => _context.TblSchStudyLevels
                            .Where(level => level.Id == sl.SchStudyLevelId)
                            .Select(level => level.SchStudyLevelName)
                            .FirstOrDefault())
                        .Where(name => name != null)
                        .ToList(),

                    FieldsOfStudy = s.TblSchFieldsofStudyChildren
                        .Select(f => _context.TblSchFieldofStudies
                            .Where(fs => fs.Id == f.SchFieldofStudyId)
                            .Select(fs => fs.SchFieldofStudyName)
                            .FirstOrDefault())
                        .Where(name => name != null)
                        .ToList()
                })
                .ToListAsync();

            // Ajax Request: Return Partial View
          

            var resultModel = new ScholarshipSearchResultViewModel
            {
                Results = model,
                StudyLevels = _context.TblSchStudyLevels.ToList(),
                Countries = _context.TblPlaceOfStudies.ToList(),
                SelectedCountry = countryName,
                SelectedLevel = studyLevelName,
                StudyLevelList = new SelectList(_context.TblSchStudyLevels, "SchStudyLevelName", "SchStudyLevelName", studyLevelName),
                CountryList = new SelectList(_context.TblPlaceOfStudies, "Name", "Name", countryName)
            };

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ScholarshipListPartial", resultModel.Results);

            }
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;
            return View("SearchScholarships", resultModel);
        }



        
       



        [HttpGet]
        [Route("scholarships/scholarships-in-{urlSlug}.aspx")]
        public async Task<IActionResult> ScholarshipsCountryWise(string urlSlug)
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            var country = await _context.TblPlaceOfStudies
                .Where(c => c.Url == urlSlug)
                .FirstOrDefaultAsync();
            if (country == null)
            {
                return NotFound();
            };
            ViewBag.CountryName = country.Name;
            var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/scholarships/scholarships-in-{urlSlug}.aspx");


            var scholarships = await _context.TblSches
                                         .Where(s => s.CountryId == country.Id && s.IsActive == true)
                                         .OrderByDescending(s => s.Id)
                                         .Take(10)
                                         .Select(s => new ScholarshipApiViewModel
                                         {
                                             SchName = string.IsNullOrWhiteSpace(s.SchName) ? "Not Available" : s.SchName,
                                             Url = string.IsNullOrWhiteSpace(s.Url) ? "Not Available" : s.Url,
                                             Deadline = s.Deadline, // Agar date null ho to handle karein
                                             Image = string.IsNullOrWhiteSpace(s.SchImage) ? "/images/default-image.png" : s.SchImage,

                                             StudyLevels = s.TblSchStudyLevelChildren
                                                 .Select(sl => _context.TblSchStudyLevels
                                                     .Where(level => level.Id == sl.SchStudyLevelId)
                                                     .Select(level => string.IsNullOrWhiteSpace(level.SchStudyLevelName)
                                                                      ? "Not Available"
                                                                      : level.SchStudyLevelName)
                                                     .FirstOrDefault())
                                                 .Where(name => name != null)
                                                 .ToList(),

                                             FieldsOfStudy = s.TblSchFieldsofStudyChildren
                                                 .Select(f => _context.TblSchFieldofStudies
                                                     .Where(fs => fs.Id == f.SchFieldofStudyId)
                                                     .Select(fs => string.IsNullOrWhiteSpace(fs.SchFieldofStudyName)
                                                                   ? "Not Available"
                                                                   : fs.SchFieldofStudyName)
                                                     .FirstOrDefault())
                                                 .Where(name => name != null)
                                                 .ToList()
                                         })
                                         .ToListAsync();


            ViewBag.LatestScholarships = scholarships;



            return View(cmsData);
        }
        [HttpGet]
        [Route("scholarships/topscholarship")]
        public async Task<IActionResult> TopScholarships(string urlSlug)
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/scholarships/topscholarship");

            var scholarshipsByCountry = await _context.TblSchTops
                                 .Where(s => s.IsActive == true)
                                 .GroupBy(s => s.CountryId)
                                 .Select(g => new ScholarshipsByCountryViewModel
                                 {
                                     Country = _context.TblPlaceOfStudies
                                                 .Where(c => c.Id == g.Key)
                                                 .Select(c => string.IsNullOrWhiteSpace(c.Name) ? "Not Available" : c.Name)
                                                 .FirstOrDefault(),
                                     Scholarships = g
                                         .OrderByDescending(s => s.Id)
                                         .Take(10)
                                         .Select(s => new ScholarshipViewModel
                                         {
                                             Name = string.IsNullOrWhiteSpace(s.TopSchName) ? "Not Available" : s.TopSchName,
                                             Url = string.IsNullOrWhiteSpace(s.Url) ? "#" : s.Url,
                                             Image = string.IsNullOrWhiteSpace(s.Image) ? "/images/default-image.png" : s.Image,
                                             Deadline = s.CreatedDate,
                                         })
                                         .ToList()
                                 })
                                 .ToListAsync();

      


            ViewBag.CmsData = cmsData;
            return View("TopScholarships", scholarshipsByCountry);
        }

        [Route("scholarships/{urlSlug}")]
        public async Task<IActionResult> TopScholarshipsDetail(string urlSlug)
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            

            var model = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/scholarships/{urlSlug}");

            if (model == null)
            {
                return await ScholarshipsDetail(urlSlug);
            }

            return View("TopScholarshipsDetail", model);
        }



        public async Task<IActionResult> ScholarshipsDetail(string urlSlug)
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            var model = await _context.TblSches
                .Where(s => s.Url.Replace(".aspx", "") == urlSlug)
                .FirstOrDefaultAsync();

            if (model == null)
                return NotFound();

            // Split keywords and fetch related scholarships
            List<TblSch> relatedScholarships = new();

            relatedScholarships = await _context.TblSches
                .Where(s => s.IsActive == true &&
                            s.Id != model.Id)
                .OrderByDescending(s => s.Id)
                .Take(8)
                .ToListAsync();

            ViewBag.RelatedScholarships = relatedScholarships;

            return View("ScholarshipsDetail", model);
        }


    }
}
