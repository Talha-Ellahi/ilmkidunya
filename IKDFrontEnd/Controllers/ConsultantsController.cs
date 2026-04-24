using IKDFrontEnd.Models;
using IKDFrontEnd.Services;
using IKDFrontEnd.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace IKDFrontEnd.Controllers
{
    public class ConsultantsController : Controller
    {
        private readonly DbikdContext _context;
        private readonly BannerService _bannerService;
        private readonly CmsRepository _cmsRepo;

        public ConsultantsController(DbikdContext context, BannerService bannerService, CmsRepository cmsRepo)
        {
            _context = context;
            _bannerService = bannerService;
            _cmsRepo = cmsRepo;
        }

        [Route("/consultants/")]
        public async Task<IActionResult> Consultant()


        {
            var sectionData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/consultants");

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

            var premiumConsultants = await _context.TblDefConsultants
                        .Where(c => c.PremiumMember == true && c.Approve == true)
                        .Select(c => new ConsultantBasicViewModel
                        {
                            Name = c.CompanyName,
                            Url = c.RewriteUrl,
                            Image = "https://cdn.ilmkidunya.com/StudyAbroad/Images/" + c.Logo, // concatenate
                            PremiumMember = c.PremiumMember,
                            Id = c.CompanyId
                        })
                        .Take(12)
                        .ToListAsync();


            if (premiumConsultants.Count < 12)
            {
                var remainingCount = 12 - premiumConsultants.Count;

                var approvedConsultants = await _context.TblDefConsultants
                    .Where(c => c.Approve == true && c.PremiumMember == false)
                    .Select(c => new ConsultantBasicViewModel
                    {
                        Name = c.CompanyName,
                        Url = c.RewriteUrl,
                        Image = c.Logo,
                        PremiumMember = c.PremiumMember,
                        Id = c.CompanyId,
                    })
                    .Take(remainingCount)
                    .ToListAsync();

                premiumConsultants.AddRange(approvedConsultants);
            }

            var allConsultants = premiumConsultants;


            var viewModel = new ConsultantHomeViewModel
            {
                Consultants = allConsultants,
                MetaData = guideDetail
            };
            var cities = await _context.TblDefCities.ToListAsync();
            ViewBag.CityList = cities;

            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;
            return View(viewModel);
        }
        [HttpGet("consultants/{detailSlug}")]
        [HttpGet("consultants/education-consultants-in-{detailSlug}.aspx")]
        public async Task<IActionResult> ConsultantsInCity(string detailSlug)
        {
            var sectionData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/consultants/education-consultants-in-{detailSlug}.aspx");


            if (sectionData == null)
            {
                string slugWithoutExt = detailSlug.Replace(".aspx", "");
                string idPart = new string(slugWithoutExt.Reverse()
                                                         .TakeWhile(char.IsDigit)
                                                         .Reverse()
                                                         .ToArray());

                if (int.TryParse(idPart, out int consultantId))
                {
                    return await ConsultantDetail(consultantId);
                    

                }

                return NotFound();
            }

            var city = await _context.TblDefCities
                            .Where(c => c.Url.ToLower() == detailSlug.Replace(".aspx", "").ToLower())
                            .FirstOrDefaultAsync();

            if (city == null)
                return NotFound();

            var consultants = await _context.TblDefConsultants
                .Where(c => c.Approve == true && c.CityId == city.CityId) 
                .OrderByDescending(c => c.PremiumMember) 
                .Select(c => new ConsultantBasicViewModel
                {
                    Name = c.CompanyName,
                    Url = c.RewriteUrl,
                    Image = c.Logo,
                    PremiumMember = c.PremiumMember,
                    Id = c.CompanyId
                })
                .ToListAsync();
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
                Detail2 = sectionData?.Desc2,
                Consultants = consultants
            };
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;
            return View("ConsultantsInCity", guideDetail);
        }

        [HttpGet("consultants/all")]
        public async Task<IActionResult> AllConsultants(string detailSlug)
        {
            var allConsultants = await _context.TblDefConsultants
                  .Where(c => c.Approve == true)
                  .OrderByDescending(c => c.PremiumMember) 
                  .Select(c => new ConsultantBasicViewModel
                  {
                      Name = c.CompanyName,
                      Url = c.RewriteUrl,
                      Image = c.Logo,
                      PremiumMember = c.PremiumMember,
                      Id = c.CompanyId,
                  })
                  .ToListAsync();


            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            return View("AllConsultants", allConsultants);
        }

      
        public async Task<IActionResult> ConsultantDetail(int id)
        {
            // Consultant record by ID
            var consultant = await _context.TblDefConsultants
                .Where(c => c.Approve == true && c.CompanyId == id)
                .Select(c => new ConsultantBasicViewModel
                {
                    Name = string.IsNullOrWhiteSpace(c.CompanyName) ? "Not Available" : c.CompanyName,
                    Url = c.RewriteUrl,
                    Image = string.IsNullOrWhiteSpace(c.Logo) ? "/images/default-image.png" : c.Logo,
                    PremiumMember = c.PremiumMember,
                    Views = c.Views ?? 0,
                    Phone = string.IsNullOrWhiteSpace(c.Phone) ? "Not Available" : c.Phone,
                    Address = string.IsNullOrWhiteSpace(c.MaillingAddress) ? "Not Available" : c.MaillingAddress,
                    Detail = string.IsNullOrWhiteSpace(c.Serviceprovide) ? "Not Available" : c.Serviceprovide,
                    Zoom = c.Zoom,
                    Latitude = c.Latitude,
                    Longitude = c.Longitude,
                    MetaTitle = c.CompanyName + " Consultancy Services for Abroad",

                    MetaDescription = !string.IsNullOrWhiteSpace(c.Serviceprovide)
                                ? c.Serviceprovide
                                : c.CompanyName + " provides education Consultancy Services for students to study abroad and how to get scholarships easily.",

                    MetaKeywords = c.CompanyName + " consultancy services, " +
                                  c.CompanyName + " education consultancy, " +
                                  c.CompanyName + " consultancy for abroad, " +
                                  c.CompanyName + " scholarship consultancy.",

                })
                .FirstOrDefaultAsync();

            if (consultant == null)
                return NotFound();

            var countries = await (from cc in _context.TblConsultantCountries
                                   join country in _context.TblDefCountries
                                       on cc.CountryId equals country.CountryId
                                   where cc.CompanyId == id
                                   select country)
                            .ToListAsync();

            ViewBag.ConsultantCountries = countries; // Changed from Countries to ConsultantCountries


            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            return View("ConsultantDetail", consultant);
        }




    }
}
