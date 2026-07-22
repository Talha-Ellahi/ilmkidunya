using IKDFrontEnd.Interfaces;
using IKDFrontEnd.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace IKDFrontEnd.Controllers
{
   
    public class ErrorController : Controller
    {
        private readonly BannerService _bannerService;
        private readonly IErrorLogService _errorLogService;

        public ErrorController(BannerService bannerService, IErrorLogService errorLogService)
        {
            _bannerService = bannerService;
            _errorLogService = errorLogService;
        }

        [Route("Error/404")]
        public async Task<IActionResult> Error404()
        {
            var feature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            if (feature != null)
            {
                var path = feature.OriginalPath + feature.OriginalQueryString;

				// Page where user clicked the broken link
				var referrer = Request.Headers["Referer"].ToString();
				// Client IP
				var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

				if (!string.IsNullOrWhiteSpace(path))
                    await _errorLogService.LogErrorAsync(path, referrer, ipAddress);
            }

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
        public async Task<IActionResult> General(int code)
        {
            if (code == 404)
                return await Error404();
            else
                return View("Error500");
        }
    }
}
