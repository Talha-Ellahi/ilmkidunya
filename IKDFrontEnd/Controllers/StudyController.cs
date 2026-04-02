using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using IKDFrontEnd.Models;
using IKDFrontEnd.ViewModels;
using IKDFrontEnd.Services;
using IKDFrontEnd.ViewModels.Common;

namespace IKDFrontEnd.Controllers
{
    public class StudyController : Controller
    {
        private readonly DbikdContext _context;
        private readonly BannerService _bannerService;
        private readonly CmsRepository _cmsRepo;
        private readonly IDistributedCache _distributedCache;

        public StudyController(
            DbikdContext context,
            BannerService bannerService,
            CmsRepository cmsRepo,
            IDistributedCache distributedCache)  // Added distributed cache parameter
        {
            _context = context;
            _bannerService = bannerService;
            _cmsRepo = cmsRepo;
            _distributedCache = distributedCache;
        }

        [Route("study")]
        public async Task<IActionResult> Index()
        {
            string cacheKey = "study_all_classes";
            List<LectureClassViewModel> allClasses = null;

            // Try to get from Redis cache
            try
            {
                var cachedData = await _distributedCache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cachedData))
                {
                    allClasses = JsonSerializer.Deserialize<List<LectureClassViewModel>>(cachedData);

                    // Optional: add debug info
                    // ViewBag.CacheSource = "Redis";
                }
            }
            catch
            {
                // If Redis fails, just continue to database
            }

            // If not in cache, get from database
            if (allClasses == null)
            {
                allClasses = await _context.TblLectureClasses
                    .OrderBy(c => c.SortOrder)
                    .Select(c => new LectureClassViewModel
                    {
                        Id = c.Id,
                        ClassName = c.ClassName,
                        ClassUrl = c.ClassUrl
                    })
                    .ToListAsync();

                // Save to Redis cache
                try
                {
                    await _distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(allClasses), new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) // Cache for 1 hour
                    });

                    // Optional: add debug info
                    // ViewBag.CacheSource = "Database";
                }
                catch
                {
                    // If Redis fails, just continue
                }
            }

            // Get banners (unchanged)
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            return View(allClasses);
        }


        [Route("study/{classSlug}.aspx")]
        public async Task<IActionResult> ClassDetails(string classSlug)
        {
            string modifiedClassSlug = classSlug;


            if (modifiedClassSlug.Contains("mdcat-test"))
            {
                modifiedClassSlug = modifiedClassSlug.Replace("mdcat-test", "mdcat-class");
            }
            else if (modifiedClassSlug.Contains("ecat-test"))
            {
                modifiedClassSlug = modifiedClassSlug.Replace("ecat-test", "ecat-class");
            }


            string subjectSlug = null;
            string classSlugWithoutSubject = modifiedClassSlug;


            if (classSlug.Contains('-'))
            {
                var splitSlug = modifiedClassSlug.Split('-');
                subjectSlug = splitSlug.LastOrDefault();
                classSlugWithoutSubject = string.Join("-", splitSlug.Take(splitSlug.Length - 1));
            }


            if (classSlugWithoutSubject.Contains("class") && !classSlugWithoutSubject.EndsWith("-class"))
            {
                classSlugWithoutSubject = classSlugWithoutSubject.Substring(0, classSlugWithoutSubject.LastIndexOf("-class") + 6);
                subjectSlug = modifiedClassSlug.Substring(modifiedClassSlug.LastIndexOf("-class") + 6);
                subjectSlug = subjectSlug.StartsWith("-") ? subjectSlug.Substring(1) : subjectSlug;
            }
            else if (!classSlugWithoutSubject.EndsWith("-class"))
            {
                classSlugWithoutSubject = modifiedClassSlug;
            }

            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;
            var lectureClass = await _context.TblLectureClasses
                .Include(c => c.TblLectureSubjects.Where(s => s.IsDelete != true).OrderBy(s => s.SortOrder))
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ClassUrl == classSlugWithoutSubject);

            if (lectureClass == null)
                return NotFound();


            if (!string.IsNullOrEmpty(modifiedClassSlug))
            {
                modifiedClassSlug += ".aspx";
                if (modifiedClassSlug.Contains("mdcat-class") && subjectSlug.Contains("class"))
                {
                    modifiedClassSlug = modifiedClassSlug.Replace("mdcat-class", "mdcat-test");
                }
                else if (modifiedClassSlug.Contains("ecat-class"))
                {
                    modifiedClassSlug = modifiedClassSlug.Replace("ecat-class", "ecat-test");
                }
            }


            var cmsData = await _cmsRepo.GetByUrlAsync($"/study/{modifiedClassSlug}");
            cmsData = cmsData ?? new TblCmsDto();


            if (!string.IsNullOrEmpty(subjectSlug) && subjectSlug != "class")
            {
                var subjectModel = await _context.TblLectureSubjects
                    .Where(s => s.IsDelete == false && s.Name.Replace(" ", "-").ToLower() == subjectSlug.ToLower() && s.LectureClassId == lectureClass.Id)
                    .Select(s => new LecturePageViewModel
                    {
                        PageHeading = cmsData.Heading,
                        PageDescription1 = cmsData.Desc1,
                        PageDescription2 = cmsData.Desc2,
                        MetaDescription = cmsData.MetaDesc,
                        PageTitle2 = s.Name,
                        ClassName = lectureClass.ClassUrl,
                        CurrentClassSlug = lectureClass.ClassUrl,
                        CurrentSubjectSlug = subjectSlug,
                        CurrentClassId = s.LectureClassId,
                        CurrentSubjectId = s.Id,
                        SubjectImage = s.Image,
                        LectureChapters = s.TblLectureChapters.Where(c => c.IsDelete == false).OrderBy(c => c.ChNumber).ToList(),
                        Teachers = s.TblLectures
                            .Where(l => l.LectureTeacher != null)
                            .GroupBy(l => l.LectureTeacher.Id)
                            .Select(g => g.FirstOrDefault().LectureTeacher)
                            .ToList(),
                        MetaTitle = cmsData.MetaTitle,
                        MetaKeywords = cmsData.MetaKeys,
                        MetaImage = cmsData.Image,
                    })
                    .FirstOrDefaultAsync();

                if (subjectModel == null)
                {
                    return NotFound();
                }

                return View("SubjectDetails", subjectModel);
            }


            var classModel = new LecturePageViewModel
            {
                PageHeading = cmsData.Heading,
                PageDescription1 = cmsData.Desc1,
                PageDescription2 = cmsData.Desc2,
                MetaDescription = cmsData.MetaDesc,
                PageTitle2 = lectureClass.ClassUrl,
                CurrentClassSlug = lectureClass.ClassUrl,
                CurrentClassId = lectureClass.Id,
                LectureSubjects = lectureClass.TblLectureSubjects.ToList(),
                MetaTitle = cmsData.MetaTitle,
                MetaKeywords = cmsData.MetaKeys,
                MetaImage = cmsData.Image,

            };

            return View("ClassDetails", classModel);
        }



        public async Task<IActionResult> SubjectDetails()
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;
            return View();
        }




        [Route("study/{classSlugSubjectSlug}/{chapterSlug}.aspx")]
        public async Task<IActionResult> ChapterDetails(string classSlugSubjectSlug, string chapterSlug)
        {
            string classSlug = "";
            string subjectSlug = "";

            if (classSlugSubjectSlug.Contains("-class"))
            {
                var parts = classSlugSubjectSlug.Split(new[] { "-class-" }, StringSplitOptions.None);
                classSlug = parts[0] + "-class";
                subjectSlug = parts.Length > 1 ? parts[1] : "";
            }
            else if (classSlugSubjectSlug.Contains("-test"))
            {
                var parts = classSlugSubjectSlug.Split(new[] { "-test-" }, StringSplitOptions.None);
                classSlug = parts[0] + "-test";
                subjectSlug = parts.Length > 1 ? parts[1] : "";
            }
            else
            {
                return NotFound();
            }

            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            var lectureClass = await _context.TblLectureClasses
                .Include(c => c.TblLectureSubjects)
                .FirstOrDefaultAsync(c => c.ClassUrl == classSlug);

            if (lectureClass == null)
                return NotFound();

            var subject = await _context.TblLectureSubjects
                .FirstOrDefaultAsync(s =>
                    s.Name.Replace(" ", "-").ToLower() == subjectSlug.ToLower() &&
                    s.LectureClassId == lectureClass.Id);

            if (subject == null)
                return NotFound();

            var chapter = await _context.TblLectureChapters
                .Include(ch => ch.TblLectures
                    .Where(l => l.LectureTopic.IsActive)  // Filter for active topics
                    )
                    .ThenInclude(l => l.LectureTopic)
                .Include(ch => ch.TblLectures
                    .Where(l => l.LectureTopic.IsActive)  // Apply same filter here
                    )
                    .ThenInclude(l => l.LectureTeacher)
                .FirstOrDefaultAsync(ch =>
                    (ch.IsDelete == false || ch.IsDelete == null) &&
                    ch.ChName.Replace(" ", "-").ToLower() == chapterSlug.ToLower() &&
                    ch.LectureSubjectId == subject.Id);


            if (chapter == null)
                return NotFound();

            // ✅ Fetch related chapters from the same subject
            var relatedChapters = await _context.TblLectureChapters
                .Where(ch => ch.LectureSubjectId == subject.Id && ch.IsDelete == false)
                .Select(ch => new
                {
                    ch.Id,
                    ch.ChName,

                })
                .ToListAsync();


            classSlug = classSlug + "-" + subjectSlug + "/" + chapterSlug + ".aspx";

            var Cmsdata = await _cmsRepo.GetByUrlAsync($"/study/{classSlug}");
            if (Cmsdata == null)
            {
                Cmsdata = new TblCmsDto();
            }

            var model = new LecturePageViewModel
            {
                PageTitle2 = chapter.ChName,
                PageHeading = Cmsdata.Heading,
                PageDescription1 = Cmsdata.Desc1,
                PageDescription2 = Cmsdata.Desc2,
                PageDescription3 = Cmsdata.Desc3,
                MetaDescription = Cmsdata.MetaDesc,
                ClassName = lectureClass.ClassName,
                ChapterName = chapterSlug,
                CurrentClassSlug = lectureClass.ClassUrl,
                CurrentSubjectSlug = subjectSlug,
                CurrentClassId = lectureClass.Id,
                CurrentSubjectId = subject.Id,
                CurrentChapterId = chapter.Id,
                Lectures = chapter.TblLectures.ToList(),
                LectureChapters = new List<TblLectureChapter> { chapter },
                Topics = chapter.TblLectureTopics.ToList(),
                Teachers = chapter.TblLectures.Select(l => l.LectureTeacher).Distinct().ToList(),
                MetaTitle = Cmsdata.MetaTitle,
                MetaKeywords = Cmsdata.MetaKeys,
                MetaImage = Cmsdata.Image,
                Chtitle = chapter.Chtitle,
                Metadesc = Cmsdata.MetaDesc,

                RelatedChapters = relatedChapters
                    .Select(rc => new TblLectureChapter
                    {
                        Id = rc.Id,
                        ChName = rc.ChName,

                    })
                    .ToList()
            };

            return View(model);
        }




    }
}
