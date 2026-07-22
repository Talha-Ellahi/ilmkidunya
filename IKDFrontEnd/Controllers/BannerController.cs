using IKDFrontEnd.DBCollege;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IKDFrontEnd.Controllers
{
	public class BannerController : Controller
	{
		private readonly DbCollegeContext _contextCollege;

		public BannerController(DbCollegeContext contextCollege)
		{
			_contextCollege = contextCollege;
		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> CountImpression(int advertiseId, string device)
		{
			DateOnly today = DateOnly.FromDateTime(DateTime.Today);

			var stat = await _contextCollege.BannerStatistics
				.FirstOrDefaultAsync(x =>
					x.AdvertisId == advertiseId &&
					x.Date == today);

			if (stat == null)
			{
				int nextId = (_contextCollege.BannerStatistics.Max(x => (int?)x.BannerStatId) ?? 0) + 1;
				stat = new BannerStatistic
				{
					BannerStatId = nextId,
					AdvertisId = advertiseId,
					Date = today,
					ImpressionCountD = 0,
					ImpressionCountM = 0,
					ClickCountD = 0,
					ClickCountM = 0
				};

				_contextCollege.BannerStatistics.Add(stat);
			}

			if (device == "Desktop")
				stat.ImpressionCountD++;
			else
				stat.ImpressionCountM++;

			await _contextCollege.SaveChangesAsync();

			return Ok();
		}

		[HttpPost]
		public async Task<IActionResult> CountClick(int advertiseId, string device)
		{
			DateOnly today = DateOnly.FromDateTime(DateTime.Today);

			var stat = await _contextCollege.BannerStatistics
				.FirstOrDefaultAsync(x =>
					x.AdvertisId == advertiseId &&
					x.Date == today);

			if (stat == null)
			{
				int nextId = (_contextCollege.BannerStatistics.Max(x => (int?)x.BannerStatId) ?? 0) + 1;
				stat = new BannerStatistic
				{
					BannerStatId = nextId,
					AdvertisId = advertiseId,
					Date = today,
					ImpressionCountD = 0,
					ImpressionCountM = 0,
					ClickCountD = 0,
					ClickCountM = 0
				};

				_contextCollege.BannerStatistics.Add(stat);
			}

			if (device == "Desktop")
				stat.ClickCountD++;
			else
				stat.ClickCountM++;

			await _contextCollege.SaveChangesAsync();

			return Ok();
		}
	}
}
