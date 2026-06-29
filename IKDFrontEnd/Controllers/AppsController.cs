using IKDFrontEnd.Interfaces;
using IKDFrontEnd.Services;
using IKDFrontEnd.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace IKDFrontEnd.Controllers
{
	public class AppsController : Controller
	{
		private readonly RandomCmsService _randomCms;
		private readonly BannerService _bannerService;
		private readonly CmsRepository _cmsRepo;
		private readonly IErrorLogService _errorLogService;
		public AppsController(RandomCmsService randomCms, BannerService bannerService, CmsRepository cmsRepo = null, IErrorLogService errorLogService = null)
		{
			_randomCms = randomCms;
			_bannerService = bannerService;
			_cmsRepo = cmsRepo;
			_errorLogService = errorLogService;
		}


		//[Route("apps")]
		//public async Task<IActionResult> Home()
		//{
		//	var banners = await _bannerService.GetBannersAsync();
		//	ViewBag.Banners = banners;
		//	return View();
		//}
		[Route("apps")]
		[Route("apps/{url}")]
		public async Task<IActionResult> GetResultsDataByUrl(string url)
		{


			var section = await _cmsRepo.GetByUrlAsync($"/apps/{url}");
			if (section == null)
			{
				//string path = "https://www.ilmkidunya.com/apps/" + url;
				//await _errorLogService.LogErrorAsync(path);
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
