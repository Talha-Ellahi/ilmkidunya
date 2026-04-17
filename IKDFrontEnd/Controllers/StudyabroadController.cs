using IKDFrontEnd.DBCollege;
using IKDFrontEnd.Models;
using IKDFrontEnd.Services;
using IKDFrontEnd.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;

namespace IKDFrontEnd.Controllers
{
    public class StudyabroadController : Controller
    {
        private readonly DbikdContext _context;
        private readonly BannerService _bannerService;
        private readonly CmsRepository _cmsRepo;
		private readonly DbCollegeContext _contextCollege;
		public StudyabroadController(DbikdContext context, BannerService bannerService, CmsRepository cmsRepo, DbCollegeContext contextCollege)
		{
			_context = context;
			_bannerService = bannerService;
			_cmsRepo = cmsRepo;
			_contextCollege = contextCollege;
		}

		[HttpGet("studyabroad")]
        public async Task<IActionResult> Home(string guideSlug) 
        {
            var sectionData = await _context.Tblpagewisecontents
                .Where(s => s.SectionId == 55)
                .FirstOrDefaultAsync();

            if (sectionData == null)
            {
                return NotFound();
            }

            var guideDetail = new GuideDetailViewModel
            {
                Id = sectionData.Id,
                Heading = sectionData.Heading,
                HeadingDesc = sectionData?.MetaDesc,
                MetaTitle = sectionData?.MetaTitle,
                MetaDesc = sectionData?.MetaDesc,
                MetaKeyword = sectionData?.MetaKeyword,
                Url = sectionData.Url,
                Detail = sectionData?.Detail,
                Detail2 = sectionData?.DetailShort
            };
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;
            return View("Home", guideDetail);
        }

		[HttpGet("studyabroad/{detailSlug}")]
		public async Task<IActionResult> Detail(string detailSlug)
		{
			// 1. Find SectionTypeImport by URL
			var sectionType = await _contextCollege.SectionTypeImports
				.FirstOrDefaultAsync(x => x.Url.Contains(detailSlug));
			if (sectionType == null)
			{
				return NotFound();
			}
			// 2. Use its Id to fetch SectionContentImport
			var sectionContent = await _contextCollege.SectionContentImports
				.FirstOrDefaultAsync(x => x.ContentId == sectionType.Id);

			if (sectionContent == null)
			{
				return NotFound();
			}

			// 3. Build view model
			var guideDetail = new GuideDetailViewModel
			{
				Heading = sectionContent.Heading,
				HeadingDesc = sectionContent.MetaDesc,
				MetaTitle = sectionContent.MetaTitle,
				MetaDesc = sectionContent.MetaDesc,
				MetaKeyword = sectionContent.MetaKeyword,
				Url = sectionType.Url,
				Detail = sectionContent.Detail,
				Detail2 = sectionContent.Detail2
			};
			// 4. Country logic (unchanged but safer)
			var countryUrl = detailSlug
				.Replace("study-abroad-in-", "")
				.Replace(".aspx", "")
				.Split('-')[0];

			var country = await _context.TblDefCountries
				.Where(c => c.Url == countryUrl)
				.Select(c => new
				{
					c.CountryName,
					c.Url,
					c.ImageUrl
				})
				.FirstOrDefaultAsync();

			ViewBag.Country = country ?? new
			{
				CountryName = "Not Available",
				Url = "Not Available",
				ImageUrl = "/images/not-available.png"
			};

			ViewBag.Banners = await _bannerService.GetBannersAsync();

			return View("Detail", guideDetail);
			
		}

		//     [HttpGet("studyabroad/{detailSlug}")]
		//     public async Task<IActionResult> Detail(string detailSlug)
		//     {
		//var sectionContent = await _contextCollege.TblAllGuidesCms
		//		  .Where(c => c.Url == "/studyabroad/" + detailSlug)
		//		  .FirstOrDefaultAsync();
		//if (sectionContent == null)
		//         {
		//             var slug = detailSlug.Split("-")[0];
		//             var guideUrl = $"https://www.ilmkidunya.com/{slug}";

		//	var guide = await _contextCollege.TblGuidesDefinations.FirstOrDefaultAsync(g => g.GuideMainUrl == guideUrl || g.GuideMainUrl == guideUrl + '/');
		//	if (guide != null)
		//             {
		//                 string redirectUrl = $"/{slug}/{detailSlug}";
		//                 return Redirect(redirectUrl);
		//             }
		//             return NotFound();
		//         }

		//         var guideDetail = new GuideDetailViewModel
		//         {

		//             Heading = sectionContent?.Heading,
		//             HeadingDesc = sectionContent?.MetaDesc,
		//             MetaTitle = sectionContent?.MetaTitle,
		//             MetaDesc = sectionContent?.MetaDesc,
		//             MetaKeyword = sectionContent?.MetaKeys,
		//             Url = sectionContent?.Url,
		//             Detail = sectionContent?.Desc1,
		//             Detail2 = sectionContent?.Desc2
		//         };


		//          var   url = detailSlug.Replace("study-abroad-in-", "")
		//                  .Replace(".aspx", "");
		//         var countryUrl = url.Split('-')[0];
		//         var country = _context.TblDefCountries
		//                .Where(c => c.Url == countryUrl)
		//                .Select(c => new {
		//                    c.CountryName,
		//                    c.Url,
		//                    c.ImageUrl
		//                })
		//                .FirstOrDefault();

		//         if (country != null)
		//         {
		//             ViewBag.Country = country;
		//         }
		//         else
		//         {
		//             ViewBag.Country = new
		//             {
		//                 CountryName = "Not Available",
		//                 Url = "Not Available",
		//                 ImageUrl = "/images/not-available.png"
		//             };
		//         }

		//         var banners = await _bannerService.GetBannersAsync();
		//         ViewBag.Banners = banners;
		//         return View("Detail", guideDetail);
		//     }


		[Route("studyabroad/country-guides.aspx")]
        public async Task<IActionResult> CountryGuideAllCountries(string guideSlug)
        {

            var sectionData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/studyabroad/country-guides.aspx");
            if (sectionData == null)
            {
                return NotFound();
            }

            var guideDetail = new GuideDetailViewModel
            {
                Id = sectionData.Id,
                Heading = sectionData?.Heading,
                HeadingDesc = sectionData?.MetaDesc,
                MetaTitle = sectionData?.MetaTitle,
                MetaDesc = sectionData?.MetaDesc,
                MetaKeyword = sectionData?.MetaKeys,
                Url = sectionData.Url,
                Detail = sectionData?.Desc1,
                Detail2 = sectionData?.Desc2
            };



            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;
           
            return View("CountryGuideAllCountries", guideDetail);

        }
    }

}
