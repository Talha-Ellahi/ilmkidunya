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
    [Route("questions")]
    public class LongQuestionsController : Controller
    {

        private readonly DbikdContext _context;
        private readonly BannerService _bannerService;
        private readonly CmsRepository _cmsRepo;
        public LongQuestionsController(DbikdContext context, BannerService bannerService , CmsRepository cmsRepo)
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
            var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/questions/");

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
                var cmsDatauni = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/questions/virtual-university-vu-online-preparation.aspx");

                ViewBag.CmsData = cmsDatauni;
                return View();

            }
            var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/questions/{Class}th-class.aspx");



            if (cmsData == null && inter != null && part != null)
            {
                cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/questions/{inter}-part{part}-online-preparation.aspx");

            }
            if (cmsData == null)
            {
                cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/questions/{Class}th-class-online-preparation.aspx");

                if (cmsData == null)
                {
                    return NotFound();
                }

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






        [Route("{url:regex(^(?!(\\d+th-class-online-preparation$|\\d+th-class$|[[a-zA-Z]]+-part\\d+-online-preparation$|virtual-university-vu-online-preparation$)).+)}.{type:alpha?}")]
        public async Task<IActionResult> GetLongQuestionsWithMultipleSets(string url)

        {
            ViewBag.Banners = await _bannerService.GetBannersAsync();
            var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/questions/{url}");

            ViewBag.CmsData = cmsData;

            if (string.IsNullOrWhiteSpace(url))
                return BadRequest("Url is required");

            // Get criteria
            var criteriaResult = (from od in _context.LongQuestionCriteriaDetails
                                  join o in _context.LongQuestionCriteria on od.OtscriteriaId equals o.Id
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

            // Count available questions by chapter IDs only (since SubjectId is not in TblLongQuestions)
            var availableQuestions = 0;
            if (chapterIdList != null && chapterIdList.Any())
            {
                availableQuestions = await _context.TblLongQuestions
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
                var relatedCriteriaDetails = await _context.LongQuestionCriteriaDetails
                    .Where(od => od.SubjectId == criteriaResult.CriteriaDetail.SubjectId && od.ClassId == criteriaResult.CriteriaDetail.ClassId
                                 && !od.ChapterIds.Contains(",")) // exclude multiple chapterIds
                    .ToListAsync();


                // Get the corresponding criteria information
                var relatedCriteriaIds = relatedCriteriaDetails
                                        .OrderBy(od => od.ChapterIds)   // sort first
                                        .Select(od => od.OtscriteriaId) // then select IDs
                                        .ToList();


                var relatedCriteria = await _context.LongQuestionCriteria
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
                            detailAvailableQuestions = await _context.TblLongQuestions
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
                var questions = await GetLongQuestionsForCustomCriteria(
                    criteriaResult.CriteriaDetail.TotalQuestions,
                    criteriaResult.CriteriaDetail.SubjectId,
                    criteriaResult.CriteriaDetail.ChapterIds);

                var subjectViewModel = new LongQuestionSubjectViewModel
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
                        var questions = await GetLongQuestionsForCustomCriteria(
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
                    var allQuestions = await GetLongQuestionsForCustomCriteria(
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
                var chapterViewModel = new CriteriaWithLongQuestionSetsVM
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


        public async Task<IActionResult> GetLongQuestionsWithDynamicSets(string url, [FromQuery] int[] setSizes = null)
        {
            if (string.IsNullOrWhiteSpace(url))
                return BadRequest("Url is required");

            // Get criteria
            var criteriaResult = (from od in _context.LongQuestionCriteriaDetails
                                  join o in _context.LongQuestionCriteria on od.OtscriteriaId equals o.Id
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
                var questions = await GetLongQuestionsForCustomCriteria(
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
        private async Task<List<QuestionAnswerVM>> GetLongQuestionsForCustomCriteria(
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
            var allQuestions = await _context.TblLongQuestions
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

            var answers = await _context.TblLongQuestionAnswerChoices
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


        [Route("download-questions/{url}")]
        public async Task<IActionResult> DownloadLongQuestionsPdf(
            string url,
            [FromServices] IConverter converter)
        {
            if (string.IsNullOrWhiteSpace(url))
                return BadRequest("Url is required");

            // Get criteria
            var criteriaResult = (from od in _context.LongQuestionCriteriaDetails
                                  join o in _context.LongQuestionCriteria on od.OtscriteriaId equals o.Id
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
            var questions = await GetLongQuestionsForCustomCriteria(
                criteriaResult.CriteriaDetail.TotalQuestions,
                criteriaResult.CriteriaDetail.SubjectId,
                criteriaResult.CriteriaDetail.ChapterIds);

            // Render Razor partial view into HTML
            var htmlContent = await RenderViewToStringAsync("_LongQuestionPdf", new LongQuestionsPdfViewModel
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



    }
}
