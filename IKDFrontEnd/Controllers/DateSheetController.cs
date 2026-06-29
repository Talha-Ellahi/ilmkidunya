using IKDFrontEnd.Interfaces;
using IKDFrontEnd.Models;
using IKDFrontEnd.Services;
using IKDFrontEnd.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace IKDFrontEnd.Controllers
{
    public class DateSheetController : Controller
    {
        private readonly DbikdContext _context;
        private readonly BannerService _bannerService;
        private readonly CmsRepository _cmsRepo;
		private readonly IErrorLogService _errorLogService;
		public DateSheetController(DbikdContext context, BannerService bannerService, CmsRepository cmsRepo, IErrorLogService errorLogService = null)
		{
			_context = context;
			_bannerService = bannerService;
			_cmsRepo = cmsRepo;
			_errorLogService = errorLogService;
		}


		[Route("date_sheets")]
        public async Task<IActionResult> Home()
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;
            return View();
        }

        [Route("date_sheets/{url}")]
        public async Task<IActionResult> GetDateSheetDataByUrl(string url)
        {
            var result = new DateSheetCriteria();

            var section = await _cmsRepo.GetByUrlAsync($"/date_sheets/{url}");
            if (section == null)
            {
				string path = $"https://www.ilmkidunya.com/date_sheets/{url}";
				await _errorLogService.LogErrorAsync(path);
				return NotFound();
            };

            result.Heading = section.Heading;
            result.MetaTitle = section.MetaTitle;
            result.MetaDescription = section.MetaKeys;
            result.MetaKeywords = section.MetaKeys;
            result.Desc1 = section.Desc1;
            result.Desc2 = section.Desc2;

            string? code = null;

            if (!string.IsNullOrEmpty(section.Desc1))
            {
                var match = Regex.Match(section.Desc1, @"##(.*?)##");
                if (match.Success)
                {
                    code = match.Groups[1].Value;
                    result.PlaceholderCode = code;

                    result.Desc1 = section.Desc1.Replace($"##{code}##", "[[CRITERIA_PLACEHOLDER]]");
                }
            }

            if (!string.IsNullOrEmpty(code))
            {
                var matchedCriteria = await _context.TblDateSheetcriteria
                    .FirstOrDefaultAsync(x => x.DatesheetCode.Replace("#", "").Replace("##", "") == code);

                if (matchedCriteria != null)
                {
                    var board = await _context.Boards.FirstOrDefaultAsync(b => b.Id == matchedCriteria.BoardId);
                    if (board != null)
                    {
                        result.BoardName = board.Name;
                        result.BoardImage = board.ImageName;
                    }

                    var archive = await _context.TblDateSheetCriteriaChildren
                        .Where(x => x.DateSheetHeadingId == matchedCriteria.Id)
                        .OrderBy(x => x.SortOrder)
                        .FirstOrDefaultAsync();

                    if (archive != null)
                    {
                        result.CriteriaHeading = archive.Subheading;
                        result.ArchiveNote = archive.Note;
                        result.ExpectedDate = archive.ExpectedDate;
                        result.ViewOnline = archive.ViewOnline;
                    }
                }
            }

            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            return View("GetDateSheetDataByUrl", result);
        }



    }
}
