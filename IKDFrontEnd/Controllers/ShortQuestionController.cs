using DinkToPdf;
using DinkToPdf.Contracts;
using IKDFrontEnd.Models;
using IKDFrontEnd.Services;
using IKDFrontEnd.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Linq;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace IKDFrontEnd.Controllers
{
    [Route("short-questions")]
    public class ShortQuestionController : Controller
    {

        private readonly DbikdContext _context;
        private readonly BannerService _bannerService;
        private readonly CmsRepository _cmsRepo;
        public ShortQuestionController(DbikdContext context, BannerService bannerService, CmsRepository cmsRepo)
        {
            _context = context;
            _bannerService = bannerService;
            QuestPDF.Settings.License = LicenseType.Community;
            _cmsRepo = cmsRepo;
        }

        [HttpGet("")]
        public async Task<IActionResult> Home()
        {             // === Banners ===
            ViewBag.Banners = await _bannerService.GetBannersAsync();
            var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/short-questions/");

            ViewBag.CmsData = cmsData;

            return View();
        }

        [Route("{class:int}th-class-online-preparation.aspx")]
        [Route("{class:int}th-class.aspx")]
        [Route("{inter:alpha}-part{part:int}-online-preparation.aspx")]
        [Route("{uni:alpha}-university-vu-online-preparation.aspx")]
        public async Task<IActionResult> Class(int Class, string? inter, int? part, string uni)
        {
            ViewBag.Banners = await _bannerService.GetBannersAsync();
            if (uni != null)
            {
                string uuni = "virtual-university-vu-online-preparation.aspx";
              var cmsDatauni = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/short-questions/virtual-university-vu-online-preparation.aspx");

                ViewBag.CmsData = cmsDatauni;
                return View();

            }
            var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/short-questions/{Class}th-class.aspx");

            if (cmsData == null && inter!=null && part != null)
            {
                 cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/short-questions/{inter}-part{part}-online-preparation.aspx");


            }
            if (cmsData == null)
            {
                cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/short-questions/{Class}th-class-online-preparation.aspx");

                ViewBag.CmsData = cmsData;
                if (Class == 11)
                {
                    ViewBag.Part = "1";
                }
                else
                {
                    ViewBag.Part = "2";
                }
                return View("Inter");
            }
            ViewBag.CmsData = cmsData;
            return View();
        }



        //[Route("{subject}-{Class:int}th-class-{medium}-online-preparation.aspx")]
        //[HttpGet("{subject}-{Class:int}th-class-online-preparation.aspx")]
        //public async Task<IActionResult> Subject(string subject, int Class, string? medium)
        //{
        //    ViewBag.Banners = await _bannerService.GetBannersAsync();
        //    TblCm cmsData = new TblCm();
        //    if (medium != null)
        //    {
        //        cmsData = await _context.TblCms
        //         .Where(c => c.Url == $"https://www.ilmkidunya.com/short-questions/{subject}-{Class}th-class-{medium}-online-preparation.aspx")
        //         .FirstOrDefaultAsync();
        //    }
        //    else
        //    {
        //        cmsData = await _context.TblCms
        //         .Where(c => c.Url == $"https://www.ilmkidunya.com/short-questions/{subject}-{Class}th-class-online-preparation.aspx")
        //         .FirstOrDefaultAsync();
        //    }


        //    ViewBag.CmsData = cmsData;
        //    return View();
        //}


        //[Route("{medium}-{Class:int}th-class-{subject}-chapter-{chapter:int}-preparation")]
        //[Route("{subject}-{Class:int}th-class-{medium}-online-preparation-{type}")]
        //[Route("{Class:int}th-{subject}-chapter-{chapter:int}-preparation")]
        //[Route("{subject}-{Class:int}th-class-online-preparation-{type}")]
        //public async Task<IActionResult> FormTest(string subject, int Class, int? chapter, string? medium, string? type)
        //{
        //    ViewBag.Banners = await _bannerService.GetBannersAsync();
        //    TblCm cmsData = new TblCm();
        //    PreparationSQ preparationSQ = new PreparationSQ();

        //    if (chapter != null && string.IsNullOrEmpty(medium))
        //    {
        //        // ✅ Chapter-specific URL
        //        string url = $"{Class}th-{subject}-chapter-{chapter}-preparation";

        //        cmsData = await _context.TblCms
        //            .Where(c => c.Url.Contains($"https://www.ilmkidunya.com/short-questions/{Class}th-{subject}-chapter-{chapter}-preparation"))
        //            .FirstOrDefaultAsync();

        //        preparationSQ.Criteria = await _context.ShortQuestionCriteria
        //            .Where(u => u.Url == $"{Class}th-{subject}-chapter-{chapter}-preparation")
        //            .FirstOrDefaultAsync();
        //    }
        //    else if (!string.IsNullOrEmpty(medium) && chapter == null)
        //    {
        //        // ✅ With medium
        //        string url = $"{subject}-{Class}th-class-{medium}-online-preparation";

        //        cmsData = await _context.TblCms
        //            .Where(c => c.Url.Contains($"https://www.ilmkidunya.com/short-questions/{subject}-{Class}th-class-{medium}-online-preparation"))
        //            .FirstOrDefaultAsync();

        //        preparationSQ.Criteria = await _context.ShortQuestionCriteria
        //            .Where(u => u.Url.Contains($"{subject}-{Class}th-class-{medium}-online-preparation"))
        //            .FirstOrDefaultAsync();
        //    }
        //    else if(!string.IsNullOrEmpty(medium) && chapter != null)
        //    {
        //        string url = $"{subject}-{Class}th-class-{medium}-online-preparation";

        //        cmsData = await _context.TblCms
        //            .Where(c => c.Url.Contains($"https://www.ilmkidunya.com/short-questions/{medium}-{Class}th-class-{subject}-chapter-{chapter}-preparation"))
        //            .FirstOrDefaultAsync();

        //        preparationSQ.Criteria = await _context.ShortQuestionCriteria
        //            .Where(u => u.Url.Contains($"{medium}-{Class}th-class-{subject}-chapter-{chapter}-preparation"))
        //            .FirstOrDefaultAsync();
        //    }
        //    else
        //    {
        //        // ✅ Without medium
        //        string url = $"{subject}-{Class}th-class-online-preparation";

        //        cmsData = await _context.TblCms
        //            .Where(c => c.Url.Contains($"https://www.ilmkidunya.com/short-questions/{subject}-{Class}th-class-online-preparation"))
        //            .FirstOrDefaultAsync();

        //        preparationSQ.Criteria = await _context.ShortQuestionCriteria
        //            .Where(u => u.Url == $"{subject}-{Class}th-class-online-preparation")
        //            .FirstOrDefaultAsync();
        //    }

        //    ViewBag.CmsData = cmsData;

        //    if (preparationSQ.Criteria != null)
        //    {
        //        preparationSQ.CriteriaDetail = await _context.ShortQuestionCriteriaDetails
        //            .Where(o => o.OtscriteriaId == preparationSQ.Criteria.Id)
        //            .FirstOrDefaultAsync();

        //        if (preparationSQ.CriteriaDetail != null)
        //        {
        //            preparationSQ.QuestionAnswer = await GetShortQuestionsForCustomCriteria(
        //                preparationSQ.CriteriaDetail.TotalQuestions,
        //                preparationSQ.CriteriaDetail.SubjectId,
        //                preparationSQ.CriteriaDetail.ChapterIds);

        //            preparationSQ.Chapters = await _context.TblChapters
        //                .Where(c => c.SubjectId == preparationSQ.CriteriaDetail.SubjectId && c.IsActive == true)
        //                .Select(c => new ChapterVM
        //                {
        //                    ChapterId = c.ChapterId,
        //                    ChapterName = c.ChapterName
        //                }).ToListAsync();

        //            preparationSQ.SubjectName = await _context.TblSubjects
        //                .Where(s => s.SubjectUrl.Contains(subject))
        //                .Select(s => s.SubjectName)
        //                .FirstOrDefaultAsync();

        //            preparationSQ.ClassName = await _context.TblClasses
        //                .Where(s => s.ClassId == preparationSQ.CriteriaDetail.ClassId)
        //                .Select(s => s.ClassName)
        //                .FirstOrDefaultAsync();

        //            preparationSQ.SubjectUrl = subject;

        //            // ✅ Fetch all related criteria URLs for same class & subject
        //            preparationSQ.ChaptersUrls = await (
        //                from detail in _context.ShortQuestionCriteriaDetails
        //                join criteria in _context.ShortQuestionCriteria
        //                    on detail.OtscriteriaId equals criteria.Id
        //                where detail.ClassId == preparationSQ.CriteriaDetail.ClassId
        //                   && detail.SubjectId == preparationSQ.CriteriaDetail.SubjectId
        //                select criteria.Url
        //            ).ToListAsync();

        //        }
        //    }

        //    return View(preparationSQ);
        //}
        //public async Task<List<QuestionAnswerVM>> GetShortQuestionsForCustomCriteria(
        //    int topQuestions,
        //    short subjectId,
        //    string? chapterIds = null)
        //{
        //    List<int>? chapterIdList = null;
        //    if (!string.IsNullOrEmpty(chapterIds))
        //    {
        //        chapterIdList = chapterIds.Split(',')
        //                                  .Select(id => Convert.ToInt32(id.Trim()))
        //                                  .ToList();
        //    }

        //    var chapterQuery = await _context.TblChapters
        //                                     .Where(c => c.SubjectId == subjectId)
        //                                     .ToListAsync();

        //    if (chapterIdList != null && chapterIdList.Any())
        //    {
        //        chapterQuery = chapterQuery
        //                       .Where(c => chapterIdList.Contains(c.ChapterId))
        //                       .ToList();
        //    }

        //    var validChapterIds = chapterQuery.Select(c => c.ChapterId).ToList();

        //    if (!validChapterIds.Any())
        //        return new List<QuestionAnswerVM>();

        //    var questionQuery = _context.TblShortQuestions
        //                                .Where(q => validChapterIds.Contains(q.ChapterId));

        //    var randomQuestions = questionQuery.OrderBy(q => Guid.NewGuid());

        //    if (topQuestions > 0)
        //    {
        //        randomQuestions = (IOrderedQueryable<TblShortQuestion>)randomQuestions.Take(topQuestions);
        //    }

        //    var questions = await randomQuestions
        //                        .Select(q => new
        //                        {
        //                            q.QuestionId,
        //                            q.QuestionDescription,
        //                            q.ChapterId
        //                        })
        //                        .ToListAsync();

        //    var questionIds = questions.Select(q => q.QuestionId).ToList();

        //    var answers = await _context.TblShortQuestionAnswerChoices
        //                                .Where(a => questionIds.Contains(a.QuestionId ?? 0))
        //                                .ToListAsync();

        //    var result = questions.Select(q => new QuestionAnswerVM
        //    {
        //        QuestionId = q.QuestionId,
        //        QuestionDescription = q.QuestionDescription,
        //        ChapterId = q.ChapterId,
        //        Answers = answers.Where(a => a.QuestionId == q.QuestionId)
        //                         .Select(a => new AnswerVM
        //                         {
        //                             ChoiceId = a.ChoiceId,
        //                             ChoiceDescription = a.ChoiceDescription,
        //                             ChoiceImage = a.ChoiceImage
        //                         }).ToList()
        //    }).ToList();

        //    return result;
        //}

        //[Route("{Class:int}th-{subject}-chapter-{chapter:int}-preparation")]

        //public IActionResult GetShortQuestionNewCriteriaByUrl(string url)
        //{
        //    if (string.IsNullOrWhiteSpace(url))
        //        return BadRequest("Url is required");

        //    // Step 1: Find ParentCriteriaID
        //    var parentCriteria = _context.ShortQuestionCriteria
        //        .FirstOrDefault(c => c.Url == url || c.Url == url + ".aspx");

        //    if (parentCriteria == null)
        //        return NotFound("No criteria found for given URL");

        //    var parentCriteriaId = parentCriteria.Id;

        //    // Step 2: Fetch joined data
        //    var result = (from od in _context.ShortQuestionCriteriaDetails
        //                  join o in _context.ShortQuestionCriteria on od.OtscriteriaId equals o.Id
        //                  join s in _context.TblSubjects on od.SubjectId equals s.SubjectId
        //                  where od.OtscriteriaId == parentCriteriaId
        //                  orderby od.SortOrder
        //                  select new
        //                  {
        //                      ParentCriteriaID = parentCriteriaId,
        //                      o.Tabs,
        //                      SubjectName = s.SubjectName,
        //                      o.TestName,
        //                      o.Url,
        //                      o.ImageName,
        //                      o.MixTabs,
        //                      // all columns from od
        //                      CriteriaDetail = od
        //                  })
        //                 .ToList();

        //    return Ok(result);
        //}

        //[Route("{url:regex(^(?!\\d+th-class$).+)}.{type:alpha?}")]
        //[Route("{url:regex(^(?!(\\d+th-class-online-preparation$)|([[a-zA-Z]]+-part\\d+-online-preparation$)).+)}.{type:alpha?}")]


        [Route("{url:regex(^(?!(\\d+th-class-online-preparation$|\\d+th-class$|[[a-zA-Z]]+-part\\d+-online-preparation$|virtual-university-vu-online-preparation$)).+)}.{type:alpha?}")]
        public async Task<IActionResult> GetShortQuestionsWithMultipleSets(string url, string type)   
        {
            ViewBag.Banners = await _bannerService.GetBannersAsync();
            if (!string.IsNullOrEmpty(type))
            {
                var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/short-questions/{url+'.'+type}");

                ViewBag.CmsData = cmsData;
            }

            else
            {
                var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/short-questions/{url}");

                ViewBag.CmsData = cmsData;
            }
            if (string.IsNullOrWhiteSpace(url))
                return BadRequest("Url is required");

            // Get criteria
            var criteriaResult = (from od in _context.ShortQuestionCriteriaDetails
                                  join o in _context.ShortQuestionCriteria on od.OtscriteriaId equals o.Id
                                  join s in _context.TblSubjects on od.SubjectId equals s.SubjectId
                                  where o.Url == url || o.Url == url + ".aspx"
                                  orderby od.SortOrder
                                  select new
                                  {
                                      ParentCriteriaID = o.Id,
                                      o.Tabs,
                                      SubjectName = s.SubjectName,
                                      o.TestName,
                                      o.Url,
                                      o.ImageName,
                                      o.MixTabs,
                                      CriteriaDetail = od
                                  })
                                .FirstOrDefault();

            if (criteriaResult == null)
                return View("Class");

            // Check if ChapterIds has more than one chapter
            var chapterIds = criteriaResult.CriteriaDetail.ChapterIds;
            var hasMultipleChapters = !string.IsNullOrEmpty(chapterIds) &&
                                     chapterIds.Split(',').Length > 1;

            // Get chapter information for the original criteria
            var chapterInfo = await GetChapterInfo(criteriaResult.CriteriaDetail.ChapterIds);

            // Get all available questions for this criteria
            List<int> chapterIdList = null;
            if (!string.IsNullOrEmpty(criteriaResult.CriteriaDetail.ChapterIds))
            {
                chapterIdList = criteriaResult.CriteriaDetail.ChapterIds.Split(',')
                    .Select(id => Convert.ToInt32(id.Trim()))
                    .ToList();
            }

            // Count available questions by chapter IDs only (since SubjectId is not in TblShortQuestions)
            var availableQuestions = 0;
            if (chapterIdList != null && chapterIdList.Any())
            {
                availableQuestions = await _context.TblShortQuestions
                    .Where(q => chapterIdList.Contains(q.ChapterId))
                    .CountAsync();
            }

            // Calculate actual number of sets we can create
            int actualNumberOfSets = 1;
            if (availableQuestions > 0 && criteriaResult.CriteriaDetail.TotalQuestions > 0)
            {
                // Calculate how many full sets we can create
                actualNumberOfSets = availableQuestions / criteriaResult.CriteriaDetail.TotalQuestions;

                // Ensure we have at least 1 set
                actualNumberOfSets = Math.Max(1, actualNumberOfSets);
            }

            if (hasMultipleChapters)
            {
                // Get all criteria details with the same subject ID
                var relatedCriteriaDetails = await _context.ShortQuestionCriteriaDetails
                    .Where(od => od.SubjectId == criteriaResult.CriteriaDetail.SubjectId && od.ClassId == criteriaResult.CriteriaDetail.ClassId
                                 && !od.ChapterIds.Contains(",")) // exclude multiple chapterIds
                    .ToListAsync();


                // Get the corresponding criteria information
                var relatedCriteriaIds = relatedCriteriaDetails
                                        .OrderBy(od => od.ChapterIds)   // sort first
                                        .Select(od => od.OtscriteriaId) // then select IDs
                                        .ToList();


                var relatedCriteria = await _context.ShortQuestionCriteria
                    .Where(o => relatedCriteriaIds.Contains(o.Id))
                    .Select(o => new
                    {
                        o.Id,
                        o.TestName,
                        o.Url,
                        o.Tabs,
                        o.ImageName,
                        o.MixTabs
                    })
                    .ToListAsync();

                // Apply filter based on URL content
                if (url.Contains("ICS", StringComparison.OrdinalIgnoreCase))
                {
                    relatedCriteria = relatedCriteria
                        .Where(r => !string.IsNullOrEmpty(r.TestName) &&
                                    r.TestName.Contains("ICS", StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
                else if (url.Contains("FSC", StringComparison.OrdinalIgnoreCase))
                {
                    relatedCriteria = relatedCriteria
                        .Where(r => !string.IsNullOrEmpty(r.TestName) &&
                                    r.TestName.Contains("FSC", StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
                else if (url.Contains("FA", StringComparison.OrdinalIgnoreCase))
                {
                    relatedCriteria = relatedCriteria
                        .Where(r => !string.IsNullOrEmpty(r.TestName) &&
                                    r.TestName.Contains("FA", StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
                else if (url.Contains("ICOM", StringComparison.OrdinalIgnoreCase))
                {
                    relatedCriteria = relatedCriteria
                        .Where(r => !string.IsNullOrEmpty(r.TestName) &&
                                    r.TestName.Contains("ICOM", StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }


                // Join with criteria details and get chapter info for each
                var relatedCriteriaWithChapters = new List<RelatedCriteriaWithChaptersVM>();

                foreach (var od in relatedCriteriaDetails)
                {
                    var o = relatedCriteria.FirstOrDefault(rc => rc.Id == od.OtscriteriaId);
                    if (o != null)
                    {
                        // Process each chapter ID individually
                        var individualChapters = new List<ChapterInfoVM>();

                        if (!string.IsNullOrEmpty(od.ChapterIds))
                        {
                            var chapterIdArray = od.ChapterIds.Split(',')
                                .Select(id => Convert.ToInt32(id.Trim()))
                                .ToArray();

                            // Get info for each chapter individually
                            for (int i = 0; i < chapterIdArray.Length; i++)
                            {
                                var chapter = await GetSingleChapterInfo(chapterIdArray[i]);
                                if (chapter != null)
                                {
                                    individualChapters.Add(chapter);
                                }
                            }

                            // Sort by chapter number - CORRECTED VERSION
                            individualChapters = individualChapters
                                .OrderBy(c => c.ChapterNumber)
                                .ToList();
                        }

                        // Calculate available questions for this specific criteria detail
                        List<int> detailChapterIdList = null;
                        if (!string.IsNullOrEmpty(od.ChapterIds))
                        {
                            detailChapterIdList = od.ChapterIds.Split(',')
                                .Select(id => Convert.ToInt32(id.Trim()))
                                .ToList();
                        }

                        var detailAvailableQuestions = 0;
                        if (detailChapterIdList != null && detailChapterIdList.Any())
                        {
                            detailAvailableQuestions = await _context.TblShortQuestions
                                .Where(q => detailChapterIdList.Contains(q.ChapterId))
                                .CountAsync();
                        }

                        relatedCriteriaWithChapters.Add(new RelatedCriteriaWithChaptersVM
                        {
                            CriteriaId = o.Id,
                            TestName = o.TestName,
                            Url = o.Url,
                            Tabs = o.Tabs,
                            ImageName = o.ImageName,
                            MixTabs = o.MixTabs,
                            TotalQuestions = od.TotalQuestions,
                            SubjectId = od.SubjectId,
                            ChapterIds = od.ChapterIds,
                            ClassId = od.ClassId,
                            Chapters = individualChapters, // Now contains individual chapter objects, not an array
                            AvailableQuestions = detailAvailableQuestions,
                            CanCreateSets = detailAvailableQuestions >= od.TotalQuestions
                        });
                    }
                }

                relatedCriteriaWithChapters = relatedCriteriaWithChapters
                    .OrderBy(c =>
                    {
                        if (string.IsNullOrWhiteSpace(c.ChapterIds))
                            return int.MaxValue; // Put entries with no ChapterIds at the end

                        // Split, parse to int, ignore invalid entries, and get the smallest ID
                        var ids = c.ChapterIds
                            .Split(',')
                            .Select(id => int.TryParse(id.Trim(), out var num) ? num : int.MaxValue)
                            .ToList();

                        return ids.Any() ? ids.Min() : int.MaxValue;
                    })
                    .ToList();

                relatedCriteriaWithChapters = relatedCriteriaWithChapters
                    .OrderBy(c => c.Chapters.Any() ? c.Chapters.Min(ch => ch.ChapterNumber) : int.MaxValue)
                    .ToList();

                // Also get a single set of questions for the original multi-chapter criteria
                var questions = await GetShortQuestionsForCustomCriteria(
                    criteriaResult.CriteriaDetail.TotalQuestions,
                    criteriaResult.CriteriaDetail.SubjectId,
                    criteriaResult.CriteriaDetail.ChapterIds);

               var subjectViewModel = new ShortQuestionSubjectViewModel
               {
                    HasMultipleChapters = true,
                    OriginalCriteria = new CriteriaViewModel
                    {
                        ParentCriteriaID = criteriaResult.ParentCriteriaID,
                        Tabs = criteriaResult.Tabs,
                        SubjectName = criteriaResult.SubjectName,
                        TestName = criteriaResult.TestName,
                        Url = criteriaResult.Url,
                        ImageName = criteriaResult.ImageName,
                        MixTabs = criteriaResult.MixTabs,
                        Chapters = chapterInfo,
                        TotalQuestions = criteriaResult.CriteriaDetail.TotalQuestions,
                        AvailableQuestions = availableQuestions,
                        CanCreateSets = availableQuestions >= criteriaResult.CriteriaDetail.TotalQuestions
                    },
                    RelatedCriteria = relatedCriteriaWithChapters.Select(rc => new RelatedCriteriaViewModel
                    {
                        CriteriaId = Convert.ToInt16(rc.GetType().GetProperty("CriteriaId").GetValue(rc)),
                        TestName = rc.GetType().GetProperty("TestName").GetValue(rc)?.ToString(),
                        Url = rc.GetType().GetProperty("Url").GetValue(rc)?.ToString(),
                        Tabs = rc.GetType().GetProperty("Tabs").GetValue(rc)?.ToString(),
                        ImageName = rc.GetType().GetProperty("ImageName").GetValue(rc)?.ToString(),
                        MixTabs = rc.GetType().GetProperty("MixTabs").GetValue(rc)?.ToString(),
                        //TotalQuestions = (int)rc.GetType().GetProperty("TotalQuestions").GetValue(rc),
                        //SubjectId = (int)rc.GetType().GetProperty("SubjectId").GetValue(rc),
                        ChapterIds = rc.GetType().GetProperty("ChapterIds").GetValue(rc)?.ToString(),
                        //ClassId = (int)rc.GetType().GetProperty("ClassId").GetValue(rc),
                        Chapters = (List<ChapterInfoVM>)rc.GetType().GetProperty("Chapters").GetValue(rc),
                        AvailableQuestions = (int)rc.GetType().GetProperty("AvailableQuestions").GetValue(rc),
                        CanCreateSets = (bool)rc.GetType().GetProperty("CanCreateSets").GetValue(rc)
                    }).ToList(),
                    QuestionSet = new QuestionSetViewModel
                    {
                        SetName = "Combined Chapter Set",
                        TotalQuestions = questions.Count,
                        Questions = questions
                    }
               };

                return View("Subject", subjectViewModel);
            }
            else
            {
                // Single chapter - generate multiple sets of questions
                var questionSets = new List<QuestionSetVM>();

                // Only create sets if we have enough questions
                if (availableQuestions >= criteriaResult.CriteriaDetail.TotalQuestions)
                {
                    for (int i = 1; i <= actualNumberOfSets; i++)
                    {
                        var questions = await GetShortQuestionsForCustomCriteria(
                            criteriaResult.CriteriaDetail.TotalQuestions,
                            criteriaResult.CriteriaDetail.SubjectId,
                            criteriaResult.CriteriaDetail.ChapterIds);

                        questionSets.Add(new QuestionSetVM
                        {
                            SetName = $"Set {i}",
                            TotalQuestions = questions.Count,
                            Questions = questions
                        });
                    }
                }
                else
                {
                    // If not enough questions for full sets, create one set with all available questions
                    var allQuestions = await GetShortQuestionsForCustomCriteria(
                        availableQuestions,
                        criteriaResult.CriteriaDetail.SubjectId,
                        criteriaResult.CriteriaDetail.ChapterIds);

                    questionSets.Add(new QuestionSetVM
                    {
                        SetName = "Available Questions",
                        TotalQuestions = allQuestions.Count,
                        Questions = allQuestions
                    });
                }

                // Prepare view model for Chapter view
                var chapterViewModel = new CriteriaWithQuestionSetsVM
                {
                    ParentCriteriaID = criteriaResult.ParentCriteriaID,
                    Tabs = criteriaResult.Tabs,
                    SubjectName = criteriaResult.SubjectName,
                    TestName = criteriaResult.TestName,
                    Url = criteriaResult.Url,
                    ImageName = criteriaResult.ImageName,
                    MixTabs = criteriaResult.MixTabs,
                    CriteriaDetail = criteriaResult.CriteriaDetail,
                    QuestionSets = questionSets,
                    Chapters = chapterInfo,
                    TotalQuestions = criteriaResult.CriteriaDetail.TotalQuestions,
                    AvailableQuestions = availableQuestions,
                    CanCreateSets = availableQuestions >= criteriaResult.CriteriaDetail.TotalQuestions,
                    ActualNumberOfSets = actualNumberOfSets
                };

                
                return View("Chapter", chapterViewModel);
            }
        }

        private async Task<List<ChapterInfoVM>> GetChapterInfo(string chapterIds)
        {
            if (string.IsNullOrEmpty(chapterIds))
                return new List<ChapterInfoVM>();

            var chapterIdList = chapterIds.Split(',')
                .Select(id => Convert.ToInt32(id.Trim()))
                .ToList();

            var chapters = await _context.TblChapters
                .Where(c => chapterIdList.Contains(c.ChapterId))
                .Select(c => new ChapterInfoVM
                {
                    ChapterId = c.ChapterId,
                    ChapterName = c.ChapterName,
                    ChapterNumber = c.ChapterNumber,
                    IsActive = c.IsActive
                })
                .OrderBy(c => c.ChapterNumber)
                .ToListAsync();

            return chapters;
        }

        // New method to get single chapter info
        private async Task<ChapterInfoVM> GetSingleChapterInfo(int chapterId)
        {
            var chapter = await _context.TblChapters
                .Where(c => c.ChapterId == chapterId)
                .Select(c => new ChapterInfoVM
                {
                    ChapterId = c.ChapterId,
                    ChapterName = c.ChapterName,
                    ChapterNumber = c.ChapterNumber,
                    IsActive = c.IsActive
                })
                .FirstOrDefaultAsync();

            return chapter;
        }


        public async Task<IActionResult> GetShortQuestionsWithDynamicSets(string url, [FromQuery] int[] setSizes = null)
        {
            if (string.IsNullOrWhiteSpace(url))
                return BadRequest("Url is required");

            // Get criteria
            var criteriaResult = (from od in _context.ShortQuestionCriteriaDetails
                                  join o in _context.ShortQuestionCriteria on od.OtscriteriaId equals o.Id
                                  join s in _context.TblSubjects on od.SubjectId equals s.SubjectId
                                  where o.Url == url || o.Url == url + ".aspx"
                                  orderby od.SortOrder
                                  select new
                                  {
                                      ParentCriteriaID = o.Id,
                                      o.Tabs,
                                      SubjectName = s.SubjectName,
                                      o.TestName,
                                      o.Url,
                                      o.ImageName,
                                      o.MixTabs,
                                      CriteriaDetail = od
                                  })
                                .FirstOrDefault();

            if (criteriaResult == null)
                return NotFound("No criteria found for given URL");

            // Default set sizes if not provided
            if (setSizes == null || setSizes.Length == 0)
            {
                setSizes = new int[] { criteriaResult.CriteriaDetail.TotalQuestions };
            }

            var questionSets = new List<QuestionSetVM>();

            foreach (var setSize in setSizes)
            {
                var questions = await GetShortQuestionsForCustomCriteria(
                    setSize,
                    criteriaResult.CriteriaDetail.SubjectId,
                    criteriaResult.CriteriaDetail.ChapterIds);

                questionSets.Add(new QuestionSetVM
                {
                    SetName = $"Set of {setSize} Questions",
                    TotalQuestions = questions.Count,
                    Questions = questions
                });
            }

            return Ok(new
            {
                Criteria = criteriaResult,
                QuestionSets = questionSets
            });
        }
        // Your existing method (slightly modified for reuse)
        private async Task<List<QuestionAnswerVM>> GetShortQuestionsForCustomCriteria(
            int topQuestions,
            short subjectId,
            string? chapterIds = null,
            bool ensureUniquePerSet = true)
        {
            List<int>? chapterIdList = null;
            if (!string.IsNullOrEmpty(chapterIds))
            {
                chapterIdList = chapterIds.Split(',')
                                          .Select(id => Convert.ToInt32(id.Trim()))
                                          .ToList();
            }

            var chapterQuery = await _context.TblChapters
                                             .Where(c => c.SubjectId == subjectId)
                                             .ToListAsync();

            if (chapterIdList != null && chapterIdList.Any())
            {
                chapterQuery = chapterQuery
                               .Where(c => chapterIdList.Contains(c.ChapterId))
                               .ToList();
            }

            var validChapterIds = chapterQuery.Select(c => c.ChapterId).ToList();

            if (!validChapterIds.Any())
                return new List<QuestionAnswerVM>();

            // Get all available questions first
            var allQuestions = await _context.TblShortQuestions
                .Where(q => validChapterIds.Contains(q.ChapterId))
                .Select(q => new
                {
                    q.QuestionId,
                    q.QuestionDescription,
                    q.ChapterId,
                    q.QuestionImage
                })
                .ToListAsync();

            // If we want unique questions per set, we need to shuffle differently
            var shuffledQuestions = allQuestions.OrderBy(q => Guid.NewGuid()).ToList();

            // Take the required number of questions
            var selectedQuestions = topQuestions > 0
                ? shuffledQuestions.Take(topQuestions).ToList()
                : shuffledQuestions;

            var questionIds = selectedQuestions.Select(q => q.QuestionId).ToList();

            var answers = await _context.TblShortQuestionAnswerChoices
                                        .Where(a => questionIds.Contains(a.QuestionId ?? 0))
                                        .ToListAsync();

            var result = selectedQuestions.Select(q => new QuestionAnswerVM
            {
                QuestionId = q.QuestionId,
                QuestionDescription = q.QuestionDescription,
                QuestionImage = string.IsNullOrEmpty(q.QuestionImage) ? null : q.QuestionImage,
                ChapterId = q.ChapterId,
                Answers = answers.Where(a => a.QuestionId == q.QuestionId)
                                 .Select(a => new AnswerVM
                                 {
                                     ChoiceId = a.ChoiceId,
                                     ChoiceDescription = a.ChoiceDescription,
                                     ChoiceImage = a.ChoiceImage
                                 }).ToList()
            }).ToList();

            return result;
        }


        [Route("download-short-questions/{url}")]
        public async Task<IActionResult> DownloadShortQuestionsPdf(
            string url,
            [FromServices] IConverter converter)
        {
            if (string.IsNullOrWhiteSpace(url))
                return BadRequest("Url is required");

            // Get criteria
            var criteriaResult = (from od in _context.ShortQuestionCriteriaDetails
                                  join o in _context.ShortQuestionCriteria on od.OtscriteriaId equals o.Id
                                  join s in _context.TblSubjects on od.SubjectId equals s.SubjectId
                                  where o.Url == url || o.Url == url + ".aspx"
                                  orderby od.SortOrder
                                  select new
                                  {
                                      ParentCriteriaID = o.Id,
                                      o.Tabs,
                                      SubjectName = s.SubjectName,
                                      o.TestName,
                                      o.Url,
                                      o.ImageName,
                                      o.MixTabs,
                                      CriteriaDetail = od
                                  })
                                .FirstOrDefault();

            if (criteriaResult == null)
                return NotFound("No criteria found for given URL");

            // Get questions
            var questions = await GetShortQuestionsForCustomCriteria(
                criteriaResult.CriteriaDetail.TotalQuestions,
                criteriaResult.CriteriaDetail.SubjectId,
                criteriaResult.CriteriaDetail.ChapterIds);

            // Render Razor partial view into HTML
            var htmlContent = await RenderViewToStringAsync("_ShortQuestionPdf", new ShortQuestionsPdfViewModel
            {
                SubjectName = criteriaResult.SubjectName,
                TestName = criteriaResult.TestName,
                Questions = questions
            });

            // Convert HTML → PDF
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
            PaperSize = PaperKind.A4,
            Orientation = Orientation.Portrait
        },
                Objects = {
            new ObjectSettings() {
                HtmlContent = htmlContent,
                WebSettings = { DefaultEncoding = "utf-8" }
            }
        }
            };

            byte[] pdfBytes = converter.Convert(doc);

            return File(pdfBytes, "application/pdf", $"{criteriaResult.SubjectName}_{criteriaResult.TestName}.pdf");
        }
        // Install QuestPDF via NuGet: Install-Package QuestPDF

        private byte[] ConvertHtmlToPdf(string htmlContent)
        {
            // QuestPDF is generally faster than DinkToPdf
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Content().Column(column =>
                    {
                        column.Item().Text(htmlContent); // Simplified example
                    });
                });
            });

            return document.GeneratePdf();
        }

        private async Task<string> RenderViewToStringAsync(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.ActionDescriptor.ActionName;

            ViewData.Model = model;

            using (var writer = new StringWriter())
            {
                var viewEngine = HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                var viewResult = viewEngine.FindView(ControllerContext, viewName, false);

                if (viewResult.View == null)
                {
                    throw new ArgumentNullException($"{viewName} does not match any available view");
                }

                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return writer.GetStringBuilder().ToString();
            }
        }

        //private byte[] ConvertHtmlToPdf(string htmlContent)
        //{
        //    // Using DinkToPdf library - you'll need to install it via NuGet
        //    var converter = new BasicConverter(new PdfTools());

        //    var doc = new HtmlToPdfDocument()
        //    {
        //        GlobalSettings = {
        //    ColorMode = ColorMode.Color,
        //    Orientation = Orientation.Portrait,
        //    PaperSize = PaperKind.A4,
        //    Margins = new MarginSettings() { Top = 10, Bottom = 10, Left = 10, Right = 10 },
        //},
        //        Objects = {
        //    new ObjectSettings() {
        //        PagesCount = true,
        //        HtmlContent = htmlContent,
        //        WebSettings = { DefaultEncoding = "utf-8" },
        //        HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
        //    }
        //}
        //    };

        //    return converter.Convert(doc);
        //}







        //[HttpGet]
        //public async Task<IActionResult> PreparationHome(string subjectUrl)
        //{
        //    // Find subject by Url
        //    var subject = await _context.TblSubjects
        //                                .FirstOrDefaultAsync(s => s.SubjectUrl == subjectUrl);

        //    if (subject == null)
        //        return NotFound();

        //    // Get class info
        //    var classObj = await _context.TblClasses.FirstOrDefaultAsync(c => c.ClassId == subject.ClassId);

        //    // Get all chapters of this subject
        //    var chapters = await _context.TblChapters
        //                                 .Where(c => c.SubjectId == subject.SubjectId && c.IsActive == true)
        //                                 .OrderBy(c => c.ChapterNumber)
        //                                 .ToListAsync();

        //    // Get all questions for the subject (Full book test)
        //    var chapterIds = chapters.Select(c => c.ChapterId).ToList();
        //    var totalQuestions = await _context.TblShortQuestions.CountAsync(q => chapterIds.Contains(q.ChapterId));

        //    var viewModel = new PreparationHomeViewModel
        //    {
        //        SubjectId = subject.SubjectId,
        //        SubjectName = subject.SubjectName,
        //        ClassName = classObj?.ClassName,
        //        SubjectUrl = subject.SubjectUrl,
        //        TotalQuestions = totalQuestions,
        //        Chapters = chapters.Select(c => new ChapterLinkViewModel
        //        {
        //            ChapterId = c.ChapterId,
        //            ChapterName = c.ChapterName
        //        }).ToList()
        //    };

        //    return View(viewModel);
        //}

    }
}
