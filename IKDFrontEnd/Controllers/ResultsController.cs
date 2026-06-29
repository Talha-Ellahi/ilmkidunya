using IKDFrontEnd.Interfaces;
using IKDFrontEnd.Services;
using IKDFrontEnd.ViewModels;
using Microsoft.AspNetCore.Mvc;





namespace IKDFrontEnd.Controllers
{
    public class IKDFrontEnd : Controller
    {
        private readonly RandomCmsService _randomCms;
        private readonly BannerService _bannerService;
        private readonly CmsRepository _cmsRepo;
		private readonly IErrorLogService _errorLogService;

		public IKDFrontEnd(RandomCmsService randomCms, BannerService bannerService, CmsRepository cmsRepo, IErrorLogService errorLogService)
		{
			_randomCms = randomCms;
			_bannerService = bannerService;
			_cmsRepo = cmsRepo;
			_errorLogService = errorLogService;
		}


		[Route("results")]
        public async Task<IActionResult> Home()
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;
            return View();
        }
        [Route("results/{url}")]
        public async Task<IActionResult> GetResultsDataByUrl(string url)
        {
           

            var section = await _cmsRepo.GetByUrlAsync($"/results/{url}");
            if (section == null)
            {
				string path = $"https://www.ilmkidunya.com/results/{url}";
				await _errorLogService.LogErrorAsync(path);
				return NotFound();
			}
                

            var result = new DateSheetCriteria
            {
                Heading = section.Heading,
                MetaTitle = section.MetaTitle,
                MetaDescription = section.MetaDesc,
                MetaKeywords = section.MetaKeys,
                Desc1 = section.Desc1,
                Desc2 = section.Desc2
            };

            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            return View("GetResultsDataByUrl", result);
        }





    }
}
