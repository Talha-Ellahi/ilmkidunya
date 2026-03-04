using IKDFrontEnd.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IKDFrontEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]


    public class BannersCacheController : ControllerBase
    {


        private readonly BannerService _bannerService;


        public BannersCacheController(BannerService bannerService)
        {
            _bannerService = bannerService;
        }

        [HttpPost("/api/banners-cache/refresh-banners")]
        public async Task<IActionResult> RefreshBanners()
        {
            await _bannerService.RefreshBannersAsync();
            return Ok(new { success = true, message = "Banners refreshed." });
        }
    }
}









