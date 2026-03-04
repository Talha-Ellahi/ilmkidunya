using IKDFrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IKDFrontEnd.Services;

namespace IKDFrontEnd.Controllers
{
	public class TestController : Controller
	{
		private readonly DbikdContext _context;
		private readonly BannerService _bannerService;

		public TestController(DbikdContext context, BannerService bannerService)
		{
			_context = context;
			_bannerService = bannerService;
		}

		public async Task<IActionResult> index()
		{
			var data = await _context.BoardTypes.ToListAsync();
			var banners = await _bannerService.GetBannersAsync();
			ViewBag.Banners = banners;
			return View(data);
		}
	}
}
