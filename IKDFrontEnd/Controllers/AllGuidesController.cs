using DinkToPdf;
using DinkToPdf.Contracts;
using HtmlAgilityPack;
using IKDFrontEnd.Models;
using IKDFrontEnd.Services;
using IKDFrontEnd.ViewModels;
using IKDFrontEnd.ViewModels.AllGuidesViewModel;
using IKDFrontEnd.ViewModels.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using static System.Net.Mime.MediaTypeNames;



namespace IKDFrontEnd.Controllers
{

    public class AllGuidesController : Controller
    {
        private readonly DbikdContext _context;
        private readonly BannerService _bannerService;
        private readonly CmsRepository _cmsRepo;

        private readonly IFtpService _ftpService;

        private readonly ICompositeViewEngine _viewEngine;

        public AllGuidesController(
            DbikdContext context,
            BannerService bannerService,
            CmsRepository cmsRepo,
            ICompositeViewEngine viewEngine, // new
            IFtpService ftpService)
        {
            _context = context;
            _bannerService = bannerService;
            _cmsRepo = cmsRepo;
            _ftpService = ftpService;
            _viewEngine = viewEngine;
        }

        protected ICompositeViewEngine ViewEngine => _viewEngine;

        [HttpGet("roll-number-slips")]
        [HttpGet("roll-number-slips/{url}")]
        [HttpGet("roll-number-slips/{url}.aspx")]
        public async Task<IActionResult> RollNoSlip(string url)
        {
            ViewBag.Banners = await _bannerService.GetBannersAsync();

            var baseUrl = "https://www.ilmkidunya.com/roll-number-slips";
            TblCmsDto cmsData = null;

            // 1️⃣ Try CMS without .aspx
            var cmsUrl = string.IsNullOrWhiteSpace(url)
                ? $"{baseUrl}/"
                : $"{baseUrl}/{url}";

            cmsData = await _cmsRepo.GetByUrlAsync(cmsUrl);

            // 2️⃣ Try CMS with .aspx
            if (cmsData == null && !string.IsNullOrWhiteSpace(url))
            {
                cmsData = await _cmsRepo.GetByUrlAsync($"{baseUrl}/{url}.aspx");
            }

            // 3️⃣ Fallback to SectionContent
            if (cmsData == null && !string.IsNullOrWhiteSpace(url))
            {
                var sectionContent = await (
                    from sc in _context.SectionContentImports.AsNoTracking()
                    join st in _context.SectionTypeImports.AsNoTracking()
                        on sc.ContentId equals st.Id
                    where st.Url == url + ".aspx"
                    select new
                    {
                        sc.Detail,
                        sc.Detail2,
                        sc.OtherDetail,
                        sc.MetaDesc,
                        sc.Heading,
                        sc.MetaKeyword,
                        sc.MetaTitle
                    }
                ).FirstOrDefaultAsync();

                if (sectionContent == null)
                {
                    return NotFound();
                }

                cmsData = new TblCmsDto
                {
                    Desc1 = sectionContent.Detail,
                    Desc2 = sectionContent.Detail2,
                    Desc3 = sectionContent.OtherDetail,
                    MetaDesc = sectionContent.MetaDesc,
                    Heading = sectionContent.Heading,
                    MetaKeys = sectionContent.MetaKeyword,
                    MetaTitle = sectionContent.MetaTitle
                };
            }

            ViewBag.CmsData = cmsData;

            return View();
        }


        [Route("ms-statistics")]
        [Route("10th-class")]
        [Route("11th-class")]
        [Route("12th-class")]
        [Route("5th-class")]
        [Route("6th-class")]
        [Route("7th-class")]
        [Route("8th-class")]
        [Route("9th-class")]
        [Route("acca")]
        [Route("acma")]
        [Route("aiou")]
        [Route("ajkpsc")]
        [Route("a-level")]
        [Route("ba")]
        [Route("bachelor-education")]
        [Route("barch")]
        [Route("bba")]
        [Route("bcom")]
        [Route("bds")]
        [Route("be-textile-engg")]
        [Route("bfa")]
        [Route("bhms")]
        [Route("bid")]
        [Route("bisp")]
        [Route("bpsc")]
        [Route("bs")]
        [Route("bs-accounting-and-finance")]
        [Route("bs-agriculture")]
        [Route("bs-arabic")]
        [Route("bs-archaeology")]
        [Route("bs-archeology")]
        [Route("bs-architect")]
        [Route("bs-aviation")]
        [Route("bs-banking-and-finance")]
        [Route("bs-biotech")]
        [Route("bs-botany")]
        [Route("bs-business-administration")]
        [Route("bsc")]
        [Route("bs-chemical-engg")]
        [Route("bs-chemistry")]
        [Route("bs-city-and-regional-planning")]
        [Route("bs-civil-engg")]
        [Route("bs-commerce")]
        [Route("bs-communication-and-design")]
        [Route("bs-computer-engg")]
        [Route("bs-criminology")]
        [Route("bscs")]
        [Route("bs-cultural-studies")]
        [Route("bs-homeopathic-and-medical-sciences")]
        [Route("bs-anthropology")]
        [Route("bs-biomedical-engineering")]
        [Route("bs-dental-technology")]
        [Route("bs-dermatology")]
        [Route("bs-disaster-management")]
        [Route("bs-economics")]
        [Route("bs-education")]
        [Route("bs-electrical-engg")]
        [Route("bs-english")]
        [Route("bs-english-literature")]
        [Route("bs-environmental-science")]
        [Route("bs-fashion-designing")]
        [Route("bs-food-science-and-technology")]
        [Route("bs-forensic-science")]
        [Route("bs-forestry")]
        [Route("bs-gender-studies")]
        [Route("bs-genetics")]
        [Route("bs-geography")]
        [Route("bs-geology")]
        [Route("bs-graphic-designing")]
        [Route("bs-gynecology")]
        [Route("bs-history")]
        [Route("bshnd")]
        [Route("bs-home-economics")]
        [Route("bs-human-anatomy")]
        [Route("bs-int-relations")]
        [Route("bs-islamic-studies")]
        [Route("bsit")]
        [Route("bs-library-and-information-sciences")]
        [Route("bs-managment")]
        [Route("bs-marine-sciences")]
        [Route("bs-masscommunication")]
        [Route("bs-mass-communication")]
        [Route("bs-mathematics")]
        [Route("bs-mechanical-engg")]
        [Route("bs-media-and-communication-studies")]
        [Route("bs-microbiology")]
        [Route("bs-multimedia-designing")]
        [Route("bsn")]
        [Route("bs-ott")]
        [Route("bspa")]
        [Route("bs-pakistan-studies")]
        [Route("bs-pathology")]
        [Route("bs-peace-and-conflict-studies")]
        [Route("bs-philosophy")]
        [Route("bs-physical-education")]
        [Route("bs-physics")]
        [Route("bs-political-science")]
        [Route("bs-poultry")]
        [Route("bs-psychology")]
        [Route("bs-public-health")]
        [Route("bs-radiology")]
        [Route("bs-social-work")]
        [Route("bs-sociology")]
        [Route("bs-software-engg")]
        [Route("bs-space-science")]
        [Route("bs-special-education")]
        [Route("bs-statistics")]
        [Route("bs-textile-engineering")]
        [Route("bs-textile-management-and-technology")]
        [Route("bs-textiles")]
        [Route("bsth")]
        [Route("bs-urdu")]
        [Route("bs-zoology")]
        [Route("btech")]
        [Route("business")]
        [Route("ca")]
        [Route("cars")]
        [Route("cgma")]
        [Route("cricket")]
        [Route("css-pakistan")]
        [Route("culture")]
        [Route("dae")]
        [Route("dnd")]
        [Route("dpt")]
        [Route("dvm")]
        [Route("ecat")]
        [Route("ehsaas-program")]
        [Route("entertainment")]
        [Route("fa")]
        [Route("features")]
        [Route("flights")]
        [Route("food")]
        [Route("fpsc")]
        [Route("fsc-pre-engineering")]
        [Route("fsc-pre-medical")]
        [Route("gat")]
        [Route("gk")]
        [Route("gmat")]
        [Route("gre")]
        [Route("hat")]
        [Route("health")]
        [Route("hec-law-gat")]
        [Route("icom")]
        [Route("ics")]
        [Route("ielts")]
        [Route("islam")]
        [Route("issb")]
        [Route("kppsc")]
        [Route("lat")]
        [Route("lifestyle")]
        [Route("llb")]
        [Route("llm")]
        [Route("ma")]
        [Route("ma-english")]
        [Route("ma-history")]
        [Route("ma-islamiat")]
        [Route("ma-philosophy")]
        [Route("ma-political-science")]
        [Route("ma-punjabi")]
        [Route("master-in-public-health")]
        [Route("ma-urdu")]
        [Route("mba")]
        [Route("mbbs")]
        [Route("mcat")]
        [Route("med")]
        [Route("mit")]
        [Route("mobiles")]
        [Route("mpa")]
        [Route("mphil")]
        [Route("mphil-sociology")]
        [Route("mphil-statistics")]
        [Route("ms")]
        [Route("ms-accounting-and-finance")]
        [Route("ms-aerospace-engg")]
        [Route("ms-arabic")]
        [Route("ms-archaeology")]
        [Route("ms-architecture")]
        [Route("ms-artificial-intelligence")]
        [Route("ms-aviation")]
        [Route("ms-biochemistry")]
        [Route("ms-biotechnology")]
        [Route("ms-botany")]
        [Route("ms-mechanical-engg")]
        [Route("msc-agriculture")]
        [Route("ms-chemical-engineering")]
        [Route("ms-chemistry")]
        [Route("ms-civil-engineering")]
        [Route("ms-conflict-and-peace-studies")]
        [Route("ms-criminology")]
        [Route("mscs")]
        [Route("ms-economics")]
        [Route("ms-electrical-engg")]
        [Route("ms-environmental-science")]
        [Route("ms-finance")]
        [Route("ms-food-science-and-technology")]
        [Route("ms-genetics")]
        [Route("ms-geography")]
        [Route("ms-hrm")]
        [Route("ms-interior-design")]
        [Route("ms-int-relation")]
        [Route("ms-it")]
        [Route("ms-library-science")]
        [Route("ms-management-sciences")]
        [Route("ms-mass-communications")]
        [Route("ms-mathematics")]
        [Route("ms-microbiology")]
        [Route("ms-pakistan-studies")]
        [Route("msph")]
        [Route("ms-physical-education")]
        [Route("ms-physics")]
        [Route("ms-political-sciences")]
        [Route("ms-psychology")]
        [Route("msse")]
        [Route("ms-special-education")]
        [Route("msth")]
        [Route("ms-zoology")]
        [Route("nat")]
        [Route("nts")]
        [Route("nust-net-entry-test")]
        [Route("o-level")]
        [Route("pharm-d")]
        [Route("phd")]
        [Route("pm-laptop-scheme")]
        [Route("pms")]
        [Route("pm-youth-loan-scheme")]
        [Route("politics")]
        [Route("ppsc")]
        [Route("pte")]
        [Route("safee-muhmmad-complete-guide.aspx")]
        [Route("sat")]
        [Route("s-biomedical-engineering")]
        [Route("s-biomedical-engineeringadmissions")]
        [Route("s-biomedical-engineeringfee")]
        [Route("s-biomedical-engineeringmerit-list")]
        [Route("shahzad-testing")]
        [Route("sindh-medical-colleges-universities-entry-test")]
        [Route("sports")]
        [Route("spsc")]
        [Route("studyabroad")]
        [Route("tech")]
        [Route("toefl")]
        [Route("travel")]
        [Route("umrah-packages")]
        [Route("usat")]
        [Route("visa")]
        [Route("world")]
        public async Task<IActionResult> GuidesView()
        {
            var Url = HttpContext.Request.Path.Value?.Trim('/') ?? "";
            string htmlFileName = Url.Replace('/', '-') + ".html"; // e.g., "10th-class.html"
            string remoteFilePath = $"files/guides/{htmlFileName}";
            string publicUrl = _ftpService.GetPublicUrl(remoteFilePath); // not used for redirect, but for logging

            // Check if file exists on FTP
            bool fileExists = await _ftpService.FileExistsAsync(remoteFilePath);
            if (fileExists)
            {
                try
                {
                    string existingHtml = await _ftpService.DownloadFileAsync(remoteFilePath);
                    return Content(existingHtml, "text/html");
                }
                catch (Exception ex)
                {
                    // Log and fall through to regeneration
                }
            }

            // --- Existing logic to build the view model ---
            ViewBag.HideHeaderLowerBanner = true;

            var detail = await _context.TblAllGuidesCms
                .Where(g => g.Url.Trim().Replace("/", "") == Url && g.IsActive == true)
                .Select(g => new GuideDetailViewModel
                {
                    Heading = string.IsNullOrWhiteSpace(g.Heading) ? "Not Available" : g.Heading,
                    HeadingDesc = string.IsNullOrWhiteSpace(g.MetaDesc) ? "Not Available" : g.MetaDesc,
                    MetaTitle = string.IsNullOrWhiteSpace(g.MetaTitle) ? "Not Available" : g.MetaTitle,
                    MetaDesc = string.IsNullOrWhiteSpace(g.MetaDesc) ? "Not Available" : g.MetaDesc,
                    MetaKeyword = string.IsNullOrWhiteSpace(g.MetaKeys) ? "Not Available" : g.MetaKeys,
                    Url = string.IsNullOrWhiteSpace(g.Url) ? "Not Available" : g.Url,
                    Detail = string.IsNullOrWhiteSpace(g.Desc1) ? "Not Available" : g.Desc1,
                    Detail2 = string.IsNullOrWhiteSpace(g.Desc2) ? "Not Available" : g.Desc2
                })
                .FirstOrDefaultAsync();

            if (detail == null) return NotFound();

            var menuItems = await _context.TblAllGuidesCms
                .Where(g => g.ShowInMenu == true && g.IsActive == true
                    && g.Url.StartsWith("/" + Url + "/")
                    && !string.IsNullOrEmpty(g.Url)
                    && !string.IsNullOrEmpty(g.HeaderIcon)
                    && !string.IsNullOrEmpty(g.HeaderName))
                .OrderBy(g => g.SortOrder)
                .Select(g => new MenuItemViewModel
                {
                    Url = g.Url!,
                    HeaderIcon = g.HeaderIcon!,
                    HeaderName = g.HeaderName!
                })
                .ToListAsync();

            var guideUrl = $"https://www.ilmkidunya.com/{Url}";
            var guide = await _context.TblGuidesDefinations
                .FirstOrDefaultAsync(g => g.GuideMainUrl == guideUrl || g.GuideMainUrl == guideUrl + '/');

            var viewModel = new GuidePageViewModel
            {
                Guide = guide,
                GuideDetail = detail,
                MenuItems = menuItems
            };

            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;
            ViewBag.HomeUrl = Url;
            ViewBag.Levels = await _context.TblXcourseLevels.AsNoTracking().ToListAsync();
            ViewBag.Cities = await _context.TblDefCities.AsNoTracking().ToListAsync();
            ViewBag.Fields = await _context.CourseCategories.AsNoTracking().ToListAsync();

            ViewBag.ShowWatsupLink = await _context.TblWhatsAppGroups
                .FirstOrDefaultAsync(g => g.GuideName.Contains(Url.ToLower()) && g.IsActive);

            ViewBag.ShowInqueryForm = guide?.ShowInquieryForm ?? false;

            var viewResult = View("GuidesView", viewModel);
            string htmlContent = await RenderViewToStringAsync(viewResult);

            // --- Upload to FTP (fire and forget would be better, but we wait for simplicity) ---
            try
            {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(htmlContent)))
                {
                    await _ftpService.UploadFileAsync(remoteFilePath, stream);
                }
            }
            catch (Exception ex)
            {
                // Append error as visible div
                string errorDiv = $@"
            <div style='background:#ffdddd; border:3px solid #cc0000; border-radius:8px; 
                        padding:20px; margin:20px; font-family:Consolas, monospace; 
                        white-space:pre-wrap; box-shadow:0 4px 8px rgba(0,0,0,0.1);'>
                <h3 style='color:#cc0000; margin-top:0;'>⚠️ Static File Upload Failed</h3>
                <p><strong>Error:</strong> {System.Net.WebUtility.HtmlEncode(ex.Message)}</p>
                <p><strong>Remote path:</strong> {System.Net.WebUtility.HtmlEncode(remoteFilePath)}</p>
                <p><em>The page content is still shown below.</em></p>
            </div>";

                int bodyIndex = htmlContent.IndexOf("<body", StringComparison.OrdinalIgnoreCase);
                if (bodyIndex >= 0)
                {
                    int bodyEndIndex = htmlContent.IndexOf('>', bodyIndex);
                    if (bodyEndIndex >= 0)
                        htmlContent = htmlContent.Insert(bodyEndIndex + 1, errorDiv);
                    else
                        htmlContent = errorDiv + htmlContent;
                }
                else
                {
                    htmlContent = errorDiv + htmlContent;
                }
            }

            return Content(htmlContent, "text/html");
        }

        [HttpGet("ms-mechanical-engg/{url2}/{url3?}")]
        [HttpGet("bs-cultural-studies/{url2}/{url3?}")]
        [HttpGet("bs-homeopathic-and-medical-sciences/{url2}/{url3?}")]
        [HttpGet("bs-anthropology/{url2}/{url3?}")]
        [HttpGet("ms-statistics/{url2}/{url3?}")]
        [HttpGet("bs-biomedical-engineering/{url2}/{url3?}")]
        [HttpGet("10th-class/{url2}/{url3?}")]
        [HttpGet("11th-class/{url2}/{url3?}")]
        [HttpGet("12th-class/{url2}/{url3?}")]
        [HttpGet("5th-class/{url2}/{url3?}")]
        [HttpGet("6th-class/{url2}/{url3?}")]
        [HttpGet("7th-class/{url2}/{url3?}")]
        [HttpGet("8th-class/{url2}/{url3?}")]
        [HttpGet("9th-class/{url2}/{url3?}")]
        [HttpGet("acca/{url2}/{url3?}")]
        [HttpGet("acma/{url2}/{url3?}")]
        [HttpGet("aiou/{url2}/{url3?}")]
        [HttpGet("ajkpsc/{url2}/{url3?}")]
        [HttpGet("a-level/{url2}/{url3?}")]
        [HttpGet("ba/{url2}/{url3?}")]
        [HttpGet("bachelor-education/{url2}/{url3?}")]
        [HttpGet("barch/{url2}/{url3?}")]
        [HttpGet("bba/{url2}/{url3?}")]
        [HttpGet("bcom/{url2}/{url3?}")]
        [HttpGet("bds/{url2}/{url3?}")]
        [HttpGet("be-textile-engg/{url2}/{url3?}")]
        [HttpGet("bfa/{url2}/{url3?}")]
        [HttpGet("bhms/{url2}/{url3?}")]
        [HttpGet("bid/{url2}/{url3?}")]
        [HttpGet("bisp/{url2}/{url3?}")]
        [HttpGet("bpsc/{url2}/{url3?}")]
        [HttpGet("bs/{url2}/{url3?}")]
        [HttpGet("bs-accounting-and-finance/{url2}/{url3?}")]
        [HttpGet("bs-agriculture/{url2}/{url3?}")]
        [HttpGet("bs-arabic/{url2}/{url3?}")]
        [HttpGet("bs-archaeology/{url2}/{url3?}")]
        [HttpGet("bs-archeology/{url2}/{url3?}")]
        [HttpGet("bs-architect/{url2}/{url3?}")]
        [HttpGet("bs-aviation/{url2}/{url3?}")]
        [HttpGet("bs-banking-and-finance/{url2}/{url3?}")]
        [HttpGet("bs-biotech/{url2}/{url3?}")]
        [HttpGet("bs-botany/{url2}/{url3?}")]
        [HttpGet("bs-business-administration/{url2}/{url3?}")]
        [HttpGet("bsc/{url2}/{url3?}")]
        [HttpGet("bs-chemical-engg/{url2}/{url3?}")]
        [HttpGet("bs-chemistry/{url2}/{url3?}")]
        [HttpGet("bs-city-and-regional-planning/{url2}/{url3?}")]
        [HttpGet("bs-civil-engg/{url2}/{url3?}")]
        [HttpGet("bs-commerce/{url2}/{url3?}")]
        [HttpGet("bs-communication-and-design/{url2}/{url3?}")]
        [HttpGet("bs-computer-engg/{url2}/{url3?}")]
        [HttpGet("bs-criminology/{url2}/{url3?}")]
        [HttpGet("bscs/{url2}/{url3?}")]
        [HttpGet("bs-dental-technology/{url2}/{url3?}")]
        [HttpGet("bs-dermatology/{url2}/{url3?}")]
        [HttpGet("bs-disaster-management/{url2}/{url3?}")]
        [HttpGet("bs-economics/{url2}/{url3?}")]
        [HttpGet("bs-education/{url2}/{url3?}")]
        [HttpGet("bs-electrical-engg/{url2}/{url3?}")]
        [HttpGet("bs-english/{url2}/{url3?}")]
        [HttpGet("bs-english-literature/{url2}/{url3?}")]
        [HttpGet("bs-environmental-science/{url2}/{url3?}")]
        [HttpGet("bs-fashion-designing/{url2}/{url3?}")]
        [HttpGet("bs-food-science-and-technology/{url2}/{url3?}")]
        [HttpGet("bs-forensic-science/{url2}/{url3?}")]
        [HttpGet("bs-forestry/{url2}/{url3?}")]
        [HttpGet("bs-gender-studies/{url2}/{url3?}")]
        [HttpGet("bs-genetics/{url2}/{url3?}")]
        [HttpGet("bs-geography/{url2}/{url3?}")]
        [HttpGet("bs-geology/{url2}/{url3?}")]
        [HttpGet("bs-graphic-designing/{url2}/{url3?}")]
        [HttpGet("bs-gynecology/{url2}/{url3?}")]
        [HttpGet("bs-history/{url2}/{url3?}")]
        [HttpGet("bshnd/{url2}/{url3?}")]
        [HttpGet("bs-home-economics/{url2}/{url3?}")]
        [HttpGet("bs-human-anatomy/{url2}/{url3?}")]
        [HttpGet("bs-int-relations/{url2}/{url3?}")]
        [HttpGet("bs-islamic-studies/{url2}/{url3?}")]
        [HttpGet("bsit/{url2}/{url3?}")]
        [HttpGet("bs-library-and-information-sciences/{url2}/{url3?}")]
        [HttpGet("bs-managment/{url2}/{url3?}")]
        [HttpGet("bs-marine-sciences/{url2}/{url3?}")]
        [HttpGet("bs-masscommunication/{url2}/{url3?}")]
        [HttpGet("bs-mass-communication/{url2}/{url3?}")]
        [HttpGet("bs-mathematics/{url2}/{url3?}")]
        [HttpGet("bs-mechanical-engg/{url2}/{url3?}")]
        [HttpGet("bs-media-and-communication-studies/{url2}/{url3?}")]
        [HttpGet("bs-microbiology/{url2}/{url3?}")]
        [HttpGet("bs-multimedia-designing/{url2}/{url3?}")]
        [HttpGet("bsn/{url2}/{url3?}")]
        [HttpGet("bs-ott/{url2}/{url3?}")]
        [HttpGet("bspa/{url2}/{url3?}")]
        [HttpGet("bs-pakistan-studies/{url2}/{url3?}")]
        [HttpGet("bs-pathology/{url2}/{url3?}")]
        [HttpGet("bs-peace-and-conflict-studies/{url2}/{url3?}")]
        [HttpGet("bs-philosophy/{url2}/{url3?}")]
        [HttpGet("bs-physical-education/{url2}/{url3?}")]
        [HttpGet("bs-physics/{url2}/{url3?}")]
        [HttpGet("bs-political-science/{url2}/{url3?}")]
        [HttpGet("bs-poultry/{url2}/{url3?}")]
        [HttpGet("bs-psychology/{url2}/{url3?}")]
        [HttpGet("bs-public-health/{url2}/{url3?}")]
        [HttpGet("bs-radiology/{url2}/{url3?}")]
        [HttpGet("bs-social-work/{url2}/{url3?}")]
        [HttpGet("bs-sociology/{url2}/{url3?}")]
        [HttpGet("bs-software-engg/{url2}/{url3?}")]
        [HttpGet("bs-space-science/{url2}/{url3?}")]
        [HttpGet("bs-special-education/{url2}/{url3?}")]
        [HttpGet("bs-statistics/{url2}/{url3?}")]
        [HttpGet("bs-textile-engineering/{url2}/{url3?}")]
        [HttpGet("bs-textile-management-and-technology/{url2}/{url3?}")]
        [HttpGet("bs-textiles/{url2}/{url3?}")]
        [HttpGet("bsth/{url2}/{url3?}")]
        [HttpGet("bs-urdu/{url2}/{url3?}")]
        [HttpGet("bs-zoology/{url2}/{url3?}")]
        [HttpGet("btech/{url2}/{url3?}")]
        [HttpGet("business/{url2}/{url3?}")]
        [HttpGet("ca/{url2}/{url3?}")]
        [HttpGet("cars/{url2}/{url3?}")]
        [HttpGet("cgma/{url2}/{url3?}")]
        [HttpGet("cricket/{url2}/{url3?}")]
        [HttpGet("css-pakistan/{url2}/{url3?}")]
        [HttpGet("culture/{url2}/{url3?}")]
        [HttpGet("dae/{url2}/{url3?}")]
        [HttpGet("dnd/{url2}/{url3?}")]
        [HttpGet("dpt/{url2}/{url3?}")]
        [HttpGet("dvm/{url2}/{url3?}")]
        [HttpGet("ecat/{url2}/{url3?}")]
        [HttpGet("ehsaas-program/{url2}/{url3?}")]
        [HttpGet("entertainment/{url2}/{url3?}")]
        [HttpGet("fa/{url2}/{url3?}")]
        [HttpGet("features/{url2}/{url3?}")]
        [HttpGet("flights/{url2}/{url3?}")]
        [HttpGet("food/{url2}/{url3?}")]
        [HttpGet("fpsc/{url2}/{url3?}")]
        [HttpGet("fsc-pre-engineering/{url2}/{url3?}")]
        [HttpGet("fsc-pre-medical/{url2}/{url3?}")]
        [HttpGet("gat/{url2}/{url3?}")]
        [HttpGet("gk/{url2}/{url3?}")]
        [HttpGet("gmat/{url2}/{url3?}")]
        [HttpGet("gre/{url2}/{url3?}")]
        [HttpGet("hat/{url2}/{url3?}")]
        [HttpGet("health/{url2}/{url3?}")]
        [HttpGet("hec-law-gat/{url2}/{url3?}")]
        [HttpGet("icom/{url2}/{url3?}")]
        [HttpGet("ics/{url2}/{url3?}")]
        [HttpGet("ielts/{url2}/{url3?}")]
        [HttpGet("islam/{url2}/{url3?}")]
        [HttpGet("issb/{url2}/{url3?}")]
        [HttpGet("kppsc/{url2}/{url3?}")]
        [HttpGet("lat/{url2}/{url3?}")]
        [HttpGet("lifestyle/{url2}/{url3?}")]
        [HttpGet("llb/{url2}/{url3?}")]
        [HttpGet("llm/{url2}/{url3?}")]
        [HttpGet("ma/{url2}/{url3?}")]
        [HttpGet("ma-english/{url2}/{url3?}")]
        [HttpGet("ma-history/{url2}/{url3?}")]
        [HttpGet("ma-islamiat/{url2}/{url3?}")]
        [HttpGet("ma-philosophy/{url2}/{url3?}")]
        [HttpGet("ma-political-science/{url2}/{url3?}")]
        [HttpGet("ma-punjabi/{url2}/{url3?}")]
        [HttpGet("master-in-public-health/{url2}/{url3?}")]
        [HttpGet("ma-urdu/{url2}/{url3?}")]
        [HttpGet("mba/{url2}/{url3?}")]
        [HttpGet("mbbs/{url2}/{url3?}")]
        [HttpGet("mcat/{url2}/{url3?}")]
        [HttpGet("med/{url2}/{url3?}")]
        [HttpGet("mit/{url2}/{url3?}")]
        [HttpGet("mobiles/{url2}/{url3?}")]
        [HttpGet("mpa/{url2}/{url3?}")]
        [HttpGet("mphil/{url2}/{url3?}")]
        [HttpGet("mphil-sociology/{url2}/{url3?}")]
        [HttpGet("mphil-statistics/{url2}/{url3?}")]
        [HttpGet("ms/{url2}/{url3?}")]
        [HttpGet("ms-accounting-and-finance/{url2}/{url3?}")]
        [HttpGet("ms-aerospace-engg/{url2}/{url3?}")]
        [HttpGet("ms-arabic/{url2}/{url3?}")]
        [HttpGet("ms-archaeology/{url2}/{url3?}")]
        [HttpGet("ms-architecture/{url2}/{url3?}")]
        [HttpGet("ms-artificial-intelligence/{url2}/{url3?}")]
        [HttpGet("ms-aviation/{url2}/{url3?}")]
        [HttpGet("ms-biochemistry/{url2}/{url3?}")]
        [HttpGet("ms-biotechnology/{url2}/{url3?}")]
        [HttpGet("ms-botany/{url2}/{url3?}")]
        [HttpGet("msc-agriculture/{url2}/{url3?}")]
        [HttpGet("ms-chemical-engineering/{url2}/{url3?}")]
        [HttpGet("ms-chemistry/{url2}/{url3?}")]
        [HttpGet("ms-civil-engineering/{url2}/{url3?}")]
        [HttpGet("ms-conflict-and-peace-studies/{url2}/{url3?}")]
        [HttpGet("ms-criminology/{url2}/{url3?}")]
        [HttpGet("mscs/{url2}/{url3?}")]
        [HttpGet("ms-economics/{url2}/{url3?}")]
        [HttpGet("ms-electrical-engg/{url2}/{url3?}")]
        [HttpGet("ms-environmental-science/{url2}/{url3?}")]
        [HttpGet("ms-finance/{url2}/{url3?}")]
        [HttpGet("ms-food-science-and-technology/{url2}/{url3?}")]
        [HttpGet("ms-genetics/{url2}/{url3?}")]
        [HttpGet("ms-geography/{url2}/{url3?}")]
        [HttpGet("ms-hrm/{url2}/{url3?}")]
        [HttpGet("ms-interior-design/{url2}/{url3?}")]
        [HttpGet("ms-int-relation/{url2}/{url3?}")]
        [HttpGet("ms-it/{url2}/{url3?}")]
        [HttpGet("ms-library-science/{url2}/{url3?}")]
        [HttpGet("ms-management-sciences/{url2}/{url3?}")]
        [HttpGet("ms-mass-communications/{url2}/{url3?}")]
        [HttpGet("ms-mathematics/{url2}/{url3?}")]
        [HttpGet("ms-microbiology/{url2}/{url3?}")]
        [HttpGet("ms-pakistan-studies/{url2}/{url3?}")]
        [HttpGet("msph/{url2}/{url3?}")]
        [HttpGet("ms-physical-education/{url2}/{url3?}")]
        [HttpGet("ms-physics/{url2}/{url3?}")]
        [HttpGet("ms-political-sciences/{url2}/{url3?}")]
        [HttpGet("ms-psychology/{url2}/{url3?}")]
        [HttpGet("msse/{url2}/{url3?}")]
        [HttpGet("ms-special-education/{url2}/{url3?}")]
        [HttpGet("msth/{url2}/{url3?}")]
        [HttpGet("ms-zoology/{url2}/{url3?}")]
        [HttpGet("nat/{url2}/{url3?}")]
        [HttpGet("nts/{url2}/{url3?}")]
        [HttpGet("nust-net-entry-test/{url2}/{url3?}")]
        [HttpGet("o-level/{url2}/{url3?}")]
        [HttpGet("pharm-d/{url2}/{url3?}")]
        [HttpGet("phd/{url2}/{url3?}")]
        [HttpGet("pm-laptop-scheme/{url2}/{url3?}")]
        [HttpGet("pms/{url2}/{url3?}")]
        [HttpGet("pm-youth-loan-scheme/{url2}/{url3?}")]
        [HttpGet("politics/{url2}/{url3?}")]
        [HttpGet("ppsc/{url2}/{url3?}")]
        [HttpGet("pte/{url2}/{url3?}")]
        [HttpGet("safee-muhmmad-complete-guide.aspx/{url2}/{url3?}")]
        [HttpGet("sat/{url2}/{url3?}")]
        [HttpGet("s-biomedical-engineering/{url2}/{url3?}")]
        [HttpGet("s-biomedical-engineeringadmissions/{url2}/{url3?}")]
        [HttpGet("s-biomedical-engineeringfee/{url2}/{url3?}")]
        [HttpGet("s-biomedical-engineeringmerit-list/{url2}/{url3?}")]
        [HttpGet("shahzad-testing/{url2}/{url3?}")]
        [HttpGet("sindh-medical-colleges-universities-entry-test/{url2}/{url3?}")]
        [HttpGet("sports/{url2}/{url3?}")]
        [HttpGet("spsc/{url2}/{url3?}")]
        [HttpGet("studyabroad/{url2}/{url3?}")]
        [HttpGet("tech/{url2}/{url3?}")]
        [HttpGet("toefl/{url2}/{url3?}")]
        [HttpGet("travel/{url2}/{url3?}")]
        [HttpGet("umrah-packages/{url2}/{url3?}")]
        [HttpGet("usat/{url2}/{url3?}")]
        [HttpGet("visa/{url2}/{url3?}")]
        [HttpGet("world/{url2}/{url3?}")]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> GuidesDetailView(string url2, string? url3)
        {
            // Build the full request path without the leading slash
            var pathSegments = HttpContext.Request.Path.Value?.Trim('/').Split('/');
            var fullPath = string.Join("/", pathSegments); // e.g., "bs-chemistry/admissions-in-lahore"
            string remoteFilePath = $"files/guides/{fullPath}.html";

            // Check if file exists on FTP
            bool fileExists = await _ftpService.FileExistsAsync(remoteFilePath);
            if (fileExists)
            {
                try
                {
                    string existingHtml = await _ftpService.DownloadFileAsync(remoteFilePath);
                    return Content(existingHtml, "text/html");
                }
                catch (Exception ex)
                {
                    // Log and fall through
                }
            }

            // Check for cache-busting parameters
            bool shouldBypassCache = HttpContext.Request.Query.ContainsKey("remove_cache") ||
                                    HttpContext.Request.Query.ContainsKey("nocache");



            if (shouldBypassCache)
            {
                // Completely disable caching
                Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                Response.Headers["Pragma"] = "no-cache";
                Response.Headers["Expires"] = "0";
            }
            else
            {
                // Enable caching with 5 minutes duration
                Response.Headers["Cache-Control"] = "public, max-age=300";
            }

            var url1 = pathSegments != null && pathSegments.Length > 0 ? pathSegments[0] : "";
            var Url = url3 != null
                        ? $"/{url1}/{url2}/{url3}"
                        : $"/{url1}/{url2}";


            if (string.IsNullOrEmpty(url1) || string.IsNullOrEmpty(url2))
                return NotFound();
            var menuItems = await _context.TblAllGuidesCms
                 .Where(g => g.ShowInMenu == true && g.IsActive == true
                     && g.Url.StartsWith("/" + url1 + "/")
                     && !string.IsNullOrEmpty(g.Url)
                     && !string.IsNullOrEmpty(g.HeaderIcon)
                     && !string.IsNullOrEmpty(g.HeaderName))
                 .OrderBy(g => g.SortOrder)
                 .Select(g => new MenuItemViewModel
                 {
                     Url = g.Url!,
                     HeaderIcon = g.HeaderIcon!,
                     HeaderName = g.HeaderName!
                 })
                 .ToListAsync();
            //var banners = await _bannerService.GetBannersAsync();
            //ViewBag.Banners = banners;
            ViewBag.HomeUrl = url1;
            ViewBag.Levels = await _context.TblXcourseLevels.AsNoTracking().ToListAsync();
            ViewBag.Cities = await _context.TblDefCities.AsNoTracking().ToListAsync();
            ViewBag.Fields = await _context.CourseCategories.AsNoTracking().ToListAsync();
            var guideUrl = $"https://www.ilmkidunya.com/{url1}";
            var guide = await _context.TblGuidesDefinations.FirstOrDefaultAsync(g => g.GuideMainUrl == guideUrl || g.GuideMainUrl == guideUrl + '/');


            ViewBag.ShowWatsupLink = await _context.TblWhatsAppGroups
                                      .FirstOrDefaultAsync(g => g.GuideName.Contains(url1.ToLower()) && g.IsActive);
            if (guide != null)
            {
                ViewBag.ShowInqueryForm = guide.ShowInquieryForm;
            }
            else
            {
                ViewBag.ShowInqueryForm = false;
            }
            List<CitywithAdmissionAndCollegeCountViewModel> cityList = new List<CitywithAdmissionAndCollegeCountViewModel>();

            // --- Format course name for UI ---
            var courseName = url1.Replace("-", " ").Trim();
            var words = courseName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++)
                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();

            string formattedCourse = string.Join(" ", words);
            ViewBag.Course = formattedCourse;

            // --- Determine City Data Source ---
            if (url2.Contains("admissions", StringComparison.OrdinalIgnoreCase))
            {
                cityList = GetCitiesWithAdmissions(url1, 5);
            }
            else if (url2.Contains("colleges", StringComparison.OrdinalIgnoreCase) ||
                     url2.Contains("universities", StringComparison.OrdinalIgnoreCase))
            {
                cityList = GetCitiesWithColleges(url1, 5);
            }



            var detail = await _context.TblAllGuidesCms
                     .Where(g => g.Url == Url && g.IsActive == true)
                     .Select(g => new GuideDetailViewModel
                     {
                         Heading = string.IsNullOrWhiteSpace(g.Heading) ? "Not Available" : g.Heading,
                         HeadingDesc = string.IsNullOrWhiteSpace(g.MetaDesc) ? "Not Available" : g.MetaDesc,
                         MetaTitle = string.IsNullOrWhiteSpace(g.MetaTitle) ? "Not Available" : g.MetaTitle,
                         MetaDesc = string.IsNullOrWhiteSpace(g.MetaDesc) ? "Not Available" : g.MetaDesc,
                         MetaKeyword = string.IsNullOrWhiteSpace(g.MetaKeys) ? "Not Available" : g.MetaKeys,
                         Url = string.IsNullOrWhiteSpace(g.Url) ? "Not Available" : g.Url,
                         Detail = string.IsNullOrWhiteSpace(g.Desc1) ? "Not Available" : g.Desc1,
                         Detail2 = g.Desc2,
                     })
                     .FirstOrDefaultAsync();



            // --- Dynamic Admission/College Handling ---
            if (detail == null ||
                url2.Contains("universities-in") || url2.Contains("colleges-in") ||
                url2.Contains("admissions-in"))
            {
                if (cityList != null && cityList.Any())
                {
                    var firstItemType = cityList.FirstOrDefault()?.ItemType;


                    // Handle Admissions Case
                    if (string.Equals(firstItemType, "Admissions", StringComparison.OrdinalIgnoreCase))
                    {
                        string city = pathSegments.Length > 1
                            ? pathSegments[1].Replace("admissions-in-", "", StringComparison.OrdinalIgnoreCase).Replace("admissions", "")
                            : string.Empty;

                        var admissionsInCity = GetAdmissions(url1, 1, 10, city);
                        string course = formattedCourse;
                        string secondPart = url1;
                        var admissionSbInCity = new System.Text.StringBuilder();

                        // Check if any admission has a non-empty CityName
                        bool showCityColumn = admissionsInCity.Any(a => !string.IsNullOrWhiteSpace(a.CityName));

                        admissionSbInCity.AppendLine($@"
<div class=""contact-sec fee-box"">
<div class=""container"">
<div class=""row"">
<div class=""col-lg-12"">
<h2>Admissions in different Institutes for {(guide != null ? guide.Abrevation : course)}</h2> 
<div class=""responsive-table"" style=""margin-bottom: 30px;"">
<table class=""table mid"">
<thead>
<tr>
<th><span>Date Posted</span></th>
<th><span>Institute Name</span></th>");

                        if (showCityColumn)
                        {
                            admissionSbInCity.AppendLine(@"<th><span>City</span></th>");
                        }

                        admissionSbInCity.AppendLine($@"
<th><span>Program Name</span></th>
<th><span>Admission Image</span></th>
</tr>
</thead>
<tbody>");

                        foreach (var admission in admissionsInCity)
                        {
                            admissionSbInCity.AppendLine($@"
<tr>
<td data-label=""Date Posted"">{admission.Dated?.ToString("dd-MMM-yyyy")}</td>
<td>
<a href=""/colleges/{admission.CollegeUrl}-admission.aspx"" target=""_blank"">
<span>
<img src=""https://cdn.ilmkidunya.com/Inst/logos/{admission.CollegeLogo}""
                             alt=""""
                             style=""max-height:40px; max-width:40px;"" />
                        {admission.CollegeName}
</span>
</a>
</td>");

                            if (showCityColumn)
                            {
                                admissionSbInCity.AppendLine($@"<td data-label=""City"">{admission.CityName}</td>");
                            }

                            admissionSbInCity.AppendLine($@"
<td data-label=""Courses/Programs""><ul><li>{admission.CourseName}</li></ul></td>
<td data-label=""Admission Notice"">");

                            if (!string.IsNullOrEmpty(admission.NoticeImageThumb) &&
                                admission.NoticeImageThumb != "/images/no-image.png")
                            {
                                admissionSbInCity.AppendLine($@"
<a href=""/colleges/{admission.CollegeUrl}-admission.aspx"" target=""_blank"">
<img src=""https://admissions.ilmkidunya.com/admission_notices/Images/NoticeAds/{admission.NoticeImageThumb}""
                         alt=""Admission notice for {admission.CollegeName}""
                         style=""max-height:80px; max-width:80px;"" />
</a>");
                            }
                            else
                            {
                                admissionSbInCity.AppendLine($@"
<a href=""/colleges/{admission.CollegeUrl}-admission.aspx"" target=""_blank"">
<img src=""https://cdn.traliv.com/wwwroot/images/not-available.png""
                         alt=""No Image Available""
                         style=""max-height:80px; max-width:80px;"" />
</a>");
                            }

                            admissionSbInCity.AppendLine("</td></tr>");
                        }

                        admissionSbInCity.AppendLine($@"
</tbody>
</table>
<div class=""load-more-container"" style=""text-align:center;margin-top:20px;"">
<button type=""button""
                                class=""load-more-btn""
                                data-type=""admissionsInCity""
                                data-course=""{secondPart}""
                                data-page=""2""
                                style=""padding:10px 20px;background:#007bff;color:white;border:none;border-radius:5px;cursor:pointer;"">
                            Load More Colleges
</button>
</div>
</div>
</div>
</div>
</div>
</div>");


                        if (detail != null)
                        {
                            detail.Detail = admissionSbInCity + detail.Detail;
                            detail.HeadingDesc = $"Apply for {course} Admission 2025 in {city}. Explore eligibility criteria, fee structure, admission schedule, and online application process.";
                        }
                        else
                        {
                            detail = new GuideDetailViewModel
                            {
                                Heading = $"{course} Admission 2025 in {city} | Eligibility, Fee & Last Date",
                                HeadingDesc = $"Apply for {course} Admission 2025 in {city}. Explore eligibility criteria, fee structure, admission schedule, and online application process.",
                                MetaTitle = $"{course} Admission 2025 in {city} | Eligibility, Fee & Last Date",
                                MetaDesc = $"Apply for {course} Admission  2025 in {city}. Check eligibility, fee structure, last date, and online application process here.",
                                MetaKeyword = $"{course}, admissions, universities, {city}, Pakistan",
                                Url = Url,
                                Detail = admissionSbInCity.ToString(),
                                Detail2 = "Automatically generated admission listing."
                            };
                        }
                    }
                    // Handle Colleges / Universities Case
                    else
                    {
                        string city = pathSegments.Length > 1
                            ? pathSegments[1].Replace("colleges-in-", "", StringComparison.OrdinalIgnoreCase).Replace("universities-in-", "", StringComparison.OrdinalIgnoreCase)
                            : string.Empty;

                        var collegesInCity = GetCollegesWithCourses(url1, 1, 10, city);
                        string course = formattedCourse;
                        string secondPart = url1;

                        if (collegesInCity == null || !collegesInCity.Any())
                        {
                            detail = new GuideDetailViewModel
                            {
                                Heading = $"Colleges in {city.Replace("-", " ")} for {course}",
                                Detail = $"<p>No universities found in {city.Replace("-", " ").ToUpper()} offering {course}.</p>",
                                MetaTitle = $"Colleges and Universities in {city} offering {course}",
                                MetaDesc = $"Find colleges and universities in {city} offering programs for {course}.",
                                MetaKeyword = $"{course}, universities, colleges, {city}, Pakistan",
                                Url = Url
                            };
                            return View("GuidesDetailView", new GuidePageViewModel { GuideDetail = detail, CityList = cityList, MenuItems = menuItems, });
                        }

                        var universitySb = new System.Text.StringBuilder();

                        universitySb.AppendLine($@"
<div class=""contact-sec fee-box"">
    <div class=""container"">
        <div class=""row"">
            <div class=""col-lg-12"">
                <h2>Universities in {city.Replace("-", " ").ToUpper()} offering {(guide != null ? guide.Abrevation : course)}</h2>
                
                <!-- Desktop View -->
                <div class=""responsive-table desktop-view"" style=""margin-bottom:30px;"">
                    <table class=""table mid"">
                        <thead>
                            <tr>
                                <th><span>Institution Name</span></th>
                                <th><span>Program Name</span></th>
                                <th><span>Location</span></th>
                            </tr>
                        </thead>
                        <tbody id=""desktopCollegesList"">");

                        foreach (var institution in collegesInCity)
                        {
                            universitySb.AppendLine($@"
        <tr>
            <td>
                <a href=""/colleges/{institution.CollegeUrl}.aspx"" target=""_blank"">
                    <span>
                        <img src=""https://cdn.ilmkidunya.com/Inst/logos/{institution.CollegeLogo}"" 
                             alt=""{institution.CollegeName}"" 
                             onerror=""this.onerror=null;this.src='https://www.ilmkidunya.com/images/no-colleege-thumb.jpg';""
                             style=""max-height:40px;max-width:40px;margin-right:8px;"" />
                        {institution.CollegeName}
                    </span>
                </a>
            </td>
            <td><span>{institution.CourseName}</span></td>
            <td><span>{institution.CollegeAddress}</span></td>
        </tr>");
                        }

                        universitySb.AppendLine($@"
                        </tbody>
                    </table>
                    <div class=""load-more-container"" style=""text-align:center;margin-top:20px;"">
                        <button type=""button"" 
                                class=""load-more-btn"" 
                                data-type=""universitiesInCity""
                                data-layout=""table""
                                data-course=""{secondPart}""
                                data-city=""{city}""
                                data-page=""2""
                                style=""padding:10px 20px;background:#007bff;color:white;border:none;border-radius:5px;cursor:pointer;"">
                            Load More Universities
                        </button>
                    </div>
                    <div id=""noMoreDesktopColleges"" style=""display: none; text-align: center; margin-top: 20px;""></div>
                </div>

                <!-- Mobile View -->
                <div class=""admission-mobile"">
                    <ul class=""admission-list"" id=""mobileCollegesList"">");

                        foreach (var institution in collegesInCity)
                        {
                            universitySb.AppendLine($@"
                        <li>
                            <figure> 
                                <img src=""https://cdn.ilmkidunya.com/Inst/logos/{institution.CollegeLogo}"" 
                                     alt=""{institution.CollegeName}"" 
                                     onerror=""this.onerror=null;this.src='https://www.ilmkidunya.com/images/no-colleege-thumb.jpg';""
                                     style=""max-height:60px;max-width:60px;margin-right:8px;"" />
                            </figure>
                            <div class=""text-box"">
                                <a href=""/colleges/{institution.CollegeUrl}.aspx"" target=""_blank"">
                                    {institution.CollegeName}
                                </a>
                                <span>Program: {institution.CourseName}</span>
                                <span>Location: {institution.CollegeAddress}</span>
                            </div>
                        </li>");
                        }

                        universitySb.AppendLine($@"
                    </ul>
                    <div class=""load-more-container"" style=""text-align: center; margin-top: 20px;"">
                        <button type=""button""
                                class=""load-more-btn""
                                data-type=""universitiesInCity""
                                data-layout=""list""
                                data-course=""{secondPart}""
                                data-city=""{city}""
                                data-page=""2""
                                style=""padding: 10px 20px; background: #007bff; color: white; border: none; border-radius: 5px; cursor: pointer;"">
                            Load More Universities
                        </button>
                    </div>
                    <div id=""noMoreMobileColleges"" style=""display: none; text-align: center; margin-top: 20px;""></div>
                </div>
            </div>
        </div>
    </div>
</div>");

                        if (detail != null)
                        {
                            detail.Detail = universitySb + detail.Detail;
                            detail.HeadingDesc = $"Explore the list of {course} universities in Lahore for 2025. Find admission details, fee structures, eligibility criteria and admission process.";

                        }
                        else
                        {
                            detail = new GuideDetailViewModel
                            {
                                Heading = $"{course} Universities in {city.Replace("-", " ")} 2025 | list of Colleges Offering {course}",
                                HeadingDesc = $"Explore the list of {course} universities in Lahore for 2025. Find admission details, fee structures, eligibility criteria and admission process.",
                                MetaTitle = $"{course} Universities in {city} 2025 | list of Colleges Offering {course}",
                                MetaDesc = $"Explore the list of {course} universities in Lahore for 2025. Find admission details, fee structures, eligibility criteria and admission process.",
                                MetaKeyword = $"{course}, universities, colleges, {city}, Pakistan",
                                Url = Url,
                                Detail = universitySb.ToString(),
                                Detail2 = "Automatically generated list of universities offering this course."
                            };
                        }


                    }
                }
                else
                {
                    //if (url2.Contains("universities-in") || url2.Contains("colleges-in"))
                    //{
                    //    // Extract city name from url2
                    //    string cityName = url2;
                    //    if (url2.Contains("universities-in-"))
                    //    {
                    //        cityName = url2.Replace("universities-in-", "");
                    //    }
                    //    else if (url2.Contains("colleges-in-"))
                    //    {
                    //        cityName = url2.Replace("colleges-in-", "");
                    //    }

                    //    // Clean up any trailing slashes or other characters
                    //    cityName = cityName.Trim().ToLower();

                    //    // Redirect to the specific colleges page for this city with .aspx extension
                    //    string redirectUrl = $"/colleges/colleges-in-{cityName}.aspx";
                    //    return Redirect(redirectUrl);
                    //}

                    // If not a universities/colleges URL, return NotFound
                    return NotFound();
                }
            }


            if (!string.IsNullOrEmpty(detail.Detail))
            {
                detail.Detail = ReplacePlaceholders(detail.Detail, pathSegments, guide);
            }

            //var menuItems = await _context.TblAllGuidesCms
            //       .Where(g => g.ShowInMenu == true && g.IsActive == true
            //           && g.Url.StartsWith("/" + url1 + "/")
            //           && !string.IsNullOrEmpty(g.Url)
            //           && !string.IsNullOrEmpty(g.HeaderIcon)
            //           && !string.IsNullOrEmpty(g.HeaderName))
            //       .OrderBy(g => g.SortOrder)
            //       .Select(g => new MenuItemViewModel
            //       {
            //           Url = g.Url!,
            //           HeaderIcon = g.HeaderIcon!,
            //           HeaderName = g.HeaderName!
            //       })
            //       .ToListAsync();

            var viewModel = new GuidePageViewModel
            {
                Guide = guide,
                GuideDetail = detail,
                MenuItems = menuItems,
                CityList = cityList
            };


            // ... after all existing logic, where the method originally returned View(viewModel) ...

            var viewResult = View("GuidesDetailView", viewModel);
            string htmlContent = await RenderViewToStringAsync(viewResult);

            // --- Upload to FTP ---
            try
            {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(htmlContent)))
                {
                    await _ftpService.UploadFileAsync(remoteFilePath, stream);
                }
            }
            catch (Exception ex)
            {
                string errorDiv = $@"
            <div style='background:#ffdddd; border:3px solid #cc0000; border-radius:8px; 
                        padding:20px; margin:20px; font-family:Consolas, monospace; 
                        white-space:pre-wrap; box-shadow:0 4px 8px rgba(0,0,0,0.1);'>
                <h3 style='color:#cc0000; margin-top:0;'>⚠️ Static File Upload Failed</h3>
                <p><strong>Error:</strong> {System.Net.WebUtility.HtmlEncode(ex.Message)}</p>
                <p><strong>Remote path:</strong> {System.Net.WebUtility.HtmlEncode(remoteFilePath)}</p>
                <p><em>The page content is still shown below.</em></p>
            </div>";

                int bodyIndex = htmlContent.IndexOf("<body", StringComparison.OrdinalIgnoreCase);
                if (bodyIndex >= 0)
                {
                    int bodyEndIndex = htmlContent.IndexOf('>', bodyIndex);
                    if (bodyEndIndex >= 0)
                        htmlContent = htmlContent.Insert(bodyEndIndex + 1, errorDiv);
                    else
                        htmlContent = errorDiv + htmlContent;
                }
                else
                {
                    htmlContent = errorDiv + htmlContent;
                }
            }

            return Content(htmlContent, "text/html");
        }



        private async Task<string> RenderViewToStringAsync(ViewResult viewResult)
        {
            using (var sw = new StringWriter())
            {
                viewResult.ViewData = ViewData;
                viewResult.TempData = TempData;

                var viewEngineResult = viewResult.ViewName != null
                    ? ViewEngine?.FindView(ControllerContext, viewResult.ViewName, true)
                    : ViewEngine?.FindView(ControllerContext, ControllerContext.ActionDescriptor.ActionName, true);

                if (viewEngineResult?.View == null)
                {
                    throw new Exception($"View '{viewResult.ViewName ?? ControllerContext.ActionDescriptor.ActionName}' not found.");
                }

                var view = viewEngineResult.View;

                var viewContext = new ViewContext(
                    ControllerContext,
                    view,
                    ViewData,
                    TempData,
                    sw,
                    new HtmlHelperOptions()
                );

                await view.RenderAsync(viewContext);
                return sw.ToString();
            }
        }



        public List<CollegeWithCourseViewModel> GetCollegesWithCourses(string course, int page = 1, int pageSize = 10, string? city = null)
        {
            course = _context.TblGuidesDefinations.Where(g => g.GuideMainUrl.ToLower().Contains(course.ToLower()))
                    .Select(g => g.Abrevation.ToLower().Replace("-", " "))
                    .FirstOrDefault() ?? course;

            course = course.ToLower().Replace("-", " ");

            TblDefCity City = new TblDefCity();
            if (!string.IsNullOrEmpty(city))
            {
                City = _context.TblDefCities
                    .Where(c => c.CityName.ToLower() == city.ToLower())
                    .FirstOrDefault();
            }

            var query = new List<CollegeWithCourseViewModel>();

            if (City != null && City.CityId != 0)
            {
                query = (from c in _context.TblColleges
                         join cs in _context.Courses on c.Id equals cs.CollegeId
                         where cs.Name.ToLower().Contains(course) && c.CityId == City.CityId
                         group new { c, cs } by c.Id into g
                         select new CollegeWithCourseViewModel
                         {
                             CollegeId = g.Key ?? 0,
                             CollegeName = g.First().c.Name,
                             CollegeUrl = g.First().c.Url,
                             CollegeAddress = g.First().c.Address,
                             CollegeLogo = g.First().c.Logo,

                             CourseId = g.First().cs.Id,
                             CourseName = g.First().cs.Name,
                             Duration = g.First().cs.Duration,
                             Fee = g.First().cs.Fee
                         })
                        .OrderBy(c => c.CollegeName)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
            }
            else
            {
                query = (from c in _context.TblColleges
                         join cs in _context.Courses on c.Id equals cs.CollegeId
                         where cs.Name.ToLower().Contains(course)
                         group new { c, cs } by c.Id into g
                         select new CollegeWithCourseViewModel
                         {
                             CollegeId = g.Key ?? 0,
                             CollegeName = g.First().c.Name,
                             CollegeUrl = g.First().c.Url,
                             CollegeAddress = g.First().c.Address,
                             CollegeLogo = g.First().c.Logo,

                             CourseId = g.First().cs.Id,
                             CourseName = g.First().cs.Name,
                             Duration = g.First().cs.Duration,
                             Fee = g.First().cs.Fee
                         })
                        .OrderBy(c => c.CollegeName)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
            }

            return query;
        }

        public List<ViewModels.AllGuidesViewModel.MeritListViewModel> GetMeritLists(string course, int page = 1, int pageSize = 10)
        {
            course = _context.TblGuidesDefinations.Where(g => g.GuideMainUrl.ToLower().Contains(course.ToLower()))
                    .Select(g => g.Abrevation.ToLower().Replace("-", " "))
                    .FirstOrDefault() ?? course;
            course = course.ToLower().Replace("-", " ");

            var courses = _context.Courses
                .Where(c => c.Name.ToLower().Contains(course))
                .AsNoTracking()
                .ToList();

            var courseIds = courses
                .Select(c => c.Id)
                .Distinct()
                .ToList();

            var meritListWithColleges = (from m in _context.TblMeritLists
                                         join c in _context.TblColleges
                                             on m.CollegeId equals c.Id
                                         join co in _context.Courses on m.CourseId equals co.Id
                                         where courseIds.Contains((int)m.CourseId)
                                         orderby m.AddedDate descending
                                         select new ViewModels.AllGuidesViewModel.MeritListViewModel
                                         {
                                             MeritAddedDate = m.DateOfIssue,
                                             MeritListId = m.Id,
                                             MeritListName = m.MeritListName,
                                             MeritValue = m.MeritValue,
                                             MeritFileName = m.FileName,
                                             Year = m.Year,
                                             CourseName = co.Name,
                                             CollegeId = c.Id,
                                             CollegeName = c.Name,
                                             CollegeUrl = c.Url,
                                             CollegeLogo = c.Logo,
                                             CollegeAddress = c.Address
                                         })
                                         .Skip((page - 1) * pageSize)
                                         .Take(pageSize)
                                         .ToList();

            return meritListWithColleges;
        }

        public List<CollegeCourseFeeViewModel> GetCollegesWithCourseFees(string course, int page = 1, int pageSize = 10)
        {
            course = _context.TblGuidesDefinations.Where(g => g.GuideMainUrl.ToLower().Contains(course.ToLower()))
                  .Select(g => g.Abrevation.ToLower().Replace("-", " "))
                  .FirstOrDefault() ?? course;

            course = course.ToLower().Replace("-", " ");

            var courses = _context.Courses
                .Where(c => c.Name.ToLower().Contains(course))
                .ToList();

            if (!courses.Any())
                return new List<CollegeCourseFeeViewModel>();

            var query = (from c in _context.TblColleges
                         join cs in _context.Courses on c.Id equals cs.CollegeId
                         where courses.Select(x => x.Id).Contains(cs.Id) && cs.Fee != null
                         select new CollegeCourseFeeViewModel
                         {
                             CollegeId = (int)c.Id,
                             CollegeName = c.Name,
                             CollegeUrl = c.Url,
                             CollegeLogo = c.Logo,
                             CollegeAddress = c.Address,

                             CourseId = cs.Id,
                             CourseName = cs.Name,
                             Duration = cs.Duration,
                             Fee = cs.Fee
                         })
                        .OrderByDescending(c => c.CourseId)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

            return query;
        }

        public List<AdmissionWithCollegeViewModel> GetAdmissions(string course, int page = 1, int pageSize = 10, string? city = null)
        {
            course = _context.TblGuidesDefinations.Where(g => g.GuideMainUrl.ToLower().Contains(course.ToLower()))
                  .Select(g => g.Abrevation.ToLower().Replace("-", " "))
                  .FirstOrDefault() ?? course;
            course = course.ToLower().Replace("-", " ");

            var courses = _context.Courses
                .Where(c => c.Name.ToLower().Contains(course))
                .Select(c => c.Id)
                .ToList();

            if (!courses.Any())
                return new List<AdmissionWithCollegeViewModel>();

            TblDefCity City = new TblDefCity();
            if (!string.IsNullOrEmpty(city))
            {
                City = _context.TblDefCities
                    .Where(c => c.CityName.ToLower() == city.ToLower().Replace("-", " "))
                    .FirstOrDefault();

            }

            var query = new List<AdmissionWithCollegeViewModel>();
            if (City.CityId != 0)
            {
                query = (from ac in _context.TblAdmissionCourses
                         join a in _context.TblAdmissions on ac.NoticeId equals a.Id
                         join col in _context.TblColleges on a.CollegeId equals col.Id
                         join c in _context.Courses on ac.CourseId equals c.Id
                         where courses.Contains((int)ac.CourseId) && (col.CityId == City.CityId)
                         select new AdmissionWithCollegeViewModel
                         {
                             AdmissionId = a.Id,
                             AdmissionTitle = a.AdmissionTitle,
                             Dated = a.Dated,
                             courseId = ac.CourseId,
                             LastDate = a.LastDate,
                             NoticeImageThumb = a.NoticeImageThumb,
                             NoticeImageLarge = a.NoticeImageLarge,
                             Url = a.Url,
                             CourseName = c.Name,
                             CollegeId = col.Id,
                             CollegeName = col.Name,
                             CollegeUrl = col.Url,
                             CollegeLogo = col.Logo,
                             CollegeAddress = col.Address,
                             CityId = col.CityId,
                             CityName = City != null ? City.CityName : ""
                         })
                       .AsEnumerable()
                       .DistinctBy(a => a.AdmissionId)
                       .GroupBy(x => new { x.CollegeId, x.courseId })
                       .Select(g => g.OrderByDescending(x => x.Dated).First())
                       .OrderByDescending(x => x.Dated)
                       .Skip((page - 1) * pageSize)
                       .Take(pageSize)
                       .ToList();
            }
            else
            {
                query = (from ac in _context.TblAdmissionCourses
                         join a in _context.TblAdmissions on ac.NoticeId equals a.Id
                         join col in _context.TblColleges on a.CollegeId equals col.Id
                         join c in _context.Courses on ac.CourseId equals c.Id
                         where courses.Contains((int)ac.CourseId)
                         select new AdmissionWithCollegeViewModel
                         {
                             AdmissionId = a.Id,
                             AdmissionTitle = a.AdmissionTitle,
                             Dated = a.Dated,
                             courseId = ac.CourseId,
                             LastDate = a.LastDate,
                             NoticeImageThumb = a.NoticeImageThumb,
                             NoticeImageLarge = a.NoticeImageLarge,
                             Url = a.Url,
                             CourseName = c.Name,
                             CollegeId = col.Id,
                             CollegeName = col.Name,
                             CollegeUrl = col.Url,
                             CollegeLogo = col.Logo,
                             CollegeAddress = col.Address,
                             CityId = col.CityId,
                             CityName = City != null ? City.CityName : ""
                         })
                         .AsEnumerable()
                         .DistinctBy(a => a.AdmissionId)
                         .GroupBy(x => new { x.CollegeId, x.courseId })
                         .Select(g => g.OrderByDescending(x => x.Dated).First())
                         .OrderByDescending(x => x.Dated)
                         .Skip((page - 1) * pageSize)
                         .Take(pageSize)
                         .ToList();
            }




            foreach (var admission in query)
            {
                admission.NoticeImageThumb = GetAdmissionLogoPath(admission.NoticeImageThumb, admission.Dated);
            }

            return query;
        }


        public List<CitywithAdmissionAndCollegeCountViewModel> GetCitiesWithAdmissions(string course, int minAdmissions = 5)
        {

            course = _context.TblGuidesDefinations.Where(g => g.GuideMainUrl.ToLower().Contains(course.ToLower()))
                .Select(g => g.Abrevation.ToLower().Replace("-", " "))
                .FirstOrDefault() ?? course;

            course = course.ToLower().Replace("-", " ");

            var courses = _context.Courses
                .Where(c => c.Name.ToLower().Contains(course))
                .Select(c => c.Id).Take(10)
                .ToList();

            if (!courses.Any())
                return new List<CitywithAdmissionAndCollegeCountViewModel>();

            var citiesWithAdmissions = (from ac in _context.TblAdmissionCourses
                                        join a in _context.TblAdmissions on ac.NoticeId equals a.Id
                                        join col in _context.TblColleges on a.CollegeId equals col.Id
                                        join city in _context.TblDefCities on col.CityId equals city.CityId
                                        join c in _context.Courses on ac.CourseId equals c.Id
                                        where courses.Contains((int)ac.CourseId)
                                        select new { a, col, city, ac })
                                      .AsEnumerable()
                                      .DistinctBy(x => x.a.Id) // Remove duplicate admissions
                                      .GroupBy(x => new { x.col.Id, x.ac.CourseId }) // Same grouping as second query
                                      .Select(g => g.OrderByDescending(x => x.a.Dated).First()) // Take latest per group
                                      .GroupBy(x => new { x.city.CityId, x.city.CityName }) // Now group by city
                                      .Select(g => new CitywithAdmissionAndCollegeCountViewModel
                                      {
                                          CityId = g.Key.CityId,
                                          CityName = g.Key.CityName,
                                          ItemCount = g.Count(),
                                          ItemType = "Admissions"
                                      })
                                      .Where(c => c.ItemCount >= minAdmissions)
                                      .OrderBy(c => c.CityName)
                                      .ToList();

            return citiesWithAdmissions;
        }



        // ViewModel for city with admission count
        public class CitywithAdmissionAndCollegeCountViewModel
        {
            public int CityId { get; set; }
            public string CityName { get; set; }
            public int ItemCount { get; set; }
            public string ItemType { get; set; }
        }



        public List<CitywithAdmissionAndCollegeCountViewModel> GetCitiesWithColleges(string course, int minColleges = 5)
        {
            course = _context.TblGuidesDefinations.Where(g => g.GuideMainUrl.ToLower().Contains(course.ToLower()))
                    .Select(g => g.Abrevation.ToLower().Replace("-", " "))
                    .FirstOrDefault() ?? course;

            course = course.ToLower().Replace("-", " ");


            // Get cities with college count for the specified course
            var citiesWithColleges = (from c in _context.TblColleges
                                      join cs in _context.Courses on c.Id equals cs.CollegeId
                                      join city in _context.TblDefCities on c.CityId equals city.CityId
                                      where cs.Name.ToLower().Contains(course)
                                      group new { c, cs, city } by new { city.CityId, city.CityName } into g
                                      where g.Count() >= minColleges
                                      select new CitywithAdmissionAndCollegeCountViewModel
                                      {
                                          CityId = g.Key.CityId,
                                          CityName = g.Key.CityName,
                                          ItemCount = g.Count(),
                                          ItemType = "Universities"
                                      })
                                    .OrderBy(c => c.CityName)
                                    .ToList();

            return citiesWithColleges;
        }

        // ViewModel for city with college count


        public string ReplacePlaceholders(string inputHtml, string[] pathSegment, TblGuidesDefination guide)
        {
            if (string.IsNullOrEmpty(inputHtml))
                return inputHtml;

            return Regex.Replace(inputHtml, @"##(.*?)##", match =>
            {
                // Extract the content inside the ##
                string content = match.Groups[1].Value; // e.g. "collegeList_bs-chemistry"

                // Split by underscore
                var parts = content.Split('_', 2); // Split into 2 parts only

                if (parts.Length != 2)
                    return match.Value; // leave the placeholder unchanged

                string firstPart = parts[0]; // e.g. "collegeList"
                string secondPart = parts[1]; // e.g. "bs-chemistry"
                string city = pathSegment[1].Replace("admissions-in-", "");
                secondPart = secondPart.Replace("-", " ");

                // Split words and capitalize first letter of each
                var words = secondPart.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < words.Length; i++)
                {
                    words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
                }
                string course = string.Join(" ", words);

                switch (firstPart)
                {
                    case "collegesList":
                        var colleges = GetCollegesWithCourses(secondPart);
                        if (colleges == null || !colleges.Any())
                            return "<p>No colleges found.</p>";

                        var sb = new StringBuilder();

                        sb.AppendLine($@"
        <h3>List of Institutes which provide {(guide != null ? guide.Abrevation : course)}</h3>
        
        <!-- Desktop View -->
        <div class=""responsive-table desktop-view"">
            <table class=""table mid"">
                <thead>
                    <tr>
                        <th><span>Institution Name</span></th>
                        <th><span>Program Name</span></th>
                        <th><span>Location</span></th>
                    </tr>
                </thead>
                <tbody id=""desktopCollegesList"">
    ");

                        foreach (var institution in colleges)
                        {
                            sb.AppendLine($@"
                <tr>
                    <td>
                        <a href=""/colleges/{institution.CollegeUrl}.aspx"" target=""_blank"">
                            <span>
                                <img src=""https://cdn.ilmkidunya.com/Inst/logos/{institution.CollegeLogo}"" 
                                     alt=""{institution.CollegeName}"" 
                                     onerror=""this.onerror=null;this.src='https://www.ilmkidunya.com/images/no-colleege-thumb.jpg';"" />
                                {institution.CollegeName}
                            </span>
                        </a>
                    </td>
                    <td><span>{institution.CourseName}</span></td>
                    <td><span>{institution.CollegeAddress}</span></td>
                </tr>");
                        }

                        sb.AppendLine(@$"
                </tbody>
            </table>
            <div class=""load-more-container"" style=""text-align: center; margin-top: 20px;"">
                <button type=""button"" 
                        class=""load-more-btn"" 
                        data-type=""collegesList""
                        data-layout=""table""
                        data-course=""{secondPart}""
                        data-page=""2""
                        style=""padding: 10px 20px; background: #007bff; color: white; border: none; border-radius: 5px; cursor: pointer;"">
                    Load More Colleges
                </button>
            </div>
            <div id=""noMoreDesktopColleges"" style=""display: none; text-align: center; margin-top: 20px;""></div>
        </div>

        <!-- Mobile View -->
        <div class=""admission-mobile"">
            <ul class=""admission-list"" id=""mobileCollegesList"">
    ");

                        foreach (var institution in colleges)
                        {
                            sb.AppendLine($@"
                <li>
                    <figure> 
                        <img src=""https://cdn.ilmkidunya.com/Inst/logos/{institution.CollegeLogo}"" 
                             alt=""{institution.CollegeName}"" 
                             onerror=""this.onerror=null;this.src='https://www.ilmkidunya.com/images/no-colleege-thumb.jpg';"" /> 
                    </figure>
                    <div class=""text-box"">
                        <a href=""/colleges/{institution.CollegeUrl}.aspx"" target=""_blank"">
                            {institution.CollegeName}
                        </a>
                        <span>Program: {institution.CourseName}</span>
                        <span>Location: {institution.CollegeAddress}</span>
                    </div>
                </li>");
                        }

                        sb.AppendLine($@"
            </ul>
            <div class=""load-more-container"" style=""text-align: center; margin-top: 20px;"">
                <button type=""button""
                        class=""load-more-btn""
                        data-type=""collegesList""
                        data-layout=""list""
                        data-course=""{secondPart}""
                        data-city=""{city}""
                        data-page=""2""
                        style=""padding: 10px 20px; background: #007bff; color: white; border: none; border-radius: 5px; cursor: pointer;"">
                    Load More Colleges
                </button>
            </div>
            <div id=""noMoreMobileColleges"" style=""display: none; text-align: center; margin-top: 20px;""></div>
        </div>
    ");

                        return sb.ToString();

                    case "meritsList":
                        var meritLists = GetMeritLists(secondPart);
                        if (meritLists == null || !meritLists.Any())
                            return "<p>No merit lists found.</p>";

                        var meritSb = new StringBuilder();
                        meritSb.AppendLine($@"
<div class=""contact-sec fee-box"">
    <div class=""container"">
        <div class=""row"">
            <div class=""col-lg-12"">
                <h2>Latest Merit Lists of {(guide != null ? guide.Abrevation : course)} in different Institutes</h2>
                
                <!-- Desktop View -->
                <div class=""responsive-table desktop-view"" style=""margin-bottom: 30px;"">
                    <table class=""table mid"">
                        <thead>
                            <tr>
                                <th><span>Date</span></th>
                                <th><span>Institute</span></th>
                                <th><span>Program Name</span></th>
                                <th><span>Merit List</span></th>
                            </tr>
                        </thead>
                        <tbody id=""desktopMeritsList"">");

                        foreach (var merit in meritLists)
                        {
                            string displayDate = merit.MeritAddedDate;
                            if (DateTime.TryParse(merit.MeritAddedDate, out var parsedDate))
                            {
                                displayDate = parsedDate.ToString("dd-MMM-yyyy");
                            }

                            meritSb.AppendLine($@"
        <tr>
            <td>{displayDate}</td>
            <td>
                <a href=""/colleges/{merit.CollegeUrl}-merit-lists.aspx"" target=""_blank"">
                    <span>{merit.CollegeName}</span>
                </a>
            </td>
            <td>{merit.CourseName}</td>
            <td>
                <a href=""javascript:void(0)"" 
                   onclick=""openMeritList('https://ikddata.ilmkidunya.com/images/MeritListFiles/{merit.MeritFileName}', '{merit.MeritListName} ({merit.MeritValue})')"" 
                   class="""">
                    {merit.MeritListName}
                </a>
            </td>
        </tr>");
                        }

                        meritSb.AppendLine(@$"
                        </tbody>
                    </table>
                    <div class=""load-more-container"" style=""text-align: center; margin-top: 20px;"">
                        <button type=""button"" 
                                class=""load-more-btn"" 
                                data-type=""meritsList""
                                data-layout=""table""
                                data-course=""{secondPart}""
                                data-page=""2""
                                style=""padding: 10px 20px; background: #007bff; color: white; border: none; border-radius: 5px; cursor: pointer;"">
                            Load More Merit Lists
                        </button>
                    </div>
                    <div id=""noMoreDesktopMerits"" style=""display: none; text-align: center; margin-top: 20px;""></div>
                </div>

                <!-- Mobile View -->
                <div class=""admission-mobile"">
                    <ul class=""admission-list"" id=""mobileCollegesList"">");

                        foreach (var merit in meritLists)
                        {
                            string displayDate = merit.MeritAddedDate;
                            if (DateTime.TryParse(merit.MeritAddedDate, out var parsedDate))
                            {
                                displayDate = parsedDate.ToString("dd-MMM-yyyy");
                            }

                            meritSb.AppendLine($@"
                        <li>
                            <div class=""text-box"">
                                <a href=""/colleges/{merit.CollegeUrl}-merit-lists.aspx"" target=""_blank"">
                                    {merit.CollegeName}
                                </a>
                                <span>Date: {displayDate}</span>
                                <span>Program: {merit.CourseName}</span>
                                <span>
                                    <a href=""javascript:void(0)"" 
                                       onclick=""openMeritList('https://ikddata.ilmkidunya.com/images/MeritListFiles/{merit.MeritFileName}', '{merit.MeritListName} ({merit.MeritValue})')"">
                                        {merit.MeritListName}
                                    </a>
                                </span>
                            </div>
                        </li>");
                        }

                        meritSb.AppendLine($@"
                    </ul>
                    <div class=""load-more-container"" style=""text-align: center; margin-top: 20px;"">
                        <button type=""button""
                                class=""load-more-btn""
                                data-type=""meritsList""
                                data-layout=""list""
                                data-course=""{secondPart}""
                                data-page=""2""
                                style=""padding: 10px 20px; background: #007bff; color: white; border: none; border-radius: 5px; cursor: pointer;"">
                            Load More Merit Lists
                        </button>
                    </div>
                    <div id=""noMoreMobileMerits"" style=""display: none; text-align: center; margin-top: 20px;""></div>
                </div>
            </div>
        </div>
    </div>
</div>

<!-- Merit List Side Viewer -->
<div class=""merit-side"" id=""meritViewer""
     style=""position: fixed; right: -100%; top: 0; opacity: 0; visibility: hidden; z-index: 999; transition: 0.3s ease-in-out; width:80%; height:100%; background:#fff; box-shadow:-2px 0 8px rgba(0,0,0,0.2);"">
    <div class=""merit-box"" style=""height:100%; display:flex; flex-direction:column;"">
        <div class=""top-box"" style=""display:flex; justify-content:space-between; align-items:center; padding:10px; border-bottom:1px solid #ddd;"">
            <h2 id=""meritTitle"" style=""margin:0;"">Merit List</h2>
            <div class=""close-btn"">
                <a href=""#"" id=""closeMerit"">
                    <img src=""https://images.ishallwin.com/ot-images/red-cross.webp"" alt=""close"">
                </a>
            </div>
        </div>
        <iframe id=""meritFrame"" src="""" style=""flex:1; border:none;""></iframe>
    </div>
</div>

<script>
    const meritBox = document.getElementById('meritViewer');
    const meritFrame = document.getElementById('meritFrame');
    const meritTitle = document.getElementById('meritTitle');
    const closeMerit = document.getElementById('closeMerit');

    function openMeritList(fileUrl, title) {{
        meritFrame.src = fileUrl;
        meritTitle.textContent = title;
        meritBox.style.right = '0';
        meritBox.style.opacity = '1';
        meritBox.style.visibility = 'visible';
    }}

    closeMerit.addEventListener('click', function(e) {{
        e.preventDefault();
        meritBox.style.right = '-100%';
        meritBox.style.opacity = '0';
        meritBox.style.visibility = 'hidden';
        meritFrame.src = '';
    }});
</script>
");

                        return meritSb.ToString();

                    case "feesList":
                        var feeData = GetCollegesWithCourseFees(secondPart);
                        if (feeData == null || !feeData.Any())
                            return "<p>No fee information found.</p>";

                        var feeSb = new StringBuilder();
                        feeSb.AppendLine($@"
        <div class=""contact-sec fee-box"">
            <div class=""container"">
                <div class=""row"">
                    <div class=""col-lg-12"">
                        <h2>Fee list of different Institutes which provide {(guide != null ? guide.Abrevation : course)}</h2>
                        
                        <!-- Desktop View -->
                        <div class=""responsive-table desktop-view"" style=""margin-bottom: 30px;"">
                            <table class=""table mid"">
                                <thead>
                                    <tr>
                                        <th><span>Institute</span></th>
                                        <th><span>Program Name</span></th>
                                        <th><span>Duration</span></th>
                                        <th><span>Fee Structure</span></th>
                                    </tr>
                                </thead>
                                <tbody id=""desktopFeesList"">");

                        foreach (var fee in feeData)
                        {
                            feeSb.AppendLine($@"
            <tr>
                <td>
                    <a href=""/colleges/{fee.CollegeUrl}-fee-structure.aspx"" target=""_blank"">
                        <span>{fee.CollegeName}</span>
                    </a>
                </td>
                <td>
                    <span>{fee.CourseName}</span>
                </td>
                <td><span>{fee.Duration ?? "N/A"}</span></td>
                <td>
                    <ul>
                        {(!string.IsNullOrEmpty(fee.Fee) ? $"<li>{fee.Fee}</li>" : "<li> - </li>")}
                    </ul>
                </td>
            </tr>");
                        }

                        feeSb.AppendLine(@$"
                                </tbody>
                            </table>
                            <div class=""load-more-container"" style=""text-align: center; margin-top: 20px;"">
                                <button type=""button"" 
                                        class=""load-more-btn"" 
                                        data-type=""feesList""
                                        data-layout=""table""
                                        data-course=""{secondPart}""
                                        data-page=""2""
                                        style=""padding: 10px 20px; background: #007bff; color: white; border: none; border-radius: 5px; cursor: pointer;"">
                                    Load More Fee Lists
                                </button>
                            </div>
                            <div id=""noMoreDesktopFees"" style=""display: none; text-align: center; margin-top: 20px;""></div>
                        </div>

                        <!-- Mobile View -->
                        <div class=""admission-mobile"">
                            <ul class=""admission-list"" id=""mobileCollegesList"">");

                        foreach (var fee in feeData)
                        {
                            feeSb.AppendLine($@"
                                <li>
                                    <div class=""text-box"">
                                        <a href=""/colleges/{fee.CollegeUrl}-fee-structure.aspx"" target=""_blank"">
                                            {fee.CollegeName}
                                        </a>
                                        <span>Program: {fee.CourseName}</span>
                                        <span>Duration: {fee.Duration ?? "N/A"}</span>
                                        <span>Fee: {(!string.IsNullOrEmpty(fee.Fee) ? fee.Fee : "N/A")}</span>
                                    </div>
                                </li>");
                        }

                        feeSb.AppendLine($@"
                            </ul>
                            <div class=""load-more-container"" style=""text-align: center; margin-top: 20px;"">
                                <button type=""button""
                                        class=""load-more-btn""
                                        data-type=""feesList""
                                        data-layout=""list""
                                        data-course=""{secondPart}""
                                        data-page=""2""
                                        style=""padding: 10px 20px; background: #007bff; color: white; border: none; border-radius: 5px; cursor: pointer;"">
                                    Load More Fee Lists
                                </button>
                            </div>
                            <div id=""noMoreMobileFees"" style=""display: none; text-align: center; margin-top: 20px;""></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>");
                        return feeSb.ToString();

                    case "admissionsList":
                        var admissions = GetAdmissions(secondPart);
                        if (admissions == null || !admissions.Any())
                            return "<p>No admissions found.</p>";

                        var admissionSb = new StringBuilder();
                        admissionSb.AppendLine($@"
    <div class=""contact-sec fee-box"">
        <div class=""container"">
            <div class=""row"">
                <div class=""col-lg-12"">
                    <h2>Admissions in different Institutes for {(guide != null ? guide.Abrevation : course)}</h2> 
                    
                    <!-- Desktop View -->
                    <div class=""responsive-table desktop-view"" style=""margin-bottom: 30px;"">
                        <table class=""table mid"">
                            <thead>
                                <tr>
                                    <th><span>Date Posted</span></th>
                                    <th><span>Last Date</span></th>
                                    <th><span>Institute Name</span></th>
                                    <th><span>Program Name</span></th>
                                    <th><span>Admission Image</span></th>
                                </tr>
                            </thead>
                            <tbody id=""desktopAdmissionsList"">");

                        foreach (var admission in admissions)
                        {
                            admissionSb.AppendLine($@"
            <tr>
                <td data-label=""Date Posted"">{admission.Dated?.ToString("dd-MMM-yyyy")}</td>
                <td data-label=""Last Date"">{(admission.LastDate?.ToString("dd-MMM-yyyy") ?? "N/A")}</td>
                <td>
                    <a href=""/colleges/{admission.CollegeUrl}-admission.aspx"" target=""_blank"">
                        <span>
                            <img src=""https://cdn.ilmkidunya.com/Inst/logos/{admission.CollegeLogo}"" 
                                 alt=""{admission.CollegeName}"" 
                                 onerror=""this.onerror=null;this.src='https://www.ilmkidunya.com/images/no-colleege-thumb.jpg';""
                                 style=""max-height:40px; max-width:40px;"" />
                            {admission.CollegeName}
                        </span>
                    </a>
                </td>
                <td data-label=""Courses/Programs"">
                    <ul>
                        <li>{admission.CourseName}</li>
                    </ul>
                </td>
                <td data-label=""Admission Notice"">");

                            if (!string.IsNullOrEmpty(admission.NoticeImageThumb) && admission.NoticeImageThumb != "/images/no-image.png")
                            {
                                admissionSb.AppendLine($@"
        <a href=""/colleges/{admission.CollegeUrl}-admission.aspx"" target=""_blank"">
            <img src=""https://admissions.ilmkidunya.com/admission_notices/Images/NoticeAds/{admission.NoticeImageThumb}"" 
                 alt=""Admission notice for {admission.CollegeName}"" 
                 style=""max-height:80px; max-width:80px;"" />
        </a>");
                            }
                            else
                            {
                                admissionSb.AppendLine($@"
        <a href=""/colleges/{admission.CollegeUrl}-admission.aspx"" target=""_blank"">
            <img src=""https://cdn.traliv.com/wwwroot/images/not-available.png"" 
                 alt=""No Image Available"" 
                 style=""max-height:80px; max-width:80px;"" />
        </a>");
                            }

                            admissionSb.AppendLine("</td></tr>");
                        }

                        admissionSb.AppendLine(@$"
                            </tbody>
                        </table>
                        <div class=""load-more-container"" style=""text-align: center; margin-top: 20px;"">
                            <button type=""button"" 
                                    class=""load-more-btn"" 
                                    data-type=""admissionsList""
                                    data-layout=""table""
                                    data-course=""{secondPart}""
                                    data-page=""2""
                                    style=""padding: 10px 20px; background: #007bff; color: white; border: none; border-radius: 5px; cursor: pointer;"">
                                Load More Admissions
                            </button>
                        </div>
                        <div id=""noMoreDesktopAdmissions"" style=""display: none; text-align: center; margin-top: 20px;""></div>
                    </div>

                    <!-- Mobile View -->
                    <div class=""admission-mobile"">
                        <ul class=""admission-list"" id=""mobileCollegesList"">");

                        foreach (var admission in admissions)
                        {
                            admissionSb.AppendLine($@"
                            <li>
                                <figure> 
                                    <img src=""https://cdn.ilmkidunya.com/Inst/logos/{admission.CollegeLogo}"" 
                                         alt=""{admission.CollegeName}"" 
                                         onerror=""this.onerror=null;this.src='https://www.ilmkidunya.com/images/no-colleege-thumb.jpg';""
                                         style=""max-height:60px;max-width:60px;margin-right:8px;"" />
                                </figure>
                                <div class=""text-box"">
                                    <a href=""/colleges/{admission.CollegeUrl}-admission.aspx"" target=""_blank"">
                                        {admission.CollegeName}
                                    </a>
                                    <span>Date Posted: {admission.Dated?.ToString("dd-MMM-yyyy")}</span>
                                    <span>Last Date: {(admission.LastDate?.ToString("dd-MMM-yyyy") ?? "N/A")}</span>
                                    <span>Program: {admission.CourseName}</span>
                                </div>
                            </li>");
                        }

                        admissionSb.AppendLine($@"
                        </ul>
                        <div class=""load-more-container"" style=""text-align: center; margin-top: 20px;"">
                            <button type=""button""
                                    class=""load-more-btn""
                                    data-type=""admissionsList""
                                    data-layout=""list""
                                    data-course=""{secondPart}""
                                    data-page=""2""
                                    style=""padding: 10px 20px; background: #007bff; color: white; border: none; border-radius: 5px; cursor: pointer;"">
                                Load More Admissions
                            </button>
                        </div>
                        <div id=""noMoreMobileAdmissions"" style=""display: none; text-align: center; margin-top: 20px;""></div>
                    </div>
                </div>
            </div>
        </div>
    </div>");

                        return admissionSb.ToString();

                    case "admissionsInCity":
                        var admissionsInCity = GetAdmissions(secondPart, 1, 10, city);
                        if (admissionsInCity == null || !admissionsInCity.Any())
                            return "<p>No admissions found.</p>";

                        var admissionSbInCity = new StringBuilder();
                        admissionSbInCity.AppendLine($@"
    <div class=""contact-sec fee-box"">
        <div class=""container"">
            <div class=""row"">
                <div class=""col-lg-12"">
                    <h2>Admissions in different Institutes for {(guide != null ? guide.Abrevation : course)} in {city?.Replace("-", " ").ToUpper()}</h2> 
                    
                    <!-- Desktop View -->
                    <div class=""responsive-table desktop-view"" style=""margin-bottom: 30px;"">
                        <table class=""table mid"">
                            <thead>
                                <tr>
                                    <th><span>Date Posted</span></th>
                                    <th><span>Institute Name</span></th>
                                    <th><span>City</span></th>
                                    <th><span>Program Name</span></th>
                                    <th><span>Admission Image</span></th>
                                </tr>
                            </thead>
                            <tbody id=""desktopAdmissionsInCityList"">");

                        foreach (var admission in admissionsInCity)
                        {
                            admissionSbInCity.AppendLine($@"
            <tr>
                <td data-label=""Date Posted"">{admission.Dated?.ToString("dd-MMM-yyyy")}</td>
                <td>
                    <a href=""/colleges/{admission.CollegeUrl}-admission.aspx"" target=""_blank"">
                        <span>
                            <img src=""https://cdn.ilmkidunya.com/Inst/logos/{admission.CollegeLogo}"" 
                                 alt=""{admission.CollegeName}"" 
                                 onerror=""this.onerror=null;this.src='https://www.ilmkidunya.com/images/no-colleege-thumb.jpg';""
                                 style=""max-height:40px; max-width:40px;"" />
                            {admission.CollegeName}
                        </span>
                    </a>
                </td>
                <td data-label=""City"">{admission.CityName}</td>
                <td data-label=""Courses/Programs"">
                    <ul>
                        <li>{admission.CourseName}</li>
                    </ul>
                </td>
                <td data-label=""Admission Notice"">");

                            if (!string.IsNullOrEmpty(admission.NoticeImageThumb) && admission.NoticeImageThumb != "/images/no-image.png")
                            {
                                admissionSbInCity.AppendLine($@"
        <a href=""/colleges/{admission.CollegeUrl}-admission.aspx"" target=""_blank"">
            <img src=""https://admissions.ilmkidunya.com/admission_notices/Images/NoticeAds/{admission.NoticeImageThumb}"" 
                 alt=""Admission notice for {admission.CollegeName}"" 
                 style=""max-height:80px; max-width:80px;"" />
        </a>");
                            }
                            else
                            {
                                admissionSbInCity.AppendLine($@"
        <a href=""/colleges/{admission.CollegeUrl}-admission.aspx"" target=""_blank"">
            <img src=""https://cdn.traliv.com/wwwroot/images/not-available.png"" 
                 alt=""No Image Available"" 
                 style=""max-height:80px; max-width:80px;"" />
        </a>");
                            }

                            admissionSbInCity.AppendLine("</td></tr>");
                        }

                        admissionSbInCity.AppendLine(@$"
                            </tbody>
                        </table>
                        <div class=""load-more-container"" style=""text-align: center; margin-top: 20px;"">
                            <button type=""button"" 
                                    class=""load-more-btn"" 
                                    data-type=""admissionsInCity""
                                    data-layout=""table""
                                    data-course=""{secondPart}""
                                    data-city=""{city}""
                                    data-page=""2""
                                    style=""padding: 10px 20px; background: #007bff; color: white; border: none; border-radius: 5px; cursor: pointer;"">
                                Load More Admissions
                            </button>
                        </div>
                        <div id=""noMoreDesktopAdmissionsInCity"" style=""display: none; text-align: center; margin-top: 20px;""></div>
                    </div>

                    <!-- Mobile View -->
                    <div class=""admission-mobile"">
                        <ul class=""admission-list"" id=""mobileCollegesList"">");

                        foreach (var admission in admissionsInCity)
                        {
                            admissionSbInCity.AppendLine($@"
                            <li>
                                <figure> 
                                    <img src=""https://cdn.ilmkidunya.com/Inst/logos/{admission.CollegeLogo}"" 
                                         alt=""{admission.CollegeName}"" 
                                         onerror=""this.onerror=null;this.src='https://www.ilmkidunya.com/images/no-colleege-thumb.jpg';""
                                         style=""max-height:60px;max-width:60px;margin-right:8px;"" />
                                </figure>
                                <div class=""text-box"">
                                    <a href=""/colleges/{admission.CollegeUrl}-admission.aspx"" target=""_blank"">
                                        {admission.CollegeName}
                                    </a>
                                    <span>Date Posted: {admission.Dated?.ToString("dd-MMM-yyyy")}</span>
                                    <span>City: {admission.CityName}</span>
                                    <span>Program: {admission.CourseName}</span>
                                </div>
                            </li>");
                        }

                        admissionSbInCity.AppendLine($@"
                        </ul>
                        <div class=""load-more-container"" style=""text-align: center; margin-top: 20px;"">
                            <button type=""button""
                                    class=""load-more-btn""
                                    data-type=""admissionsInCity""
                                    data-layout=""list""
                                    data-course=""{secondPart}""
                                    data-city=""{city}""
                                    data-page=""2""
                                    style=""padding: 10px 20px; background: #007bff; color: white; border: none; border-radius: 5px; cursor: pointer;"">
                                Load More Admissions
                            </button>
                        </div>
                        <div id=""noMoreMobileAdmissionsInCity"" style=""display: none; text-align: center; margin-top: 20px;""></div>
                    </div>
                </div>
            </div>
        </div>
    </div>");

                        return admissionSbInCity.ToString();

                    case "universitiesInCity":
                        var universitiesInCity = GetCollegesWithCourses(secondPart, 1, 10, city);
                        if (universitiesInCity == null || !universitiesInCity.Any())
                            return "<p>No universities found in this city.</p>";

                        var universitySb = new StringBuilder();
                        universitySb.AppendLine($@"
<div class=""contact-sec fee-box"">
    <div class=""container"">
        <div class=""row"">
            <div class=""col-lg-12"">
                <h2>Universities in {city?.Replace("-", " ").ToUpper()} offering {(guide != null ? guide.Abrevation : course)}</h2>
                
                <!-- Desktop View -->
                <div class=""responsive-table desktop-view"" style=""margin-bottom: 30px;"">
                    <table class=""table mid"">
                        <thead>
                            <tr>
                                <th><span>Institution Name</span></th>
                                <th><span>Program Name</span></th>
                                <th><span>Location</span></th>
                            </tr>
                        </thead>
                        <tbody id=""desktopUniversitiesInCityList"">");

                        foreach (var institution in universitiesInCity)
                        {
                            universitySb.AppendLine($@"
                            <tr>
                                <td>
                                    <a href=""/colleges/{institution.CollegeUrl}.aspx"" target=""_blank"">
                                        <span>
                                            <img src=""https://cdn.ilmkidunya.com/Inst/logos/{institution.CollegeLogo}"" 
                                                 alt=""{institution.CollegeName}"" 
                                                 onerror=""this.onerror=null;this.src='https://www.ilmkidunya.com/images/no-colleege-thumb.jpg';"" />
                                            {institution.CollegeName}
                                        </span>
                                    </a>
                                </td>
                                <td><span>{institution.CourseName}</span></td>
                                <td><span>{institution.CollegeAddress}</span></td>
                            </tr>");
                        }

                        universitySb.AppendLine(@$"
                        </tbody>
                    </table>
                    <div class=""load-more-container"" style=""text-align: center; margin-top: 20px;"">
                        <button type=""button"" 
                                class=""load-more-btn"" 
                                data-type=""universitiesInCity""
                                data-layout=""table""
                                data-course=""{secondPart}""
                                data-city=""{city}""
                                data-page=""2""
                                style=""padding: 10px 20px; background: #007bff; color: white; border: none; border-radius: 5px; cursor: pointer;"">
                            Load More Universities
                        </button>
                    </div>
                    <div id=""noMoreDesktopUniversitiesInCity"" style=""display: none; text-align: center; margin-top: 20px;""></div>
                </div>

                <!-- Mobile View -->
                <div class=""admission-mobile"">
                    <ul class=""admission-list"" id=""mobileCollegesList"">");

                        foreach (var institution in universitiesInCity)
                        {
                            universitySb.AppendLine($@"
                        <li>
                            <figure> 
                                <img src=""https://cdn.ilmkidunya.com/Inst/logos/{institution.CollegeLogo}"" 
                                     alt=""{institution.CollegeName}"" 
                                     onerror=""this.onerror=null;this.src='https://www.ilmkidunya.com/images/no-colleege-thumb.jpg';"" /> 
                            </figure>
                            <div class=""text-box"">
                                <a href=""/colleges/{institution.CollegeUrl}.aspx"" target=""_blank"">
                                    {institution.CollegeName}
                                </a>
                                <span>Program: {institution.CourseName}</span>
                                <span>Location: {institution.CollegeAddress}</span>
                            </div>
                        </li>");
                        }

                        universitySb.AppendLine($@"
                    </ul>
                    <div class=""load-more-container"" style=""text-align: center; margin-top: 20px;"">
                        <button type=""button""
                                class=""load-more-btn""
                                data-type=""universitiesInCity""
                                data-layout=""list""
                                data-course=""{secondPart}""
                                data-city=""{city}""
                                data-page=""2""
                                style=""padding: 10px 20px; background: #007bff; color: white; border: none; border-radius: 5px; cursor: pointer;"">
                            Load More Universities
                        </button>
                    </div>
                    <div id=""noMoreMobileUniversitiesInCity"" style=""display: none; text-align: center; margin-top: 20px;""></div>
                </div>
            </div>
        </div>
    </div>
</div>");

                        return universitySb.ToString();

                    case "CityListForAdmissions":
                        var citiesWithAdmissions = GetCitiesWithAdmissions(secondPart, 5);
                        if (citiesWithAdmissions == null || !citiesWithAdmissions.Any())
                            return "<p>No cities with sufficient admissions found.</p>";

                        var citySb = new StringBuilder();
                        citySb.AppendLine($@"
<div class=""flex-container"">
    <div class=""col-lg-4"">
        <div class=""table-of-content"" style=""margin-right:0;padding-bottom:15px;width:100%;"">
            <h3><a href=""#"">Cities with {(guide != null ? guide.Abrevation : course)} Admissions</a></h3>
            
            <!-- Desktop View -->
            <div class=""desktop-view"">
                <table class=""table mid"">
                    <thead>
                        <tr>
                            <th><span>City Name</span></th>
                            <th><span>Admission Count</span></th>
                        </tr>
                    </thead>
                    <tbody id=""desktopCityAdmissionsList"">");

                        foreach (var citi in citiesWithAdmissions)
                        {
                            string cityUrl = citi.CityName.ToLower().Replace(" ", "-");
                            citySb.AppendLine($@"
                        <tr>
                            <td>
                                <a href=""/{secondPart.ToLower().Replace(" ", "-")}/admissions-in-{cityUrl}"">{citi.CityName}</a>
                            </td>
                            <td><span>({citi.ItemCount})</span></td>
                        </tr>");
                        }

                        citySb.AppendLine(@$"
                    </tbody>
                </table>
                <div class=""load-more-container"" style=""text-align: center; margin-top: 20px;"">
                    <button type=""button"" 
                            class=""load-more-btn"" 
                            data-type=""CityListForAdmissions""
                            data-layout=""table""
                            data-course=""{secondPart}""
                            data-page=""2""
                            style=""padding: 10px 20px; background: #007bff; color: white; border: none; border-radius: 5px; cursor: pointer;"">
                        Load More Cities
                    </button>
                </div>
                <div id=""noMoreDesktopCityAdmissions"" style=""display: none; text-align: center; margin-top: 20px;""></div>
            </div>

            <!-- Mobile View -->
            <div class=""admission-mobile"">
                <ul class=""admission-list"" id=""mobileCollegesList"">");

                        foreach (var citi in citiesWithAdmissions)
                        {
                            string cityUrl = citi.CityName.ToLower().Replace(" ", "-");
                            citySb.AppendLine($@"
                    <li class=""citywise-list"">
                        <div class=""text-box"">
                            <a href=""/{secondPart.ToLower().Replace(" ", "-")}/admissions-in-{cityUrl}"">
                                {citi.CityName} ({citi.ItemCount})
                            </a>
                        </div>
                    </li>");
                        }

                        citySb.AppendLine($@"
                </ul>
                <div class=""load-more-container"" style=""text-align: center; margin-top: 20px;"">
                    <button type=""button""
                            class=""load-more-btn""
                            data-type=""CityListForAdmissions""
                            data-layout=""list""
                            data-course=""{secondPart}""
                            data-page=""2""
                            style=""padding: 10px 20px; background: #007bff; color: white; border: none; border-radius: 5px; cursor: pointer;"">
                        Load More Cities
                    </button>
                </div>
                <div id=""noMoreMobileCityAdmissions"" style=""display: none; text-align: center; margin-top: 20px;""></div>
            </div>
        </div>
    </div>
</div>");

                        return citySb.ToString();

                    case "CityListForColleges":
                        var citiesWithColleges = GetCitiesWithColleges(secondPart, 5);
                        if (citiesWithColleges == null || !citiesWithColleges.Any())
                            return "<p>No cities with sufficient colleges found.</p>";

                        var cityCollegeSb = new StringBuilder();
                        cityCollegeSb.AppendLine($@"
<div class=""flex-container"">
    <div class=""col-lg-4"">
        <div class=""table-of-content"" style=""margin-right:0;padding-bottom:15px;width:100%;"">
            <h3>Cities with {(guide != null ? guide.Abrevation : course)} Colleges</h3>
            
            <!-- Desktop View -->
            <div class=""desktop-view"">
                <table class=""table mid"">
                    <thead>
                        <tr>
                            <th><span>City Name</span></th>
                            <th><span>College Count</span></th>
                        </tr>
                    </thead>
                    <tbody id=""desktopCityCollegesList"">");

                        foreach (var citi in citiesWithColleges)
                        {
                            string cityUrl = citi.CityName.ToLower().Replace(" ", "-");
                            string courseUrl = secondPart.ToLower().Replace(" ", "-");
                            cityCollegeSb.AppendLine($@"
                        <tr>
                            <td>
                                <a href=""/{courseUrl}/colleges-in-{cityUrl}"">{citi.CityName}</a>
                            </td>
                            <td><span>({citi.ItemCount})</span></td>
                        </tr>");
                        }

                        cityCollegeSb.AppendLine(@$"
                    </tbody>
                </table>
                <div class=""load-more-container"" style=""text-align: center; margin-top: 20px;"">
                    <button type=""button"" 
                            class=""load-more-btn"" 
                            data-type=""CityListForColleges""
                            data-layout=""table""
                            data-course=""{secondPart}""
                            data-page=""2""
                            style=""padding: 10px 20px; background: #007bff; color: white; border: none; border-radius: 5px; cursor: pointer;"">
                        Load More Cities
                    </button>
                </div>
                <div id=""noMoreDesktopCityColleges"" style=""display: none; text-align: center; margin-top: 20px;""></div>
            </div>

            <!-- Mobile View -->
            <div class=""admission-mobile"">
                <ul class=""admission-list"" id=""mobileCollegesList"">");

                        foreach (var citi in citiesWithColleges)
                        {
                            string cityUrl = citi.CityName.ToLower().Replace(" ", "-");
                            string courseUrl = secondPart.ToLower().Replace(" ", "-");
                            cityCollegeSb.AppendLine($@"
                    <li class=""citywise-list"">
                        <div class=""text-box"">
                            <a href=""/{courseUrl}/colleges-in-{cityUrl}"">
                                {citi.CityName} ({citi.ItemCount})
                            </a>
                        </div>
                    </li>");
                        }

                        cityCollegeSb.AppendLine($@"
                </ul>
                <div class=""load-more-container"" style=""text-align: center; margin-top: 20px;"">
                    <button type=""button""
                            class=""load-more-btn""
                            data-type=""CityListForColleges""
                            data-layout=""list""
                            data-course=""{secondPart}""
                            data-page=""2""
                            style=""padding: 10px 20px; background: #007bff; color: white; border: none; border-radius: 5px; cursor: pointer;"">
                        Load More Cities
                    </button>
                </div>
                <div id=""noMoreMobileCityColleges"" style=""display: none; text-align: center; margin-top: 20px;""></div>
            </div>
        </div>
    </div>
</div>");

                        return cityCollegeSb.ToString();

                    default:
                        return match.Value;
                }

                //            switch (firstPart)
                //            {
                //                case "collegesList":
                //                     var colleges = GetCollegesWithCourses(secondPart);
                //                     if (colleges == null || !colleges.Any())
                //                         return "<p>No colleges found.</p>";

                //                     var sb = new StringBuilder();

                //                     sb.AppendLine($@"
                //     <h3>List of Institutes which provide {(guide != null ? guide.Abrevation : course)}</h3>
                //     <div class=""responsive-table desktop-view"">
                //         <table class=""table mid"">
                //             <thead>
                //                 <tr>
                //                     <th><span>Institution Name</span></th>
                //                     <th><span>Program Name</span></th>
                //                     <th><span>Location</span></th>
                //                 </tr>
                //             </thead>
                //             <tbody id=""institutionList"">
                // ");

                //                     foreach (var institution in colleges)
                //                     {
                //                         sb.AppendLine($@"
                //             <tr>
                //                 <td>
                //                     <a href=""/colleges/{institution.CollegeUrl}.aspx"" target=""_blank"">
                //                         <span>
                //                             <img src=""https://cdn.ilmkidunya.com/Inst/logos/{institution.CollegeLogo}"" alt="""" onerror=""this.onerror=null;this.src='https://www.ilmkidunya.com/images/no-colleege-thumb.jpg';"" />
                //                             {institution.CollegeName}
                //                         </span>
                //                     </a>
                //                 </td>
                //                 <td><span>{institution.CourseName}</span></td>
                //                 <td><span>{institution.CollegeAddress}</span></td>
                //             </tr>");
                //                     }

                //                     sb.AppendLine(@$"
                //         </tbody>
                //     </table>
                //     <div class=""load-more-container"" style=""text-align: center; margin-top: 20px;"">
                //         <button type=""button"" 
                //                 class=""load-more-btn"" 
                //                 data-type=""collegesList""
                //                 data-layout=""table""
                //                 data-course=""{secondPart}""
                //                 data-page=""2""
                //                 style=""padding: 10px 20px; background: #007bff; color: white; border: none; border-radius: 5px; cursor: pointer;"">
                //             Load More Colleges
                //         </button>
                //     </div>
                //     <div id=""noMoreDesktop"" style=""display: none; text-align: center; margin-top: 20px;""></div>
                // </div>");

                //                     // Mobile View
                //                     sb.AppendLine($@"
                //     <div class=""admission-mobile"">
                //         <ul class=""admission-list"" id=""mobileInstitutionList"">
                // ");

                //                     foreach (var institution in colleges)
                //                     {
                //                         sb.AppendLine($@"
                //             <li>
                //                 <figure> 
                //                     <img src=""https://cdn.ilmkidunya.com/Inst/logos/{institution.CollegeLogo}"" 
                //                          alt="""" 
                //                          onerror=""this.onerror=null;this.src='https://www.ilmkidunya.com/images/no-colleege-thumb.jpg';"" /> 
                //                 </figure>
                //                 <div class=""text-box"">
                //                     <a href=""/colleges/{institution.CollegeUrl}.aspx"" target=""_blank"">
                //                         {institution.CollegeName}
                //                     </a>
                //                     <span>Program: {institution.CourseName}</span>
                //                     <span>Location: {institution.CollegeAddress}</span>
                //                 </div>
                //             </li>");
                //                     }

                //                     sb.AppendLine($@"
                //         </ul>
                //         <div class=""load-more-container"" style=""text-align: center; margin-top: 20px;"">
                //             <button type=""button""
                //                     class=""load-more-btn""
                //                     data-type=""collegesList""
                //                     data-layout=""list""
                //                     data-course=""{secondPart}""
                //                     data-city=""{city}""
                //                     data-page=""2""
                //                     style=""padding: 10px 20px; background: #007bff; color: white; border: none; border-radius: 5px; cursor: pointer;"">
                //                 Load More Colleges
                //             </button>
                //         </div>
                //         <div id=""noMoreMobile"" style=""display: none; text-align: center; margin-top: 20px;""></div>
                //     </div>
                // ");

                //                     return sb.ToString();

                //                 case "meritsList":
                //                     var meritLists = GetMeritLists(secondPart);
                //                     if (meritLists == null || !meritLists.Any())
                //                         return "<p>No merit lists found.</p>";

                //                     var meritSb = new StringBuilder();
                //                     meritSb.AppendLine($@"
                //////<div class=""contact-sec fee-box"">
                // <div class=""container"">
                //     <div class=""row"">
                //         <div class=""col-lg-12"">
                //             <h2>Latest Merit Lists of {(guide != null ? guide.Abrevation : course)} in different Institutes</h2>
                //             <div class=""responsive-table"" style=""margin-bottom: 30px;"">
                //                 <table class=""table mid"">
                //                     <thead>
                //                         <tr>
                //                             <th><span>Date</span></th>
                //                             <th><span>Institute</span></th>
                //                             <th><span>Program Name</span></th>
                //                             <th><span>Merit List</span></th>
                //                         </tr>
                //                     </thead>
                //                     <tbody>");

                //                     foreach (var merit in meritLists)
                //                     {
                //                         string displayDate = merit.MeritAddedDate;

                //                         if (DateTime.TryParse(merit.MeritAddedDate, out var parsedDate))
                //                         {
                //                             displayDate = parsedDate.ToString("dd-MMM-yyyy");
                //                         }

                //                         meritSb.AppendLine($@"
                //     <tr>
                //         <td>{displayDate}</td>
                //         <td>
                //             <a href=""/colleges/{merit.CollegeUrl}-merit-lists.aspx"" target=""_blank"">
                //                 <span>{merit.CollegeName}</span>
                //             </a>
                //         </td>
                //         <td>{merit.CourseName}</td>
                //         <td>
                //             <a href=""javascript:void(0)"" 
                //                onclick=""openMeritList('https://ikddata.ilmkidunya.com/images/MeritListFiles/{merit.MeritFileName}', '{merit.MeritListName} ({merit.MeritValue})')"" 
                //                class="""">
                //                 {merit.MeritListName}
                //             </a>
                //         </td>
                //     </tr>");
                //                     }


                //                     meritSb.AppendLine(@$"
                //                     </tbody>
                //                 </table>
                //             <div class=""load-more-container"" style=""text-align: center; margin-top: 20px;"">
                //                 <button type=""button"" 
                //                         class=""load-more-btn"" 
                //                         data-type=""meritsList""
                //                         data-course=""{secondPart}""
                //                         data-page=""2""
                //                         style=""padding: 10px 20px; background: #007bff; color: white; border: none; border-radius: 5px; cursor: pointer;"">
                //                     Load More Colleges
                //                 </button>
                //             </div>
                //             </div>
                //         </div>
                //     </div>
                // </div>
                //////</div>






                //////<!-- Merit List Side Viewer -->
                //////<div class=""merit-side"" id=""meritViewer""
                //  style=""position: fixed; right: -100%; top: 0; opacity: 0; visibility: hidden; z-index: 999; transition: 0.3s ease-in-out; width:80%; height:100%; background:#fff; box-shadow:-2px 0 8px rgba(0,0,0,0.2);"">
                // <div class=""merit-box"" style=""height:100%; display:flex; flex-direction:column;"">
                //     <div class=""top-box"" style=""display:flex; justify-content:space-between; align-items:center; padding:10px; border-bottom:1px solid #ddd;"">
                //         <h2 id=""meritTitle"" style=""margin:0;"">Merit List</h2>
                //         <div class=""close-btn"">
                //             <a href=""#"" id=""closeMerit"">
                //                 <img src=""https://images.ishallwin.com/ot-images/red-cross.webp"" alt=""close"">
                //             </a>
                //         </div>
                //     </div>
                //     <iframe id=""meritFrame"" src="""" style=""flex:1; border:none;""></iframe>
                // </div>
                //////</div>

                //////<script>
                // const meritBox = document.getElementById('meritViewer');
                // const meritFrame = document.getElementById('meritFrame');
                // const meritTitle = document.getElementById('meritTitle');
                // const closeMerit = document.getElementById('closeMerit');

                // function openMeritList(fileUrl, title) {{
                //     meritFrame.src = fileUrl;
                //     meritTitle.textContent = title;
                //     meritBox.style.right = '0';
                //     meritBox.style.opacity = '1';
                //     meritBox.style.visibility = 'visible';
                // }}

                // closeMerit.addEventListener('click', function(e) {{
                //     e.preventDefault();
                //     meritBox.style.right = '-100%';
                //     meritBox.style.opacity = '0';
                //     meritBox.style.visibility = 'hidden';
                //     meritFrame.src = '';
                // }});
                //////</script>
                //////");

                //                     return meritSb.ToString();

                //                 case "feesList":
                //                     var feeData = GetCollegesWithCourseFees(secondPart);
                //                     if (feeData == null || !feeData.Any())
                //                         return "<p>No fee information found.</p>";

                //                     var feeSb = new StringBuilder();
                //                     feeSb.AppendLine($@"
                //     <div class=""contact-sec fee-box"">
                //         <div class=""container"">
                //             <div class=""row"">
                //                 <div class=""col-lg-12"">
                //                     <h2>Fee list of different Institutes which provide {(guide != null ? guide.Abrevation : course)}</h2>
                //                     <div class=""responsive-table"" style=""margin-bottom: 30px;"">
                //                         <table class=""table mid"">
                //                             <thead>
                //                                 <tr>
                //                                     <th><span>Institute</span></th>
                //                                     <th><span>Program Name</span></th>
                //                                     <th><span>Duration</span></th>
                //                                     <th><span>Fee Structure</span></th>
                //                                 </tr>
                //                             </thead>
                //                             <tbody>");

                //                     foreach (var fee in feeData)
                //                     {
                //                         feeSb.AppendLine($@"
                //         <tr>
                //             <td>
                //                 <a href=""/colleges/{fee.CollegeUrl}-fee-structure.aspx"" target=""_blank"">
                //                     <span>{fee.CollegeName}</span>
                //                 </a>
                //             </td>
                //             <td>
                //                     <span>{fee.CourseName}</span>
                //             </td>
                //             <td><span>{fee.Duration ?? "N/A"}</span></td>
                //             <td>
                //                 <ul>
                //                     {(!string.IsNullOrEmpty(fee.Fee) ? $"<li>{fee.Fee}</li>" : "<li> - </li>")}
                //                 </ul>
                //             </td>
                //         </tr>");
                //                     }

                //                     feeSb.AppendLine(@$"
                //                             </tbody>
                //                         </table>
                //             <div class=""load-more-container"" style=""text-align: center; margin-top: 20px;"">
                //                 <button type=""button"" 
                //                         class=""load-more-btn"" 
                //                         data-type=""feesList""
                //                         data-course=""{secondPart}""
                //                         data-page=""2""
                //                         style=""padding: 10px 20px; background: #007bff; color: white; border: none; border-radius: 5px; cursor: pointer;"">
                //                     Load More Colleges
                //                 </button>
                //             </div>
                //                     </div>
                //                 </div>
                //             </div>
                //         </div>
                //     </div>");
                //                     return feeSb.ToString();

                //                 case "admissionsList":
                //                     var admissions = GetAdmissions(secondPart);
                //                     if (admissions == null || !admissions.Any())
                //                         return "<p>No admissions found.</p>";

                //                     var admissionSb = new StringBuilder();
                //                     admissionSb.AppendLine($@"
                // <div class=""contact-sec fee-box"">
                //     <div class=""container"">
                //         <div class=""row"">
                //             <div class=""col-lg-12"">
                //                 <h2>Admissions in different Institutes for {(guide != null ? guide.Abrevation : course)}</h2> 
                //                 <div class=""responsive-table"" style=""margin-bottom: 30px;"">
                //                     <table class=""table mid"">
                //                         <thead>
                //                             <tr>
                //                                 <th><span>Date Posted</span></th>
                //                                 <th><span>Last Date</span></th>
                //                                 <th><span>Institute Name</span></th>
                //                                 <th><span>Program Name</span></th>
                //                                 <th><span>Admission Image</span></th>
                //                             </tr>
                //                         </thead>
                //                         <tbody>");

                //                     foreach (var admission in admissions)
                //                     {
                //                         admissionSb.AppendLine($@"
                //         <tr>
                //             <td data-label=""Date Posted"">{admission.Dated?.ToString("dd-MMM-yyyy")}</td>
                //             <td data-label=""Last Date"">{(admission.LastDate?.ToString("dd-MMM-yyyy") ?? "N/A")}</td>
                //             <td>
                //                 <a href=""/colleges/{admission.CollegeUrl}-admission.aspx"" target=""_blank"">
                //                     <span>
                //                         <img src=""https://cdn.ilmkidunya.com/Inst/logos/{admission.CollegeLogo}"" alt="""" style=""max-height:40px; max-width:40px;"" />
                //                         {admission.CollegeName}
                //                     </span>
                //                 </a>
                //             </td>
                //             <td data-label=""Courses/Programs"">
                //                 <ul>
                //                     <li>{admission.CourseName}</li>
                //                 </ul>
                //             </td>
                //             <td data-label=""Admission Notice"">");

                //                         if (!string.IsNullOrEmpty(admission.NoticeImageThumb) && admission.NoticeImageThumb != "/images/no-image.png")
                //                         {
                //                             admissionSb.AppendLine($@"
                //     <a href=""/colleges/{admission.CollegeUrl}-admission.aspx"" target=""_blank"">
                //         <img src=""https://admissions.ilmkidunya.com/admission_notices/Images/NoticeAds/{admission.NoticeImageThumb}"" 
                //              alt=""Admission notice for {admission.CollegeName}"" 
                //              style=""max-height:80px; max-width:80px;"" />
                //     </a>");
                //                         }
                //                         else
                //                         {
                //                             admissionSb.AppendLine($@"
                //     <a href=""/colleges/{admission.CollegeUrl}-admission.aspx"" target=""_blank"">
                //         <img src=""https://cdn.traliv.com/wwwroot/images/not-available.png"" 
                //              alt=""No Image Available"" 
                //              style=""max-height:80px; max-width:80px;"" />
                //     </a>");
                //                         }



                //                         admissionSb.AppendLine("</td></tr>");
                //                     }

                //                     admissionSb.AppendLine(@$"
                //                         </tbody>
                //                     </table>
                //             <div class=""load-more-container"" style=""text-align: center; margin-top: 20px;"">
                //                 <button type=""button"" 
                //                         class=""load-more-btn"" 
                //                         data-type=""admissionsList""
                //                         data-course=""{secondPart}""
                //                         data-page=""2""
                //                         style=""padding: 10px 20px; background: #007bff; color: white; border: none; border-radius: 5px; cursor: pointer;"">
                //                     Load More Colleges
                //                 </button>
                //             </div>
                //                 </div>
                //             </div>
                //         </div>
                //     </div>
                // </div>");

                //                     return admissionSb.ToString();

                //                 case "admissionsInCity":
                //                     var admissionsInCity = GetAdmissions(secondPart, 1, 10, city);
                //                     if (admissionsInCity == null || !admissionsInCity.Any())
                //                         return "<p>No admissions found.</p>";

                //                     var admissionSbInCity = new StringBuilder();
                //                     admissionSbInCity.AppendLine($@"
                // <div class=""contact-sec fee-box"">
                //     <div class=""container"">
                //         <div class=""row"">
                //             <div class=""col-lg-12"">
                //                 <h2>Admissions in different Institutes for {(guide != null ? guide.Abrevation : course)}</h2> 
                //                 <div class=""responsive-table"" style=""margin-bottom: 30px;"">
                //                     <table class=""table mid"">
                //                         <thead>
                //                             <tr>
                //                                 <th><span>Date Posted</span></th>
                //                                 <th><span>Institute Name</span></th>
                //                                 <th><span>City</span></th>
                //                                 <th><span>Program Name</span></th>
                //                                 <th><span>Admission Image</span></th>
                //                             </tr>
                //                         </thead>
                //                         <tbody>");

                //                     foreach (var admission in admissionsInCity)
                //                     {
                //                         admissionSbInCity.AppendLine($@"
                //         <tr>
                //             <td data-label=""Date Posted"">{admission.Dated?.ToString("dd-MMM-yyyy")}</td>
                //             <td>
                //                 <a href=""/colleges/{admission.CollegeUrl}-admission.aspx"" target=""_blank"">
                //                     <span>
                //                         <img src=""https://cdn.ilmkidunya.com/Inst/logos/{admission.CollegeLogo}"" alt="""" style=""max-height:40px; max-width:40px;"" />
                //                         {admission.CollegeName}
                //                     </span>
                //                 </a>
                //             </td>
                //             <td data-label=""City"">{admission.CityName}</td>

                //             <td data-label=""Courses/Programs"">
                //                 <ul>
                //                     <li>{admission.CourseName}</li>
                //                 </ul>
                //             </td>
                //             <td data-label=""Admission Notice"">");

                //                         if (!string.IsNullOrEmpty(admission.NoticeImageThumb) && admission.NoticeImageThumb != "/images/no-image.png")
                //                         {
                //                             admissionSbInCity.AppendLine($@"
                //     <a href=""/colleges/{admission.CollegeUrl}-admission.aspx"" target=""_blank"">
                //         <img src=""https://admissions.ilmkidunya.com/admission_notices/Images/NoticeAds/{admission.NoticeImageThumb}"" 
                //              alt=""Admission notice for {admission.CollegeName}"" 
                //              style=""max-height:80px; max-width:80px;"" />
                //     </a>");
                //                         }
                //                         else
                //                         {
                //                             admissionSbInCity.AppendLine($@"
                //     <a href=""/colleges/{admission.CollegeUrl}-admission.aspx"" target=""_blank"">
                //         <img src=""https://cdn.traliv.com/wwwroot/images/not-available.png"" 
                //              alt=""No Image Available"" 
                //              style=""max-height:80px; max-width:80px;"" />
                //     </a>");
                //                         }



                //                         admissionSbInCity.AppendLine("</td></tr>");
                //                     }

                //                     admissionSbInCity.AppendLine(@$"
                //                         </tbody>
                //                     </table>
                //             <div class=""load-more-container"" style=""text-align: center; margin-top: 20px;"">
                //                 <button type=""button"" 
                //                         class=""load-more-btn"" 
                //                         data-type=""admissionsInCity""
                //                         data-course=""{secondPart}""
                //                         data-page=""2""
                //                         style=""padding: 10px 20px; background: #007bff; color: white; border: none; border-radius: 5px; cursor: pointer;"">
                //                     Load More Colleges
                //                 </button>
                //             </div>
                //                 </div>
                //             </div>
                //         </div>
                //     </div>
                // </div>");

                //                     return admissionSbInCity.ToString();

                //                 case "universitiesInCity":
                //                     var universitiesInCity = GetCollegesWithCourses(secondPart, 1, 10, city);
                //                     if (universitiesInCity == null || !universitiesInCity.Any())
                //                         return "<p>No universities found in this city.</p>";

                //                     var universitySb = new StringBuilder();
                //                     universitySb.AppendLine($@"
                //////<div class=""contact-sec fee-box"">
                // <div class=""container"">
                //     <div class=""row"">
                //         <div class=""col-lg-12"">
                //             <h2>Universities in {city?.Replace("-", " ").ToUpper()} offering {(guide != null ? guide.Abrevation : course)}</h2>

                //             <!-- Desktop View -->
                //             <div class=""responsive-table desktop-view"" style=""margin-bottom: 30px;"">
                //                 <table class=""table mid"">
                //                     <thead>
                //                         <tr>
                //                             <th><span>Institution Name</span></th>
                //                             <th><span>Program Name</span></th>
                //                             <th><span>Location</span></th>
                //                         </tr>
                //                     </thead>
                //                     <tbody id=""desktopUniversityList"">");

                //                     foreach (var institution in universitiesInCity)
                //                     {
                //                         universitySb.AppendLine($@"
                //                         <tr>
                //                             <td>
                //                                 <a href=""/colleges/{institution.CollegeUrl}.aspx"" target=""_blank"">
                //                                     <span>
                //                                         <img src=""https://cdn.ilmkidunya.com/Inst/logos/{institution.CollegeLogo}"" alt="""" onerror=""this.onerror=null;this.src='https://www.ilmkidunya.com/images/no-colleege-thumb.jpg';"" />
                //                                         {institution.CollegeName}
                //                                     </span>
                //                                 </a>
                //                             </td>
                //                             <td><span>{institution.CourseName}</span></td>
                //                             <td><span>{institution.CollegeAddress}</span></td>
                //                         </tr>");
                //                     }

                //                     universitySb.AppendLine(@$"
                //                     </tbody>
                //                 </table>
                //                 <div class=""load-more-container"" style=""text-align: center; margin-top: 20px;"">
                //                     <button type=""button"" 
                //                             class=""load-more-btn"" 
                //                             data-type=""universitiesInCity""
                //                             data-layout=""table""
                //                             data-course=""{secondPart}""
                //                             data-city=""{city}""
                //                             data-page=""2""
                //                             style=""padding: 10px 20px; background: #007bff; color: white; border: none; border-radius: 5px; cursor: pointer;"">
                //                         Load More Universities
                //                     </button>
                //                 </div>
                //                 <div id=""noMoreDesktopUniversities"" style=""display: none; text-align: center; margin-top: 20px;""></div>
                //             </div>

                //             <!-- Mobile View -->
                //             <div class=""admission-mobile"">
                //                 <ul class=""admission-list"" id=""mobileUniversityList"">");

                //                     foreach (var institution in universitiesInCity)
                //                     {
                //                         universitySb.AppendLine($@"
                //                     <li>
                //                         <figure> 
                //                             <img src=""https://cdn.ilmkidunya.com/Inst/logos/{institution.CollegeLogo}"" 
                //                                  alt="""" 
                //                                  onerror=""this.onerror=null;this.src='https://www.ilmkidunya.com/images/no-colleege-thumb.jpg';"" /> 
                //                         </figure>
                //                         <div class=""text-box"">
                //                             <a href=""/colleges/{institution.CollegeUrl}.aspx"" target=""_blank"">
                //                                 {institution.CollegeName}
                //                             </a>
                //                             <span>Program: {institution.CourseName}</span>
                //                             <span>Location: {institution.CollegeAddress}</span>
                //                         </div>
                //                     </li>");
                //                     }

                //                     universitySb.AppendLine($@"
                //                 </ul>
                //                 <div class=""load-more-container"" style=""text-align: center; margin-top: 20px;"">
                //                     <button type=""button""
                //                             class=""load-more-btn""
                //                             data-type=""universitiesInCity""
                //                             data-layout=""list""
                //                             data-course=""{secondPart}""
                //                             data-city=""{city}""
                //                             data-page=""2""
                //                             style=""padding: 10px 20px; background: #007bff; color: white; border: none; border-radius: 5px; cursor: pointer;"">
                //                         Load More Universities
                //                     </button>
                //                 </div>
                //                 <div id=""noMoreMobileUniversities"" style=""display: none; text-align: center; margin-top: 20px;""></div>
                //             </div>
                //         </div>
                //     </div>
                // </div>
                //////</div>");

                //                     return universitySb.ToString();

                //                 case "CityListForAdmissions":
                //                     var citiesWithAdmissions = GetCitiesWithAdmissions(secondPart, 5);
                //                     if (citiesWithAdmissions == null || !citiesWithAdmissions.Any())
                //                         return "<p>No cities with sufficient admissions found.</p>";

                //                     var citySb = new StringBuilder();
                //                     citySb.AppendLine($@"
                //////<div class=""flex-container"">
                // <div class=""col-lg-4"">
                //     <div class=""table-of-content"" style=""margin-right:0;padding-bottom:15px;width:100%;"">
                //         <h3><a href=""#"">Cities with {(guide != null ? guide.Abrevation : course)} Admissions</a></h3>
                //         <ul>");

                //                     foreach (var citi in citiesWithAdmissions)
                //                     {
                //                         string cityUrl = citi.CityName.ToLower().Replace(" ", "-");
                //                         citySb.AppendLine($@"
                //             <li class=""citywise-list"">
                //                 <a href=""/{secondPart.ToLower().Replace(" ", "-")}/admissions-in-{cityUrl}"">{citi.CityName} ({citi.ItemCount})</a>
                //             </li>");
                //                     }

                //                     citySb.AppendLine(@"
                //         </ul>
                //     </div>
                // </div>
                //////</div>");

                //                     return citySb.ToString();

                //                 case "CityListForColleges":
                //                     var citiesWithColleges = GetCitiesWithColleges(secondPart, 5);
                //                     if (citiesWithColleges == null || !citiesWithColleges.Any())
                //                         return "<p>No cities with sufficient colleges found.</p>";

                //                     var cityCollegeSb = new StringBuilder();
                //                     cityCollegeSb.AppendLine($@"
                //////<div class=""flex-container"">
                // <div class=""col-lg-4"">
                //     <div class=""table-of-content"" style=""margin-right:0;padding-bottom:15px;width:100%;"">
                //         <h3>Cities with {(guide != null ? guide.Abrevation : course)} Colleges</h3>
                //         <ul>");

                //                     foreach (var citi in citiesWithColleges)
                //                     {
                //                         string cityUrl = citi.CityName.ToLower().Replace(" ", "-");
                //                         string courseUrl = secondPart.ToLower().Replace(" ", "-");
                //                         cityCollegeSb.AppendLine($@"
                //             <li class=""citywise-list"">
                //                 <a href=""/{courseUrl}/colleges-in-{cityUrl}"">{citi.CityName} ({citi.ItemCount})</a>
                //             </li>");
                //                     }

                //                     cityCollegeSb.AppendLine(@"
                //         </ul>
                //     </div>
                // </div>
                //////</div>");

                //                     return cityCollegeSb.ToString();

                //                    default:
                //                        return match.Value; // if unknown, keep placeholder as-is
                //                }
            });
        }


        public string LoadMoreData(string type, string course, int page, string? city = null, string? layout = "")
        {
            course = course.ToLower().Replace("-", " ");
            var words = course.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++)
            {
                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
            }
            string formattedCourse = string.Join(" ", words);

            switch (type)
            {
                case "collegesList":
                    var colleges = GetCollegesWithCourses(course, page);
                    if (colleges == null || !colleges.Any())
                        return string.Empty;

                    var sb = new StringBuilder();
                    if (layout == "table")
                    {
                        foreach (var institution in colleges)
                        {
                            sb.AppendLine($@"
    <tr>
        <td>
            <a href=""/colleges/{institution.CollegeUrl}.aspx"" target=""_blank"">
                <span>
                    <img src=""https://cdn.ilmkidunya.com/Inst/logos/{institution.CollegeLogo}"" alt="""" />
                    {institution.CollegeName}
                </span>
            </a>
        </td>
        <td><span>{institution.CourseName}</span></td>
        <td><span>{institution.CollegeAddress}</span></td>
    </tr>");
                        }
                    }
                    else if (layout == "list")
                    {
                        foreach (var institution in colleges)
                        {
                            sb.AppendLine($@"
        <li>
            <figure>
                <img src=""https://cdn.ilmkidunya.com/Inst/logos/{institution.CollegeLogo}""
                     alt=""""
                     style=""max-height:60px;max-width:60px;margin-right:8px;"" />
            </figure>
            <div class=""text-box"">
                <a href=""/colleges/{institution.CollegeUrl}.aspx"">{institution.CollegeName}</a>
                <span>{institution.CourseName}</span>
                
            </div>
        </li>");
                        }
                    }

                    return sb.ToString();

                case "meritsList":
                    var meritLists = GetMeritLists(course, page);
                    if (meritLists == null || !meritLists.Any())
                        return string.Empty;

                    var meritSb = new StringBuilder();

                    if (layout == "table")
                    {
                        foreach (var merit in meritLists)
                        {
                            string displayDate = merit.MeritAddedDate;
                            if (DateTime.TryParse(merit.MeritAddedDate, out var parsedDate))
                            {
                                displayDate = parsedDate.ToString("dd-MMM-yyyy");
                            }

                            meritSb.AppendLine($@"
            <tr>
                <td>{displayDate}</td>
                <td>
                    <a href=""/colleges/{merit.CollegeUrl}-merit-lists.aspx"" target=""_blank"">
                        <span>{merit.CollegeName}</span>
                    </a>
                </td>
                <td>{merit.CourseName}</td>
                <td>
                    <a href=""javascript:void(0)"" 
                       onclick=""openMeritList('https://ikddata.ilmkidunya.com/images/MeritListFiles/{merit.MeritFileName}', '{merit.MeritListName} ({merit.MeritValue})')"" 
                       class="""">
                        {merit.MeritListName}
                    </a>
                </td>
            </tr>");
                        }
                    }
                    else if (layout == "list")
                    {
                        foreach (var merit in meritLists)
                        {
                            string displayDate = merit.MeritAddedDate;
                            if (DateTime.TryParse(merit.MeritAddedDate, out var parsedDate))
                            {
                                displayDate = parsedDate.ToString("dd-MMM-yyyy");
                            }

                            meritSb.AppendLine($@"
            <li>
                <div class=""text-box"">
                    <a href=""/colleges/{merit.CollegeUrl}-merit-lists.aspx"" target=""_blank"">
                        {merit.CollegeName}
                    </a>
                    <span>Date: {displayDate}</span>
                    <span>Program: {merit.CourseName}</span>
                    <span>
                        <a href=""javascript:void(0)"" 
                           onclick=""openMeritList('https://ikddata.ilmkidunya.com/images/MeritListFiles/{merit.MeritFileName}', '{merit.MeritListName} ({merit.MeritValue})')"">
                            {merit.MeritListName}
                        </a>
                    </span>
                </div>
            </li>");
                        }
                    }
                    return meritSb.ToString();

                case "feesList":
                    var feeData = GetCollegesWithCourseFees(course, page);
                    if (feeData == null || !feeData.Any())
                        return string.Empty;

                    var feeSb = new StringBuilder();

                    if (layout == "table")
                    {
                        foreach (var fee in feeData)
                        {
                            feeSb.AppendLine($@"
            <tr>
                <td>
                    <a href=""/colleges/{fee.CollegeUrl}-fee-structure.aspx"" target=""_blank"">
                        <span>{fee.CollegeName}</span>
                    </a>
                </td>
                <td>
                    <span>{fee.CourseName}</span>
                </td>
                <td><span>{fee.Duration ?? "N/A"}</span></td>
                <td>
                    <ul>
                        {(!string.IsNullOrEmpty(fee.Fee) ? $"<li>{fee.Fee}</li>" : "<li> - </li>")}
                    </ul>
                </td>
            </tr>");
                        }
                    }
                    else if (layout == "list")
                    {
                        foreach (var fee in feeData)
                        {
                            feeSb.AppendLine($@"
            <li>
                <div class=""text-box"">
                    <a href=""/colleges/{fee.CollegeUrl}-fee-structure.aspx"" target=""_blank"">
                        {fee.CollegeName}
                    </a>
                    <span>Program: {fee.CourseName}</span>
                    <span>Duration: {fee.Duration ?? "N/A"}</span>
                    <span>Fee: {(!string.IsNullOrEmpty(fee.Fee) ? fee.Fee : "N/A")}</span>
                </div>
            </li>");
                        }
                    }
                    return feeSb.ToString();

                case "admissionsList":
                    var admissions = GetAdmissions(course, page);
                    if (admissions == null || !admissions.Any())
                        return string.Empty;

                    var admissionSb = new StringBuilder();

                    if (layout == "table")
                    {
                        foreach (var admission in admissions)
                        {
                            admissionSb.AppendLine($@"
            <tr>
                <td data-label=""Date Posted"">{admission.Dated?.ToString("dd-MMM-yyyy")}</td>
                <td data-label=""Last Date"">{(admission.LastDate?.ToString("dd-MMM-yyyy") ?? "N/A")}</td>
                <td>
                    <a href=""/colleges/{admission.CollegeUrl}-admission.aspx"" target=""_blank"">
                        <span>
                            <img src=""https://cdn.ilmkidunya.com/Inst/logos/{admission.CollegeLogo}"" 
                                 alt="""" 
                                 onerror=""this.onerror=null;this.src='https://www.ilmkidunya.com/images/no-colleege-thumb.jpg';""
                                 style=""max-height:40px; max-width:40px;"" />
                            {admission.CollegeName}
                        </span>
                    </a>
                </td>
                <td data-label=""Courses/Programs"">
                    <ul>
                        <li>{admission.CourseName}</li>
                    </ul>
                </td>
                <td data-label=""Admission Notice"">");

                            if (!string.IsNullOrEmpty(admission.NoticeImageThumb) && admission.NoticeImageThumb != "/images/no-image.png")
                            {
                                admissionSb.AppendLine($@"
                <a href=""/colleges/{admission.CollegeUrl}-admission.aspx"" target=""_blank"">
                    <img src=""https://admissions.ilmkidunya.com/admission_notices/Images/NoticeAds/{admission.NoticeImageThumb}"" 
                         alt=""Admission notice for {admission.CollegeName}"" 
                         style=""max-height:80px; max-width:80px;"" />
                </a>");
                            }
                            else
                            {
                                admissionSb.AppendLine($@"
                <a href=""/colleges/{admission.CollegeUrl}-admission.aspx"" target=""_blank"">
                    <img src=""https://cdn.traliv.com/wwwroot/images/not-available.png"" 
                         alt=""No Image Available"" 
                         style=""max-height:80px; max-width:80px;"" />
                </a>");
                            }

                            admissionSb.AppendLine("</td></tr>");
                        }
                    }
                    else if (layout == "list")
                    {
                        foreach (var admission in admissions)
                        {
                            admissionSb.AppendLine($@"
            <li>
                <figure> 
                    <img src=""https://cdn.ilmkidunya.com/Inst/logos/{admission.CollegeLogo}"" 
                         alt=""{admission.CollegeName}"" 
                         onerror=""this.onerror=null;this.src='https://www.ilmkidunya.com/images/no-colleege-thumb.jpg';""
                         style=""max-height:60px;max-width:60px;margin-right:8px;"" />
                </figure>
                <div class=""text-box"">
                    <a href=""/colleges/{admission.CollegeUrl}-admission.aspx"" target=""_blank"">
                        {admission.CollegeName}
                    </a>
                    <span>Date Posted: {admission.Dated?.ToString("dd-MMM-yyyy")}</span>
                    <span>Last Date: {(admission.LastDate?.ToString("dd-MMM-yyyy") ?? "N/A")}</span>
                    <span>Program: {admission.CourseName}</span>
                </div>
            </li>");
                        }
                    }
                    return admissionSb.ToString();

                case "admissionsInCity":
                    var admissionsInCity = GetAdmissions(course, page, 10, city);
                    if (admissionsInCity == null || !admissionsInCity.Any())
                        return string.Empty;

                    var admissionSbInCity = new StringBuilder();

                    if (layout == "table")
                    {
                        foreach (var admission in admissionsInCity)
                        {
                            admissionSbInCity.AppendLine($@"
            <tr>
                <td data-label=""Date Posted"">{admission.Dated?.ToString("dd-MMM-yyyy")}</td>
                <td>
                    <a href=""/colleges/{admission.CollegeUrl}-admission.aspx"" target=""_blank"">
                        <span>
                            <img src=""https://cdn.ilmkidunya.com/Inst/logos/{admission.CollegeLogo}"" 
                                 alt="""" 
                                 onerror=""this.onerror=null;this.src='https://www.ilmkidunya.com/images/no-colleege-thumb.jpg';""
                                 style=""max-height:40px; max-width:40px;"" />
                            {admission.CollegeName}
                        </span>
                    </a>
                </td>
                <td data-label=""City"">{admission.CityName}</td>
                <td data-label=""Courses/Programs"">
                    <ul>
                        <li>{admission.CourseName}</li>
                    </ul>
                </td>
                <td data-label=""Admission Notice"">");

                            if (!string.IsNullOrEmpty(admission.NoticeImageThumb) && admission.NoticeImageThumb != "/images/no-image.png")
                            {
                                admissionSbInCity.AppendLine($@"
                <a href=""/colleges/{admission.CollegeUrl}-admission.aspx"" target=""_blank"">
                    <img src=""https://admissions.ilmkidunya.com/admission_notices/Images/NoticeAds/{admission.NoticeImageThumb}"" 
                         alt=""Admission notice for {admission.CollegeName}"" 
                         style=""max-height:80px; max-width:80px;"" />
                </a>");
                            }
                            else
                            {
                                admissionSbInCity.AppendLine($@"
                <a href=""/colleges/{admission.CollegeUrl}-admission.aspx"" target=""_blank"">
                    <img src=""https://cdn.traliv.com/wwwroot/images/not-available.png"" 
                         alt=""No Image Available"" 
                         style=""max-height:80px; max-width:80px;"" />
                </a>");
                            }

                            admissionSbInCity.AppendLine("</td></tr>");
                        }
                    }
                    else if (layout == "list")
                    {
                        foreach (var admission in admissionsInCity)
                        {
                            admissionSbInCity.AppendLine($@"
            <li>
                <figure> 
                    <img src=""https://cdn.ilmkidunya.com/Inst/logos/{admission.CollegeLogo}"" 
                         alt=""{admission.CollegeName}"" 
                         onerror=""this.onerror=null;this.src='https://www.ilmkidunya.com/images/no-colleege-thumb.jpg';""
                         style=""max-height:60px;max-width:60px;margin-right:8px;"" />
                </figure>
                <div class=""text-box"">
                    <a href=""/colleges/{admission.CollegeUrl}-admission.aspx"" target=""_blank"">
                        {admission.CollegeName}
                    </a>
                    <span>Date Posted: {admission.Dated?.ToString("dd-MMM-yyyy")}</span>
                    <span>City: {admission.CityName}</span>
                    <span>Program: {admission.CourseName}</span>
                </div>
            </li>");
                        }
                    }
                    return admissionSbInCity.ToString();

                case "universitiesInCity":
                    var collegesInCity = GetCollegesWithCourses(course, page, 10, city);
                    if (collegesInCity == null || !collegesInCity.Any())
                        return string.Empty;

                    var collegeCitySb = new StringBuilder();

                    if (layout == "table")
                    {
                        foreach (var institution in collegesInCity)
                        {
                            collegeCitySb.AppendLine($@"
<tr>
    <td>
        <a href=""/colleges/{institution.CollegeUrl}.aspx"" target=""_blank"">
            <span>
                <img src=""https://cdn.ilmkidunya.com/Inst/logos/{institution.CollegeLogo}"" 
                     alt=""{institution.CollegeName}"" 
                     onerror=""this.onerror=null;this.src='https://www.ilmkidunya.com/images/no-colleege-thumb.jpg';"" />
                {institution.CollegeName}
            </span>
        </a>
    </td>
    <td><span>{institution.CourseName}</span></td>
    <td><span>{institution.CollegeAddress}</span></td>
</tr>");
                        }
                    }
                    else if (layout == "list")
                    {
                        foreach (var institution in collegesInCity)
                        {
                            collegeCitySb.AppendLine($@"
<li>
    <figure> 
        <img src=""https://cdn.ilmkidunya.com/Inst/logos/{institution.CollegeLogo}"" 
             alt=""{institution.CollegeName}"" 
             onerror=""this.onerror=null;this.src='https://www.ilmkidunya.com/images/no-colleege-thumb.jpg';""
             style=""max-height:60px;max-width:60px;margin-right:8px;"" />
    </figure>
    <div class=""text-box"">
        <a href=""/colleges/{institution.CollegeUrl}.aspx"" target=""_blank"">
            {institution.CollegeName}
        </a>
        <span>Program: {institution.CourseName}</span>
        <span>Location: {institution.CollegeAddress}</span>
    </div>
</li>");
                        }
                    }
                    return collegeCitySb.ToString();

                default:
                    return string.Empty;
            }
        }



        [HttpPost]
        public IActionResult LoadMoreData([FromBody] LoadMoreRequest request)
        {
            try
            {
                Console.WriteLine($"LoadMoreData called: Type={request.Type}, Course={request.Course}, Page={request.Page}, City={request.City}, Layout={request.Layout}");

                var html = LoadMoreData(request.Type, request.Course, request.Page, request.City, request.Layout);

                Console.WriteLine($"Returning HTML length: {html?.Length ?? 0}");
                return Content(html, "text/html");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LoadMoreData: {ex.Message}");
                return StatusCode(500, "Error loading more data");
            }
        }

        public class LoadMoreRequest
        {
            public string Type { get; set; }
            public string Course { get; set; }
            public int Page { get; set; }

            public string City { get; set; }

            public string? Layout { get; set; }
        }
        private string GetAdmissionLogoPath(string fileName, DateTime? dated)
        {
            if (string.IsNullOrEmpty(fileName) || dated == null)
                return ""; // fallback image

            var year = dated.Value.Year;
            var month = dated.Value.Month.ToString(); // e.g. 03 for March

            return $"{year}/{month}/thumb/{fileName}";
        }
    }
}