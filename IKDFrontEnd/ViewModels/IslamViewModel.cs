using IKDFrontEnd.BookModels;
using IKDFrontEnd.Models;

namespace IKDFrontEnd.ViewModels
{
    public class IslamViewModel
    {
    }
    public class CityPrayerTimeViewModel
    {
        public string CityName { get; set; }
        public string CountryName { get; set; }
        public string Fajr { get; set; }
        public string Dhuhr { get; set; }
        public string Asr { get; set; }
        public string Maghrib { get; set; }
        public string Isha { get; set; }
        public string HanafiTime { get; set; }
        public string ShafiTime { get; set; }
    }
    public class PrayerTimesHomeViewModel
    {
        public List<CityPrayerTimeViewModel> CityPrayerTimes { get; set; } = new();
        public List<City> AllCities { get; internal set; }
        public Country Country { get; internal set; }
    }
    public class AladhanResponse
    {
        public List<AladhanData> Data { get; set; }
    }

    public class AladhanData
    {
        public AladhanTimings Timings { get; set; }
        public AladhanDate Date { get; set; }
    }

    public class AladhanTimings
    {
        public string Fajr { get; set; }
        public string Sunrise { get; set; }
        public string Dhuhr { get; set; }
        public string Asr { get; set; }
        public string Maghrib { get; set; }
        public string Isha { get; set; }
    }

    public class AladhanDate
    {
        public GregorianDate Gregorian { get; set; }
    }

    public class GregorianDate
    {
        public string Date { get; set; }
    }

    public class PrayerTimingDto
    {
        public DateTime Date { get; set; }
        public string DateReadable { get; set; }

        // Original API times
        public string Fajr { get; set; }
        public string Sunrise { get; set; }
        public string Dhuhr { get; set; }
        public string Asr { get; set; }
        public string Maghrib { get; set; }
        public string Isha { get; set; }

        // Formatted times (read-only)
        public string FajrFormatted => FormatPrayerTime(Fajr);
        public string SunriseFormatted => FormatPrayerTime(Sunrise);
        public string DhuhrFormatted => FormatPrayerTime(Dhuhr);
        public string AsrFormatted => FormatPrayerTime(Asr);
        public string MaghribFormatted => FormatPrayerTime(Maghrib);
        public string IshaFormatted => FormatPrayerTime(Isha);

        private string FormatPrayerTime(string timeString)
        {
            if (string.IsNullOrEmpty(timeString))
                return string.Empty;

            var timePart = timeString.Split(' ')[0];

            if (TimeSpan.TryParse(timePart, out var time))
            {
                var dateTime = DateTime.Today.Add(time);
                return dateTime.ToString("h:mm tt");
            }

            return timeString;
        }
    }

    public class CityPrayerTimesViewModel
    {
        public City City { get; set; }
        public Country Country { get; set; }

        // For current day
        public PrayerTimingDto TodayHanafi { get; set; }
        public PrayerTimingDto TodayShafi { get; set; }

        // For 7 days
        public List<PrayerTimingDto> HanafiWeekly { get; set; }
        public List<PrayerTimingDto> ShafiWeekly { get; set; }

        // For 30 days
        public List<PrayerTimingDto> HanafiMonthly { get; set; }
        public List<PrayerTimingDto> ShafiMonthly { get; set; }
        public List<City> AllCities { get; internal set; }
    }

    public class DynamicPrayerTimingViewModel
    {
        public string PrayerName { get; set; }
        public City City { get; set; }
        public Country Country { get; set; }

        public string HanafiTime { get; set; }
        public string ShafiTime { get; set; }

        public string NextPrayerName { get; set; }
        public string NextHanafiTime { get; set; }
        public string NextShafiTime { get; set; }

        public List<CityPrayerTimeViewModel> FavouriteCitiesPrayerTimes { get; set; }

        public List<OtherPrayerTimeViewModel> OtherPrayersTimes { get; set; }
    }



    public class OtherPrayerTimeViewModel
    {
        public string PrayerName { get; set; }
        public string HanafiTime { get; set; }
        public string ShafiTime { get; set; }
        public string Link { get; set; } // URL to the dynamic prayer page
    }
    public class IslamicDateString
    {
        public string IslamicDateFormatted { get; set; }
        public string EnglishDateFormatted { get; set; }
        public DateTime GregorianDate { get; set; }
    }

    public class RamadanTimingDto
    {
        public DateTime Date { get; set; }
        public List<IslamicDateString> Dates { get; set; }
        public string DateReadable { get; set; }
        public string Sehri { get; set; }
        public string Iftar { get; set; }
        public string Fajr { get; set; }
        public string Maghrib { get; set; }
        public string SehriJafaria { get; set; }
        public string IftarJafaria { get; set; }
        // Add these missing properties
        public string CityName { get; set; }
        public string CountryName { get; set; }
        public string Slug { get; set; }
        public string IslamicDate { get; set; } // e.g., "1st Jamadi al-Akhirah 1445H"
        public string IslamicDateEnglish { get; set; } // e.g., "1st Jumada al-Akhirah 1445H"
        public string GregorianDateString { get; set; } // e.g., "November 21, 2025"
    }

    public class RamadanHomeViewModel
    {
        public RamadanTimingDto TodayHanafi { get; set; }
        public RamadanTimingDto TodayJafaria { get; set; }
        public RamadanTimingDto TomorrowHanafi { get; set; }
        public RamadanTimingDto TomorrowJafaria { get; set; }
        public List<CityRamadanTimeViewModel> PopularCitiesTimings { get; set; } // Changed from List<CityRamadanTimeViewModel>
        public List<Country> Countries { get; set; }
        public string NextEventHanafi { get; set; }
        public TimeSpan TimeLeftHanafi { get; set; }
        public string NextEventTimeHanafi { get; set; }
        public string NextEventJafaria { get; set; }
        public TimeSpan TimeLeftJafaria { get; set; }
        public string NextEventTimeJafaria { get; set; }
        public string Location { get; set; }
        public Country Country { get; set; }
        public City City { get; set; }
        public List<City> AllCities { get; set; }
    }

    public class CityRamadanTimeViewModel
    {
        public string CityName { get; set; }
        public string CountryName { get; set; }
        public string Sehri { get; set; }
        public string Iftar { get; set; }
        public string Slug { get; set; }
        public string SehriJafaria { get; set; }
        public string IftarJafaria { get; set; }
    }

    public class CityRamadanViewModel
    {
        public City City { get; set; }
        public Country Country { get; set; }
        public List<City> AllCities { get; set; }

        public RamadanTimingDto TodayHanafi { get; set; }
        public RamadanTimingDto TodayJafaria { get; set; }
        public RamadanTimingDto TomorrowHanafi { get; set; }
        public RamadanTimingDto TomorrowJafaria { get; set; }

        public string NextEventHanafi { get; set; }
        public TimeSpan TimeLeftHanafi { get; set; }
        public string NextEventTimeHanafi { get; set; }
        public string NextEventJafaria { get; set; }
        public TimeSpan TimeLeftJafaria { get; set; }
        public string NextEventTimeJafaria { get; set; }

        public List<RamadanTimingDto> HanafiRamadanMonthly { get; set; }
        public List<RamadanTimingDto> ShafiRamadanMonthly { get; set; }

        public List<Country> Countries { get; set; }
        public string Location { get; set; }
    }

}
