using IKDFrontEnd.Models;
using IKDFrontEnd.Services;
using IKDFrontEnd.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;




namespace IKDFrontEnd.Controllers
{

    public class FlagsController : Controller
    {
        private readonly DbikdContext _context;
        private readonly BannerService _bannerService;


        public FlagsController(DbikdContext context, BannerService bannerService)
        {
            _context = context;
            _bannerService = bannerService;

        }




        [Route("flags/")]
        public async Task<IActionResult> FlagsHome()
        {
            var flags = await (from f in _context.TblFlags
                               join c in _context.Countries on f.CountryId equals c.Id
                               where f.IsActive == true
                               orderby c.Id
                               select new FlagWithCountryViewModel
                               {
                                   FlagId = f.Id,
                                   FlagImage = c.Name.ToLower().Replace(" ", "-") + ".png",
                                   CountryName = c.Name,
                                   CountryUrl = c.Name.ToLower()
                               })
                               .ToListAsync();
            ViewBag.Banners = await _bannerService.GetBannersAsync();
            return View(flags);
        }


        [Route("flags/{countryUrl}")]
        public async Task<IActionResult> FlagsDetail(string countryUrl)
        {
            var flag = await (from f in _context.TblFlags
                              join c in _context.Countries on f.CountryId equals c.Id
                              where c.Name.ToLower() == countryUrl.ToLower()
                              orderby c.Id
                              select new FlagWithCountryViewModel
                              {
                                  MetaTitle = "Flag of " + c.Name + " | History, Meaning, Color, Symbol, Pictures",
                                  MetaDesc = "National Flag of " + c.Name + ", history, meaning, color, symbol, pictures in png, jpeg, and pdf, design, and other information about " + c.Name + " flag are available here.",
                                  MetaKeyword = "Flag of " + c.Name + ", " + c.Name + " Flag, Flag of " + c.Name + " History, Flag of " + c.Name + " Meaning, Flag of " + c.Name + " Color, Flag of " + c.Name + " Symbol, Flag of " + c.Name + " Pictures, " + c.Name + " Flag icon, " + c.Name + " Flag emoji.",
                                  Desc1 = f.Desc1,
                                  Desc2 = f.Desc2,
                                  Desc3 = f.Desc3,
                                  Desc4 = f.Desc4,
                                  Detail = f.Detail,
                                  File1 = f.File1,
                                  File2 = f.File2,
                                  File3 = f.File3,
                                  File4 = f.File4,
                                  File5 = f.File5,
                                  CountryId = f.CountryId,
                                  Continent = f.Continent,
                                  Language = f.Language,
                                  Capital = f.Capital,
                                  Border = f.Border,
                                  Government = f.Government,
                                  Neighbour = f.Neighbour,
                                  Faqs = f.Faqs,
                                  IsActive = f.IsActive,

                                  FlagImage = c.Name.ToLower().Replace(" ", "-") + ".png",
                                  CountryName = c.Name,
                                  CountryUrl = c.Url
                              })
                         .FirstOrDefaultAsync();

            if (flag == null)
            {
                return NotFound();
            }
            if (flag != null && !string.IsNullOrEmpty(flag.Neighbour))
            {
                // Split neighbour IDs string (comma separated)
                var neighbourIds = flag.Neighbour.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                 .Select(id => int.Parse(id.Trim()))
                                                 .ToList();

                // Fetch actual country names from Countries table
                var neighbours = await _context.Countries
                                               .Where(x => neighbourIds.Contains(x.Id))
                                               .Select(x => x.Name)
                                               .ToListAsync();

                flag.NeighbourCountries = neighbours;
            }
            if (flag != null && !string.IsNullOrEmpty(flag.Faqs))
            {
                try
                {
                    flag.FaqList = JsonSerializer.Deserialize<List<FaqItem>>(flag.Faqs);
                }
                catch
                {
                    flag.FaqList = new List<FaqItem>();
                }
            }
            ViewBag.Banners = await _bannerService.GetBannersAsync();
            return View(flag);
        }





    }
}
