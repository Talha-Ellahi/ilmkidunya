using IKDFrontEnd.Models;
using IKDFrontEnd.Services;
using IKDFrontEnd.ViewModels.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IKDFrontEnd.Controllers
{
    public class UmrahPackagesController : Controller
    {
        private readonly ILogger<ToursController> _logger;
        private readonly DbikdContext _context;
        private readonly BannerService _bannerService;
        private readonly CmsRepository _cmsRepo;
        public UmrahPackagesController(ILogger<ToursController> logger, DbikdContext dbikdContext, BannerService bannerService, CmsRepository cmsRepo)
        {
            _logger = logger;
            _context = dbikdContext;
            _bannerService = bannerService;
            _cmsRepo = cmsRepo;
        }


        [HttpGet("umrah-packages")]
        [HttpGet("umrah-packages/{url}")]
        public async Task<IActionResult> Index(string url)
        {
            ViewBag.Banners = await _bannerService.GetBannersAsync();

 
            var content = new TblUrlcontent();

            if (!string.IsNullOrEmpty(url))
            {
                content = await _context.TblUrlcontents.Where(u => u.Url == $"https://www.ilmkidunya.com/umrah-packages/{url}").FirstOrDefaultAsync();

            }
            else
            {
                 content = await _context.TblUrlcontents.Where(u => u.Url == $"https://www.ilmkidunya.com/umrah-packages/").FirstOrDefaultAsync();

            }

            if (content == null)
            {
                return NotFound();
            }

             

            return View(content);
        }
    }
}
