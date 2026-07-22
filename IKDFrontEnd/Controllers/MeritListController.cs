using IKDFrontEnd.DBCollege;
using IKDFrontEnd.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IKDFrontEnd.Controllers
{
	public class MeritListController : Controller
	{
		private readonly BannerService _bannerService;
		private readonly DbCollegeContext _contextCollege;
		public MeritListController(BannerService bannerService, DbCollegeContext contextCollege = null)
		{
			_bannerService = bannerService;
			_contextCollege = contextCollege;
		}

		[Route("merit-list")]
		public async Task<IActionResult> Index()
		{
			var banners = await _bannerService.GetBannersAsync();
			ViewBag.Banners = banners;
			var colleges= await _contextCollege.TblColleges.OrderBy(c => c.Name).Take(50).ToListAsync();
			return View(colleges);
		}

		[HttpGet]
		public async Task<IActionResult> LoadMore(int skip = 0)
		{
			var colleges = await _contextCollege.TblColleges
				.OrderBy(c => c.Name)
				.Skip(skip)
				.Take(50)
				.ToListAsync();

			return PartialView("_CollegeList", colleges);
		}
	}
}
