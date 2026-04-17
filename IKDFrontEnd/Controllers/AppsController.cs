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
		public AppsController(RandomCmsService randomCms, BannerService bannerService, CmsRepository cmsRepo = null)
		{
			_randomCms = randomCms;
			_bannerService = bannerService;
			_cmsRepo = cmsRepo;
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
				return NotFound();

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
