using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IKDFrontEnd.Models;
using IKDFrontEnd.ViewModels;
using System.Net.Http;
using System.Linq;
using System.Buffers.Text;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using IKDFrontEnd.Services;
using IKDFrontEnd.ViewModels.Common;
using System.Security.Policy;
using System;

namespace IKDFrontEnd.Controllers
{
    [Route("tutors")] // Base route prefix: /tutors

    public class TutorsController : Controller
    {
        DbikdContext _context;
        BannerService _bannerService;
        private readonly CmsRepository _cmsRepo;
        public TutorsController(DbikdContext context, BannerService bannnerService, CmsRepository cmsRepo)
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
            var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/tutors");

            if (cmsData == null)
            {
                cmsData = new TblCmsDto
                {
                    MetaTitle = "Home Tuitions & Tutors in Pakistan - Lahore, Karachi, Islamabad",
                    MetaKeys = "home tuition, tutors in Pakistan, tutors in Lahore, tutors in Karachi, tutors in Islamabad, online tutoring, private tutors, subject tutors, skill learning",
                    MetaDesc = "Find experienced home tutors and online tuition services across Pakistan, including Lahore, Karachi, and Islamabad. Learn any subject or skill with expert guidance at your convenience.",
                    Heading = "Write down the subject or skill you want to learn",
                    Desc1 = "",
                    Desc2 = "",
                    Desc3 = ""
                };
            }
            ViewBag.CmsData = cmsData;


            ViewBag.Subjects = _context.TblDefSubjects.ToList();
            ViewBag.EducationLevel = _context.TblEducationLevels.ToList();
            ViewBag.Cities = _context.TblDefCities.Where(c => c.IsActive == true).ToList();

            var subjectData = await _context.TblDefSubjects
                .Where(subject => subject.IsGeneral != true && !string.IsNullOrEmpty(subject.SubjectIcon) && subject.SubjectIcon != "default.png")
                .Select(subject => new SubjectWithTutorCountViewModel
                {
                    SubjectId = subject.SubjectId,
                    SubjectName = subject.SubjectName != null
                        ? subject.SubjectName.Replace("\r", "").Replace("\n", "").Trim()
                        : "",
                    SubjectIcon = "https://www.ilmkidunya.com/images/icons/" + subject.SubjectIcon,
                    TutorCount = _context.TblTutorSubjects.Count(ts => ts.SubjectId == subject.SubjectId)
                })
                .Take(14)
                .ToListAsync();

            var skillsData = await _context.TblDefSubjects
                .Where(subject => subject.IsGeneral == true && !string.IsNullOrEmpty(subject.SubjectIcon) && subject.SubjectIcon != "default.png")
                .Select(subject => new SkillWithTutorCountViewModel
                {
                    SubjectId = subject.SubjectId,
                    SubjectName = subject.SubjectName != null
                        ? subject.SubjectName.Replace("\r", "").Replace("\n", "").Trim()
                        : "",
                    SubjectIcon = "https://images.ishallwin.com/tutors/" + subject.SubjectIcon,
                    TutorCount = _context.TblTutorSubjects.Count(ts => ts.SubjectId == subject.SubjectId)
                })
                .Take(6)
                .ToListAsync();

            var cityData = await _context.TblDefCities
                .Where(c => c.IsImageAvailable == true)
                .Select(city => new CityWithTutorCountViewModel
                {
                    CityId = city.CityId,
                    CityName = city.CityName,
                    ImageName = "https://images.ishallwin.com/bs-chem/" + city.ImageName.Replace(".png", ".webp"),
                    TutorCount = _context.TblTltutors
                        .Join(_context.TblDefMemberInfo2s,
                              tutor => tutor.MemberId,
                              member => member.MemberId,
                              (tutor, member) => new { tutor, member })
                        .Count(x => x.member.CityId == city.CityId)
                })
                .OrderByDescending(c => c.TutorCount)
                .ToListAsync();

            var educationLevelData = await _context.TblEducationLevels
                .Where(lvl => !string.IsNullOrEmpty(lvl.LevelImage))
                .Select(lvl => new EducationLevelWithTutorCountViewModel
                {
                    EducationLevelId = lvl.EducationLevelId,
                    EducationLevelName = lvl.EducationLevelName.Replace("\r", "").Replace("\n", "").Trim(),
                    LevelImage = lvl.LevelImage,
                    Url = lvl.Url,
                    TutorCount = _context.TblTutorLevels.Count(tl => tl.EducationLevelId == lvl.EducationLevelId)
                })
                .Take(6)
                .ToListAsync();

            int row = 0; // for pagination: row * 8


            var baseTutorData = await (
                from tutor in _context.TblTltutors
                join member in _context.TblDefMemberInfo2s
                    on tutor.MemberId equals member.MemberId
                join city in _context.TblDefCities
                    on member.CityId equals (decimal?)city.CityId
                where !string.IsNullOrEmpty(member.MemberName)
                    && member.CityId != null
                    && member.Gender != null
                    && !string.IsNullOrEmpty(tutor.CourseName)
                    && _context.TblTutorSubjects.Any(ts => ts.TlTutorId == tutor.TlTutorId)
                orderby tutor.TlTutorId descending
                select new
                {
                    tutor.TlTutorId,
                    tutor.AboutTutoring,
                    tutor.Experience,
                    tutor.Availability,
                    tutor.CourseName,
                    tutor.TutoringOptions,
                    member.MemberId,               // ✅ Add this line
                    member.MemberName,
                    member.Gender,
                    member.ImageName,
                    city.CityName
                }

            )
            .Skip(row * 8)
            .Take(6)
            .ToListAsync();

            // Then get all related subjects for these tutors in a single query
            var tutorIds = baseTutorData.Select(x => x.TlTutorId).ToList();

            var tutorSubjectsDict = await _context.TblTutorSubjects
                .Where(ts => tutorIds.Contains(ts.TlTutorId ?? 0))
                .Join(_context.TblDefSubjects,
                      ts => ts.SubjectId,
                      s => s.SubjectId,
                      (ts, s) => new { ts.TlTutorId, s.SubjectName })
                .GroupBy(x => x.TlTutorId)
                .ToDictionaryAsync(g => g.Key, g => g.Select(x => x.SubjectName).Take(3).ToList());

            // Final projection
            var latestTutors = baseTutorData.Select(x => new TutorCardViewModel
            {
                TutorId = x.TlTutorId,
                MemberId = x.MemberId,
                TutorName = x.MemberName,
                CityName = x.CityName ?? "N/A",
                Gender = x.Gender ?? false,
                ProfileImage = string.IsNullOrEmpty(x.ImageName)
                                             ? ((x.Gender ?? false)
                                                 ? "https://cdn.ilmkidunya.com/images/noimagemale.jpg"
                                                 : "https://images.ishallwin.com/tutors/noimagefemale.jpg")
                                             : "https://cdn.ilmkidunya.com//Membership/Thumb/" + x.ImageName,

                TeachingSummary = !string.IsNullOrWhiteSpace(x.AboutTutoring)
                    ? x.AboutTutoring
                    : "Teaching",
                Experience = x.Experience,
                Availability = x.Availability,
                CourseName = x.CourseName,
                TutoringOptions = x.TutoringOptions,
                Subjects = tutorSubjectsDict.ContainsKey((int)x.TlTutorId) ? tutorSubjectsDict[(int)x.TlTutorId] : new List<string>()
            }).ToList();





            var viewModel = new CombinedSubjectCityLevelViewModel
            {
                Subjects = subjectData,
                Cities = cityData,
                EducationLevels = educationLevelData,
                Skills = skillsData,
                LatestTutors = latestTutors
            };



            return View(viewModel);
        }


        [HttpGet("tutors-for-all-subjects.aspx")]
        public async Task<IActionResult> TutorForAllSubjects()
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            // Get CMS meta data
            var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/tutors/tutors-for-all-subjects.aspx");
            ViewBag.CmsData = cmsData;

            // Step 1: Fetch basic subject data from DB
            var rawSubjects = await _context.TblDefSubjects
                .Where(subject => subject.IsGeneral != true &&
                                  !string.IsNullOrEmpty(subject.SubjectIcon) &&
                                  subject.SubjectIcon != "default.png")
                .Select(subject => new SubjectWithTutorCountViewModel
                {
                    SubjectId = subject.SubjectId,
                    SubjectName = subject.SubjectName != null
                        ? subject.SubjectName.Replace("\r", "").Replace("\n", "").Trim()
                        : "",
                    TutorCount = _context.TblTutorSubjects.Count(ts => ts.SubjectId == subject.SubjectId)
                })
                .ToListAsync();

            // Step 2: Call the GetSubjectImageUrl function in-memory
            foreach (var subject in rawSubjects)
            {
                subject.SubjectIcon = GetSubjectImageUrl(subject.SubjectName);
            }

            // Step 3: Sort by tutor count
            var allSubjects = rawSubjects.OrderByDescending(s => s.TutorCount).ToList();

            return View(allSubjects);
        }




        [HttpGet("home-tutors-for-{subjectSlug}-{id:int}.aspx")]

        public async Task<IActionResult> TutorSubjectDetail(string subjectSlug, short id)
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            var url = $"https://www.ilmkidunya.com/tutors/home-tutors-for-{subjectSlug}-{id}.aspx";
            var cmsData = await _cmsRepo.GetByUrlAsync($"{url}");
            await _cmsRepo.GetByUrlAsync($"{url}");
            ViewBag.CmsData = cmsData;

            var subject = await _context.TblDefSubjects.FirstOrDefaultAsync(s => s.SubjectId == id);
            if (subject == null) return NotFound();
            if (subject.IsGeneral == true)
            {
                ViewBag.Skills = "true";
            }
            else
            {
                ViewBag.Skills = "false";
            }

            ViewBag.SubjectName = subject.SubjectName;
            ViewBag.SubjectId = id;

            // 1. Get all tutor IDs for this subject
            var tutorIds = await _context.TblTutorSubjects
                .Where(ts => ts.SubjectId == id)
                .Select(ts => ts.TlTutorId ?? 0)
                .Distinct()
                .ToListAsync();

            // 2. Get Cities for these tutors
            var cityData = await (
                 from ts in _context.TblTutorSubjects
                 where ts.SubjectId == subject.SubjectId
                 join t in _context.TblTltutors on (int)ts.TlTutorId equals t.TlTutorId
                 join m in _context.TblDefMemberInfo2s on t.MemberId equals m.MemberId
                 where m.CityId != null
                 join c in _context.TblDefCities on m.CityId equals (decimal?)c.CityId
                 where c.IsImageAvailable == true
                 group c by c.CityId into g
                 select new CityWithTutorCountViewModel
                 {
                     CityId = g.Key,
                     CityName = g.First().CityName,
                     ImageName = "https://images.ishallwin.com/bs-chem/" + g.First().ImageName.Replace(".png", ".webp"),
                     TutorCount = g.Count()
                 }
             )
             .OrderByDescending(x => x.TutorCount)
             .ToListAsync();


            // 3. Get Education Levels for these tutors
            var educationLevelData = (await _context.TblTutorLevels
                .Where(tl => tutorIds.Contains(tl.TlTutorId ?? 0))
                .Join(_context.TblEducationLevels,
                      tl => tl.EducationLevelId,
                      lvl => lvl.EducationLevelId,
                      (tl, lvl) => lvl)
                .GroupBy(lvl => lvl.EducationLevelId)
                .Select(g => new EducationLevelWithTutorCountViewModel
                {
                    EducationLevelId = g.Key,
                    EducationLevelName = g.First().EducationLevelName,
                    LevelImage = g.First().LevelImage,
                    Url = g.First().Url,
                    TutorCount = g.Count()
                })
                .OrderByDescending(x => x.EducationLevelId)
                .ToListAsync())
                .AsEnumerable() // convert to LINQ-to-Objects
                .Reverse()
                .ToList();


            // You may also want to fetch the tutors themselves (optional):
            var baseTutorData = await (
                from tutor in _context.TblTltutors
                join member in _context.TblDefMemberInfo2s on tutor.MemberId equals member.MemberId
                join city in _context.TblDefCities on member.CityId equals (decimal?)city.CityId
                where tutorIds.Contains((int)tutor.TlTutorId)
                select new
                {
                    tutor.TlTutorId,
                    tutor.MemberId,
                    tutor.AboutTutoring,
                    tutor.Experience,
                    tutor.Availability,
                    tutor.CourseName,
                    tutor.TutoringOptions,
                    member.MemberName,
                    member.Gender,
                    member.ImageName,
                    city.CityName
                }
            ).Take(8).ToListAsync();

            // Subjects for tutors
            var tutorSubjectsDict = await _context.TblTutorSubjects
                .Where(ts => tutorIds.Contains(ts.TlTutorId ?? 0))
                .Join(_context.TblDefSubjects,
                      ts => ts.SubjectId,
                      s => s.SubjectId,
                      (ts, s) => new { ts.TlTutorId, s.SubjectName })
                .GroupBy(x => x.TlTutorId)
                .ToDictionaryAsync(g => g.Key ?? 0, g => g.Select(x => x.SubjectName).Take(3).ToList());

            // Final mapped tutors
            var latestTutors = baseTutorData.Select(x => new TutorCardViewModel
            {
                TutorId = x.TlTutorId,
                MemberId = (decimal)x.MemberId,
                TutorName = x.MemberName,
                CityName = x.CityName ?? "N/A",
                Gender = x.Gender ?? false,
                ProfileImage = string.IsNullOrEmpty(x.ImageName)
                                        ? ((x.Gender ?? false)
                                            ? "https://cdn.ilmkidunya.com/images/noimagemale.jpg"
                                            : "https://images.ishallwin.com/tutors/noimagefemale.jpg")
                                        : "https://cdn.ilmkidunya.com//Membership/Thumb/" + x.ImageName,
                TeachingSummary = string.IsNullOrWhiteSpace(x.AboutTutoring) ? "Teaching" : x.AboutTutoring,
                Experience = x.Experience,
                Availability = x.Availability,
                CourseName = x.CourseName,
                TutoringOptions = x.TutoringOptions,
                Subjects = tutorSubjectsDict.ContainsKey((int)x.TlTutorId) ? tutorSubjectsDict[(int)x.TlTutorId] : new List<string>()
            }).ToList();

            var viewModel = new CombinedSubjectCityLevelViewModel
            {
                Cities = cityData,
                EducationLevels = educationLevelData,
                LatestTutors = latestTutors,
                subjectName = subjectSlug
            };

            return View(viewModel);
        }


        [HttpGet("tutors-for-all-levels.aspx")]
        public async Task<IActionResult> TutorEducationLevelDetail()
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/tutors/tutors-for-all-levels.aspx");
            ViewBag.CmsData = cmsData;

            var educationLevelData = await _context.TblEducationLevels
    .Where(lvl => !string.IsNullOrEmpty(lvl.LevelImage))
    .Select(lvl => new EducationLevelWithTutorCountViewModel
    {
        EducationLevelId = lvl.EducationLevelId,
        EducationLevelName = lvl.EducationLevelName.Replace("\r", "").Replace("\n", "").Trim(),
        LevelImage = lvl.LevelImage,
        Url = lvl.Url,
        TutorCount = _context.TblTutorLevels.Count(tl => tl.EducationLevelId == lvl.EducationLevelId)
    })
    .ToListAsync();

            var viewModel = new CombinedSubjectCityLevelViewModel
            {

                EducationLevels = educationLevelData,

            };



            return View(viewModel);
        }





        [HttpGet("tutors-for-level-{levelSlug}.aspx")]

        public async Task<IActionResult> TutorForLevel(string levelSlug, int id)
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            var url = $"https://www.ilmkidunya.com/tutors/tutors-for-level-{levelSlug}.aspx";
            var cmsData = await _cmsRepo.GetByUrlAsync($"{url}");

            ViewBag.CmsData = cmsData;

            ViewBag.LevelId = id;

            // ---- SUBJECTS ----
            List<SubjectWithTutorCountViewModel> subjects = null;

            if (id != 1)
            {
                var validSubjectIds = new List<short> { 1, 2, 3, 4, 5, 6, 29, 30, 34 };

                subjects = await _context.TblDefSubjects
                    .Where(s => validSubjectIds.Contains(s.SubjectId))
                    .Select(s => new
                    {
                        Subject = s,
                        TutorCount = _context.TblTutorSubjects
                            .Where(ts => ts.SubjectId == s.SubjectId)
                            .Join(_context.TblTutorLevels,
                                  ts => ts.TlTutorId,
                                  tl => tl.TlTutorId,
                                  (ts, tl) => tl)
                            .Count(tl => tl.EducationLevelId == id)
                    })
                    .Where(x => x.TutorCount > 0)
                    .Select(x => new SubjectWithTutorCountViewModel
                    {
                        SubjectId = x.Subject.SubjectId,
                        SubjectName = x.Subject.SubjectName.Trim(),
                        TutorCount = x.TutorCount
                    })
                    .ToListAsync();
                // Apply image URL logic using the helper method
                foreach (var subject in subjects)
                {
                    subject.SubjectIcon = GetSubjectImageUrl(subject.SubjectName);
                }

            }

            // ---- TUTOR IDs teaching this education level ----
            var tutorIds = await _context.TblTutorLevels
            .Where(tl => tl.EducationLevelId == id)
            .Select(tl => tl.TlTutorId)
            .Distinct()
            .ToListAsync();

            // ---- CITIES ----
            var cityData = await (
                from tutor in _context.TblTltutors
                join member in _context.TblDefMemberInfo2s on tutor.MemberId equals member.MemberId
                join city in _context.TblDefCities on member.CityId equals (decimal?)city.CityId
                where tutorIds.Contains((int)tutor.TlTutorId)
                      && city.IsImageAvailable == true
                group city by city.CityId into g
                select new CityWithTutorCountViewModel
                {
                    CityId = g.Key,
                    CityName = g.First().CityName,
                    ImageName = "https://images.ishallwin.com/bs-chem/" + g.First().ImageName.Replace(".png", ".webp"),
                    TutorCount = g.Count()
                }
            ).OrderByDescending(x => x.TutorCount)
             .ToListAsync();

            // ---- TUTORS (detailed) ----
            var baseTutorData = await (
                from tutor in _context.TblTltutors
                join member in _context.TblDefMemberInfo2s on tutor.MemberId equals member.MemberId
                join city in _context.TblDefCities on member.CityId equals (decimal?)city.CityId
                where tutorIds.Contains((int)tutor.TlTutorId)
                orderby tutor.TlTutorId descending
                select new
                {
                    tutor.TlTutorId,
                    tutor.MemberId,
                    tutor.AboutTutoring,
                    tutor.Experience,
                    tutor.Availability,
                    tutor.CourseName,
                    tutor.TutoringOptions,
                    member.MemberName,
                    member.Gender,
                    member.ImageName,
                    city.CityName
                }
            ).Take(8).ToListAsync();

            // ---- SUBJECTS per Tutor ----
            var tutorSubjectsDict = await _context.TblTutorSubjects
                .Where(ts => tutorIds.Contains(ts.TlTutorId ?? 0))
                .Join(_context.TblDefSubjects,
                      ts => ts.SubjectId,
                      s => s.SubjectId,
                      (ts, s) => new { ts.TlTutorId, s.SubjectName })
                .GroupBy(x => x.TlTutorId)
                .ToDictionaryAsync(
                    g => g.Key ?? 0,
                    g => g.Select(x => x.SubjectName).Take(3).ToList()
                );

            // ---- Final Tutors Projection ----
            var latestTutors = baseTutorData.Select(x => new TutorCardViewModel
            {
                TutorId = x.TlTutorId,
                MemberId = (decimal)x.MemberId,
                TutorName = x.MemberName,
                CityName = x.CityName ?? "N/A",
                Gender = x.Gender ?? false,
                ProfileImage = string.IsNullOrEmpty(x.ImageName)
                                            ? ((x.Gender ?? false)
                                                ? "https://cdn.ilmkidunya.com/images/noimagemale.jpg"
                                                : "https://images.ishallwin.com/tutors/noimagefemale.jpg")
                                            : "https://cdn.ilmkidunya.com//Membership/Thumb/" + x.ImageName,
                TeachingSummary = string.IsNullOrWhiteSpace(x.AboutTutoring) ? "Teaching" : x.AboutTutoring,
                Experience = x.Experience,
                Availability = x.Availability,
                CourseName = x.CourseName,
                TutoringOptions = x.TutoringOptions,
                Subjects = tutorSubjectsDict.ContainsKey((int)x.TlTutorId) ? tutorSubjectsDict[(int)x.TlTutorId] : new List<string>()
            }).ToList();

            // ---- FINAL VIEWMODEL ----
            var viewModel = new CombinedSubjectCityLevelViewModel
            {
                Subjects = subjects,
                Cities = cityData,
                LatestTutors = latestTutors,
                educationLevelName = levelSlug
            };

            return View(viewModel);
        }




        [HttpGet("tutors-for-all-skills.aspx")]
        public async Task<IActionResult> TutorForAllSkills()
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            // Get CMS meta data
            var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/tutors/tutors-for-all-subjects.aspx");

            ViewBag.CmsData = cmsData;

            // Step 1: Fetch general (IsGeneral == true) subjects only
            var rawSubjects = await _context.TblDefSubjects
                .Where(subject => subject.IsGeneral == true &&
                                  !string.IsNullOrEmpty(subject.SubjectIcon) &&
                                  subject.SubjectIcon != "default.png")
                .Select(subject => new SubjectWithTutorCountViewModel
                {
                    SubjectId = subject.SubjectId,
                    SubjectName = subject.SubjectName != null
                        ? subject.SubjectName.Replace("\r", "").Replace("\n", "").Trim()
                        : "",
                    TutorCount = _context.TblTutorSubjects.Count(ts => ts.SubjectId == subject.SubjectId)
                })
                .ToListAsync();

            // Step 2: Call the GetSubjectImageUrl function in-memory
            foreach (var subject in rawSubjects)
            {
                subject.SubjectIcon = GetSubjectImageUrl(subject.SubjectName);
            }

            // Step 3: Sort by tutor count
            var allSubjects = rawSubjects.OrderByDescending(s => s.TutorCount).ToList();

            return View(allSubjects);
        }


        [HttpGet("tutors-for-all-cities.aspx")]
        public async Task<IActionResult> TutorForAllCities()
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;            
            // Get CMS meta data
            var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/tutors/tutors-for-all-cities.aspx");

            ViewBag.CmsData = cmsData;

            var cityData = await _context.TblDefCities
                .Select(city => new CityWithTutorCountViewModel
                {
                    CityId = city.CityId,
                    CityName = city.CityName,
                    ImageName = "https://images.ishallwin.com/bs-chem/" + city.ImageName.Replace(".png", ".webp"),
                    TutorCount = _context.TblTltutors
                        .Join(_context.TblDefMemberInfo2s,
                              tutor => tutor.MemberId,
                              member => member.MemberId,
                              (tutor, member) => new { tutor, member })
                        .Count(x => x.member.CityId == city.CityId)
                })
                .OrderByDescending(c => c.TutorCount)
                .ToListAsync();



            return View(cityData);
        }


        [HttpGet("home-tutors-in-{citySlug}.aspx")]
        public async Task<IActionResult> TutorCityDetail(string citySlug)
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            var city = await _context.TblDefCities.Where(c => c.CityName == citySlug).FirstOrDefaultAsync();
            if (city == null)
            {
                return NotFound();
            }

            ViewBag.CityId = city.CityId;

            var url = $"https://www.ilmkidunya.com/tutors/home-tutors-in-{citySlug}.aspx";
            var cmsData = await _cmsRepo.GetByUrlAsync($"{url}");

            if (cmsData == null)
            {
                cmsData = new TblCmsDto
                {
                    MetaTitle = $"Home Tutors in {city.CityName} - Find Qualified Private Tutors Near You",
                    MetaKeys = $"home tutors in {city.CityName}, private tutors {city.CityName}, tuition services {city.CityName}, online tutors {city.CityName}, best tutors {city.CityName}, subject tutors {city.CityName}",
                    MetaDesc = $"Find expert home tutors in {city.CityName} for all subjects and grades. Learn at home or online with professional and experienced teachers.",
                    Heading = $"Home Tutors in {city.CityName}",
                    Desc1 = "",
                    Desc2 = "",
                    Desc3 = ""
                };
            }

            ViewBag.CmsData = cmsData;



            var validSubjectIds = new List<short> { 1, 2, 3, 4, 5, 6, 29, 30, 34 };



            var subject = (from t in _context.TblTltutors
                           join m in _context.TblDefMemberInfo2s on t.MemberId equals m.MemberId
                           join ts in _context.TblTutorSubjects on (int)t.TlTutorId equals ts.TlTutorId
                           join s in _context.TblDefSubjects on ts.SubjectId equals s.SubjectId
                           where m.CityId == city.CityId && s.IsGeneral == false
                           group t by new { s.SubjectId, s.SubjectName } into g // removed s.SubjectIcon here
                           select new SubjectWithTutorCountViewModel
                           {
                               SubjectId = g.Key.SubjectId,
                               SubjectName = g.Key.SubjectName,
                               TutorCount = g.Select(x => x.TlTutorId).Distinct().Count()
                           })
                           .OrderByDescending(x => x.TutorCount)
                           .ToList();

            // Apply subject image logic using GetSubjectImageUrl
            foreach (var s in subject)
            {
                s.SubjectIcon = GetSubjectImageUrl(s.SubjectName);
            }







            var educationLevels = (from t in _context.TblTltutors
                                   join m in _context.TblDefMemberInfo2s on t.MemberId equals m.MemberId
                                   join tl in _context.TblTutorLevels on (int)t.TlTutorId equals tl.TlTutorId
                                   join el in _context.TblEducationLevels on tl.EducationLevelId equals el.EducationLevelId
                                   where m.CityId == city.CityId
                                   group t by new { el.EducationLevelId, el.EducationLevelName, el.LevelImage, el.Url } into g
                                   select new EducationLevelWithTutorCountViewModel
                                   {
                                       EducationLevelId = g.Key.EducationLevelId,
                                       EducationLevelName = g.Key.EducationLevelName,
                                       LevelImage = g.Key.LevelImage,
                                       Url = g.Key.Url,
                                       TutorCount = g.Select(x => x.TlTutorId).Distinct().Count()
                                   })
                                  .OrderByDescending(x => x.EducationLevelId)
                                  .ToList()
                                  .AsEnumerable() // ensure LINQ-to-Objects
                                  .Reverse()
                                  .ToList();





            var latestTutors = (from t in _context.TblTltutors
                                join m in _context.TblDefMemberInfo2s on t.MemberId equals m.MemberId
                                join c in _context.TblDefCities on m.CityId equals c.CityId
                                where m.CityId == city.CityId
                                select new TutorCardViewModel
                                {
                                    TutorId = t.TlTutorId,
                                    MemberId = (decimal)t.MemberId,
                                    TutorName = m.MemberName,
                                    CityName = c.CityName,
                                    ProfileImage = string.IsNullOrEmpty(m.ImageName)
                                                   ? ((m.Gender ?? false)
                                                       ? "https://cdn.ilmkidunya.com/images/noimagemale.jpg"
                                                       : "https://images.ishallwin.com/tutors/noimagefemale.jpg")
                                                   : "https://cdn.ilmkidunya.com//Membership/Thumb/" + m.ImageName,
                                    Gender = (bool)m.Gender,
                                    TeachingSummary = t.AboutTutoring,
                                    Experience = t.Experience,
                                    Availability = t.Availability,
                                    CourseName = t.CourseName,
                                    TutoringOptions = t.TutoringOptions,
                                    Subjects = (from ts in _context.TblTutorSubjects
                                                join s in _context.TblDefSubjects on ts.SubjectId equals s.SubjectId
                                                where ts.TlTutorId == t.TlTutorId
                                                select s.SubjectName).Take(3).ToList()
                                }).Take(8).ToList();



            var viewModel = new CombinedSubjectCityLevelViewModel
            {
                Subjects = subject,
                EducationLevels = educationLevels,
                LatestTutors = latestTutors,
                cityName = citySlug,
            };
            return View(viewModel);
        }



        //[HttpGet("searchresult.aspx")]
        //[HttpGet("home-tutors-for-{educationLevelSlug}-class-{subjectSlug}.aspx")]
        //[HttpGet("home-tutors-for-{subjectSlug}-in-{citySlug}.aspx")]
        //[HttpGet("tutors-for-{educationLevelSlug}-in-{citySlug}.aspx")]
        //public async Task<IActionResult> SearchTutors(
        //    int? SubjID,
        //    int? CityID,
        //    int? LevelID,
        //    string educationLevelSlug,
        //    string subjectSlug,
        //    string citySlug)
        //{
        //    // Construct URL from route slug values
        //    string url = null;

        //    if (!string.IsNullOrEmpty(citySlug) && !string.IsNullOrEmpty(subjectSlug) && string.IsNullOrEmpty(educationLevelSlug))
        //    {
        //        url = $"https://www.ilmkidunya.com/tutors/home-tutors-for-{subjectSlug}-in-{citySlug}.aspx";
        //    }
        //    else if (!string.IsNullOrEmpty(educationLevelSlug) && !string.IsNullOrEmpty(citySlug) && string.IsNullOrEmpty(subjectSlug))
        //    {
        //        url = $"https://www.ilmkidunya.com/tutors/tutors-for-{educationLevelSlug}-in-{citySlug}.aspx";
        //    }
        //    else if (!string.IsNullOrEmpty(educationLevelSlug) && !string.IsNullOrEmpty(subjectSlug) && string.IsNullOrEmpty(citySlug))
        //    {
        //        url = $"https://www.ilmkidunya.com/tutors/home-tutors-for-{educationLevelSlug}-class-{subjectSlug}.aspx";
        //    }
        //    else
        //    {
        //        url = "https://www.ilmkidunya.com/tutors/searchresult.aspx";
        //    }

        //    var cmsData = await _context.TblCms.FirstOrDefaultAsync(c => c.Url == url);

        //    if (cmsData == null)
        //    {
        //        cmsData = new TblCm
        //        {
        //            Desc1 = "",
        //            Desc2 = "",
        //            Desc3 = "",
        //            PageName = "Search Tutor", 

        //        };
        //    }
        //    ViewBag.CmsData = cmsData;

        //    // Normalize function (used for all slugs)
        //    static string NormalizeSlug(string input) =>
        //        input?.Trim().ToLower().Replace(" ", "-");

        //    // Resolve SubjectId from slug if needed
        //    if (SubjID == null && !string.IsNullOrWhiteSpace(subjectSlug))
        //    {
        //        string normalized = NormalizeSlug(subjectSlug);
        //        var subject = _context.TblDefSubjects
        //            .AsEnumerable()
        //            .FirstOrDefault(s => normalized.Contains(NormalizeSlug(s.SubjectName)));

        //        if (subject != null)
        //            SubjID = subject.SubjectId;
        //    }

        //    // Resolve EducationLevelId from slug if needed
        //    if (LevelID == null && !string.IsNullOrWhiteSpace(educationLevelSlug))
        //    {
        //        string normalized = NormalizeSlug(educationLevelSlug);
        //        var level = _context.TblEducationLevels
        //            .AsEnumerable()
        //            .FirstOrDefault(e => normalized.Contains(NormalizeSlug(e.EducationLevelName)));

        //        if (level != null)
        //            LevelID = level.EducationLevelId;
        //    }

        //    // Resolve CityId from slug if needed
        //    if (CityID == null && !string.IsNullOrWhiteSpace(citySlug))
        //    {
        //        string normalized = NormalizeSlug(citySlug);
        //        var city = _context.TblDefCities
        //            .AsEnumerable()
        //            .FirstOrDefault(c => normalized.Contains(NormalizeSlug(c.CityName)));

        //        if (city != null)
        //            CityID = city.CityId;
        //    }

        //    var model = new TutorSearchFormViewModel
        //    {
        //        SubjectId = SubjID,
        //        CityId = CityID,
        //        EducationLevelId = LevelID
        //    };

        //    // Main query
        //    var query = from m in _context.TblDefMemberInfo2s
        //                join t in _context.TblTltutors on m.MemberId equals t.MemberId
        //                join c in _context.TblDefCities on m.CityId equals c.CityId
        //                join ts in _context.TblTutorSubjects on (int)t.TlTutorId equals ts.TlTutorId into tsJoin
        //                from ts in tsJoin.DefaultIfEmpty()
        //                join tl in _context.TblTutorLevels on (int)t.TlTutorId equals tl.TlTutorId into tlJoin
        //                from tl in tlJoin.DefaultIfEmpty()
        //                select new
        //                {
        //                    m,
        //                    t,
        //                    c,
        //                    SubjectId = ts.SubjectId,
        //                    EducationLevelId = tl.EducationLevelId
        //                };

        //    // Apply dynamic filters
        //    if (model.CityId is > 0)
        //        query = query.Where(x => x.m.CityId == model.CityId);

        //    if (model.SubjectId is > 0)
        //        query = query.Where(x => x.SubjectId == model.SubjectId);

        //    if (model.EducationLevelId is > 0)
        //        query = query.Where(x => x.EducationLevelId == model.EducationLevelId);

        //    // Final projection
        //    var tutors = query
        //        .AsEnumerable()
        //        .GroupBy(x => x.t.TlTutorId)
        //        .Select(g => g.First())
        //        .Select(x => new TutorCardViewModel
        //        {
        //            TutorId = x.t.TlTutorId,
        //            MemberId = x.m.MemberId,
        //            TutorName = x.m.MemberName,
        //            CityName = x.c.CityName,
        //            ProfileImage = string.IsNullOrEmpty(x.m.ImageName)
        //                ? ((x.m.Gender ?? false)
        //                    ? "https://cdn.ilmkidunya.com/images/noimagemale.jpg"
        //                    : "https://images.ishallwin.com/tutors/noimagefemale.jpg")
        //                : "https://cdn.ilmkidunya.com//Membership/Thumb/" + x.m.ImageName,
        //            Gender = x.m.Gender ?? true,
        //            TeachingSummary = x.t.AboutTutoring,
        //            Experience = x.t.Experience,
        //            Availability = x.t.Availability,
        //            CourseName = x.t.CourseName,
        //            TutoringOptions = x.t.TutoringOptions,
        //            Subjects = _context.TblTutorSubjects
        //                       .Where(ts => ts.TlTutorId == x.t.TlTutorId)
        //                       .Join(_context.TblDefSubjects,
        //                             ts => ts.SubjectId,
        //                             s => s.SubjectId,
        //                             (ts, s) => s.SubjectName)
        //                       .Take(4)
        //                       .ToList()
        //        })
        //        .Take(10)
        //        .ToList();

        //    ViewBag.SubjectId = SubjID;
        //    ViewBag.CityId = CityID;
        //    ViewBag.LevelId = LevelID;

        //    return View("TutorResults", tutors);
        //}




        [HttpGet("searchresult.aspx")]
        [HttpGet("home-tutors-for-{educationLevelSlug}-class-{subjectSlug}.aspx")]
        [HttpGet("home-tutors-for-{subjectSlug}-in-{citySlug}.aspx")]
        [HttpGet("tutors-for-{educationLevelSlug}-in-{citySlug}.aspx")]
        public async Task<IActionResult> SearchTutors(int? SubjID, int? CityID, int? LevelID, string educationLevelSlug, string subjectSlug, string citySlug)
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;
            // Construct URL from route slug values
            string url;

            if (!string.IsNullOrEmpty(citySlug) && !string.IsNullOrEmpty(subjectSlug) && string.IsNullOrEmpty(educationLevelSlug))
            {
                url = $"https://www.ilmkidunya.com/tutors/home-tutors-for-{subjectSlug}-in-{citySlug}.aspx";
            }
            else if (!string.IsNullOrEmpty(educationLevelSlug) && !string.IsNullOrEmpty(citySlug) && string.IsNullOrEmpty(subjectSlug))
            {
                url = $"https://www.ilmkidunya.com/tutors/tutors-for-{educationLevelSlug}-in-{citySlug}.aspx";
            }
            else if (!string.IsNullOrEmpty(educationLevelSlug) && !string.IsNullOrEmpty(subjectSlug) && string.IsNullOrEmpty(citySlug))
            {
                url = $"https://www.ilmkidunya.com/tutors/home-tutors-for-{educationLevelSlug}-class-{subjectSlug}.aspx";
            }
            else
            {
                url = "https://www.ilmkidunya.com/tutors/searchresult.aspx";
            }

            var cmsData = await _cmsRepo.GetByUrlAsync($"{url}");
            await _cmsRepo.GetByUrlAsync($"{url}");
            ViewBag.CmsData = cmsData ?? new TblCmsDto
            {
                Desc1 = "",
                Desc2 = "",
                Desc3 = "",
                PageName = "Search Tutor"
            };

            // Normalize function
            static string NormalizeSlug(string input) =>
                input?.Trim().ToLower().Replace(" ", "-");

            // Slug-to-ID resolution
            if (SubjID == null && !string.IsNullOrWhiteSpace(subjectSlug))
            {
                var normalized = NormalizeSlug(subjectSlug);

                var subjects = await _context.TblDefSubjects.ToListAsync(); // now in-memory

                var subject = subjects.FirstOrDefault(s => normalized.Contains(NormalizeSlug(s.SubjectName)));

                SubjID = subject?.SubjectId;
            }

            if (LevelID == null && !string.IsNullOrWhiteSpace(educationLevelSlug))
            {
                var normalized = NormalizeSlug(educationLevelSlug);

                var levels = await _context.TblEducationLevels.ToListAsync(); // materialize to memory

                var level = levels.FirstOrDefault(e => normalized.Contains(NormalizeSlug(e.EducationLevelName)));

                LevelID = level?.EducationLevelId;
            }

            if (CityID == null && !string.IsNullOrWhiteSpace(citySlug))
            {
                var normalized = NormalizeSlug(citySlug);

                var cities = await _context.TblDefCities.ToListAsync(); // materialize to memory

                var city = cities.FirstOrDefault(c => normalized.Contains(NormalizeSlug(c.CityName)));

                CityID = city?.CityId;
            }

            var model = new TutorSearchFormViewModel
            {
                SubjectId = SubjID,
                CityId = CityID,
                EducationLevelId = LevelID
            };

            // Build main query with filters
            var baseQuery = from m in _context.TblDefMemberInfo2s
                            join t in _context.TblTltutors on m.MemberId equals t.MemberId
                            join c in _context.TblDefCities on m.CityId equals c.CityId
                            join ts in _context.TblTutorSubjects on (int)t.TlTutorId equals ts.TlTutorId into tsJoin
                            from ts in tsJoin.DefaultIfEmpty()
                            join tl in _context.TblTutorLevels on (int)t.TlTutorId equals tl.TlTutorId into tlJoin
                            from tl in tlJoin.DefaultIfEmpty()
                            select new
                            {
                                m,
                                t,
                                c,
                                ts,
                                tl
                            };

            if (model.CityId is > 0)
                baseQuery = baseQuery.Where(x => x.m.CityId == model.CityId);

            if (model.SubjectId is > 0)
                baseQuery = baseQuery.Where(x => x.ts.SubjectId == model.SubjectId);

            if (model.EducationLevelId is > 0)
                baseQuery = baseQuery.Where(x => x.tl.EducationLevelId == model.EducationLevelId);

            var tutorsRaw = await baseQuery
                .GroupBy(x => x.t.TlTutorId)
                .Select(g => g.FirstOrDefault())
                .Take(10)
                .ToListAsync();

            var tutorIds = tutorsRaw.Select(x => x.t.TlTutorId).ToList();

            var tutorSubjectsMap = await _context.TblTutorSubjects
                .Where(ts => tutorIds.Contains((int)ts.TlTutorId))
                .Join(_context.TblDefSubjects,
                      ts => ts.SubjectId,
                      s => s.SubjectId,
                      (ts, s) => new { ts.TlTutorId, s.SubjectName })
                .GroupBy(x => x.TlTutorId)
                .ToDictionaryAsync(g => g.Key, g => g.Select(x => x.SubjectName).Take(4).ToList());

            var tutors = tutorsRaw.Select(x => new TutorCardViewModel
            {
                TutorId = x.t.TlTutorId,
                MemberId = x.m.MemberId,
                TutorName = x.m.MemberName,
                CityName = x.c.CityName,
                ProfileImage = string.IsNullOrEmpty(x.m.ImageName)
                    ? ((x.m.Gender ?? false)
                        ? "https://cdn.ilmkidunya.com/images/noimagemale.jpg"
                        : "https://images.ishallwin.com/tutors/noimagefemale.jpg")
                    : "https://cdn.ilmkidunya.com//Membership/Thumb/" + x.m.ImageName,
                Gender = x.m.Gender ?? true,
                TeachingSummary = x.t.AboutTutoring,
                Experience = x.t.Experience,
                Availability = x.t.Availability,
                CourseName = x.t.CourseName,
                TutoringOptions = x.t.TutoringOptions,
                Subjects = tutorSubjectsMap.ContainsKey((int)x.t.TlTutorId)
                    ? tutorSubjectsMap[(int)x.t.TlTutorId]
                    : new List<string>()
            }).ToList();

            ViewBag.SubjectId = SubjID;
            ViewBag.CityId = CityID;
            ViewBag.LevelId = LevelID;

            return View("TutorResults", tutors);
        }




        [HttpGet("LoadMore")]
        public IActionResult LoadMore(int? subjId, int? cityId, int? levelId, int skip = 0)
        {
            var filteredTutors = _context.TblTltutors
                .Join(_context.TblDefMemberInfo2s,
                    t => t.MemberId,
                    m => m.MemberId,
                    (t, m) => new { t, m })
                .Join(_context.TblDefCities,
                    tm => tm.m.CityId,
                    c => c.CityId,
                    (tm, c) => new { tm.t, tm.m, c })
                .Where(x => cityId == null || x.m.CityId == cityId) // filter by city

                // Only filter subjects/levels if provided
                .Where(x =>
                    subjId == null || _context.TblTutorSubjects.Any(ts =>
                        ts.TlTutorId == x.t.TlTutorId && ts.SubjectId == subjId))
                .Where(x =>
                    levelId == null || _context.TblTutorLevels.Any(tl =>
                        tl.TlTutorId == x.t.TlTutorId && tl.EducationLevelId == levelId))
                .Distinct()
                .Skip(skip)
                .Take(10)
                .ToList();

            // Final projection
            var tutors = filteredTutors.Select(x => new TutorCardViewModel
            {
                TutorId = x.t.TlTutorId,
                MemberId = x.m.MemberId,
                TutorName = x.m.MemberName,
                CityName = x.c.CityName,
                ProfileImage = string.IsNullOrEmpty(x.m.ImageName)
                    ? ((x.m.Gender ?? false)
                        ? "https://cdn.ilmkidunya.com/images/noimagemale.jpg"
                        : "https://images.ishallwin.com/tutors/noimagefemale.jpg")
                    : "https://cdn.ilmkidunya.com//Membership/Thumb/" + x.m.ImageName,
                Gender = x.m.Gender ?? true,
                TeachingSummary = x.t.AboutTutoring,
                Experience = x.t.Experience,
                Availability = x.t.Availability,
                CourseName = x.t.CourseName,
                TutoringOptions = x.t.TutoringOptions,
                Subjects = _context.TblTutorSubjects
                           .Where(ts => ts.TlTutorId == x.t.TlTutorId)
                           .Join(_context.TblDefSubjects,
                                 ts => ts.SubjectId,
                                 s => s.SubjectId,
                                 (ts, s) => s.SubjectName)
                           .Take(3)
                           .ToList()
            }).ToList();

            return PartialView("_TutorListPartial", tutors);
        }



        [HttpGet("{slug}-home-tutor-{memberId:int}.aspx")]
        public async Task<IActionResult> TutorDetail(string slug, int memberId)

        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            var tutorData = (from t in _context.TblTltutors
                             join m in _context.TblDefMemberInfo2s on t.MemberId equals m.MemberId
                             join c in _context.TblDefCities on m.CityId equals c.CityId
                             where m.MemberId == memberId
                             select new
                             {
                                 Tutor = t,
                                 Member = m,
                                 City = c
                             }).FirstOrDefault();

            if (tutorData == null)
                return NotFound();

            ViewBag.CityId = tutorData.City.CityId;

            var subjectList = await _context.TblTutorSubjects
                .Where(ts => ts.TlTutorId == tutorData.Tutor.TlTutorId)
                .Join(_context.TblDefSubjects,
                      ts => ts.SubjectId,
                      s => s.SubjectId,
                      (ts, s) => s.SubjectName)
                .ToListAsync();

            var levelList = await _context.TblTutorLevels
                .Where(tl => tl.TlTutorId == tutorData.Tutor.TlTutorId)
                .Join(_context.TblEducationLevels,
                      tl => tl.EducationLevelId,
                      l => l.EducationLevelId,
                      (tl, l) => l.EducationLevelName)
                .ToListAsync();

            var viewModel = new TutorDetailViewModel
            {
                TutorId = (int)tutorData.Tutor.TlTutorId,
                MemberId = (int)tutorData.Member.MemberId,
                Email = tutorData.Member.Email,
                TutorName = tutorData.Member.MemberName,
                Gender = tutorData.Member.Gender ?? true,
                ProfileImage = string.IsNullOrEmpty(tutorData.Member.ImageName)
                    ? ((tutorData.Member.Gender ?? false)
                        ? "https://cdn.ilmkidunya.com/images/noimagemale.jpg"
                        : "https://images.ishallwin.com/tutors/noimagefemale.jpg")
                    : "https://cdn.ilmkidunya.com//Membership/Thumb/" + tutorData.Member.ImageName,
                CityName = tutorData.City.CityName,
                Experience = tutorData.Tutor.Experience,
                Availability = tutorData.Tutor.Availability,
                CourseName = tutorData.Tutor.CourseName != null ? tutorData.Tutor.CourseName : "No",
                TeachingSummary = tutorData.Tutor.AboutTutoring,
                TutoringOptions = tutorData.Tutor.TutoringOptions,
                Subjects = subjectList,
                EducationLevels = levelList
            };

            // Get similar tutors in the same city (excluding the current tutor)
            var similarTutors = (from t in _context.TblTltutors
                                 join m in _context.TblDefMemberInfo2s on t.MemberId equals m.MemberId
                                 join c in _context.TblDefCities on m.CityId equals c.CityId
                                 where m.CityId == tutorData.Member.CityId &&
                                       t.TlTutorId != tutorData.Tutor.TlTutorId
                                 select new TutorCardViewModel
                                 {
                                     TutorId = t.TlTutorId,
                                     MemberId = m.MemberId,
                                     TutorName = m.MemberName,
                                     CityName = c.CityName,
                                     ProfileImage = string.IsNullOrEmpty(m.ImageName)
                                        ? ((m.Gender ?? false)
                                            ? "https://cdn.ilmkidunya.com/images/noimagemale.jpg"
                                            : "https://images.ishallwin.com/tutors/noimagefemale.jpg")
                                        : "https://cdn.ilmkidunya.com//Membership/Thumb/" + m.ImageName,
                                     Gender = m.Gender ?? true,
                                     TeachingSummary = t.AboutTutoring,
                                     Experience = t.Experience,
                                     Availability = t.Availability,
                                     CourseName = t.CourseName,
                                     TutoringOptions = t.TutoringOptions,
                                     Subjects = _context.TblTutorSubjects
                                         .Where(ts => ts.TlTutorId == t.TlTutorId)
                                         .Join(_context.TblDefSubjects,
                                               ts => ts.SubjectId,
                                               s => s.SubjectId,
                                               (ts, s) => s.SubjectName)
                                         .Take(3)
                                         .ToList()
                                 }).Take(4).ToList();

            viewModel.SimilarTutors = similarTutors;

            return View("TutorDetail", viewModel);
        }


        [HttpGet("tutor-inquiry.aspx")]

        public IActionResult TutorInquiry()
        {
            // Get active cities
            var cities = _context.TblDefCities
                .Where(c => c.IsActive == true)
                .OrderBy(c => c.CityName)
                .Select(c => new SelectListItem
                {
                    Value = c.CityId.ToString(),
                    Text = c.CityName
                })
                .ToList();

            // Get subjects
            var subjects = _context.TblDefSubjects
                .OrderBy(s => s.SubjectName)
                .Select(s => new SelectListItem
                {
                    Value = s.SubjectId.ToString(),
                    Text = s.SubjectName
                })
                .ToList();

            var levels = _context.TblEducationLevels
                .OrderBy(l => l.SortOrder)
                .Select(l => new SelectListItem
                {
                    Value = l.EducationLevelId.ToString(),
                    Text = l.EducationLevelName
                }).ToList();
            ViewBag.ShowPassword = false;
            ViewBag.CityList = new SelectList(cities, "Value", "Text");
            ViewBag.SubjectList = new SelectList(subjects, "Value", "Text");
            ViewBag.LevelList = new SelectList(levels, "Value", "Text");

            return View(new RegisterViewModel());
        }


        [HttpPost("tutor-inquiry.aspx")]
        public async Task<IActionResult> TutorInquiry(RegisterViewModel model)
        {

            var cities = _context.TblDefCities
    .Where(c => c.IsActive == true)
    .OrderBy(c => c.CityName)
    .Select(c => new SelectListItem
    {
        Value = c.CityId.ToString(),
        Text = c.CityName
    })
    .ToList();

            // Get subjects
            var subjects = _context.TblDefSubjects
                .OrderBy(s => s.SubjectName)
                .Select(s => new SelectListItem
                {
                    Value = s.SubjectId.ToString(),
                    Text = s.SubjectName
                })
                .ToList();

            var levels = _context.TblEducationLevels
                .OrderBy(l => l.SortOrder)
                .Select(l => new SelectListItem
                {
                    Value = l.EducationLevelId.ToString(),
                    Text = l.EducationLevelName
                }).ToList();
            ViewBag.CityList = new SelectList(cities, "Value", "Text");
            ViewBag.SubjectList = new SelectList(subjects, "Value", "Text");
            ViewBag.LevelList = new SelectList(levels, "Value", "Text");
            ModelState.Remove("Password");

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var memberIdCookie = Request.Cookies["memberid"];
                TblDefMemberInfo2 member = null;

                if (!string.IsNullOrEmpty(memberIdCookie))
                {
                    int memberId = Convert.ToInt32(memberIdCookie);
                    member = await _context.TblDefMemberInfo2s.FirstOrDefaultAsync(m => m.MemberId == memberId);
                }
                else if (!string.IsNullOrEmpty(model.Email))
                {
                    member = await _context.TblDefMemberInfo2s.FirstOrDefaultAsync(m => m.Email == model.Email);
                }

                // ✅ If user did not provide a password AND is not already registered
                if (string.IsNullOrWhiteSpace(model.Password) && member == null)
                {
                    ModelState.AddModelError("Password", "Password is required for new users.");
                    ViewBag.ShowPassword = true; // Optional: use this flag in view to unhide password
                    return View(model);
                }

                // ✅ If member already exists
                if (member != null)
                {
                    await SaveInquiryAsync((int)member.MemberId, model);
                    Response.Cookies.Append("ThankYou", "TutorInquiry");
                    return RedirectToAction("ThankYou", "Tutors");
                }

                // ✅ New member registration
                var fullName = model.Name.Trim();
                var nameParts = fullName.Contains(" ") ? fullName.Split(' ', 2) : new[] { fullName, fullName };

                var newMember = new TblDefMemberInfo2
                {
                    MemberName = fullName,
                    MemberFirstName = nameParts[0],
                    MemberLastName = nameParts[1],
                    Password = Rc4Encryption.Encrypt(model.Email, model.Password),
                    Email = model.Email,
                    CityId = model.CityId,
                    MobileNumber = model.PhoneNumber,
                    Gender = true,
                    Dated = DateTime.Now,
                    MemberSource = "ilmkidunya",
                    MemberSourceId = 1,
                    CountryId = 1,
                    QualificationId = 6,
                    Rights = 0,
                    JoinRewards = false,
                    MemberTypeId = 1
                };

                _context.TblDefMemberInfo2s.Add(newMember);
                await _context.SaveChangesAsync();

                // Generate rewrite URL
                string urlName = Regex.Replace(newMember.MemberName.ToLower(), @"[^\w@-]", " ");
                urlName = Regex.Replace(urlName, @"\s{1,}", "-");
                newMember.RewriteUrl = $"{urlName}-{newMember.MemberId}";
                await _context.SaveChangesAsync();

                // Set cookies
                Response.Cookies.Append("memberid", newMember.MemberId.ToString());
                Response.Cookies.Append("member_name", newMember.MemberName);

                // Send welcome email
                //string template = EmailHelper.LoadTemplate("register.htm");
                //template = template.Replace("{#email#}", model.Email)
                //                   .Replace("{#pass#}", model.Password)
                //                   .Replace("{#date#}", DateTime.Now.ToString("dd/MM/yyyy"));

                //await EmailHelper.SendAsync(model.Email, "ilmkidunya.com Registration Notification !", template, "Register", "Registration");

                await SaveInquiryAsync((int)newMember.MemberId, model);

                Response.Cookies.Append("ThankYou", "TutorInquiry");
                return RedirectToAction("ThankYou", "Tutors");
            }
            catch (Exception ex)
            {
                // TODO: Add proper logging
                throw;
            }
        }

        [HttpPost("contactTutor")]
        public async Task<IActionResult> ContactTutor([FromBody] TutorContactViewModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Url))
                return BadRequest();

            // Extract member ID from URL
            int memberId = 0;
            try
            {
                memberId = Convert.ToInt32(model.Url.Replace(".aspx", "").Split('-').Last());
            }
            catch
            {
                return BadRequest("Invalid Member ID");
            }

            // Save to database
            var contact = new TutorContactForm
            {
                MemberId = memberId,
                Name = model.Name,
                Contact = model.Contact,
                Subjects = model.Subject,
                Dated = DateTime.UtcNow.AddHours(5)
            };

            _context.TutorContactForms.Add(contact);
            await _context.SaveChangesAsync();

            // Prepare and send email using EmailHelper
            try
            {
                //string emailHtml = EmailHelper.LoadTemplate("tutors.html", "/Html/newemailtemplate/");
                //emailHtml = emailHtml.Replace("{#Name#}", model.TutorName);
                //emailHtml = emailHtml.Replace("{#PersonName#}", model.Name);
                //emailHtml = emailHtml.Replace("{#Contact#}", model.Contact);
                //emailHtml = emailHtml.Replace("{#Subjects#}", model.Subject);
                //emailHtml = emailHtml.Replace("{#Link#}", $"<a href='{model.Url}'>Click Here</a>");

                string to = "contact@instutor.com";
                string cc = model.TutorEmail;
                string subject = "Tutor Contacted From ilmkidunya.com";

                //await EmailHelper.SendAsync(
                //    toEmail: to,
                //    subject: subject,
                //    bodyHtml: emailHtml,
                //    pageName: "TutorContact",
                //    purpose: "Tutor Inquiry",
                //    cc: string.IsNullOrWhiteSpace(cc) ? null : new List<string> { cc }
                //);
            }
            catch (Exception ex)
            {
                // Log error (optional)
                return StatusCode(500, "Email failed to send.");
            }

            return Ok(new { message = "Contact saved and email sent successfully." });
        }



        private async Task<int> SaveInquiryAsync(int memberId, RegisterViewModel model)
        {
            var inquiry = new TutorInquiry
            {
                MemberId = memberId,
                Name = model.Name,
                CityId = model.CityId,
                Email = model.Email,
                Contact = model.PhoneNumber,
                Subjects = string.Join(",", model.SelectedSubjectIds),
                LevelId = model.EducationLevelId,
                TutoringOptions = string.Join(",", model.SelectedTutoringOptionIds),
                Date = DateTime.Now,
                Detail = model.Detail,
                IsShared = model.IsShared,
                RequestType = model.RequestType
            };

            _context.TutorInquiries.Add(inquiry);
            await _context.SaveChangesAsync();

            //await SendTutorNotificationEmailAsync(inquiry);

            return inquiry.Id;
        }

        private async Task SendTutorNotificationEmailAsync(TutorInquiry inquiry)
        {
            var tutorEmails = await (from t in _context.TblTltutors
                                     join m in _context.TblDefMemberInfo2s on t.MemberId equals m.MemberId
                                     where _context.TblTutorLevels.Any(l => l.TlTutorId == t.TlTutorId && l.EducationLevelId == inquiry.LevelId)
                                        || _context.TblTutorSubjects.Any(s => s.TlTutorId == t.TlTutorId && inquiry.Subjects.Contains(s.SubjectId.ToString()))
                                     where m.CityId == inquiry.CityId
                                     select m.Email).Distinct().ToListAsync();

            if (tutorEmails.Any())
            {
                string template = EmailHelper.LoadTemplate("/tutors/Html/EmailTutor.htm");
                template = template.Replace("{#Name#}", inquiry.Name)
                                   .Replace("{#Email#}", inquiry.Email)
                                   .Replace("{#Mobile#}", inquiry.Contact)
                                   .Replace("{#City#}", _context.TblDefCities.Find(inquiry.CityId)?.CityName)
                                   .Replace("{#Subjects#}", inquiry.Subjects) // Replace with names if needed
                                   .Replace("{#Level#}", _context.TblEducationLevels.Find(inquiry.LevelId)?.EducationLevelName)
                                   .Replace("{#TutoringOpt#}", inquiry.TutoringOptions) // Replace with names if needed
                                   .Replace("{#Detail#}", inquiry.Detail);

                await EmailHelper.SendMassEmailAsync(tutorEmails, "Home Tutor Required!", template);
            }
        }
        public static int LevenshteinDistance(string s, string t)
        {
            if (string.IsNullOrEmpty(s)) return t?.Length ?? 0;
            if (string.IsNullOrEmpty(t)) return s.Length;

            int[,] d = new int[s.Length + 1, t.Length + 1];

            for (int i = 0; i <= s.Length; i++) d[i, 0] = i;
            for (int j = 0; j <= t.Length; j++) d[0, j] = j;

            for (int i = 1; i <= s.Length; i++)
            {
                for (int j = 1; j <= t.Length; j++)
                {
                    int cost = s[i - 1] == t[j - 1] ? 0 : 1;

                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            return d[s.Length, t.Length];
        }

        private static string GetSubjectImageUrl(string subjectName)
        {
            if (string.IsNullOrWhiteSpace(subjectName))
                return "https://images.ishallwin.com/ot-images/default.webp";

            var knownImages = new Dictionary<string, string>
    {
        { "english", "eng" },
        { "computer science", "computer-science" },
        { "chemistry", "chemistry" },
        { "biology", "biology" },
        { "mathematics", "mathematics" },
        { "general science", "science" },
        { "general maths", "maths" },
        { "physics", "physics" },
        { "pak studies", "pak-studies" },
        { "islamiat", "islamiat" },
        { "home economics", "home-economics" },
        { "civics", "civics" },
        { "education", "education" },
        { "economics", "economics" }
    };

            subjectName = subjectName.ToLower().Trim();

            foreach (var kvp in knownImages)
            {
                int maxLen = Math.Max(subjectName.Length, kvp.Key.Length);
                int dist = LevenshteinDistance(subjectName, kvp.Key);
                double similarity = 1.0 - (double)dist / maxLen;

                if (similarity >= 0.8)
                    return $"https://images.ishallwin.com/ot-images/{kvp.Value}.webp";
            }

            var slug = subjectName.Replace(" ", "-").ToLower();
            return $"https://images.ishallwin.com/ot-images/{slug}.webp";
        }

        public static class Rc4Encryption
        {
            public static string Encrypt(string key, string data)
            {
                var S = new byte[256];
                var K = new byte[256];
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);
                byte[] keyBytes = Encoding.UTF8.GetBytes(key);

                for (int i = 0; i < 256; i++)
                {
                    S[i] = (byte)i;
                    K[i] = keyBytes[i % keyBytes.Length];
                }

                int j = 0;
                for (int i = 0; i < 256; i++)
                {
                    j = (j + S[i] + K[i]) % 256;
                    (S[i], S[j]) = (S[j], S[i]);
                }

                int iIndex = 0;
                j = 0;
                byte[] result = new byte[dataBytes.Length];

                for (int x = 0; x < dataBytes.Length; x++)
                {
                    iIndex = (iIndex + 1) % 256;
                    j = (j + S[iIndex]) % 256;
                    (S[iIndex], S[j]) = (S[j], S[iIndex]);
                    int t = (S[iIndex] + S[j]) % 256;
                    result[x] = (byte)(dataBytes[x] ^ S[t]);
                }

                return Convert.ToBase64String(result);
            }

            public static string Decrypt(string key, string encryptedBase64)
            {
                byte[] encryptedBytes = Convert.FromBase64String(encryptedBase64);
                string decrypted = Encoding.UTF8.GetString(EncryptToBytes(key, encryptedBytes));
                return decrypted;
            }

            private static byte[] EncryptToBytes(string key, byte[] data)
            {
                var S = new byte[256];
                var K = new byte[256];

                byte[] keyBytes = Encoding.UTF8.GetBytes(key);

                for (int i = 0; i < 256; i++)
                {
                    S[i] = (byte)i;
                    K[i] = keyBytes[i % keyBytes.Length];
                }

                int j = 0;
                for (int i = 0; i < 256; i++)
                {
                    j = (j + S[i] + K[i]) % 256;
                    (S[i], S[j]) = (S[j], S[i]);
                }

                int iIndex = 0;
                j = 0;
                byte[] result = new byte[data.Length];

                for (int x = 0; x < data.Length; x++)
                {
                    iIndex = (iIndex + 1) % 256;
                    j = (j + S[iIndex]) % 256;
                    (S[iIndex], S[j]) = (S[j], S[iIndex]);
                    int t = (S[iIndex] + S[j]) % 256;
                    result[x] = (byte)(data[x] ^ S[t]);
                }

                return result;
            }
        }


    }


    public class ExcludeWordsConstraint : IRouteConstraint
    {
        private static readonly string[] ForbiddenWords = { "-in-", "-class-" };

        public bool Match(HttpContext httpContext, IRouter route, string parameterName, RouteValueDictionary values,
                          RouteDirection routeDirection)
        {
            if (values.TryGetValue(parameterName, out var value) && value != null)
            {
                var slug = value.ToString().ToLower();
                return !ForbiddenWords.Any(w => slug.Contains(w));
            }
            return false;
        }
    }



}
