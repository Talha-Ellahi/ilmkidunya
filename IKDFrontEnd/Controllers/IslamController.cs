using IKDFrontEnd.Models;
using IKDFrontEnd.Services;
using IKDFrontEnd.ViewModels;
using IKDFrontEnd.ViewModels.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace IKDFrontEnd.Controllers
{
    [Route("islam")]
    public class IslamController : Controller
    {

        private readonly DbikdContext _context;
        private readonly BannerService _bannerService;
        private readonly CmsRepository _cmsRepo;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ILogger<IslamController> _logger;


        public IslamController(DbikdContext context, BannerService bannerService, CmsRepository cmsRepo, IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<IslamController> logger)
        {
            _context = context;
            _bannerService = bannerService;
            _cmsRepo = cmsRepo;
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
        }

        #region Namaz Timings
        [Route("namaz")]
        public async Task<IActionResult> NamazHome()
        {
            // ✅ Load banners and CMS metadata
            ViewBag.Banners = await _bannerService.GetBannersAsync();
            var cmsData = await _cmsRepo.GetByUrlAsync("https://www.ilmkidunya.com/islam/");

            if (cmsData == null)
            {
                cmsData = new TblCmsDto
                {
                    MetaTitle = "Worldwide Islamic Namaz Timings 2026",
                    MetaDesc = "Get accurate global Islamic namaz timings. View daily Salah schedule including Fajr, Dhuhr, Asr, Maghrib, and Isha worldwide.",
                    MetaKeys = "",
                    Heading = "Namaz Timings Worldwide – Find Accurate Namaz Times by Country & City",
                    Desc1 = "",
                    Desc2 = ""
                };
            }
            ViewBag.CmsData = cmsData;

            // ✅ Define cities
            var cities = new Dictionary<string, (double lat, double lon, string country)>
            {
                {"Mecca", (21.3891, 39.8579, "Saudi Arabia")},
                {"Medina", (24.5247, 39.5692, "Saudi Arabia")},
                {"Islamabad", (33.6844, 73.0479, "Pakistan")},
                {"Lahore", (31.5204, 74.3587, "Pakistan")},
                {"Karachi", (24.8607, 67.0011, "Pakistan")},
                {"New York", (40.7128, -74.0060, "USA")},
                {"London", (51.5074, -0.1278, "United Kingdom")},
                {"Delhi", (28.6139, 77.2090, "India")},
                {"Mumbai", (19.0760, 72.8777, "India")},
                {"Jakarta", (-6.2088, 106.8456, "Indonesia")},
                {"Dhaka", (23.8103, 90.4125, "Bangladesh")},
                {"Cairo", (30.0444, 31.2357, "Egypt")},
                {"Lagos", (6.5244, 3.3792, "Nigeria")},
                {"Istanbul", (41.0082, 28.9784, "Turkey")},
                {"Tehran", (35.6892, 51.3890, "Iran")},
                {"Baghdad", (33.3152, 44.3661, "Iraq")},
                {"Kuala Lumpur", (3.1390, 101.6869, "Malaysia")},
                {"Ankara", (39.9334, 32.8597, "Turkey")},
                {"Chittagong", (22.3475, 91.8123, "Bangladesh")},
                {"Kabul", (34.5553, 69.2075, "Afghanistan")},
                {"Alexandria", (31.2001, 29.9187, "Egypt")},
                {"Cape Town", (-33.9249, 18.4241, "South Africa")}
            };

            var model = new PrayerTimesHomeViewModel
            {
                CityPrayerTimes = await GetPrayerTimingsAsync(cities)
            };

            return View(model);
        }






        [Route("{prayerName:regex(^fajr$|^dhuhr$|^zuhr$|^asr$|^maghrib$|^isha$|^sunrise$)}-namaz-timing")]
        public async Task<IActionResult> DynamicNamazTiming(string prayerName)
        {
            prayerName = prayerName.ToLower().Trim();

            // Prayer mapping (API field names)
            var prayerMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "fajr", "Fajr" },
        { "dhuhr", "Dhuhr" },
        { "zuhr", "Dhuhr" },
        { "asr", "Asr" },
        { "maghrib", "Maghrib" },
        { "isha", "Isha" },
        { "sunrise", "Sunrise" }
    };

            if (!prayerMap.ContainsKey(prayerName))
                return NotFound();

            string prayerField = prayerMap[prayerName];

            // Define allPrayerOrder here so it's available throughout the method
            var allPrayerOrder = new List<string> { "Fajr", "Dhuhr", "Asr", "Maghrib", "Isha" };

            // Get user IP
            var ip = GetUserIp();
            if (ip == "::1" || ip == "127.0.0.1")
                ip = "103.255.4.18"; // Lahore IP for local testing

            try
            {
                ViewBag.Banners = await _bannerService.GetBannersAsync();
            }
            catch (Exception ex)
            {
                return RedirectToAction("ServerError", "Error", new
                {
                    reason = $"Error loading banners for {prayerName} prayer times: {ex.Message} | IP: {ip} | Stage: Banners"
                });
            }

            try
            {
                var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/islam/{prayerName}-namaz-timing");
                ViewBag.CmsData = cmsData;
            }
            catch (Exception ex)
            {
                return RedirectToAction("ServerError", "Error", new
                {
                    reason = $"Error loading CMS data for {prayerName} prayer times: {ex.Message} | IP: {ip} | Stage: CMS Data"
                });
            }

            try
            {
                ViewBag.Countries = GetAllCountries();
            }
            catch (Exception ex)
            {
                return RedirectToAction("ServerError", "Error", new
                {
                    reason = $"Error loading countries for {prayerName} prayer times: {ex.Message} | IP: {ip} | Stage: Countries"
                });
            }

            dynamic loc = null;
            try
            {
                // Get location
                loc = await GetLocationFromIp(ip);
                if (loc == null || !loc.success)
                    return View("Error");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ServerError", "Error", new
                {
                    reason = $"Error getting location from IP for {prayerName} prayer times: {ex.Message} | IP: {ip} | Stage: Location from IP"
                });
            }

            City nearestCity = null;
            try
            {
                // Find nearest city
                nearestCity = await FindNearestCityAsync(loc.latitude, loc.longitude);
                if (nearestCity == null)
                    return View("Error");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ServerError", "Error", new
                {
                    reason = $"Error finding nearest city for {prayerName} prayer times: {ex.Message} | IP: {ip} | Coordinates: {loc?.latitude}, {loc?.longitude} | Stage: Nearest City"
                });
            }

            double lat = (double)nearestCity.Latitude;
            double lon = (double)nearestCity.Longitude;

            // Use IEnumerable instead of dynamic to avoid lambda issues
            IEnumerable<dynamic> hanafi = null;
            IEnumerable<dynamic> shafi = null;
            try
            {
                // Load today's timings
                hanafi = await GetMonthlyPrayerTimings(lat, lon, 2);
                shafi = await GetMonthlyPrayerTimings(lat, lon, 1);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ServerError", "Error", new
                {
                    reason = $"Error loading prayer timings for {prayerName}: {ex.Message} | IP: {ip} | Coordinates: {lat}, {lon} | Stage: Prayer Timings API"
                });
            }

            var today = DateTime.Now.Date;

            // Cast to List or Enumerable to use LINQ properly
            var hanafiList = hanafi as IList<dynamic> ?? hanafi?.ToList();
            var shafiList = shafi as IList<dynamic> ?? shafi?.ToList();

            var todayHanafi = hanafiList?.FirstOrDefault(t =>
            {
                try { return ((DateTime)t.Date).Date == today; }
                catch { return false; }
            });
            var todayShafi = shafiList?.FirstOrDefault(t =>
            {
                try { return ((DateTime)t.Date).Date == today; }
                catch { return false; }
            });

            if (todayHanafi == null || todayShafi == null)
                return View("Error");

            string hanafiTime = "";
            string shafiTime = "";
            try
            {
                // Extract current prayer time
                hanafiTime = ToAmPm(GetPrayerTimeByName(todayHanafi, prayerField));
                shafiTime = ToAmPm(GetPrayerTimeByName(todayShafi, prayerField));
            }
            catch (Exception ex)
            {
                return RedirectToAction("ServerError", "Error", new
                {
                    reason = $"Error extracting prayer times for {prayerName}: {ex.Message} | IP: {ip} | Prayer Field: {prayerField} | Stage: Extract Prayer Times"
                });
            }

            string nextField = "";
            string nextHanafi = "";
            string nextShafi = "";
            try
            {
                // GET NEXT PRAYER (name + time)
                string currentField = prayerField;
                nextField = GetNextPrayerField(allPrayerOrder, currentField);
                nextHanafi = ToAmPm(GetPrayerTimeByName(todayHanafi, nextField));
                nextShafi = ToAmPm(GetPrayerTimeByName(todayShafi, nextField));
            }
            catch (Exception ex)
            {
                return RedirectToAction("ServerError", "Error", new
                {
                    reason = $"Error getting next prayer for {prayerName}: {ex.Message} | IP: {ip} | Current Field: {prayerField} | Stage: Next Prayer"
                });
            }

            Country country = null;
            try
            {
                country = await _context.Countries.FirstOrDefaultAsync(c => c.Id == nearestCity.CountryId);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ServerError", "Error", new
                {
                    reason = $"Error loading country data for {prayerName}: {ex.Message} | IP: {ip} | Country ID: {nearestCity.CountryId} | Stage: Country Data"
                });
            }

            var favouriteCitiesPrayerTimes = new List<CityPrayerTimeViewModel>();
            try
            {
                var favouriteCities = await _context.Cities
                    .Where(c => c.CountryId == country.Id && c.IsFavourite == true)
                    .ToListAsync();

                foreach (var city in favouriteCities)
                {
                    double cLat = (double)city.Latitude;
                    double cLon = (double)city.Longitude;

                    IEnumerable<dynamic> hanafiTimes = null;
                    IEnumerable<dynamic> shafiTimes = null;
                    try
                    {
                        hanafiTimes = await GetMonthlyPrayerTimings(cLat, cLon, 2);
                        shafiTimes = await GetMonthlyPrayerTimings(cLat, cLon, 1);
                    }
                    catch (Exception ex)
                    {
                        // Continue with other cities even if one fails
                        continue;
                    }

                    var hanafiTimesList = hanafiTimes as IList<dynamic> ?? hanafiTimes?.ToList();
                    var shafiTimesList = shafiTimes as IList<dynamic> ?? shafiTimes?.ToList();

                    var todayHanafiCity = hanafiTimesList?.FirstOrDefault(t =>
                    {
                        try { return ((DateTime)t.Date).Date == today; }
                        catch { return false; }
                    });
                    var todayShafiCity = shafiTimesList?.FirstOrDefault(t =>
                    {
                        try { return ((DateTime)t.Date).Date == today; }
                        catch { return false; }
                    });

                    favouriteCitiesPrayerTimes.Add(new CityPrayerTimeViewModel
                    {
                        CityName = city.CityName,
                        HanafiTime = todayHanafiCity != null ? ToAmPm(GetPrayerTimeByName(todayHanafiCity, prayerField)) : "--:--",
                        ShafiTime = todayShafiCity != null ? ToAmPm(GetPrayerTimeByName(todayShafiCity, prayerField)) : "--:--"
                    });
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("ServerError", "Error", new
                {
                    reason = $"Error loading favourite cities for {prayerName}: {ex.Message} | IP: {ip} | Stage: Favourite Cities"
                });
            }

            List<OtherPrayerTimeViewModel> otherPrayersTimes = null;
            try
            {
                otherPrayersTimes = allPrayerOrder
                    .Where(p => !string.Equals(p, prayerField, StringComparison.OrdinalIgnoreCase))
                    .Select(p =>
                    {
                        var hanafiTimeOther = ToAmPm(GetPrayerTimeByName(todayHanafi, p));
                        var shafiTimeOther = ToAmPm(GetPrayerTimeByName(todayShafi, p));

                        return new OtherPrayerTimeViewModel
                        {
                            PrayerName = p,
                            HanafiTime = hanafiTimeOther,
                            ShafiTime = shafiTimeOther,
                            Link = Url.Action("DynamicNamazTiming", new { prayerName = p.ToLower() })
                        };
                    }).ToList();
            }
            catch (Exception ex)
            {
                return RedirectToAction("ServerError", "Error", new
                {
                    reason = $"Error loading other prayer times for {prayerName}: {ex.Message} | IP: {ip} | Stage: Other Prayers"
                });
            }

            try
            {
                var model = new DynamicPrayerTimingViewModel
                {
                    PrayerName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(prayerName),
                    City = nearestCity,
                    Country = country,

                    HanafiTime = hanafiTime,
                    ShafiTime = shafiTime,

                    NextPrayerName = nextField,
                    NextHanafiTime = nextHanafi,
                    NextShafiTime = nextShafi,

                    FavouriteCitiesPrayerTimes = favouriteCitiesPrayerTimes,
                    OtherPrayersTimes = otherPrayersTimes
                };

                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ServerError", "Error", new
                {
                    reason = $"Error creating view model for {prayerName}: {ex.Message} | IP: {ip} | Stage: View Model Creation"
                });
            }
        }





        [Route("{cityOrCountry:regex(^(?!fajr$)(?!dhuhr$)(?!zuhr$)(?!asr$)(?!maghrib$)(?!isha$)(?!sunrise$)[[\\p{{L}}0-9\\s\\-\\.']]+$)}-namaz-timing")]
        public async Task<IActionResult> NamazTiming(string cityOrCountry)
        {
            ViewBag.Banners = await _bannerService.GetBannersAsync();

            var slug = NormalizeSlug(cityOrCountry);

            var country = (await _context.Countries
                    .AsNoTracking()
                    .ToListAsync())
                .FirstOrDefault(c => NormalizeSlug(c.Name) == slug);

            var city = (await _context.Cities
                    .AsNoTracking()
                    .ToListAsync())
                .FirstOrDefault(c => NormalizeSlug(c.CityName) == slug);


            var cmsData = await _cmsRepo.GetByUrlAsync(
                $"https://www.ilmkidunya.com/islam/{cityOrCountry}-namaz-timing"
            );

            // ===========================
            // COUNTRY CASE
            // ===========================
            if (country != null)
            {
                if (cmsData == null)
                {
                    cmsData = new TblCmsDto
                    {
                        MetaTitle = $"Islamic Namaz Timings in {NormalizeDisplay(country.Name)}",
                        MetaDesc = $"Accurate Timings in {NormalizeDisplay(country.Name)}.",
                        Heading = $"{NormalizeDisplay(country.Name)} Prayer Times"
                    };
                }

                ViewBag.CmsData = cmsData;

                var dbCities = await _context.Cities
                    .AsNoTracking()
                    .Where(c => c.CountryId == country.Id)
                    .ToListAsync();

                // Normalize city names for display
                dbCities.ForEach(c => c.CityName = NormalizeDisplay(c.CityName));

                var cityDict = dbCities
                    .Where(c => c.Latitude.HasValue && c.Longitude.HasValue)
                    .GroupBy(c => c.CityName)
                    .Select(g => g.First())
                    .ToDictionary(
                        c => c.CityName,
                        c => ((double)c.Latitude.Value, (double)c.Longitude.Value, NormalizeDisplay(country.Name))
                    );

                var model = new PrayerTimesHomeViewModel
                {
                    CityPrayerTimes = await GetPrayerTimingsAsync(cityDict),
                    AllCities = dbCities,
                    Country = country
                };

                return View("CountryNamaz", model);
            }

            // ===========================
            // CITY CASE
            // ===========================
            if (city != null)
            {
                // Normalize city display name
                city.CityName = NormalizeDisplay(city.CityName);

                if (cmsData == null)
                {
                    cmsData = new TblCmsDto
                    {
                        MetaTitle = $"{city.CityName} Prayer Times",
                        MetaDesc = $"Get {city.CityName} Namaz timings.",
                        Heading = $"{city.CityName} Prayer Times"
                    };
                }

                ViewBag.CmsData = cmsData;

                double lat = (double)(city.Latitude ?? 0);
                double lon = (double)(city.Longitude ?? 0);

                var hanafi = await GetMonthlyPrayerTimings(lat, lon, 2);
                var shafi = await GetMonthlyPrayerTimings(lat, lon, 1);

                var today = DateTime.UtcNow.Date;

                var todayHanafi = hanafi.FirstOrDefault(t => t.Date.Date == today)
                    ?? hanafi.FirstOrDefault(t => t.Date.Date == DateTime.Now.Date);

                var todayShafi = shafi.FirstOrDefault(t => t.Date.Date == today)
                    ?? shafi.FirstOrDefault(t => t.Date.Date == DateTime.Now.Date);

                var allCities = await _context.Cities
                    .AsNoTracking()
                    .Where(c => c.CountryId == city.CountryId)
                    .ToListAsync();

                // Normalize all city names
                allCities.ForEach(c => c.CityName = NormalizeDisplay(c.CityName));

                var model = new CityPrayerTimesViewModel
                {
                    City = city,
                    Country = await _context.Countries.AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Id == city.CountryId),

                    AllCities = allCities,

                    TodayHanafi = todayHanafi,
                    TodayShafi = todayShafi,
                    HanafiWeekly = hanafi.Take(7).ToList(),
                    ShafiWeekly = shafi.Take(7).ToList(),
                    HanafiMonthly = hanafi,
                    ShafiMonthly = shafi
                };

                return View("CityNamaz", model);
            }

            return NotFound();
        }


        private static string NormalizeDisplay(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;


            //input = Uri.UnescapeDataString(input);
            input = input.Replace("-", " ");
            return input;

        }


        private static string NormalizeSlug(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            //input = Uri.UnescapeDataString(input);
            input = input.ToLower().Replace("-", " ");
            return input;

        }

        private async Task<List<PrayerTimingDto>> GetMonthlyPrayerTimings(double lat, double lon, int school)
        {
            using var httpClient = new HttpClient();
            var url = $"https://api.aladhan.com/v1/calendar?latitude={lat}&longitude={lon}&method=2&school={school}&days=30";

            var response = await httpClient.GetStringAsync(url);
            var result = JsonConvert.DeserializeObject<AladhanResponse>(response);

            return result?.Data?.Select(d => new PrayerTimingDto
            {
                Date = DateTime.ParseExact(d.Date.Gregorian.Date, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture),
                DateReadable = d.Date.Gregorian.Date,
                Fajr = FormatApiTime(d.Timings.Fajr),
                Sunrise = FormatApiTime(d.Timings.Sunrise),
                Dhuhr = FormatApiTime(d.Timings.Dhuhr),
                Asr = FormatApiTime(d.Timings.Asr),
                Maghrib = FormatApiTime(d.Timings.Maghrib),
                Isha = FormatApiTime(d.Timings.Isha)
            }).ToList() ?? new List<PrayerTimingDto>();
        }


        private async Task<List<PrayerTimingDto>> GetPrayerTimingsAsync(double lat, double lon, int school)
        {
            try
            {
                using var httpClient = new HttpClient();

                // 🕌 Using Aladhan monthly calendar API
                var url = $"https://api.aladhan.com/v1/calendar?latitude={lat}&longitude={lon}&method=2&school={school}&days=30";

                var response = await httpClient.GetStringAsync(url);
                var result = JsonConvert.DeserializeObject<AladhanResponse>(response);

                if (result?.Data == null)
                {
                    Console.WriteLine("⚠️ No data returned from Aladhan API.");
                    return new List<PrayerTimingDto>();
                }

                // ✅ Convert API response to your PrayerTimingDto list
                return result?.Data?.Select(d => new PrayerTimingDto
                {
                    Date = DateTime.ParseExact(d.Date.Gregorian.Date, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture),
                    DateReadable = d.Date.Gregorian.Date,
                    Fajr = FormatApiTime(d.Timings.Fajr),
                    Sunrise = FormatApiTime(d.Timings.Sunrise),
                    Dhuhr = FormatApiTime(d.Timings.Dhuhr),
                    Asr = FormatApiTime(d.Timings.Asr),
                    Maghrib = FormatApiTime(d.Timings.Maghrib),
                    Isha = FormatApiTime(d.Timings.Isha)
                }).ToList() ?? new List<PrayerTimingDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error fetching prayer timings: {ex.Message}");
                return new List<PrayerTimingDto>();
            }
        }

        private async Task<List<CityPrayerTimeViewModel>> GetPrayerTimingsAsync(Dictionary<string, (double lat, double lon, string country)> cities)
        {
            var results = new List<CityPrayerTimeViewModel>();

            foreach (var city in cities)
            {
                try
                {
                    // Use your new monthly method here
                    var monthlyTimings = await GetPrayerTimingsAsync(city.Value.lat, city.Value.lon, 1);

                    // Just take today's timings
                    var today = DateTime.Now.Date;
                    var todayTiming = monthlyTimings.FirstOrDefault(t => t.Date.Date == today);

                    if (todayTiming != null)
                    {
                        results.Add(new CityPrayerTimeViewModel
                        {
                            CityName = city.Key,
                            CountryName = city.Value.country,
                            Fajr = todayTiming.Fajr,
                            Dhuhr = todayTiming.Dhuhr,
                            Asr = todayTiming.Asr,
                            Maghrib = todayTiming.Maghrib,
                            Isha = todayTiming.Isha
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error fetching timings for {city.Key}: {ex.Message}");
                }
            }

            return results;
        }


        private string FormatApiTime(string apiTime)
        {
            if (string.IsNullOrEmpty(apiTime))
                return string.Empty;

            var timePart = apiTime.Split(' ')[0];

            if (TimeSpan.TryParse(timePart, out var time))
            {
                var dateTime = DateTime.Today.Add(time);
                return dateTime.ToString("h:mm tt");
            }

            return apiTime;
        }






        private string GetUserIp()
        {
            try
            {
                string ip = null;

                // 1️⃣ Try RemoteIpAddress (base source)
                ip = HttpContext.Connection.RemoteIpAddress?.ToString();

                // 2️⃣ Try load balancer / proxy headers
                if (HttpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
                {
                    // Use the first IP in the forwarded-for list
                    ip = forwardedFor.ToString().Split(',').FirstOrDefault()?.Trim();
                }
                else if (HttpContext.Request.Headers.TryGetValue("X-Real-IP", out var realIp))
                {
                    ip = realIp.ToString().Trim();
                }

                // 3️⃣ Handle localhost or null IPs
                if (string.IsNullOrWhiteSpace(ip) || ip == "::1" || ip == "127.0.0.1")
                {
                    ip = "103.255.4.18";// fallback safe public IP
                }

                return ip;
            }
            catch (Exception ex)
            {
                // Optional: log error (Uncomment if using ILogger)
                // _logger.LogError(ex, "Failed to detect user IP address");

                // Fallback to safe IP
                return "103.255.4.18";
            }
        }

        private async Task<IpLocationDto> GetLocationFromIp(string ip)
        {
            using var http = new HttpClient();
            var json = await http.GetStringAsync($"https://ipwho.is/{ip}");
            return JsonConvert.DeserializeObject<IpLocationDto>(json);
        }

        public class IpLocationDto
        {
            public bool success { get; set; }
            public string city { get; set; }
            public string country { get; set; }
            public double latitude { get; set; }
            public double longitude { get; set; }
        }

        private async Task<City> FindNearestCityAsync(double lat, double lon)
        {
            // Load all cities with coordinates
            var cities = await _context.Cities
                .Where(c => c.Latitude.HasValue && c.Longitude.HasValue)
                .ToListAsync();

            City nearest = null;
            double minDistance = double.MaxValue;

            foreach (var c in cities)
            {
                double d = Haversine(lat, lon, (double)c.Latitude, (double)c.Longitude);
                if (d < minDistance)
                {
                    minDistance = d;
                    nearest = c;
                }
            }

            return nearest;
        }

        private double Haversine(double lat1, double lon1, double lat2, double lon2)
        {
            double R = 6371; // km
            double dLat = (lat2 - lat1) * Math.PI / 180;
            double dLon = (lon2 - lon1) * Math.PI / 180;

            lat1 *= Math.PI / 180;
            lat2 *= Math.PI / 180;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c;
        }

        private string GetPrayerTimeByName(PrayerTimingDto dto, string prayerField)
        {
            if (dto == null)
                return null;

            return prayerField switch
            {
                "Fajr" => dto.Fajr,
                "Dhuhr" => dto.Dhuhr,
                "Asr" => dto.Asr,
                "Maghrib" => dto.Maghrib,
                "Isha" => dto.Isha,
                "Sunrise" => dto.Sunrise,
                _ => null
            };
        }
        private string GetNextPrayerField(List<string> order, string current)
        {
            int index = order.IndexOf(current);

            if (index == -1)
                return "Dhuhr"; // fallback

            // Loop back to start (Isha → Fajr)
            int nextIndex = (index + 1) % order.Count;

            return order[nextIndex];
        }

        private string ToAmPm(string time)
        {
            if (string.IsNullOrWhiteSpace(time))
                return "";

            if (DateTime.TryParse(time, out DateTime t))
                return t.ToString("hh:mm tt"); // 12-hour format

            return time; // fallback
        }


        private List<Country> GetAllCountries()
        {
            var countries = _context.Countries.ToList();
            return countries;
        }

        [Route("getAllCities/{id}")]
        public async Task<List<City>> GetAllCities(int id)
        {
            var cities = await _context.Cities.Where(c => c.CountryId == id).ToListAsync();
            return cities;
        }

        #endregion Namaz Timings


        #region Enhanced Ramadan Methods

        private readonly object _prayerTimingLock = new object();
        private readonly SemaphoreSlim _apiSemaphore = new SemaphoreSlim(2, 2); // Limit to 2 concurrent API calls

        [Route("ramadan-calendar")]
        public async Task<IActionResult> RamadanCalendar()
        {
            try
            {
                // ✅ Load banners and CMS metadata
                ViewBag.Banners = await _bannerService.GetBannersAsync();
                var cmsData = await _cmsRepo.GetByUrlAsync("https://www.ilmkidunya.com/islam/ramadan-calendar");

                if (cmsData == null)
                {
                    cmsData = new TblCmsDto
                    {
                        MetaTitle = "Ramadan Calendar 2026 - Sehri & Iftar Timings Worldwide",
                        MetaDesc = "Get accurate Ramadan Sehri and Iftar timings for Hanafi and Jafaria fiqh. Worldwide Ramadan calendar with Islamic dates and prayer times.",
                        MetaKeys = "ramadan calendar, sehri time, iftar time, ramadan 2026, islamic calendar, hanafi, jafaria",
                        Heading = "Ramadan Calendar 2026 – Sehri & Iftar Timings",
                        Desc1 = "", // Dynamic content
                        Desc2 = ""
                    };
                }
                ViewBag.CmsData = cmsData;

                // Get user location
                var ip = GetUserIp();
                if (ip == "::1" || ip == "127.0.0.1")
                    ip = "103.255.4.18"; // Lahore IP for local testing

                var loc = await GetLocationFromIp(ip);
                if (loc == null || !loc.success)
                    return View("Error");

                // Find nearest city
                var nearestCity = await FindNearestCityAsync(loc.latitude, loc.longitude);

                if (nearestCity == null)
                    return View("Error");

                double lat = (double)nearestCity.Latitude;
                double lon = (double)nearestCity.Longitude;

                var country = await _context.Countries.FirstOrDefaultAsync(c => c.Id == nearestCity.CountryId);
                string location = $"{country?.Name}, {nearestCity.CityName}";

                // Get today's Ramadan timing for both schools - process sequentially
                var todayHanafi = await GetTodayRamadanTimingSafe(lat, lon, 2); // Hanafi
                var todayJafaria = await GetTodayRamadanTimingSafe(lat, lon, 3); // Jafaria (new fiqh)

                // Apply Jafaria adjustments (10 minutes earlier for Sehri)
                if (todayJafaria != null)
                {
                    todayJafaria.Sehri = AdjustTimeForJafaria(todayJafaria.Sehri, -10); // 10 minutes earlier
                    todayJafaria.Iftar = AdjustTimeForJafaria(todayJafaria.Iftar, +10); // Iftar remains the same for Jafaria
                }

                // Get tomorrow's timings for both fiqas
                var tomorrowHanafi = await GetTomorrowRamadanTimingSafe(lat, lon, 2);
                var tomorrowJafaria = await GetTomorrowRamadanTimingSafe(lat, lon, 3);

                if (tomorrowJafaria != null)
                {
                    tomorrowJafaria.Sehri = AdjustTimeForJafaria(tomorrowJafaria.Sehri, -10); // 10 minutes earlier
                    tomorrowJafaria.Iftar = AdjustTimeForJafaria(tomorrowJafaria.Iftar, +10);
                }

                // Determine next event and time left for both schools
                var (nextEventHanafi, timeLeftHanafi, nextEventTimeHanafi) = CalculateNextRamadanEvent(todayHanafi);
                var (nextEventJafaria, timeLeftJafaria, nextEventTimeJafaria) = CalculateNextRamadanEvent(todayJafaria);

                // Get popular cities timings with proper error handling
                var popularCitiesTimings = await GetCountryCitiesRamadanTimingsSafe(country.Id);

                // Get all countries for listing
                var countries = await _context.Countries.ToListAsync();

                var model = new RamadanHomeViewModel
                {
                    TodayHanafi = todayHanafi,
                    TodayJafaria = todayJafaria,
                    TomorrowHanafi = tomorrowHanafi,
                    TomorrowJafaria = tomorrowJafaria,
                    PopularCitiesTimings = popularCitiesTimings,
                    Countries = countries,
                    NextEventHanafi = nextEventHanafi,
                    TimeLeftHanafi = timeLeftHanafi,
                    NextEventTimeHanafi = nextEventTimeHanafi,
                    NextEventJafaria = nextEventJafaria,
                    TimeLeftJafaria = timeLeftJafaria,
                    NextEventTimeJafaria = nextEventTimeJafaria,
                    Location = location,
                    City = nearestCity
                };

                return View(model);
            }
            catch (Exception ex)
            {
                // Log the actual error
                _logger.LogError(ex, "Error loading Ramadan calendar");
                return RedirectToAction("ServerError", "Error", new { reason = $"Error loading Ramadan calendar: {ex.Message}" });
            }
        }

        private string AdjustTimeForJafaria(string time, int minutesAdjustment)
        {
            if (DateTime.TryParse(time, out DateTime dateTime))
            {
                return dateTime.AddMinutes(minutesAdjustment).ToString("hh:mm tt");
            }
            return time;
        }

        // Safe version with semaphore
        private async Task<RamadanTimingDto> GetTodayRamadanTimingSafe(double lat, double lon, int school)
        {
            await _apiSemaphore.WaitAsync();
            try
            {
                return await GetTodayRamadanTiming(lat, lon, school);
            }
            finally
            {
                _apiSemaphore.Release();
            }
        }

        // Safe version with semaphore
        private async Task<RamadanTimingDto> GetTomorrowRamadanTimingSafe(double lat, double lon, int school)
        {
            await _apiSemaphore.WaitAsync();
            try
            {
                return await GetTomorrowRamadanTiming(lat, lon, school);
            }
            finally
            {
                _apiSemaphore.Release();
            }
        }

        private async Task<RamadanTimingDto> GetTodayRamadanTiming(double lat, double lon, int school)
        {
            try
            {
                var prayerTimings = await GetMonthlyPrayerTimings(lat, lon, school);
                var today = DateTime.Now.Date;

                var todayTiming = prayerTimings.FirstOrDefault(t => t.Date.Date == today);

                if (todayTiming != null)
                {
                    var timing = new RamadanTimingDto
                    {
                        Date = todayTiming.Date,
                        DateReadable = todayTiming.DateReadable,
                        Sehri = todayTiming.Fajr, // Sehri ends at Fajr
                        Iftar = todayTiming.Maghrib, // Iftar starts at Maghrib
                        Fajr = todayTiming.Fajr,
                        Maghrib = todayTiming.Maghrib,
                        IslamicDateEnglish = GetIslamicDateEnglish(today)
                    };

                    return timing;
                }

                return new RamadanTimingDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting Ramadan timing for lat: {lat}, lon: {lon}, school: {school}");
                return new RamadanTimingDto();
            }
        }

        //[Route("{cityOrCountry:regex(^(?!fajr$)(?!dhuhr$)(?!zuhr$)(?!asr$)(?!maghrib$)(?!isha$)(?!sunrise$)[[A-Za-z\\s-]]+$)}-ramadan-calendar")]
        [Route("{cityOrCountry:regex(^(?!fajr$)(?!dhuhr$)(?!zuhr$)(?!asr$)(?!maghrib$)(?!isha$)(?!sunrise$)[[\\p{{L}}0-9\\s\\-\\.']]+$)}-ramadan-calendar")]
        public async Task<IActionResult> CityOrCountryRamadanTiming(string cityOrCountry)
        {
            try
            {
                ViewBag.Banners = await _bannerService.GetBannersAsync();

                var slug = cityOrCountry.ToLower().Replace("-", " ").Trim();

                // Check if it's a country
                var country = await _context.Countries
                    .FirstOrDefaultAsync(c => c.Name.ToLower() == slug);

                // COUNTRY CASE
                if (country != null)
                {
                    // Get CMS data
                    var cmsData = await _cmsRepo.GetByUrlAsync(
                        $"https://www.ilmkidunya.com/islam/{cityOrCountry}-ramadan-calendar"
                    );

                    if (cmsData == null)
                    {
                        cmsData = new TblCmsDto
                        {
                            MetaTitle = $"{country.Name} Ramadan Timings 2026 - Sehri & Iftar Times",
                            MetaDesc = $"Accurate Ramadan Sehri and Iftar timings for all cities in {country.Name}. Complete Ramadan calendar 2026.",
                            MetaKeys = $"{country.Name} ramadan, {country.Name} sehri iftar time, {country.Name} islamic calendar",
                            Heading = $"{country.Name} Ramadan Calendar 2026",
                            Desc1 = "",
                            Desc2 = ""
                        };
                    }
                    ViewBag.CmsData = cmsData;

                    // 1. Get all favourite cities today's sehr and iftar timing for both fiqas
                    var favouriteCitiesTimings = await GetFavouriteCitiesRamadanTimingsSafe(country.Id);

                    // 2. Get all countries
                    var allCountries = await _context.Countries.ToListAsync();

                    var model = new RamadanHomeViewModel
                    {
                        PopularCitiesTimings = favouriteCitiesTimings,
                        Countries = allCountries,
                        Country = country,
                        AllCities = await _context.Cities.Where(c => c.CountryId == country.Id).ToListAsync()
                    };

                    return View("CountryRamadan", model);
                }

                // Check if it's a city
                var city = await _context.Cities
                    .FirstOrDefaultAsync(c => c.CityName.ToLower() == slug);

                // CITY CASE
                if (city != null)
                {
                    // Get CMS data
                    var cmsData = await _cmsRepo.GetByUrlAsync(
                        $"https://www.ilmkidunya.com/islam/{cityOrCountry}-ramadan-calendar"
                    );

                    if (cmsData == null)
                    {
                        cmsData = new TblCmsDto
                        {
                            MetaTitle = $"{city.CityName} Ramadan Timings 2026 - Sehri & Iftar Times",
                            MetaDesc = $"Accurate Ramadan Sehri and Iftar timings for {city.CityName}. Complete Ramadan calendar 2026 with Hanafi and Jafaria fiqh.",
                            MetaKeys = $"{city.CityName} ramadan, {city.CityName} sehri iftar time, islamic calendar",
                            Heading = $"{city.CityName} Ramadan Calendar 2026",
                            Desc1 = "",
                            Desc2 = ""
                        };
                    }
                    ViewBag.CmsData = cmsData;

                    double lat = (double)(city.Latitude ?? 0);
                    double lon = (double)(city.Longitude ?? 0);

                    // 1. Ramadan timing for both fiqas as in RamadanCalendar() action method
                    var todayHanafi = await GetTodayRamadanTimingSafe(lat, lon, 2); // Hanafi
                    var todayJafaria = await GetTodayRamadanTimingSafe(lat, lon, 3); // Jafaria

                    // Apply Jafaria adjustments (10 minutes earlier for Sehri)
                    if (todayJafaria != null)
                    {
                        todayJafaria.Sehri = AdjustTimeForJafaria(todayJafaria.Sehri, -10);
                        todayJafaria.Iftar = AdjustTimeForJafaria(todayJafaria.Iftar, +10);
                    }

                    // Get tomorrow's timings for both fiqas
                    var tomorrowHanafi = await GetTomorrowRamadanTimingSafe(lat, lon, 2);
                    var tomorrowJafaria = await GetTomorrowRamadanTimingSafe(lat, lon, 3);

                    if (tomorrowJafaria != null)
                    {
                        tomorrowJafaria.Sehri = AdjustTimeForJafaria(tomorrowJafaria.Sehri, -10);
                        tomorrowJafaria.Iftar = AdjustTimeForJafaria(tomorrowJafaria.Iftar, +10);
                    }

                    // Determine next event and time left for both schools
                    var (nextEventHanafi, timeLeftHanafi, nextEventTimeHanafi) = CalculateNextRamadanEvent(todayHanafi);
                    var (nextEventJafaria, timeLeftJafaria, nextEventTimeJafaria) = CalculateNextRamadanEvent(todayJafaria);

                    // 2. Sehr and iftar timing for the current ISLAMIC month
                    var islamicDates = GetAllDatesOfCurrentIslamicMonthFormatted();

                    // Get prayer timings in BATCH using Calendar API
                    List<PrayerTimingDto> prayerTimings;

                    if (islamicDates.All(d => d.GregorianDate.Month == DateTime.UtcNow.Month))
                    {
                        // All dates are in current month - use existing method
                        var currentMonthTimings = await GetMonthlyPrayerTimingsSafe(lat, lon, 2);
                        prayerTimings = currentMonthTimings
                            .Where(t => islamicDates.Any(id => id.GregorianDate.Date == t.Date.Date))
                            .ToList();
                    }
                    else
                    {
                        // Dates span multiple months - use Aladhan calendar API (1-2 calls)
                        prayerTimings = await GetPrayerTimingsForIslamicMonthAladhan(lat, lon, islamicDates);
                    }

                    // If we still don't have all timings, fallback to individual calls with delays
                    if (prayerTimings.Count < islamicDates.Count)
                    {
                        var missingDates = islamicDates
                            .Where(id => !prayerTimings.Any(t => t.Date.Date == id.GregorianDate.Date))
                            .ToList();

                        foreach (var islamicDate in missingDates)
                        {
                            try
                            {
                                // Add delay to prevent rate limiting
                                await Task.Delay(2000); // 2 second delay

                                var timing = await GetPrayerTimingForDateAladhanNewtonsoft(lat, lon, islamicDate.GregorianDate);
                                if (timing != null)
                                {
                                    prayerTimings.Add(timing);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, $"Error getting prayer timing for {islamicDate.GregorianDate:yyyy-MM-dd}");
                            }
                        }
                    }

                    // Now create hanafiRamadanMonthly from prayerTimings
                    var hanafiRamadanMonthly = new List<RamadanTimingDto>();

                    foreach (var islamicDate in islamicDates)
                    {
                        var prayerTiming = prayerTimings.FirstOrDefault(t => t.Date.Date == islamicDate.GregorianDate.Date);

                        hanafiRamadanMonthly.Add(new RamadanTimingDto
                        {
                            Date = islamicDate.GregorianDate,
                            DateReadable = islamicDate.GregorianDate.ToString("dd MMMM yyyy"),
                            Sehri = prayerTiming?.Fajr ?? "N/A",
                            SehriJafaria = prayerTiming != null ? AdjustTimeForJafaria(prayerTiming.Fajr, -10) : "N/A",
                            Iftar = prayerTiming?.Maghrib ?? "N/A",
                            IftarJafaria = prayerTiming != null ? AdjustTimeForJafaria(prayerTiming.Maghrib, +10) : "N/A",
                            Fajr = prayerTiming?.Fajr ?? "N/A",
                            Maghrib = prayerTiming?.Maghrib ?? "N/A",
                            IslamicDate = islamicDate.IslamicDateFormatted,
                            IslamicDateEnglish = islamicDate.EnglishDateFormatted,
                            GregorianDateString = islamicDate.GregorianDate.ToString("MMMM dd, yyyy")
                        });
                    }

                    // 3. All countries
                    var allCountries = await _context.Countries.ToListAsync();

                    var countryObj = await _context.Countries.FirstOrDefaultAsync(c => c.Id == city.CountryId);
                    var allCitiesInCountry = await _context.Cities.Where(c => c.CountryId == city.CountryId).ToListAsync();

                    var model = new CityRamadanViewModel
                    {
                        City = city,
                        Country = countryObj,
                        AllCities = allCitiesInCountry,

                        TodayHanafi = todayHanafi,
                        TodayJafaria = todayJafaria,
                        TomorrowHanafi = tomorrowHanafi,
                        TomorrowJafaria = tomorrowJafaria,

                        NextEventHanafi = nextEventHanafi,
                        TimeLeftHanafi = timeLeftHanafi,
                        NextEventTimeHanafi = nextEventTimeHanafi,
                        NextEventJafaria = nextEventJafaria,
                        TimeLeftJafaria = timeLeftJafaria,
                        NextEventTimeJafaria = nextEventTimeJafaria,

                        HanafiRamadanMonthly = hanafiRamadanMonthly,

                        Countries = allCountries,
                        Location = $"{countryObj?.Name}, {city.CityName}"
                    };

                    return View("CityRamadan", model);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading Ramadan timing for {cityOrCountry}");
                return RedirectToAction("ServerError", "Error", new { reason = $"Error loading Ramadan timing for {cityOrCountry}: {ex.Message}" });
            }
        }
        private async Task<PrayerTimingDto> GetPrayerTimingForAnyDate(double lat, double lon, int fiqa, DateTime date)
        {
            // Method 1: Check if it's in current month
            var currentMonth = DateTime.UtcNow.Month;
            if (date.Month == currentMonth)
            {
                var currentTimings = await GetMonthlyPrayerTimingsSafe(lat, lon, fiqa);
                return currentTimings.FirstOrDefault(t => t.Date.Date == date.Date);
            }

            // Method 2: Use Aladhan API for other months WITH DELAY
            await Task.Delay(1000); // Add 1 second delay to prevent rate limiting

            return await GetPrayerTimingForDateAladhanNewtonsoft(lat, lon, date);
        }


        private async Task<PrayerTimingDto> GetPrayerTimingForDateAladhanNewtonsoft(double lat, double lon, DateTime date)
        {
            // Add delay to prevent rate limiting
            await Task.Delay(1000);

            try
            {
                var dateStr = date.ToString("dd-MM-yyyy");
                var url = $"http://api.aladhan.com/v1/timings/{dateStr}?latitude={lat}&longitude={lon}&method=2&school=1";

                using var httpClient = new HttpClient();

                // Add headers to be polite
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("IlmKiDunya/1.0");
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<AladhanResponseNewtonsoft>(json);

                    if (result.Code == 200)
                    {
                        return new PrayerTimingDto
                        {
                            Date = date,
                            DateReadable = date.ToString("dd MMMM yyyy"),
                            Fajr = FormatTimeWithAmPm(result.Data.Timings.Fajr),
                            Maghrib = FormatTimeWithAmPm(result.Data.Timings.Maghrib)
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting prayer timing from Aladhan API for {date:yyyy-MM-dd}");
            }

            return null;
        }
        private async Task<List<PrayerTimingDto>> GetPrayerTimingsForIslamicMonthAladhan(double lat, double lon, List<IslamicDateString> islamicDates)
        {
            if (!islamicDates.Any())
                return new List<PrayerTimingDto>();

            // Get the start and end dates of Islamic month
            var startDate = islamicDates.Min(d => d.GregorianDate);
            var endDate = islamicDates.Max(d => d.GregorianDate);

            var timings = new List<PrayerTimingDto>();

            try
            {
                // Get prayer timings for each month needed
                var monthsNeeded = new List<(int year, int month)>();

                // Add all months that contain our Islamic dates
                var currentDate = startDate;
                while (currentDate <= endDate)
                {
                    var monthYear = (currentDate.Year, currentDate.Month);
                    if (!monthsNeeded.Contains(monthYear))
                    {
                        monthsNeeded.Add(monthYear);
                    }
                    currentDate = currentDate.AddMonths(1);
                }

                foreach (var (year, month) in monthsNeeded)
                {
                    try
                    {
                        // Get entire month data in ONE API call
                        var url = $"http://api.aladhan.com/v1/calendar/{year}/{month}?latitude={lat}&longitude={lon}&method=2&school=1";

                        using var httpClient = new HttpClient();

                        // Add headers to be polite
                        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("IlmKiDunya/1.0");
                        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                        var response = await httpClient.GetAsync(url);

                        if (response.IsSuccessStatusCode)
                        {
                            var json = await response.Content.ReadAsStringAsync();
                            var result = JsonConvert.DeserializeObject<AladhanCalendarResponse>(json);

                            if (result?.Code == 200 && result.Data != null)
                            {
                                foreach (var dayData in result.Data)
                                {
                                    try
                                    {
                                        var date = DateTime.ParseExact(dayData.Date.Gregorian.Date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                                        // Only add if within our Islamic month range
                                        if (date >= startDate && date <= endDate)
                                        {
                                            timings.Add(new PrayerTimingDto
                                            {
                                                Date = date,
                                                DateReadable = date.ToString("dd MMMM yyyy"),
                                                Fajr = FormatTimeWithAmPm(dayData.Timings.Fajr),
                                                Maghrib = FormatTimeWithAmPm(dayData.Timings.Maghrib)
                                            });
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError(ex, $"Error parsing date: {dayData.Date.Gregorian?.Date}");
                                    }
                                }
                            }
                        }

                        // Small delay between month requests
                        await Task.Delay(500);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error getting prayer timings calendar from Aladhan API for {year}-{month}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetPrayerTimingsForIslamicMonthAladhan");
            }

            return timings;
        }
        // Response classes for Newtonsoft.Json
        public class AladhanResponseNewtonsoft
        {
            [JsonProperty("code")]
            public int Code { get; set; }

            [JsonProperty("data")]
            public AladhanData Data { get; set; }
        }

        public class AladhanData
        {
            [JsonProperty("timings")]
            public AladhanTimings Timings { get; set; }
        }

        public class AladhanTimings
        {
            [JsonProperty("Fajr")]
            public string Fajr { get; set; }

            [JsonProperty("Maghrib")]
            public string Maghrib { get; set; }
        }
        // Add these classes to your controller
        public class AladhanCalendarResponse
        {
            [JsonProperty("code")]
            public int Code { get; set; }

            [JsonProperty("data")]
            public List<AladhanDayData> Data { get; set; }
        }

        public class AladhanDayData
        {
            [JsonProperty("date")]
            public AladhanDateInfo Date { get; set; }

            [JsonProperty("timings")]
            public AladhanTimings Timings { get; set; }
        }

        public class AladhanDateInfo
        {
            [JsonProperty("gregorian")]
            public AladhanGregorianDate Gregorian { get; set; }
        }

        public class AladhanGregorianDate
        {
            [JsonProperty("date")]
            public string Date { get; set; }
        }
        private string FormatTimeWithAmPm(string time)
        {
            if (string.IsNullOrEmpty(time) || time == "N/A")
                return time;

            // Remove any extra text/annotations (some APIs add "(PK)" etc.)
            var cleanTime = time.Split(' ')[0];

            // Check if it's already in 12-hour format with AM/PM
            if (cleanTime.Contains("AM", StringComparison.OrdinalIgnoreCase) ||
                cleanTime.Contains("PM", StringComparison.OrdinalIgnoreCase))
            {
                return cleanTime;
            }

            // Parse 24-hour format and convert to 12-hour with AM/PM
            if (DateTime.TryParseExact(cleanTime, "HH:mm", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var parsedTime))
            {
                return parsedTime.ToString("h:mm tt");
            }

            // If parsing fails, try other formats
            if (DateTime.TryParse(cleanTime, out parsedTime))
            {
                return parsedTime.ToString("h:mm tt");
            }

            return time; // Return original if all parsing fails
        }
        private string GetFallbackIslamicDate(DateTime date)
        {
            var hijri = new System.Globalization.HijriCalendar();
            int day = hijri.GetDayOfMonth(date);
            int month = hijri.GetMonth(date);
            int year = hijri.GetYear(date);

            string[] islamicMonths = {
        "Muharram", "Safar", "Rabi al-Awwal", "Rabi al-Thani",
        "Jamadi al-Awwal", "Jamadi al-Akhirah", "Rajab", "Shaban",
        "Ramadan", "Shawwal", "Dhul Qadah", "Dhul Hijjah"
    };

            return $"{GetDayWithSuffix(day)} {islamicMonths[month - 1]} {year}H";
        }

        private string GetFallbackIslamicDateEnglish(DateTime date)
        {
            var hijri = new System.Globalization.HijriCalendar();
            int day = hijri.GetDayOfMonth(date);
            int month = hijri.GetMonth(date);
            int year = hijri.GetYear(date);

            string[] islamicMonthsEnglish = {
        "Muharram", "Safar", "Rabi' al-Awwal", "Rabi' al-Thani",
        "Jumada al-Ula", "Jumada al-Akhirah", "Rajab", "Sha'ban",
        "Ramadan", "Shawwal", "Dhu al-Qadah", "Dhu al-Hijjah"
    };

            return $"{GetDayWithSuffix(day)} {islamicMonthsEnglish[month - 1]} {year}H";
        }
        private List<IslamicDateString> GetAllDatesOfCurrentIslamicMonthFormatted()
        {
            var dates = new List<IslamicDateString>();
            var hijri = new System.Globalization.HijriCalendar();
            var today = DateTime.UtcNow;

            // Get current Islamic year and month
            int islamicYear = hijri.GetYear(today);
            int islamicMonth = hijri.GetMonth(today);

            // Islamic month names
            string[] islamicMonths = {
        "Muharram", "Safar", "Rabi al-Awwal", "Rabi al-Thani",
        "Jamadi al-Awwal", "Jamadi al-Akhirah", "Rajab", "Shaban",
        "Ramadan", "Shawwal", "Dhul Qadah", "Dhul Hijjah"
    };

            // English month names
            string[] islamicMonthsEnglish = {
        "Muharram", "Safar", "Rabi' al-Awwal", "Rabi' al-Thani",
        "Jumada al-Ula", "Jumada al-Akhirah", "Rajab", "Sha'ban",
        "Ramadan", "Shawwal", "Dhu al-Qadah", "Dhu al-Hijjah"
    };

            string monthName = islamicMonths[islamicMonth - 1];
            string monthNameEnglish = islamicMonthsEnglish[islamicMonth - 1];

            // Get number of days in current Islamic month
            int daysInMonth = hijri.GetDaysInMonth(islamicYear, islamicMonth);

            // Add all dates from 1st to last day of Islamic month
            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime gregorianDate = hijri.ToDateTime(islamicYear, islamicMonth, day, 0, 0, 0, 0);

                // Add ordinal suffix (st, nd, rd, th)
                string dayWithSuffix = GetDayWithSuffix(day);

                dates.Add(new IslamicDateString
                {
                    IslamicDateFormatted = $"{dayWithSuffix} {monthName} {islamicYear}H",
                    EnglishDateFormatted = $"{dayWithSuffix} {monthNameEnglish} {islamicYear}H",
                    GregorianDate = gregorianDate
                });
            }

            return dates;
        }

        // Helper method to add ordinal suffix to day
        private string GetDayWithSuffix(int day)
        {
            if (day >= 11 && day <= 13) return day + "th";

            return (day % 10) switch
            {
                1 => day + "st",
                2 => day + "nd",
                3 => day + "rd",
                _ => day + "th"
            };
        }
        // Safe version of GetMonthlyPrayerTimings with semaphore
        private async Task<List<PrayerTimingDto>> GetMonthlyPrayerTimingsSafe(double lat, double lon, int school)
        {
            await _apiSemaphore.WaitAsync();
            try
            {
                return await GetMonthlyPrayerTimings(lat, lon, school);
            }
            finally
            {
                _apiSemaphore.Release();
            }
        }

        // Helper method to get favourite cities Ramadan timings for both fiqas (Fixed Race Condition)
        private async Task<List<CityRamadanTimeViewModel>> GetFavouriteCitiesRamadanTimingsSafe(int countryId)
        {
            var cities = await _context.Cities
                .Where(c => c.CountryId == countryId && c.IsFavourite == true && c.Latitude.HasValue && c.Longitude.HasValue)
                .ToListAsync();

            var results = new List<CityRamadanTimeViewModel>();
            var countryName = (await _context.Countries.FirstOrDefaultAsync(c => c.Id == countryId))?.Name;

            // Process sequentially with delays to avoid overwhelming the API
            foreach (var city in cities)
            {
                try
                {
                    double lat = (double)city.Latitude;
                    double lon = (double)city.Longitude;

                    // Get timings for both fiqas with semaphore protection
                    var hanafiTiming = await GetTodayRamadanTimingSafe(lat, lon, 2);
                    var jafariaTiming = await GetTodayRamadanTimingSafe(lat, lon, 3);

                    // Apply Jafaria adjustment
                    if (jafariaTiming != null)
                    {
                        jafariaTiming.Sehri = AdjustTimeForJafaria(jafariaTiming.Sehri, -10);
                        jafariaTiming.Iftar = AdjustTimeForJafaria(jafariaTiming.Iftar, +10);
                    }

                    results.Add(new CityRamadanTimeViewModel
                    {
                        CityName = city.CityName,
                        CountryName = countryName,
                        Sehri = hanafiTiming?.Sehri,
                        Iftar = hanafiTiming?.Iftar,
                        SehriJafaria = jafariaTiming?.Sehri,
                        IftarJafaria = jafariaTiming?.Iftar
                    });

                    // Small delay between cities to prevent overwhelming external APIs
                    await Task.Delay(50);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error getting timing for city: {city.CityName}");
                    // Continue with other cities even if one fails
                }
            }

            return results;
        }

        private (string nextEvent, TimeSpan timeLeft, string nextEventTime) CalculateNextRamadanEvent(RamadanTimingDto timing)
        {
            try
            {
                var now = DateTime.Now;
                var today = DateTime.Today;

                // Parse Sehri and Iftar times
                if (DateTime.TryParse(timing.Sehri, out DateTime sehriTime) && DateTime.TryParse(timing.Iftar, out DateTime iftarTime))
                {
                    // Create DateTime objects for today's events
                    var sehriToday = today.Add(sehriTime.TimeOfDay);
                    var iftarToday = today.Add(iftarTime.TimeOfDay);

                    if (now < sehriToday)
                    {
                        // Before Sehri time
                        return ("Sehri", sehriToday - now, timing.Sehri);
                    }
                    else if (now < iftarToday)
                    {
                        // Before Iftar time
                        return ("Iftar", iftarToday - now, timing.Iftar);
                    }
                    else
                    {
                        // After Iftar, next is tomorrow's Sehri
                        var sehriTomorrow = today.AddDays(1).Add(sehriTime.TimeOfDay);
                        return ("Sehri", sehriTomorrow - now, timing.Sehri);
                    }
                }

                return ("Error", TimeSpan.Zero, "--:--");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating next Ramadan event");
                return ("Error", TimeSpan.Zero, "--:--");
            }
        }

        private async Task<List<CityRamadanTimeViewModel>> GetCountryCitiesRamadanTimingsSafe(int countryId)
        {
            var cities = await _context.Cities
                .Where(c => c.CountryId == countryId && c.Latitude.HasValue && c.Longitude.HasValue && c.IsFavourite == true)
                .ToListAsync();

            var results = new List<CityRamadanTimeViewModel>();
            var countryName = (await _context.Countries.FirstOrDefaultAsync(c => c.Id == countryId))?.Name;

            // Process sequentially with delays
            foreach (var city in cities)
            {
                try
                {
                    double lat = (double)city.Latitude;
                    double lon = (double)city.Longitude;

                    var timing = await GetTodayRamadanTimingSafe(lat, lon, 2);
                    results.Add(new CityRamadanTimeViewModel
                    {
                        CityName = city.CityName,
                        CountryName = countryName,
                        Sehri = timing.Sehri,
                        Iftar = timing.Iftar,
                        SehriJafaria = AdjustTimeForJafaria(timing.Sehri, -10),
                        IftarJafaria = AdjustTimeForJafaria(timing.Iftar, +10)
                    });

                    // Small delay between cities
                    await Task.Delay(50);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error getting timing for city: {city.CityName}");
                    // Continue with other cities
                }
            }

            return results;
        }

        // Helper methods for Islamic dates
        private string GetIslamicDate(DateTime date)
        {
            try
            {
                // Implement Islamic date conversion
                return "6 Ramadan 1445 AH";
            }
            catch
            {
                return "Ramadan 1445 AH";
            }
        }

        private string GetIslamicDateEnglish(DateTime date)
        {
            try
            {
                // Implement English Islamic date conversion
                return "6 Ramadan 1445 AH";
            }
            catch
            {
                return "Ramadan 1445 AH";
            }
        }

        private async Task<RamadanTimingDto> GetTomorrowRamadanTiming(double lat, double lon, int school)
        {
            try
            {
                var prayerTimings = await GetMonthlyPrayerTimingsSafe(lat, lon, school);
                var tomorrow = DateTime.Now.Date.AddDays(1);

                var tomorrowTiming = prayerTimings.FirstOrDefault(t => t.Date.Date == tomorrow);

                if (tomorrowTiming != null)
                {
                    return new RamadanTimingDto
                    {
                        Date = tomorrowTiming.Date,
                        DateReadable = tomorrowTiming.DateReadable,
                        Sehri = tomorrowTiming.Fajr, // Sehri ends at Fajr
                        Iftar = tomorrowTiming.Maghrib, // Iftar starts at Maghrib
                        Fajr = tomorrowTiming.Fajr,
                        Maghrib = tomorrowTiming.Maghrib,
                        IslamicDateEnglish = GetIslamicDateEnglish(tomorrow)
                    };
                }

                return new RamadanTimingDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting tomorrow's Ramadan timing for lat: {lat}, lon: {lon}, school: {school}");
                return new RamadanTimingDto();
            }
        }

        // Add this method if you don't have it
        private async Task<RamadanTimingDto> GetTodayRamadanTiming(double lat, double lon)
        {
            return await GetTodayRamadanTimingSafe(lat, lon, 2);
        }

        #endregion


        #region Quran

        [HttpGet]
        [Route("quran-{slug}")]
        public async Task<IActionResult> QuranDetail(string slug)
        {
            ViewBag.Banners = await _bannerService.GetBannersAsync();
            try
            {
                var cmsData = await _context.TblUrlcontents.Where(u => u.Url == $"https://www.ilmkidunya.com/islam/quran-{slug}").AsNoTracking().FirstOrDefaultAsync();
                if (cmsData == null)
                    return NotFound();
             
                return View(cmsData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading Quran page for slug: {slug}");
                return RedirectToAction("ServerError", "Error", new { reason = $"Error loading Quran page: {ex.Message}" });
            }
        }





        #endregion Quran


    }
}
