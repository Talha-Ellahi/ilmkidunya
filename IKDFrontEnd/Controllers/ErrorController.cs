using IKDFrontEnd.Services;
using Microsoft.AspNetCore.Mvc;

namespace IKDFrontEnd.Controllers
{
   
    public class ErrorController : Controller
    {
        private readonly BannerService _bannerService;

        public ErrorController (BannerService bannerService)
        {
            _bannerService = bannerService;
        }

        [Route("Error/404")]
        public async Task<IActionResult> Error404()
        {
            ViewBag.HideGoogleAds = true;
            ViewBag.Banners = await _bannerService.GetBannersAsync();
            return View("Error404");
        }

        [Route("Error/500")]
        [Route("Error/ServerError")]
        public IActionResult ServerError(string reason = "An unexpected error occurred while processing your request.")
        {
            var model = new Models.ErrorViewModel
            {
                Reason = reason
            };
            return View("Error500", model);
        }


        [Route("Error/{code}")]
        public IActionResult General(int code)
        {
            if (code == 404)
                return View("Error404");
            else
                return View("Error500");
        }
    }
}
