using IKDFrontEnd.Models;
using IKDFrontEnd.Services;
using IKDFrontEnd.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace IKDFrontEnd.Controllers
{
    //[Route("{guideSlug}")]
    public class CmsGuideController : Controller
    {
        private readonly DbikdContext _context;
        private readonly BannerService _bannerService;

        public CmsGuideController(DbikdContext context, BannerService bannerService)
        {
            _context = context;
            _bannerService = bannerService;
        }

     


        //[HttpGet("")]
        public async Task<IActionResult> Index(string guideSlug)
        {
            if (string.IsNullOrEmpty(guideSlug))
                return NotFound();

            GuideDetailViewModel? guideDetail = null;

            // ✅ Try from TblGuides and TblGuideDetails
            guideDetail = (from g in _context.TblGuides
                           join d in _context.TblGuideDetails on g.Id equals d.GuideId
                           where g.Url != null && g.Url.Replace("/", "") == guideSlug
                           orderby d.SortOrder
                           select new GuideDetailViewModel
                           {
                               Id = d.GuideId,
                               Heading = d.Heading,
                               HeadingDesc = d.HeadingDesc,
                               MetaTitle = d.MetaTitle,
                               MetaDesc = d.MetaDesc,
                               MetaKeyword = d.MetaKeyword,
                               Url = d.Url,
                               Detail = d.Detail,
                               Detail2 = d.Detail2
                           }).FirstOrDefault();

            if (guideDetail?.Id != null)
            {
                await SetPageUrlsInViewBag(guideDetail.Id ?? 0);
            }

            // ✅ If guideDetail still null, try section mapping
            if (guideDetail == null)
            {
                var sectionIdMapping = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "10th-class", 128 }, { "11th-class", 131 }, { "12th-class", 132 },
            { "5th-class", 129 }, { "8th-class", 130 }, { "9th-class", 127 },
            { "a-level", 76 }, { "acca", 95 }, { "ba", 80 }, { "bachelor-education", 104 },
            { "bba", 86 }, { "bcom", 91 }, { "bds", 83 }, { "bs-masscommunication", 77 },
            { "bsc", 90 }, { "bscs", 84 }, { "ca", 93 }, { "cgma", 161 }, { "dae", 101 },
            { "dpt", 112 }, { "ecat", 38 }, { "fa", 99 }, { "fpsc", 188 },
            { "fsc-pre-engineering", 97 }, { "fsc-pre-medical", 98 }, { "gmat", 41 },
            { "gre", 42 }, { "ics", 113 }, { "ielts", 43 }, { "issb", 45 },
            { "lat", 183 }, { "llb", 89 }, { "ma", 169 }, { "ma-english", 166 },
            { "ma-history", 168 }, { "ma-islamiat", 163 }, { "ma-philosophy", 167 },
            { "ma-political-science", 162 }, { "ma-punjabi", 164 }, { "ma-urdu", 165 },
            { "mba", 87 }, { "mbbs", 82 }, { "mcat", 46 }, { "mscs", 85 },
            { "nust-net-entry-test", 182 }, { "o-level", 75 }, { "pharm-d", 84 },
            { "pms", 50 }, { "ppsc", 192 }, { "sat", 52 }, { "spsc", 193 },
            { "toefl", 58 }, { "studyabroad", 55 }
        };

                if (!sectionIdMapping.TryGetValue(guideSlug, out int sectionTypeId))
                {
                    // ✅ Try from TblUrlcontentMigrates (only if not found in section mapping)
                    var urlContentPages = await _context.TblUrlcontentMigrates
                        .Where(s => s.Url == $"https://www.ilmkidunya.com/{guideSlug}/")
                        .FirstOrDefaultAsync();

                    if (urlContentPages != null)
                    {
                        guideDetail = new GuideDetailViewModel
                        {
                            Id = urlContentPages.Id,
                            Heading = urlContentPages.PageName,
                            HeadingDesc = urlContentPages.MetaDescription,
                            MetaTitle = urlContentPages.MetaTitle,
                            MetaDesc = urlContentPages.MetaDescription,
                            MetaKeyword = urlContentPages.MetaKeywords,
                            Url = urlContentPages.Url,
                            Detail = urlContentPages.Content1,
                            Detail2 = urlContentPages.Content2
                        };

                        ViewBag.Banners = await _bannerService.GetBannersAsync();
                        ViewBag.HomeUrl = guideSlug;
                        return View("UrlContentHome", guideDetail); // ✅ Render separate view
                    }

                    return NotFound();
                }

                // ✅ Load from Tblpagewisecontents if section mapping found
                var sectionData = await _context.Tblpagewisecontents
                    .Where(s => s.SectionId == sectionTypeId)
                    .FirstOrDefaultAsync();

                if (sectionData == null)
                {
                    return NotFound();
                }

                guideDetail = new GuideDetailViewModel
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

                await SetPageUrlsInViewBag2(sectionData.SectionId, guideSlug);
            }

            ViewBag.Banners = await _bannerService.GetBannersAsync();
            ViewBag.HomeUrl = guideSlug;
            return View("Index", guideDetail); // Default view
        }


        //[HttpGet("{detailSlug1}/{slug2?}")]
        public async Task<IActionResult> GuidesDetail(string guideSlug, string detailSlug1, string? slug2)
        {
            string detailSlug = (detailSlug1 + "/" + slug2).ToLower().Trim('/');

            // ✅ 1. Check in TblUrlcontentMigrates
            var urlContentPages = await _context.TblUrlcontentMigrates
                .Where(s => s.Url == $"https://www.ilmkidunya.com/{guideSlug}/{detailSlug}")
                .FirstOrDefaultAsync();

            if (urlContentPages != null)
            {
                var guideDetail = new GuideDetailViewModel
                {
                    Id = urlContentPages.Id,
                    Heading = urlContentPages.PageName,
                    HeadingDesc = urlContentPages.MetaDescription,
                    MetaTitle = urlContentPages.MetaTitle,
                    MetaDesc = urlContentPages.MetaDescription,
                    MetaKeyword = urlContentPages.MetaKeywords,
                    Url = urlContentPages.Url,
                    Detail = urlContentPages.Content1,
                    Detail2 = urlContentPages.Content2
                };

                ViewBag.GuideSlug = guideSlug;
                ViewBag.DetailSlug = detailSlug;
                ViewBag.HomeUrl = guideSlug;
                ViewBag.Banners = await _bannerService.GetBannersAsync();

                // ✅ Render separate view for URL content
                return View("UrlContentDetail", guideDetail);
            }

            // ✅ 2. Fallback to guide logic
            GuideDetailViewModel? guideDetailFallback = null;

            var guideId = await _context.TblGuides
                .Where(g => g.Url != null && g.Url.Replace("/", "") == guideSlug)
                .Select(g => g.Id)
                .FirstOrDefaultAsync();

            var firstKeyword = detailSlug.Split('-')[0].ToLower();

            if (guideId != 0)
            {
                guideDetailFallback = await _context.TblGuideDetails
                    .Where(d => d.GuideId == guideId &&
                                d.Url != null &&
                                d.Url.ToLower().Contains(firstKeyword))
                    .Select(d => new GuideDetailViewModel
                    {
                        Id = d.GuideId,
                        Heading = d.Heading,
                        HeadingDesc = d.HeadingDesc,
                        MetaTitle = d.MetaTitle,
                        MetaDesc = d.MetaDesc,
                        MetaKeyword = d.MetaKeyword,
                        Url = d.Url,
                        Detail = d.Detail,
                        Detail2 = d.Detail2
                    })
                    .FirstOrDefaultAsync();

                if (guideDetailFallback?.Id != null)
                {
                    await SetPageUrlsInViewBag(guideDetailFallback.Id ?? 0);
                }
            }
            else
            {
                var sectionIdMapping = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "10th-class", 128 }, { "11th-class", 131 }, { "12th-class", 132 }, { "5th-class", 129 },
            { "8th-class", 130 }, { "9th-class", 127 }, { "a-level", 76 }, { "acca", 95 },
            { "ba", 80 }, { "bachelor-education", 104 }, { "bba", 86 }, { "bcom", 91 },
            { "bds", 83 }, { "bs-masscommunication", 77 }, { "bsc", 90 }, { "bscs", 84 },
            { "ca", 93 }, { "cgma", 161 }, { "dae", 101 }, { "dpt", 112 }, { "ecat", 38 },
            { "fa", 99 }, { "fpsc", 188 }, { "fsc-pre-engineering", 97 }, { "fsc-pre-medical", 98 },
            { "gmat", 41 }, { "gre", 42 }, { "ics", 113 }, { "ielts", 43 }, { "issb", 45 },
            { "lat", 183 }, { "llb", 89 }, { "ma", 169 }, { "ma-english", 166 }, { "ma-history", 168 },
            { "ma-islamiat", 163 }, { "ma-philosophy", 167 }, { "ma-political-science", 162 },
            { "ma-punjabi", 164 }, { "ma-urdu", 165 }, { "mba", 87 }, { "mbbs", 82 },
            { "mcat", 46 }, { "mscs", 85 }, { "nust-net-entry-test", 182 }, { "o-level", 75 },
            { "pharm-d", 84 }, { "pms", 50 }, { "ppsc", 192 }, { "sat", 52 }, { "spsc", 193 },
            { "toefl", 58 }, { "studyabroad", 55 }
        };

                if (!sectionIdMapping.TryGetValue(guideSlug, out int sectionTypeId))
                {
                    return NotFound();
                }

                var sectionData = await _context.SectionTypeImports
                    .Where(s => s.SectionId == sectionTypeId &&
                                s.Url != null &&
                                s.Url == detailSlug)
                    .FirstOrDefaultAsync();

                if (sectionData == null)
                {
                    return NotFound();
                }

                var sectionContent = await _context.SectionContentImports
                    .Where(c => c.ContentId == sectionData.Id)
                    .FirstOrDefaultAsync();

                guideDetailFallback = new GuideDetailViewModel
                {
                    Id = sectionData.Id,
                    Heading = sectionContent?.Heading,
                    HeadingDesc = sectionContent?.MetaDesc,
                    MetaTitle = sectionContent?.MetaTitle,
                    MetaDesc = sectionContent?.MetaDesc,
                    MetaKeyword = sectionContent?.MetaKeyword,
                    Url = sectionData.Url,
                    Detail = sectionContent?.Detail,
                    Detail2 = sectionContent?.OtherDetail
                };

                await SetPageUrlsInViewBag2(sectionData.SectionId, guideSlug);
            }

            if (guideDetailFallback == null)
            {
                return NotFound("Detail not found.");
            }

            ViewBag.GuideSlug = guideSlug;
            ViewBag.DetailSlug = detailSlug;
            ViewBag.HomeUrl = guideSlug;
            ViewBag.Banners = await _bannerService.GetBannersAsync();

            return View("GuidesDetail", guideDetailFallback);
        }







        private async Task SetPageUrlsInViewBag(int guideId)
        {
            var pages = await _context.TblGuideDetails
                .Where(p => p.GuideId == guideId)
                .Select(p => p.Url)
                .ToListAsync();

            ViewBag.FeeUrl = pages.FirstOrDefault(p => p.Contains("fee"));
            ViewBag.AdmissionsUrl = pages.FirstOrDefault(p => p.Contains("admission"));
            ViewBag.MeritUrl = pages.FirstOrDefault(p => p.Contains("merit"));
            ViewBag.SubjectsUrl = pages.FirstOrDefault(p => p.Contains("subjects"));
            ViewBag.UniversitiesUrl = pages.FirstOrDefault(p => p.Contains("universities"));
            ViewBag.ScopeUrl = pages.FirstOrDefault(p => p.Contains("scope"));
            ViewBag.SalaryUrl = pages.FirstOrDefault(p => p.Contains("salary"));
            ViewBag.JobsUrl = pages.FirstOrDefault(p => p.Contains("jobs"));
            ViewBag.PastPaperUrl = pages.FirstOrDefault(p => p.Contains("papers"));
            ViewBag.EligibilityUrl = pages.FirstOrDefault(p => p.Contains("eligibility"));
            ViewBag.SyllabusUrl = pages.FirstOrDefault(p => p.Contains("syllabus"));
            ViewBag.StudyAbroadUrl = pages.FirstOrDefault(p => p.Contains("Study-Abroad"));

        }

        private async Task SetPageUrlsInViewBag2(int? guideId, string guideSlug)
        {
            var pages = await _context.SectionTypeImports
                .Where(s => s.SectionId == guideId)
                .Select(p => p.Url)
                .ToListAsync();

            Func<string?, string?> formatUrl = (url) => url != null ? $"/{guideSlug}/{url.TrimStart('/')}" : null;

            ViewBag.FeeUrl = formatUrl(pages.FirstOrDefault(p => p.Contains("fee")));
            ViewBag.AdmissionsUrl = formatUrl(pages.FirstOrDefault(p => p.Contains("admission")));
            ViewBag.MeritUrl = formatUrl(pages.FirstOrDefault(p => p.Contains("merit")));
            ViewBag.SubjectsUrl = formatUrl(pages.FirstOrDefault(p => p.Contains("subjects")));
            ViewBag.UniversitiesUrl = formatUrl(pages.FirstOrDefault(p => p.Contains("universities")));
            ViewBag.ScopeUrl = formatUrl(pages.FirstOrDefault(p => p.Contains("scope")));
            ViewBag.SalaryUrl = formatUrl(pages.FirstOrDefault(p => p.Contains("salary")));
            ViewBag.JobsUrl = formatUrl(pages.FirstOrDefault(p => p.Contains("jobs")));
            ViewBag.PastPaperUrl = formatUrl(pages.FirstOrDefault(p => p.Contains("papers")));
            ViewBag.EligibilityUrl = formatUrl(pages.FirstOrDefault(p => p.Contains("eligibility")));
            ViewBag.SyllabusUrl = formatUrl(pages.FirstOrDefault(p => p.Contains("syllabus")));
            ViewBag.StudyAbroadUrl = formatUrl(pages.FirstOrDefault(p => p.Contains("Study-Abroad")));
        }




    }


}
