using IKDFrontEnd.Models;
using IKDFrontEnd.Services;
using IKDFrontEnd.ViewModels.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IKDFrontEnd.Controllers
{
    public class ToursController : Controller
    {
        private readonly ILogger<ToursController> _logger;
        private readonly DbikdContext _context;
        private readonly BannerService _bannerService;
        private readonly CmsRepository _cmsRepo;
        public ToursController(ILogger<ToursController> logger, DbikdContext dbikdContext, BannerService bannerService, CmsRepository cmsRepo)
        {
            _logger = logger;
            _context = dbikdContext;
            _bannerService = bannerService;
            _cmsRepo = cmsRepo;
        }

        [HttpGet("tours")]
        [HttpGet("tours/{url}")]
        public async Task<IActionResult> Home(string url)
        {
            ViewBag.Banners = await _bannerService.GetBannersAsync();

            TblCmsDto cmsData;

            if (!string.IsNullOrEmpty(url))
            {
                cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/tours/{url}");

            }
            else
            {
                cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/tours");

            }

            if (cmsData == null)
            {
                return NotFound();
            }

            ViewBag.CmsData = cmsData;

            return View();
        }


    }
}
