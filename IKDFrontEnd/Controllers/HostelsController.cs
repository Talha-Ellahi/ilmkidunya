using IKDFrontEnd.Models;
using IKDFrontEnd.Services;
using IKDFrontEnd.ViewModels;
using IKDFrontEnd.ViewModels.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace IKDFrontEnd.Controllers
{
    [Route("hostels")]
    public class HostelsController : Controller
    {
        DbikdContext _context;
        BannerService _bannerService;
        private readonly CmsRepository _cmsRepo;
        public HostelsController(DbikdContext context, BannerService bannnerService, CmsRepository cmsRepo)
        {
            _context = context;

            _bannerService = bannnerService;
            _cmsRepo = cmsRepo;
        }


        [HttpGet("")]
        public async Task<IActionResult> Home()
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;
            var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/hostels/");
            ViewBag.CmsData = cmsData;
            var cities = await _context.TblDefCities
                .Where(c => _context.TblHostels.Any(h => h.CityId == c.CityId && h.IsActive == true && h.IsApproved == true))
                .OrderBy(c => c.CityName)
                .ToListAsync();

            var hostels = await _context.TblHostels
                .Where(h => h.IsActive == true && h.IsApproved == true)
                .OrderByDescending(h => h.Dated)
                .Take(10)
                .ToListAsync();
            var citySelectList = cities
                .Select(c => new SelectListItem
                {
                    Value = c.CityId.ToString(),
                    Text = c.CityName
                }).ToList();

            var citiesWithImages = cities
                .Where(c => !string.IsNullOrEmpty(c.ImageName) && c.IsImageAvailable == true)
                .Select(c => new CityWithImage
                {
                    CityId = c.CityId,
                    CityName = c.CityName!,
                    ImagePath = "https://images.ishallwin.com/bs-chem/" + c.ImageName.Replace(".png", ".webp")!,
                }).ToList();

            var latestHostels = hostels
                .Select(h => new HostelCard
                {
                    HostelId = h.Id,
                    HostelName = h.HostelName,
                    HostelUrl = h.RewriteUrl,
                    CityName = _context.TblDefCities
                        .FirstOrDefault(c => c.CityId == h.CityId)?.CityName ?? "N/A",
                    MainImage = string.IsNullOrEmpty(h.MainImage) ? "/images/hostel.jpg" : "/images/hostels/" + h.MainImage,
                    Price = _context.TblHostelRoomTypes
                        .Where(r => r.HostelId == h.Id)
                        .OrderBy(r => r.Cost)
                        .Select(r => r.Cost)
                        .FirstOrDefault()
                }).ToList();

            var viewModel = new HostelHomeViewModel
            {
                CitySelectList = citySelectList,
                CitiesWithImages = citiesWithImages,
                LatestHostels = latestHostels
            };

            return View(viewModel);
        }


        [HttpGet("GetHostelLocations")]
        public async Task<IActionResult> GetHostelLocations()
        {
            var data = await _context.TblHostels
                .AsNoTracking()
                .Where(h => h.IsActive == true && h.IsApproved == true && h.Latitude != null && h.Longitude != null)
                .Select(h => new
                {
                    h.Id,
                    h.HostelName,
                    h.Latitude,
                    h.Longitude,
                    h.FullAddress
                })
                .ToListAsync();

            return Json(data);
        }


        [HttpGet("city-wise-hostels-and-rooms")]
        public async Task<IActionResult> HostelsAllCities()
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;
            TblCmsDto cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/hostels/city-wise-hostels-and-rooms");

            if (cmsData == null)
            {
                cmsData = new TblCmsDto
                {
                    MetaTitle = "City Wise Hostels and Rooms for Students",
                    MetaKeys = "city wise hostels, hostels in cities, student hostels, affordable hostels, hostel rooms",
                    MetaDesc = "Find city wise hostels and rooms for students. Affordable and comfortable hostels in various cities.",
                    Heading = "City Wise Hostels and Rooms",
                    Desc1 = "",
                    Desc2 = "",
                    Desc3 = ""
                };
            }
            ViewBag.CmsData = cmsData;

            var allCities = await _context.TblDefCities
                .Where(c => _context.TblHostels.Any(h => h.CityId == c.CityId && h.IsActive == true && h.IsApproved == true))
                .ToListAsync();
            var cityList = allCities
                .Select(c => new SelectListItem
                {
                    Value = c.CityId.ToString(),
                    Text = c.CityName
                })
                .ToList();
            var citiesWithImages = allCities
                .Where(c => !string.IsNullOrEmpty(c.ImageName))
                .Select(c => new CityWithImage
                {
                    CityId = c.CityId,
                    CityName = c.CityName,
                    ImagePath = "https://images.ishallwin.com/bs-chem/" + c.ImageName.Replace(".png", ".webp"),
                }).ToList();
            ViewBag.CitySelectList = cityList;
            ViewBag.CitiesWithImages = citiesWithImages;
            var viewModel = new HostelHomeViewModel
            {
               
                CitiesWithImages = citiesWithImages,
                
            };
            return View(viewModel);
        }


        [HttpGet("hostels-in-{slug:alpha}")]
        public async Task<IActionResult> HostelCityDetail(string slug)
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            TblCmsDto cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/hostels/hostels-in-{slug}");

            var city = await _context.TblDefCities
                .Where(c => c.CityName == slug)
                .FirstOrDefaultAsync();

            if (city == null)
            {
                // Handle no city found - maybe return 404 or redirect
                return NotFound($"City '{slug}' not found.");
            }

            var allCities = await _context.TblDefCities
                .Where(c => _context.TblHostels.Any(h => h.CityId == c.CityId && h.IsActive == true && h.IsApproved == true))
                .ToListAsync();

            var cityList = allCities
                .Select(c => new SelectListItem
                {
                    Value = c.CityId.ToString(),
                    Text = c.CityName
                })
                .ToList();

            var citiesWithImages = allCities
                .Where(c => !string.IsNullOrEmpty(c.ImageName) && c.IsImageAvailable == true)
                .Select(c => new CityWithImage
                {
                    CityId = c.CityId,
                    CityName = c.CityName,
                    ImagePath = "https://images.ishallwin.com/bs-chem/" + c.ImageName.Replace(".png", ".webp"),
                }).ToList();

            List<HostelCard> hostels = new();

            hostels = await _context.TblHostels
                .Where(h => h.IsActive == true && h.IsApproved == true && h.CityId == city.CityId)
                .OrderByDescending(h => h.Dated)
                .Take(10)
                .Select(h => new HostelCard
                {
                    HostelId = h.Id,
                    HostelName = h.HostelName,
                    HostelUrl = h.RewriteUrl,
                    CityName = (_context.TblDefCities
                               .Where(c => c.CityId == h.CityId)
                               .Select(c => c.CityName)
                               .FirstOrDefault()) ?? "N/A",
                    MainImage = string.IsNullOrEmpty(h.MainImage) ? "/images/hostel.jpg" : "/images/hostels/" + h.MainImage,
                    Price = _context.TblHostelRoomTypes
                        .Where(r => r.HostelId == h.Id)
                        .OrderBy(r => r.Cost)
                        .Select(r => r.Cost)
                        .FirstOrDefault()
                })
                .ToListAsync();

            if (cmsData == null)
            {
                cmsData = new TblCmsDto
                {
                    MetaTitle = "Hostels and Rooms In " + city.CityName + " For Students (with Prices)",
                    MetaKeys = "hostels in " + city.CityName + ", single room in " + city.CityName + ", best hostel in " + city.CityName + ", " + city.CityName + " hostel, cheap room in " + city.CityName + ", cheap hostel in " + city.CityName + ", hostel for students in " + city.CityName + ", affordable hostels in " + city.CityName + ", female hostels in " + city.CityName + ".",
                    MetaDesc = "Find the best hostels and rooms In " + city.CityName + " for students. Cheap and affordable hostels or single room for male and female in " + city.CityName + ".",
                    Heading = $"Hostels & Rooms for Stay in {city.CityName}",
                    Desc1 = "",
                    Desc2 = "",
                    Desc3 = ""
                };
            }

            ViewBag.CmsData = cmsData;
            ViewBag.CityId = city.CityId;
            ViewBag.CitySelectList = cityList;
            ViewBag.CitiesWithImages = citiesWithImages;

            return View("HostelCityDetail", hostels);
        }





        [HttpGet("/hostels/load-more")]
        public async Task<IActionResult> LoadMoreHostels(int skip = 10, int? cityId = null)
        {
            var query = _context.TblHostels
                .Where(h => h.IsActive == true && h.IsApproved == true);

            if (cityId.HasValue && cityId.Value > 0)
            {
                query = query.Where(h => h.CityId == cityId.Value);
            }

            var hostels = await query
                .OrderByDescending(h => h.Dated)
                .Skip(skip)
                .Take(10)
                .ToListAsync();

            var hostelCards = hostels.Select(h => new HostelCard
            {
                HostelId = h.Id,
                HostelName = h.HostelName,
                HostelUrl = h.RewriteUrl,
                CityName = _context.TblDefCities
                    .FirstOrDefault(c => c.CityId == h.CityId)?.CityName ?? "N/A",
                MainImage = string.IsNullOrEmpty(h.MainImage) ? "/images/hostel.jpg" : "/images/hostels/" + h.MainImage,
                Price = _context.TblHostelRoomTypes
                    .Where(r => r.HostelId == h.Id)
                    .OrderBy(r => r.Cost)
                    .Select(r => r. Cost)
                    .FirstOrDefault()
            });

            return PartialView("_HostelCardsPartial", hostelCards);
        }



        [HttpPost]
        [Route("hostels/save")]
        public async Task<IActionResult> SaveHostel([FromForm] TblHostel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (model.Id == 0)
                {
                    // Insert new hostel
                    model.Dated = model.Dated ?? DateTime.UtcNow;  // set current date if null
                    _context.TblHostels.Add(model);
                }
                else
                {
                    // Update existing hostel
                    var existingHostel = await _context.TblHostels.FindAsync(model.Id);
                    if (existingHostel == null)
                        return NotFound($"Hostel with ID {model.Id} not found.");

                    // Update fields
                    existingHostel.HostelName = model.HostelName;
                    existingHostel.MemberId = model.MemberId;
                    existingHostel.CityId = model.CityId;
                    existingHostel.CityAreaId = model.CityAreaId;
                    existingHostel.FullAddress = model.FullAddress;
                    existingHostel.HostelDetails = model.HostelDetails;
                    existingHostel.Latitude = model.Latitude;
                    existingHostel.Longitude = model.Longitude;
                    existingHostel.Zoomlevel = model.Zoomlevel;
                    existingHostel.FeatureIds = model.FeatureIds;
                    existingHostel.RewriteUrl = model.RewriteUrl;
                    existingHostel.IsActive = model.IsActive;
                    existingHostel.IsApproved = model.IsApproved;
                    existingHostel.IsFeatured = model.IsFeatured;
                    existingHostel.IsHostel = model.IsHostel;
                    existingHostel.RoomHeading = model.RoomHeading;
                    existingHostel.PhoneNumber = model.PhoneNumber;
                    existingHostel.Price = model.Price;
                    existingHostel.IsMale = model.IsMale;
                    existingHostel.Dated = model.Dated ?? existingHostel.Dated;
                    existingHostel.MainImage = model.MainImage;
                    existingHostel.CityAreaName = model.CityAreaName;

                    _context.TblHostels.Update(existingHostel);
                }

                await _context.SaveChangesAsync();

                return Ok(new { Success = true, HostelId = model.Id == 0 ? model.Id : model.Id });
            }
            catch (Exception ex)
            {
                // log error if needed
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }


        [HttpGet("hostel-registration")]
        public async Task<IActionResult> HostelRegistration()
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            // Get cities
            var cities = await _context.TblDefCities
                .Where(c => c.IsActive == true)
                .OrderBy(c => c.CityName)
                .Select(c => new SelectListItem
                {
                    Value = c.CityId.ToString(),
                    Text = c.CityName
                })
                .ToListAsync();

            // Get hostel features
            var features = await _context.TblHostelFeatures
                .OrderBy(f => f.FeatureName)
                .Select(f => new HostelFeature
                {
                    FeatureId = f.FeatureId,
                    FeatureName = f.FeatureName ?? "",
                    IconPath = "/images/hostel-features/" + f.FeatureName.ToLower().Replace(" ", "-") + ".png"
                })
                .ToListAsync();

            // Prepare ViewModel
            var viewModel = new HostelRegistrationViewModel
            {
                Cities = cities,
                Features = features
            };
            ViewBag.Cities = cities;

            ViewBag.Genders = new List<SelectListItem>
                              {
                                  new SelectListItem { Text = "Select Gender", Value = "" },
                                  new SelectListItem { Text = "Male", Value = "1" },
                                  new SelectListItem { Text = "Female", Value = "0" }
                              };

            return View(viewModel);
        }

        [HttpPost("hostel-registration")]
        public async Task<IActionResult> HostelRegistration(HostelRegistrationViewModel model)
        {
            ModelState.Remove("Features");

            if (!ModelState.IsValid)
            {
                // Reload dropdowns if validation fails
                model.Cities = await _context.TblDefCities
                    .Where(c => c.IsActive == true)
                    .OrderBy(c => c.CityName)
                    .Select(c => new SelectListItem
                    {
                        Value = c.CityId.ToString(),
                        Text = c.CityName
                    })
                    .ToListAsync();

                model.Features = await _context.TblHostelFeatures
                    .OrderBy(f => f.FeatureName)
                    .Select(f => new HostelFeature
                    {
                        FeatureId=   f.FeatureId,
                        FeatureName = f.FeatureName ?? "",
                        IconPath = "/images/hostel-features/" + f.FeatureName.ToLower().Replace(" ", "-") + ".png"
                    })
                    .ToListAsync();
                ViewBag.Cities = model.Cities;
                ViewBag.Genders = new List<SelectListItem>
                              {
                                  new SelectListItem { Text = "Select Gender", Value = "" },
                                  new SelectListItem { Text = "Male", Value = "1" },
                                  new SelectListItem { Text = "Female", Value = "0" }
                              };

                return View(model);
            }

            // Save hostel (similar to SaveHostel method in your controller)
            var hostel = new TblHostel
            {
                HostelName = model.HostelName,
                CityId = model.CityId,
                CityAreaId = model.AreaId,
                FullAddress = model.FullAddress,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                Zoomlevel = model.ZoomLevel,
                HostelDetails = model.HostelDetails,
                FeatureIds = string.Join(",", model.SelectedFeatures),
                IsActive = true,
                IsApproved = false,
                Dated = DateTime.UtcNow,
                MemberId = model.MemberId,
                IsMale = model.Gender == 1,
                PhoneNumber = model.PhoneNumber,
                Price = null,
            };
          

            _context.TblHostels.Add(hostel);
            await _context.SaveChangesAsync();

            return RedirectToAction("HostelDetail", new { slug = hostel.RewriteUrl });
        }



        [HttpGet("GetAreasByCity")]
        public JsonResult GetAreasByCity(int cityId)
        {
            var areas = _context.TblCityAreas
                .Where(a => a.CityId == cityId)
                .Select(a => new
                {
                    id = a.Id,
                    name = a.CityArea
                })
                .ToList();

            return Json(areas);
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> HostelDetail(string slug)
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            TblCmsDto cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/hostels/{slug}");
            int hostelId = int.Parse(slug.Split('-').Last());


            int id = await _context.TblHostels
                .Where(h => h.Id == hostelId)
                .Select(h => h.Id)
                .FirstOrDefaultAsync();

            // 1. Load hostel with related data
            var hostel = await _context.TblHostels
                .AsNoTracking()
                .Include(h => h.TblHostelImages)
                .Include(h => h.TblHostelRoomTypes)
                .Where(h => h.Id == id && h.IsActive == true && h.IsApproved == true)
                .Select(h => new
                {
                    h.HostelName,
                    h.FullAddress,
                    h.CityId,
                    h.MemberId,
                    h.PhoneNumber,
                    h.Latitude,
                    h.Longitude,
                    h.FeatureIds,
                    Images = h.TblHostelImages.Select(i => "/images/hostels/" + i.ImageName).ToList(),
                    RoomTypes = h.TblHostelRoomTypes.Select(rt => new RoomType
                    {
                        RoomTypeName = rt.RoomType ?? "",
                        Details = rt.DetailsRoomType,
                        Cost = rt.Cost
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (hostel == null)
                return NotFound();

            // 2. Get member (wait before moving to next query)
            var member = await _context.TblDefMemberInfo2s
                .AsNoTracking()
                .Where(m => m.MemberId == hostel.MemberId)
                .Select(m => new { m.Email, m.Gender, m.MobileNumber })
                .FirstOrDefaultAsync();

            // 3. Get city name
            var cityName = await _context.TblDefCities
                .AsNoTracking()
                .Where(c => c.CityId == hostel.CityId)
                .Select(c => c.CityName)
                .FirstOrDefaultAsync();

            // 4. Get city list with images
            var citiesWithImages = await _context.TblDefCities
                .AsNoTracking()
                .Where(c => c.IsActive == true && c.IsImageAvailable == true && !string.IsNullOrEmpty(c.ImageName))
                .OrderBy(c => c.CityName)
                .Select(c => new CityWithImage
                {
                    CityId = c.CityId,
                    CityName = c.CityName!,
                    ImagePath = "https://images.ishallwin.com/bs-chem/" + c.ImageName.Replace(".png", ".webp")!
                })
                .ToListAsync();

            // 5. Hostel features
            List<HostelFeature> features = new();
            if (!string.IsNullOrEmpty(hostel.FeatureIds))
            {
                var featureIds = hostel.FeatureIds
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToList();

                features = await _context.TblHostelFeatures
                    .AsNoTracking()
                    .Where(f => featureIds.Contains(f.FeatureId))
                    .Select(f => new HostelFeature
                    {
                        FeatureName = f.FeatureName ?? "",
                        IconPath = "/images/hostel-features/" + f.FeatureName.ToLower() + ".png"
                    })
                    .ToListAsync();
            }

            // 6. Construct ViewModel
            var viewModel = new HostelDetailViewModel
            {
                HostelName = hostel.HostelName,
                FullAddress = hostel.FullAddress,
                CityName = cityName,
                Email = member?.Email,
                Gender = member?.Gender,
                PhoneNumber = hostel.PhoneNumber ?? member?.MobileNumber,
                Latitude = hostel.Latitude,
                Longitude = hostel.Longitude,
                ImagePaths = hostel.Images,
                Features = features,
                RoomTypes = hostel.RoomTypes,
                CityList = citiesWithImages
            };
            if (cmsData == null)
            {
                cmsData = new TblCmsDto
                {
                    MetaTitle = $"{hostel.HostelName} Updated Prices, Pictures and Facilities",
                    MetaKeys = "hostels in " + hostel.HostelName + ", single room in " + hostel.HostelName + ", best hostel in " + hostel.HostelName + ", " + hostel.HostelName + " hostel, cheap room in " + hostel.HostelName + ", cheap hostel in " + hostel.HostelName + ", hostel for students in " + hostel.HostelName + ", affordable hostels in " + hostel.HostelName + ", female hostels in " + hostel.HostelName + ".",
                    MetaDesc = "Find the best hostels and rooms In " + hostel.HostelName + " for students. Cheap and affordable hostels or single room for male and female in " + hostel.HostelName + ".",
                    Heading = $"{hostel.HostelName}",
                    Desc1 = "",
                    Desc2 = "",
                    Desc3 = ""
                };
            }
            ViewBag.CmsData = cmsData;  
            return View(viewModel);
        }




    }
}
