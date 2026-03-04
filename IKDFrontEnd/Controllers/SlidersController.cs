using IKDFrontEnd.Models;
using IKDFrontEnd.Services;
using IKDFrontEnd.ViewModels;
using IKDFrontEnd.ViewModels.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Security.Policy;

namespace IKDFrontEnd.Controllers
{
    [Route("stories")]
    public class SlidersController : Controller
    {
        private readonly DbikdContext _context;
        private readonly BannerService _bannerService;
        private readonly CmsRepository _cmsRepo;

        private readonly ILogger<SlidersController> _logger;

        public SlidersController(DbikdContext context, BannerService bannerService, ILogger<SlidersController> logger, CmsRepository cmsRepo)
        {
            _context = context;
            _bannerService = bannerService;
            _logger = logger;
            _cmsRepo = cmsRepo;
        }

        [HttpGet("")]
        public async Task<IActionResult> Home()
        {
            ViewBag.Banners = await _bannerService.GetBannersAsync();
            TblCmsDto cmsData = new TblCmsDto();
            cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/stories");

            if (cmsData == null)
            {
                cmsData = new TblCmsDto
                {
                    MetaTitle = "Stories - Ilm Ki Dunya",
                    MetaDesc = "Explore a wide range of educational stories on Ilm Ki Dunya. Stay updated with the latest trends and insights in education.",
                    MetaKeys = "Educational Stories, Learning Resources, Study Materials, Academic Insights, Ilm Ki Dunya"
                };
            }
            ViewBag.CmsData = cmsData;


            var vm = (from s in _context.WebStorySliders
                      join sl in _context.TblSlides on s.Id equals sl.SliderId
                      join c in _context.TblSliderCategories on s.SlierCategoryId equals c.Id
                      select new SliderHome
                      {
                          Slidertitle = s.SliderName,
                          ID = s.Id,
                          Categoryname = c.SliderCategoryName,
                          Image = s.MainImage,
                          Date = s.Date
                      })
                      .AsEnumerable()                   // <--- switch to client-side
                      .DistinctBy(x => x.ID)             // now works
                      .OrderByDescending(x => x.Date)
                      .ToList();



            return View(vm);
        }


        [HttpGet("{name}")]
        public async Task<IActionResult> SliderDetail(string name)
        {
            ViewBag.Banners = await _bannerService.GetBannersAsync();
            name = name.Replace("-", " ").ToLower();
            var slides = (from s in _context.TblSlides
                          join c in _context.WebStorySliders on s.SliderId equals c.Id
                          where c.SliderName == name
                          orderby s.SortOrder
                          select new SliderDetails
                          {
                              SliderName = c.SliderName,
                              MainSliderImage = c.MainImage,
                              Slidertitle = s.SliderTitle,
                              ID = s.Id,
                              SliderDesc = s.SliderDesc,
                              Image = s.MainImage,
                              Date = s.Date,
                              SortOrder = s.SortOrder,
                              SliderId = s.SliderId,
                              AuthorInfo = c.AuthorInfo
                          })
                      .ToList();
            if (!slides.Any())   
            {
                return NotFound();
            }


            var slider = (from s in _context.WebStorySliders
                          join sl in _context.TblSlides on s.Id equals sl.SliderId
                          join c in _context.TblSliderCategories on s.SlierCategoryId equals c.Id
                          select new SliderHome
                          {
                              Slidertitle = s.SliderName,
                              ID = s.Id,
                              Categoryname = c.SliderCategoryName,
                              Image = s.MainImage,
                              Date = s.Date
                          })
                      .AsEnumerable()
                      .DistinctBy(x => x.ID)
                      .OrderByDescending(x => x.Date)
                      .ToList();



            ViewBag.CmsData = new TblCm
            {
                MetaTitle = slides.FirstOrDefault()?.SliderName,
                MetaDesc = "Explore a wide range of educational stories on Ilm Ki Dunya. Stay updated with the latest trends and insights in education.",
                MetaKeys = "Educational Stories, Learning Resources, Study Materials, Academic Insights, Ilm Ki Dunya"
            };

            var vm = new SliderDetailViewModel
            {
                Slides = slides,
                Sliders = slider
            };
            return View(vm);
        }



        [HttpGet("/stories/{category:regex(^[[a-zA-Z_]]+$)}")]
        public async Task<IActionResult> CategoryWiseSlider(string category)
        {
            category = category.Replace("_", " ").ToLower();

            ViewBag.Banners = await _bannerService.GetBannersAsync();
            if (string.IsNullOrEmpty(category))
            {
                return RedirectToAction("Home");
            }
            var vm = _context.WebStorySliders
                    .Join(_context.TblSliderCategories,
                          s => s.SlierCategoryId,
                          c => c.Id,
                          (s, c) => new { s, c })
                    .Where(x => x.c.SliderCategoryName == category)
                    .OrderByDescending(x => x.s.Date)
                    .Select(x => new SliderHome
                    {
                        Slidertitle = x.s.SliderName,
                        ID = x.s.Id,
                        Categoryname = x.c.SliderCategoryName,
                        Image = x.s.MainImage
                    })
                    .ToList();

            if (vm == null)
            {
                return NotFound();
            }

            ViewBag.CmsData = new TblCm
            {
                MetaTitle = category.Replace("_"," ").ToUpperInvariant(),
                MetaDesc = "Explore a wide range of educational stories on Ilm Ki Dunya. Stay updated with the latest trends and insights in education.",
                MetaKeys = "Educational Stories, Learning Resources, Study Materials, Academic Insights, Ilm Ki Dunya"
            };
            ViewBag.Title = category.Replace("_"," ").ToUpperInvariant();
            return View(vm);
        }

    }
}
