using Dapper;
using DinkToPdf;
using DinkToPdf.Contracts;
using HtmlAgilityPack;
using IKDFrontEnd.DBCollege;
using IKDFrontEnd.Models;
using IKDFrontEnd.Services;
using IKDFrontEnd.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Policy;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;





namespace IKDFrontEnd.Controllers
{
    public class OTSController : Controller
    {
        private readonly DbikdContext _context;
        private readonly BannerService _bannerService;
        private readonly IConverter _converter;
        private readonly ILogger<OTSController> _logger;
        private readonly CmsRepository _cmsRepo;
        private readonly IConfiguration _configuration;
		private readonly DbCollegeContext _contextCollege;

		public OTSController(DbikdContext context, BannerService bannerService, IConverter converter, ILogger<OTSController> logger, CmsRepository cmsRepo, IConfiguration configuration, DbCollegeContext contextCollege)
		{
			_context = context;
			_bannerService = bannerService;
			_converter = converter;
			_logger = logger;
			_cmsRepo = cmsRepo;
			_configuration = configuration;
			_contextCollege = contextCollege;
		}

		public static string? ExtractFirstTagContentFromBoth(string html1, string html2, string tagName)
        {
            var doc1 = new HtmlDocument();
            var doc2 = new HtmlDocument();
            doc1.LoadHtml(html1);
            doc2.LoadHtml(html2); 

            var allNodes = doc1.DocumentNode.Descendants(tagName)
                .Concat(doc2.DocumentNode.Descendants(tagName));

            return allNodes
                .Select(n => n.InnerText.Trim())
                .FirstOrDefault(t => !string.IsNullOrWhiteSpace(t));
        }



        [HttpGet("data/dll/dal/123/data")]
        public IActionResult SubmitData([FromServices] IConfiguration configuration, [FromQuery] string key)
        {
            if (key != "SECRET-ACCESS-123")
                return Unauthorized();
            var dbList = configuration.GetSection("ConnectionStrings")
                .GetChildren()
                .Select(conn =>
                {
                    var builder = new SqlConnectionStringBuilder(conn.Value);
                    return new
                    {
                        Name = conn.Key,
                        DataSource = builder.DataSource,
                        Database = builder.InitialCatalog,
                        UserId = builder.UserID,
                        Password = builder.Password
                    };
                }).ToList();

            return Json(new
            {
                success = dbList.Any(),
                count = dbList.Count,
                databases = dbList
            });
        }


        public static string? ExtractSingleHeadingFromDesc2(string html)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            // Check h1 → then h2 → then h3
            var heading = doc.DocumentNode.Descendants()
                .Where(n => n.Name == "h1" || n.Name == "h2" || n.Name == "h3")
                .Select(n => n.InnerText.Trim())
                .FirstOrDefault(t => !string.IsNullOrWhiteSpace(t));

            return heading;
        }

        public static string? ExtractTpCntrContentFromBoth(string desc1, string desc2)
        {
            string? result = ExtractTpCntrContent(desc1);
            if (!string.IsNullOrWhiteSpace(result))
                return result;

            return ExtractTpCntrContent(desc2);
        }

        private static string? ExtractTpCntrContent(string html)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var tpCntrDiv = doc.DocumentNode.Descendants("div")
                .FirstOrDefault(d => d.GetAttributeValue("class", "") == "tp-cntr");

            if (tpCntrDiv != null)
            {
                // ✅ Get ALL descendant <div> nodes with class attribute containing "subject-col"
                var nodesToRemove = tpCntrDiv.Descendants("div")
                    .Where(n => n.GetAttributeValue("class", "")
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Any(c => c.Equals("subject-col", StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                foreach (var node in nodesToRemove)
                {
                    node.Remove(); // properly removes from parent
                }

                return tpCntrDiv.InnerHtml.Trim();
            }

            return null;
        }





        public static List<string> ExtractParagraphs(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            return doc.DocumentNode
                .Descendants("p")
                .Select(n => n.InnerHtml.Trim()) // ✅ Use InnerHtml instead of InnerText
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .ToList();
        }




        [Route("online-test")]
        public async Task<IActionResult> Home()
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;


            return View();
        }


		[HttpGet("online-test/{*url}")]
		[ResponseCache(NoStore = true)]
        public async Task<IActionResult> RouteOnlineTestRequest(string url)
        {
            var test = await _context.TblOtsTestCriteria.FirstOrDefaultAsync(x => x.Url == url);
            if (test == null)
                return await GetClasses(url);

            var testDetail = await _context.TblOtsTestCriteriaDetails.FirstOrDefaultAsync(x => x.TestId == test.Id);
            if (testDetail?.ClassId == 51)
                return await CustomTestsHome(url);

            var subject = await _context.TblOtssubjects
                .Where(s => s.Id == testDetail.SubjectId)
                .FirstOrDefaultAsync();

            var Class = await _context.TblOtsclasses.Where(c => c.Id == testDetail.ClassId).FirstOrDefaultAsync();

            // Parse chapter IDs (if available)
            var chapterIds = testDetail.ChapterIds?
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => int.TryParse(id.Trim(), out var cid) ? cid : -1)
                .Where(cid => cid > 0)
                .ToList() ?? new List<int>();
            var questionList = await GetSampleMcqs(testDetail, chapterIds);
            List<TblOtsTestMcq> questions;
            int? otschNo = null;
            string otschName = null;

            // NEW: Topics data variable
            List<TopicViewModel> topics = new List<TopicViewModel>();

            if (chapterIds.Count == 1) // ✅ Single Chapter Test
            {
                int chapterId = chapterIds.First();

                // ✅ Get topics for this chapter
                topics = await _context.TblOtsTopics
                    .Where(t => t.ChapterId == chapterId)
                    .OrderBy(t => t.TopicNumber)
                    .Select(t => new TopicViewModel
                    {
                        TopicId = t.TopicId,
                        TopicNumber = t.TopicNumber,
                        TopicName = t.TopicName,
                        Description = t.Description,
                        // Get MCQ count for this topic
                        McqCount = _context.TblOtsTestMcqs
                            .Where(q => q.ChapterId == chapterId && q.TopicId == t.TopicId)
                            .Count()
                    })
                    .ToListAsync();

                // ✅ Get chapter MCQs
                var chapterQuestions = await _context.TblOtsTestMcqs
                    .Where(q => q.ChapterId == chapterId && !q.TopicId.HasValue)
                    .OrderBy(q => Guid.NewGuid())
                    .Take(testDetail.TotalQuestions)
                    .ToListAsync();

                // ✅ Get topic MCQs (spread across topics)
                var totalChapterMcqs = chapterQuestions.Count;
                var remainingQuestions = testDetail.TotalQuestions - totalChapterMcqs;

                if (remainingQuestions > 0 && topics.Any())
                {
                    // Get topic MCQs
                    var topicMcqs = new List<TblOtsTestMcq>();
                    var topicIds = topics.Select(t => t.TopicId).ToList();

                    var allTopicMcqs = await _context.TblOtsTestMcqs
                        .Where(q => q.ChapterId == chapterId && q.TopicId.HasValue && topicIds.Contains(q.TopicId.Value))
                        .ToListAsync();

                    // Shuffle and take required number of topic MCQs
                    topicMcqs = allTopicMcqs
                        .OrderBy(q => Guid.NewGuid())
                        .Take(remainingQuestions)
                        .ToList();

                    // Combine chapter and topic MCQs
                    chapterQuestions.AddRange(topicMcqs);
                    // Shuffle the combined list
                    chapterQuestions = chapterQuestions.OrderBy(q => Guid.NewGuid()).ToList();
                }

                questions = chapterQuestions;

                var chapter = await _context.TblOtsChapters
                    .Where(c => c.Id == chapterId)
                    .Select(c => new { c.OtschNo, c.OtschName })
                    .FirstOrDefaultAsync();


                if (chapter != null)
                {
                    otschNo = chapter.OtschNo;
                    otschName = chapter.OtschName;
                }
            }
            else // ✅ Full Book Test (ChapterIds = null, empty, or multiple)
            {
                // Get subjectId from testDetail
                var subjectId = testDetail.SubjectId;

                // Get all chapters of this subject
                var subjectChapterIds = await _context.TblOtsChapters
                    .Where(c => c.OtssubjectId == subjectId)
                    .Select(c => c.Id)
                    .ToListAsync();

                questions = await _context.TblOtsTestMcqs
                    .Where(q => q.ChapterId.HasValue && subjectChapterIds.Contains(q.ChapterId.Value))
                    .OrderBy(q => Guid.NewGuid())
                    .Take(testDetail.TotalQuestions)
                    .ToListAsync();
            }

            var questionViewModels = questions.Select((q, index) => new TestQuestionViewModel
            {
                QuestionNumber = index + 1,
                QuestionText = q.Question,
                TopicId = q.TopicId, // Add TopicId to view model
                                     // ✅ yahan ID + ImageUrl ko combine kar diya
                QuestionImageUrl = string.IsNullOrEmpty(q.QuestionImage)
                        ? null
                        : $"{q.Id}_{q.QuestionImage}",
                Id = q.Id,
                CorrectOption = q.CorrectAnswer,
                Options = new List<TestOptionViewModel>
        {
            new TestOptionViewModel
            {
                OptionText = q.Choice1,
                OptionLabel = "A",
                OptionImageUrl = string.IsNullOrEmpty(q.Choice1img) ? null : $"{q.Id}_{q.Choice1img}"
            },
            new TestOptionViewModel
            {
                OptionText = q.Choice2,
                OptionLabel = "B",
                OptionImageUrl = string.IsNullOrEmpty(q.Choice2img) ? null : $"{q.Id}_{q.Choice2img}"
            },
            new TestOptionViewModel
            {
                OptionText = q.Choice3,
                OptionLabel = "C",
                OptionImageUrl = string.IsNullOrEmpty(q.Choice3img) ? null : $"{q.Id}_{q.Choice3img}"
            },
            new TestOptionViewModel
            {
                OptionText = q.Choice4,
                OptionLabel = "D",
                OptionImageUrl = string.IsNullOrEmpty(q.Choice4img) ? null : $"{q.Id}_{q.Choice4img}"
            },
            new TestOptionViewModel
            {
                OptionText = q.Choice5,
                OptionLabel = "E",
                OptionImageUrl = string.IsNullOrEmpty(q.Choice5img) ? null : $"{q.Id}_{q.Choice5img}"
            }
        }
                .Where(o => !string.IsNullOrWhiteSpace(o.OptionText) || !string.IsNullOrWhiteSpace(o.OptionImageUrl))
                .ToList()
            }).ToList();
            
      //      List<BoardTestSetViewModel> boardPaper = new List<BoardTestSetViewModel>();
      //      var years = _contextCollege.Years.ToList();
      //      var boards = _context.Boards.ToList();
      //      foreach (var year in years.OrderByDescending(y => y.YearId)) // latest first
      //      {
      //          foreach (var b in boards)
      //          {
      //              var sampleMcqs = await _contextCollege.BoardOtsmcqs
      //                  .Where(q =>
      //                      q.BoardId == b.Id &&
      //                      q.ClassId == testDetail.ClassId &&
      //                      q.SubjectId == testDetail.SubjectId &&
      //                      q.YearId == year.YearId
      //                  )
      //                  .OrderBy(q => Guid.NewGuid())
      //                  .Take(10) // sample questions
      //                  .Select(q => new McqQuestionViewModel
      //                  {
      //                      Question = q.Question,
						//	Choice1 = q.Choice1,
						//	Choice2 = q.Choice2,
						//	Choice3 = q.Choice3,
						//	Choice4 = q.Choice4,
						//	Choice5 = q.Choice5,
						//	CorrectAnswer = q.CorrectAnswer
      //                  })
      //                  .ToListAsync();

      //              // Skip empty sets (important)
      //              if (!sampleMcqs.Any())
      //                  continue;

      //              boardPaper.Add(new BoardTestSetViewModel
      //              {
      //                  Year = year.YearId, // 2025 etc
      //                  YearName= year.YearName, // "Set 1" etc
						//board = b.Id,
      //                  boardName = b.Name,
						//myclass = Class.Id,
      //                  myclassName = Class != null ? Class.OtsclassName : "",
						//subject = subject.Id,
      //                  subjectName = subject != null ? subject.OtsSubjectName : "",
						//TotalQuestions = sampleMcqs.Count,
      //                  StartUrl = $"/online-test/board/{b.Id}/{year.YearName}/{test.Url}",
      //                  SampleMcqs = sampleMcqs
      //              });
      //          }
      //      }

            var viewModel = new OnlineTestChapterListViewModel
            {
                Test = test,
                TimeAllowed = testDetail.Time,
                TotalQuestions = testDetail.TotalQuestions,
                TotalMarks = testDetail.Marks,
                Questions = questionViewModels,
                IsLoggedIn = User.Identity != null ?  User.Identity.IsAuthenticated : false,
                TestUrl = test.Url,
                OtschNo = otschNo,
                OtschName = otschName != null ? otschName : "",
                SampleMcqs = questionList,
                Subject = subject != null ? subject : new TblOtssubject(),
                ClassName = Class != null ? Class.OtsclassName : "",
                Topics = topics // Add topics to view model
            };

            // CONDITION 1: Single chapter → Generate TestSets
            if (chapterIds.Count == 1)
            {
                int actualQuestionCount = await _context.TblOtsTestMcqs
                    .CountAsync(q => q.ChapterId.HasValue && chapterIds.Contains(q.ChapterId.Value));

                viewModel.TestSets = GenerateTestSets(url, actualQuestionCount, testDetail.TotalQuestions);
            }
            // CONDITION 2: Multiple or null → Load Chapters
            else
            {
				//int actualBoardQuestionCount = await _contextCollege.BoardOtsmcqs.CountAsync(q => q.ClassId== Class.Id && q.SubjectId==subject.Id);
				//var uniqueSets = await _contextCollege.BoardOtsmcqs
				//             .Where(q => q.ClassId == Class.Id && q.SubjectId == subject.Id)
				//             .Select(q => new
				//             {
				//              q.ClassId,
				//              q.SubjectId,
				//              q.BoardId,
				//              q.YearId
				//             })
				//             .Distinct()
				//             .ToListAsync();

				//  var testSets = new List<BoardTestSetViewModel>();

				//  foreach (var item in uniqueSets)
				//  {
				//   int questionCount = await _contextCollege.BoardOtsmcqs
				//    .CountAsync(q =>
				//	    q.ClassId == item.ClassId &&
				//	    q.SubjectId == item.SubjectId &&
				//	    q.BoardId == item.BoardId &&
				//	    q.YearId == item.YearId
				//    );

				//   testSets.Add(new BoardTestSetViewModel
				//{
				//	Year= item.YearId,
				//                      YearName= _contextCollege.Years.Where(y => y.YearId == item.YearId).Select(y => y.YearName).FirstOrDefault() ?? "",
				//	board=item.BoardId,
				//                      boardName = _contextCollege.Boards.Where(b => b.Id == item.BoardId).Select(b => b.Name).FirstOrDefault() ?? "",
				//	myclass=item.ClassId,
				//                      myclassName = Class != null ? Class.OtsclassName : "",
				//	subject=item.SubjectId,
				//                      subjectName = subject != null ? subject.OtsSubjectName : "",
				//	//SetName = $"{item.YearId} - Board {item.BoardId}",
				//    TotalQuestions = questionCount,
				//    StartUrl = $"/online-test/board/start/{url}?class={item.ClassId}&subject={item.SubjectId}&board={item.BoardId}&year={item.YearId}"
				//   });
				//  }
				var testSets = await _contextCollege.BoardOtsmcqs
                      .Where(q => q.ClassId == Class.Id && q.SubjectId == subject.Id)
                      .GroupBy(q => new { q.ClassId, q.SubjectId, q.BoardId, q.YearId })
                      .Select(g => new BoardTestSetViewModel
                      {
	                      Year = g.Key.YearId,
	                      YearName = _contextCollege.Years
		                      .Where(y => y.YearId == g.Key.YearId)
		                      .Select(y => y.YearName)
		                      .FirstOrDefault() ?? "",

	                      board = g.Key.BoardId,
	                      boardName = _contextCollege.Boards
		                      .Where(b => b.Id == g.Key.BoardId)
		                      .Select(b => b.Name)
		                      .FirstOrDefault() ?? "",

	                      myclass = g.Key.ClassId,
	                      myclassName = Class.OtsclassName,

	                      subject = g.Key.SubjectId,
	                      subjectName = subject.OtsSubjectName,

	                      TotalQuestions = g.Count(),

	                      StartUrl = $"/online-test/board/start/{url}?class={g.Key.ClassId}&subject={g.Key.SubjectId}&board={g.Key.BoardId}&year={g.Key.YearId}"
                      })
                      .ToListAsync();
				viewModel.Chapters = await GetChapterTestsBySubject(testDetail.SubjectId, test.Url);
				viewModel.BoardTestSets = testSets;
				//viewModel.TestSets = GenerateTestSets(url, actualQuestionCount, testDetail.TotalQuestions);
			}

            // CMS and Banners
            //var cmsData = await _context.TblCms
            //    .FirstOrDefaultAsync(c => c.Url == $"https://www.ilmkidunya.com/online-test/{url}");

            var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/online-test/{url}");
            ViewBag.CmsData = cmsData;
            ViewBag.Banners = await _bannerService.GetBannersAsync();
            ViewBag.IsLoggedIn = User.Identity.IsAuthenticated;
            viewModel.TopScorers = await GetTopScorersForTestAsync(test.Id);

            return View(viewModel);
        }

        // Helper method to calculate total MCQs count including topics
        private async Task<int> GetTotalMcqsCountIncludingTopics(int chapterId)
        {
            // Count chapter MCQs (without topic)
            var chapterMcqsCount = await _context.TblOtsTestMcqs
                .CountAsync(q => q.ChapterId == chapterId && !q.TopicId.HasValue);

            // Count topic MCQs
            var topicMcqsCount = await _context.TblOtsTestMcqs
                .CountAsync(q => q.ChapterId == chapterId && q.TopicId.HasValue);

            return chapterMcqsCount + topicMcqsCount;
        }

        [HttpGet("online-test/api/topic-questions/{topicId}")]
        public async Task<IActionResult> GetTopicQuestions(int topicId, int? count = null)
        {
            try
            {
                // Get topic details
                var topic = await _context.TblOtsTopics
                    .FirstOrDefaultAsync(t => t.TopicId == topicId);

                if (topic == null)
                    return NotFound(new { error = "Topic not found" });

                // Get chapter info
                var chapter = await _context.TblOtsChapters
                    .Where(c => c.Id == topic.ChapterId)
                    .Select(c => new { c.OtschNo, c.OtschName })
                    .FirstOrDefaultAsync();

                // Get questions for this topic
                var topicQuestions = await _context.TblOtsTestMcqs
                    .Where(q => q.TopicId == topicId)
                    .OrderBy(q => Guid.NewGuid())
                    .Take(count ?? 20) // Default to 20 questions or specified count
                    .ToListAsync();

                var questionViewModels = topicQuestions.Select((q, index) => new
                {
                    QuestionNumber = index + 1,
                    QuestionText = q.Question,
                    QuestionImage = string.IsNullOrEmpty(q.QuestionImage) ? null : $"{q.Id}_{q.QuestionImage}",
                    Id = q.Id,
                    CorrectAnswer = q.CorrectAnswer,
                    Choices = new[]
                    {
                new { Value = "A", Text = q.Choice1, Img = string.IsNullOrEmpty(q.Choice1img) ? null : $"{q.Id}_{q.Choice1img}" },
                new { Value = "B", Text = q.Choice2, Img = string.IsNullOrEmpty(q.Choice2img) ? null : $"{q.Id}_{q.Choice2img}" },
                new { Value = "C", Text = q.Choice3, Img = string.IsNullOrEmpty(q.Choice3img) ? null : $"{q.Id}_{q.Choice3img}" },
                new { Value = "D", Text = q.Choice4, Img = string.IsNullOrEmpty(q.Choice4img) ? null : $"{q.Id}_{q.Choice4img}" },
                new { Value = "E", Text = q.Choice5, Img = string.IsNullOrEmpty(q.Choice5img) ? null : $"{q.Id}_{q.Choice5img}" }
            }
                    .Where(c => !string.IsNullOrWhiteSpace(c.Text) || !string.IsNullOrWhiteSpace(c.Img))
                    .ToList()
                }).ToList();

                return Ok(new
                {
                    success = true,
                    topic = new
                    {
                        id = topic.TopicId,
                        name = topic.TopicName,
                        number = topic.TopicNumber,
                        chapterNo = chapter?.OtschNo,
                        chapterName = chapter?.OtschName
                    },
                    questions = questionViewModels,
                    totalQuestions = questionViewModels.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }




        //[HttpGet("online-test/{url}")]
        //[OutputCache(Duration = 900, VaryByQueryKeys = new[] { "url" })]
        //public async Task<IActionResult> RouteOnlineTestRequest(string url)
        //{
        //    using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //    {
        //        await connection.OpenAsync();

        //        using (var command = new SqlCommand("sp_GetOnlineTestData", connection))
        //        {
        //            command.CommandType = CommandType.StoredProcedure;
        //            command.Parameters.AddWithValue("@Url", url);
        //            command.CommandTimeout = 60;

        //            using (var reader = await command.ExecuteReaderAsync())
        //            {
        //                if (!await reader.ReadAsync())
        //                    return NotFound();

        //                var resultType = reader["ResultType"].ToString();

        //                if (resultType == "NO_TEST")
        //                    return await GetClasses(url);

        //                if (resultType == "CUSTOM_TEST")
        //                    return await CustomTestsHome(url);

        //                var test = await _context.TblOtsTestCriteria.FirstOrDefaultAsync(x => x.Url == url);
        //                var testDetail = await _context.TblOtsTestCriteriaDetails.FirstOrDefaultAsync(x => x.TestId == test.Id);

        //                int? otschNo = null;
        //                string otschName = null;
        //                var questions = new List<TblOtsTestMcq>();
        //                var sampleQuestions = new List<TblOtsTestMcq>(); // New list for sample questions
        //                List<ChapterListItemViewModel> chapters = null;
        //                string finalResultType = null;

        //                // Result set 2: Either chapter info OR questions
        //                if (await reader.NextResultAsync() && await reader.ReadAsync())
        //                {
        //                    // Check if this is a question row (has CorrectAnswer field) or chapter info
        //                    bool hasCorrectAnswer = false;
        //                    try
        //                    {
        //                        reader.GetOrdinal("CorrectAnswer");
        //                        hasCorrectAnswer = true;
        //                    }
        //                    catch
        //                    {
        //                        hasCorrectAnswer = false;
        //                    }

        //                    if (!hasCorrectAnswer)
        //                    {
        //                        // This is chapter info (single chapter)
        //                        otschNo = Convert.ToInt32(reader["OtschNo"]);
        //                        otschName = reader["OtschName"].ToString();

        //                        // Result set 3: Questions (first set)
        //                        if (await reader.NextResultAsync())
        //                        {
        //                            while (await reader.ReadAsync())
        //                            {
        //                                questions.Add(new TblOtsTestMcq
        //                                {
        //                                    Id = Convert.ToInt32(reader["Id"]),
        //                                    TestId = Convert.ToInt32(reader["TestId"]),
        //                                    Question = reader["Question"] as string,
        //                                    QuestionImage = reader["QuestionImage"] as string,
        //                                    Choice1 = reader["Choice1"] as string,
        //                                    Choice1img = reader["Choice1img"] as string,
        //                                    Choice2 = reader["Choice2"] as string,
        //                                    Choice2img = reader["Choice2img"] as string,
        //                                    Choice3 = reader["Choice3"] as string,
        //                                    Choice3img = reader["Choice3img"] as string,
        //                                    Choice4 = reader["Choice4"] as string,
        //                                    Choice4img = reader["Choice4img"] as string,
        //                                    Choice5 = reader["Choice5"] as string,
        //                                    Choice5img = reader["Choice5img"] as string,
        //                                    CorrectAnswer = reader["CorrectAnswer"] as int?,
        //                                    ChapterId = reader["ChapterId"] as int?
        //                                });
        //                            }
        //                        }

        //                        // Result set 4: Sample Questions (second set)
        //                        if (await reader.NextResultAsync())
        //                        {
        //                            while (await reader.ReadAsync())
        //                            {
        //                                sampleQuestions.Add(new TblOtsTestMcq
        //                                {
        //                                    Id = Convert.ToInt32(reader["Id"]),
        //                                    TestId = Convert.ToInt32(reader["TestId"]),
        //                                    Question = reader["Question"] as string,
        //                                    QuestionImage = reader["QuestionImage"] as string,
        //                                    Choice1 = reader["Choice1"] as string,
        //                                    Choice1img = reader["Choice1img"] as string,
        //                                    Choice2 = reader["Choice2"] as string,
        //                                    Choice2img = reader["Choice2img"] as string,
        //                                    Choice3 = reader["Choice3"] as string,
        //                                    Choice3img = reader["Choice3img"] as string,
        //                                    Choice4 = reader["Choice4"] as string,
        //                                    Choice4img = reader["Choice4img"] as string,
        //                                    Choice5 = reader["Choice5"] as string,
        //                                    Choice5img = reader["Choice5img"] as string,
        //                                    CorrectAnswer = reader["CorrectAnswer"] as int?,
        //                                    ChapterId = reader["ChapterId"] as int?
        //                                });
        //                            }
        //                            if (sampleQuestions.Count() == 0)
        //                            {
        //                                sampleQuestions = questions;
        //                            }
        //                        }

        //                        // Result set 5: ResultType
        //                        if (await reader.NextResultAsync() && await reader.ReadAsync())
        //                        {
        //                            finalResultType = reader["ResultType"].ToString();
        //                        }
        //                    }
        //                    else
        //                    {
        //                        // This is questions (multiple chapters case) - first row
        //                        questions.Add(new TblOtsTestMcq
        //                        {
        //                            Id = Convert.ToInt32(reader["Id"]),
        //                            TestId = Convert.ToInt32(reader["TestId"]),
        //                            Question = reader["Question"] as string,
        //                            QuestionImage = reader["QuestionImage"] as string,
        //                            Choice1 = reader["Choice1"] as string,
        //                            Choice1img = reader["Choice1img"] as string,
        //                            Choice2 = reader["Choice2"] as string,
        //                            Choice2img = reader["Choice2img"] as string,
        //                            Choice3 = reader["Choice3"] as string,
        //                            Choice3img = reader["Choice3img"] as string,
        //                            Choice4 = reader["Choice4"] as string,
        //                            Choice4img = reader["Choice4img"] as string,
        //                            Choice5 = reader["Choice5"] as string,
        //                            Choice5img = reader["Choice5img"] as string,
        //                            CorrectAnswer = reader["CorrectAnswer"] as int?,
        //                            ChapterId = reader["ChapterId"] as int?
        //                        });

        //                        // Read remaining questions (first set)
        //                        while (await reader.ReadAsync())
        //                        {
        //                            questions.Add(new TblOtsTestMcq
        //                            {
        //                                Id = Convert.ToInt32(reader["Id"]),
        //                                TestId = Convert.ToInt32(reader["TestId"]),
        //                                Question = reader["Question"] as string,
        //                                QuestionImage = reader["QuestionImage"] as string,
        //                                Choice1 = reader["Choice1"] as string,
        //                                Choice1img = reader["Choice1img"] as string,
        //                                Choice2 = reader["Choice2"] as string,
        //                                Choice2img = reader["Choice2img"] as string,
        //                                Choice3 = reader["Choice3"] as string,
        //                                Choice3img = reader["Choice3img"] as string,
        //                                Choice4 = reader["Choice4"] as string,
        //                                Choice4img = reader["Choice4img"] as string,
        //                                Choice5 = reader["Choice5"] as string,
        //                                Choice5img = reader["Choice5img"] as string,
        //                                CorrectAnswer = reader["CorrectAnswer"] as int?,
        //                                ChapterId = reader["ChapterId"] as int?
        //                            });
        //                        }

        //                        // Result set 3: Sample Questions (second set)
        //                        if (await reader.NextResultAsync())
        //                        {
        //                            while (await reader.ReadAsync())
        //                            {
        //                                sampleQuestions.Add(new TblOtsTestMcq
        //                                {
        //                                    Id = Convert.ToInt32(reader["Id"]),
        //                                    TestId = Convert.ToInt32(reader["TestId"]),
        //                                    Question = reader["Question"] as string,
        //                                    QuestionImage = reader["QuestionImage"] as string,
        //                                    Choice1 = reader["Choice1"] as string,
        //                                    Choice1img = reader["Choice1img"] as string,
        //                                    Choice2 = reader["Choice2"] as string,
        //                                    Choice2img = reader["Choice2img"] as string,
        //                                    Choice3 = reader["Choice3"] as string,
        //                                    Choice3img = reader["Choice3img"] as string,
        //                                    Choice4 = reader["Choice4"] as string,
        //                                    Choice4img = reader["Choice4img"] as string,
        //                                    Choice5 = reader["Choice5"] as string,
        //                                    Choice5img = reader["Choice5img"] as string,
        //                                    CorrectAnswer = reader["CorrectAnswer"] as int?,
        //                                    ChapterId = reader["ChapterId"] as int?
        //                                });
        //                            }
        //                        }

        //                        // Result set 4: Chapters with actual test names
        //                        if (await reader.NextResultAsync())
        //                        {
        //                            chapters = new List<ChapterListItemViewModel>();
        //                            while (await reader.ReadAsync())
        //                            {
        //                                chapters.Add(new ChapterListItemViewModel
        //                                {
        //                                    Id = Convert.ToInt32(reader["Id"]),
        //                                    TestName = reader["TestName"]?.ToString() ?? string.Empty,
        //                                    TestUrl = reader["TestUrl"] as string ?? string.Empty,
        //                                    ChNumber = reader["OtschNo"].ToString(),
        //                                    McqCount = Convert.ToInt32(reader["McqCount"])
        //                                });
        //                            }

        //                            // Apply the same ordering logic as in LINQ
        //                            chapters = chapters
        //                                .OrderBy(r =>
        //                                {
        //                                    if (int.TryParse(r.ChNumber, out int chNo) && chNo > 0)
        //                                    {
        //                                        return chNo;
        //                                    }

        //                                    var chapterMatch = System.Text.RegularExpressions.Regex.Match(r.TestName ?? "", @"Chapter\s+(\d+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        //                                    if (chapterMatch.Success && int.TryParse(chapterMatch.Groups[1].Value, out int chapterFromName))
        //                                    {
        //                                        return chapterFromName;
        //                                    }

        //                                    var anyMatch = System.Text.RegularExpressions.Regex.Match(r.TestName ?? "", @"\d+");
        //                                    if (anyMatch.Success && int.TryParse(anyMatch.Value, out int anyNo))
        //                                    {
        //                                        return anyNo;
        //                                    }

        //                                    return int.MaxValue;
        //                                })
        //                                .ThenBy(r => r.TestName)
        //                                .ToList();
        //                        }

        //                        // Result set 5: ResultType
        //                        if (await reader.NextResultAsync() && await reader.ReadAsync())
        //                        {
        //                            finalResultType = reader["ResultType"].ToString();
        //                        }
        //                    }
        //                }

        //                var questionViewModels = questions.Select((q, index) => new TestQuestionViewModel
        //                {
        //                    QuestionNumber = index + 1,
        //                    QuestionText = q.Question,
        //                    QuestionImageUrl = string.IsNullOrEmpty(q.QuestionImage)
        //                        ? null
        //                        : $"{q.Id}_{q.QuestionImage}",
        //                    Id = q.Id,
        //                    CorrectOption = q.CorrectAnswer,
        //                    Options = new List<TestOptionViewModel>
        //            {
        //                new TestOptionViewModel
        //                {
        //                    OptionText = q.Choice1,
        //                    OptionLabel = "A",
        //                    OptionImageUrl = string.IsNullOrEmpty(q.Choice1img) ? null : $"{q.Id}_{q.Choice1img}"
        //                },
        //                new TestOptionViewModel
        //                {
        //                    OptionText = q.Choice2,
        //                    OptionLabel = "B",
        //                    OptionImageUrl = string.IsNullOrEmpty(q.Choice2img) ? null : $"{q.Id}_{q.Choice2img}"
        //                },
        //                new TestOptionViewModel
        //                {
        //                    OptionText = q.Choice3,
        //                    OptionLabel = "C",
        //                    OptionImageUrl = string.IsNullOrEmpty(q.Choice3img) ? null : $"{q.Id}_{q.Choice3img}"
        //                },
        //                new TestOptionViewModel
        //                {
        //                    OptionText = q.Choice4,
        //                    OptionLabel = "D",
        //                    OptionImageUrl = string.IsNullOrEmpty(q.Choice4img) ? null : $"{q.Id}_{q.Choice4img}"
        //                },
        //                new TestOptionViewModel
        //                {
        //                    OptionText = q.Choice5,
        //                    OptionLabel = "E",
        //                    OptionImageUrl = string.IsNullOrEmpty(q.Choice5img) ? null : $"{q.Id}_{q.Choice5img}"
        //                }
        //            }
        //                    .Where(o => !string.IsNullOrWhiteSpace(o.OptionText) || !string.IsNullOrWhiteSpace(o.OptionImageUrl))
        //                    .ToList()
        //                }).ToList();

        //                // Convert sample questions to McqQuestionViewModel (simpler structure)
        //                var sampleMcqViewModels = sampleQuestions.Select((q, index) => new McqQuestionViewModel
        //                {
        //                    Id = q.Id,
        //                    Question = q.Question,
        //                    QuestionImage = string.IsNullOrEmpty(q.QuestionImage) ? null : $"{q.Id}_{q.QuestionImage}",
        //                    Choice1 = q.Choice1,
        //                    Choice1Img = q.Choice1img,
        //                    Choice2 = q.Choice2,
        //                    Choice2Img = q.Choice2img,
        //                    Choice3 = q.Choice3,
        //                    Choice3Img = q.Choice3img,
        //                    Choice4 = q.Choice4,
        //                    Choice4Img = q.Choice4img,
        //                    CorrectAnswer = q.CorrectAnswer
        //                }).ToList();

        //                var chapterIds = testDetail.ChapterIds?
        //                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
        //                    .Select(id => int.TryParse(id.Trim(), out var cid) ? cid : -1)
        //                    .Where(cid => cid > 0)
        //                    .ToList() ?? new List<int>();

        //                var viewModel = new OnlineTestChapterListViewModel
        //                {
        //                    Test = test,
        //                    TimeAllowed = testDetail.Time,
        //                    TotalQuestions = testDetail.TotalQuestions,
        //                    TotalMarks = testDetail.Marks,
        //                    Questions = questionViewModels,
        //                    SampleMcqs = sampleMcqViewModels, // Use the new sample questions
        //                    IsLoggedIn = User.Identity.IsAuthenticated,
        //                    TestUrl = test.Url,
        //                    OtschNo = otschNo,
        //                    OtschName = otschName
        //                };

        //                if (finalResultType == "SINGLE_CHAPTER")
        //                {
        //                    int actualQuestionCount = await _context.TblOtsTestMcqs
        //                        .CountAsync(q => q.ChapterId.HasValue && chapterIds.Contains(q.ChapterId.Value));

        //                    viewModel.TestSets = GenerateTestSets(url, actualQuestionCount, testDetail.TotalQuestions);
        //                }
        //                else
        //                {
        //                    viewModel.Chapters = chapters ?? new List<ChapterListItemViewModel>();
        //                }

        //                var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/online-test/{url}");
        //                ViewBag.CmsData = cmsData;
        //                ViewBag.Banners = await _bannerService.GetBannersAsync();
        //                ViewBag.IsLoggedIn = User.Identity.IsAuthenticated;
        //                viewModel.TopScorers = await GetTopScorersForTestAsync(test.Id);

        //                return View(viewModel);
        //            }
        //        }
        //    }
        //}



        private static string FormatOption(object id, object img)
        {
            return string.IsNullOrEmpty(img?.ToString()) ? null : $"{id}_{img}";
        }





        private async Task<List<McqQuestionViewModel>> GetSampleMcqs(TblOtsTestCriteriaDetail testDetail, List<int> chapterIds)
        {
            List<McqQuestionViewModel> questionList = new List<McqQuestionViewModel>();

            if (chapterIds.Count == 1) // Single Chapter - get sample from this chapter
            {
                int chapterId = chapterIds.First();
                questionList = await _context.TblOtsTestMcqs
                    .Where(q => q.ChapterId == chapterId)
                    .OrderBy(q => Guid.NewGuid())
                    .Take(10)
                    .Select(q => new McqQuestionViewModel
                    {
                        Question = q.Question,
                        QuestionImage = q.QuestionImage,
                        Choice1 = q.Choice1,
                        Choice2 = q.Choice2,
                        Choice3 = q.Choice3,
                        Choice4 = q.Choice4,
                        CorrectAnswer = q.CorrectAnswer
                    })
                    .ToListAsync();
            }
            else // Multiple or null chapters - get sample from subject
            {
                var subjectId = testDetail.SubjectId;

                // Get all chapters of this subject
                var subjectChapterIds = await _context.TblOtsChapters
                    .Where(c => c.OtssubjectId == subjectId)
                    .Select(c => c.Id)
                    .ToListAsync();

                questionList = await _context.TblOtsTestMcqs
                    .Where(q => q.ChapterId.HasValue && subjectChapterIds.Contains(q.ChapterId.Value))
                    .OrderBy(q => Guid.NewGuid())
                    .Take(10)
                    .Select(q => new McqQuestionViewModel
                    {
                        Question = q.Question,
                        QuestionImage = q.QuestionImage,
                        Choice1 = q.Choice1,
                        Choice2 = q.Choice2,
                        Choice3 = q.Choice3,
                        Choice4 = q.Choice4,
                        CorrectAnswer = q.CorrectAnswer
                    })
                    .ToListAsync();
            }

            return questionList;
        }

        public async Task<List<ChapterListItemViewModel>> GetChapterTestsBySubject(int subjectId, string testUrl = "")
        {
            // Step 1: Extract stream
            string stream = ExtractStreamFromUrl(testUrl);
            string lowerUrl = testUrl?.ToLower() ?? "";
            bool isEcat = lowerUrl.Contains("ecat");
            bool hasPreEngOrSci = lowerUrl.Contains("pre-engineering") || lowerUrl.Contains("general-science");
            bool excludePreEngAndSci = isEcat && !hasPreEngOrSci;

            // Step 2: Get all chapters for the subject
            var chapters = await _context.TblOtsChapters
                .Where(ch => ch.OtssubjectId == subjectId)
                .ToListAsync();

            var chapterIdsStr = chapters.Select(c => c.Id.ToString()).ToList();

            // Step 3: Get all test details for these chapters
            var testDetails = await _context.TblOtsTestCriteriaDetails
                .Where(d => chapterIdsStr.Contains(d.ChapterIds) && d.SubjectId == subjectId)
                .ToListAsync();

            var testIds = testDetails.Select(d => d.TestId).Distinct().ToList();

            // Step 4: Stream-specific filtering
            var testsQuery = _context.TblOtsTestCriteria
                .Where(t => testIds.Contains(t.Id) && t.IsActive == true);

            // ✅ Apply stream filter (only same stream tests)
            if (!string.IsNullOrEmpty(stream))
            {
                testsQuery = testsQuery.Where(t =>
                    t.Url.ToLower().StartsWith($"{stream}-") ||                // e.g. "ics-part-1"
                    t.Url.ToLower().Contains($"/{stream}/") ||                 // e.g. "/ics/"
                    t.Url.ToLower().Contains($"-{stream}-") ||                 // e.g. "-ics-"
                    t.Url.ToLower().EndsWith($"-{stream}") ||                  // e.g. "part-1-ics"
                    t.Url.ToLower().EndsWith($"/{stream}")                     // e.g. "/ics"
                );
            }


            // ✅ Apply ECAT stream exclusion logic
            if (excludePreEngAndSci)
            {
                testsQuery = testsQuery.Where(t =>
                    !t.Url.ToLower().Contains("pre-engineering") &&
                    !t.Url.ToLower().Contains("general-science"));
            }

            var tests = await testsQuery.ToListAsync();


            // Step 5: Build final list
            var result = (
                from d in testDetails
                join ch in chapters on d.ChapterIds equals ch.Id.ToString()
                join t in tests on d.TestId equals t.Id
                select new ChapterListItemViewModel
                {
                    Id = ch.Id,
                    TestName = t.TestName,
                    TestUrl = t.Url,
                    ChNumber = ch.OtschNo.ToString(),
                    McqCount = _context.TblOtsTestMcqs.Count(m => m.ChapterId == ch.Id)
                }
            )
            .Distinct()
            .ToList();
            result = result
    .OrderBy(r =>
    {
        // Step 1: Try direct ChNumber
        if (int.TryParse(r.ChNumber, out int chNo) && chNo > 0)
        {
            return chNo;
        }

        // Step 2: Regex to capture "Chapter X"
        var chapterMatch = System.Text.RegularExpressions.Regex.Match(r.TestName ?? "", @"Chapter\s+(\d+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        if (chapterMatch.Success && int.TryParse(chapterMatch.Groups[1].Value, out int chapterFromName))
        {
            return chapterFromName;
        }

        // Step 3: Fallback → koi aur number agar "Chapter" na ho
        var anyMatch = System.Text.RegularExpressions.Regex.Match(r.TestName ?? "", @"\d+");
        if (anyMatch.Success && int.TryParse(anyMatch.Value, out int anyNo))
        {
            return anyNo;
        }

        // Step 4: Agar kuch bhi na mile → end me bhej do
        return int.MaxValue;
    })
    .ThenBy(r => r.TestName) // same number wale alphabetically
    .ToList();



            return result;
        }


        private List<TestSetViewModel> GenerateTestSets(string testUrl, int actualQuestionCount, int questionsPerSet)
        {
            var totalSets = (int)Math.Ceiling((double)actualQuestionCount / questionsPerSet);
            var testSets = new List<TestSetViewModel>();

            for (int i = 1; i <= totalSets; i++)
            {
                // Calculate remaining questions
                int startIndex = (i - 1) * questionsPerSet;
                int remainingQuestions = actualQuestionCount - startIndex;
                int currentSetQuestions = Math.Min(questionsPerSet, remainingQuestions);

                testSets.Add(new TestSetViewModel
                {
                    SetName = $"Set {i}",
                    TotalQuestions = currentSetQuestions, // ✅ Fix here
                    StartUrl = $"/online-test/start/{testUrl}?set={i}"
                });
            }

            return testSets;
        }

		private List<TestSetViewModel> BoardGenerateTestSets(string testUrl, int actualQuestionCount, int questionsPerSet)
		{
			var totalSets = (int)Math.Ceiling((double)actualQuestionCount / questionsPerSet);
			var testSets = new List<TestSetViewModel>();

			for (int i = 1; i <= totalSets; i++)
			{
				// Calculate remaining questions
				int startIndex = (i - 1) * questionsPerSet;
				int remainingQuestions = actualQuestionCount - startIndex;
				int currentSetQuestions = Math.Min(questionsPerSet, remainingQuestions);

				testSets.Add(new TestSetViewModel
				{
					SetName = $"Set {i}",
					TotalQuestions = currentSetQuestions, // ✅ Fix here
					StartUrl = $"/online-test/start/{testUrl}?set={i}"
				});
			}

			return testSets;
		}
		public async Task<List<TestTopScorerDto>> GetTopScorersForTestAsync(int testId)
        {
            var latestTests = await _context.TblTests
        .Where(t => t.TestId == testId && t.MemberId != null)
        .GroupBy(t => t.MemberId)
        .Select(g => new
        {
            MemberId = g.Key,
            LatestTest = g.OrderByDescending(x => x.ObtainedMarks).ThenBy(x => x.TestTime).FirstOrDefault()
        })
        .ToListAsync();

            // DEBUG: Check if MemberId = 1 exists
            Console.WriteLine($"Total unique members: {latestTests.Count}");

            var member1 = latestTests.FirstOrDefault(x => x.MemberId == 1);
            if (member1 != null)
            {
                Console.WriteLine($"MemberId 1 FOUND - ObtainedMarks: {member1.LatestTest?.ObtainedMarks}, Dated: {member1.LatestTest?.Dated}");
            }
            else
            {
                Console.WriteLine("MemberId 1 NOT FOUND in latestTests");

                // Debug: Check all members
                Console.WriteLine("All MemberIds found:");
                foreach (var item in latestTests)
                {
                    Console.WriteLine($"MemberId: {item.MemberId}, Marks: {item.LatestTest?.ObtainedMarks}, Date: {item.LatestTest?.Dated}");
                }
            }

            // Extract the test records and order by date
            var result = latestTests
                .Select(x => x.LatestTest)
                .Where(x => x != null)
                .OrderByDescending(x => x.Dated)
                .Take(50)
                .ToList();

            // DEBUG: Check if MemberId = 1 exists in final result
            var member1InResult = result.FirstOrDefault(x => x.MemberId == 1);
            if (member1InResult != null)
            {
                Console.WriteLine($"MemberId 1 FOUND in result - ObtainedMarks: {member1InResult.ObtainedMarks}");
            }
            else
            {
                Console.WriteLine("MemberId 1 NOT FOUND in final result");
            }

            // Get top 15 by marks (high) and time (low)
            var top15 = result
                .OrderByDescending(x => x.ObtainedMarks)
                .ThenBy(x => x.TestTime)
                .Take(15)
                .ToList();

            // Get member IDs for batch query - convert to decimal to match TblDefMemberInfo2s
            var memberIds = top15
                .Where(t => t.MemberId.HasValue)
                .Select(t => (decimal)t.MemberId.Value)
                .Distinct()
                .ToList();

            // Get member information in one query - use decimal type
            var members = await _context.TblDefMemberInfoikds
                .Where(m => memberIds.Contains(m.MemberId))
                .ToDictionaryAsync(m => m.MemberId, m => m.MemberName);

            // Create DTOs - convert back to int for comparison
            var top15Dtos = top15.Where(t => t.MemberId.HasValue).Select(t => new TestTopScorerDto
            {
                MemberName = members.TryGetValue((decimal)t.MemberId.Value, out var name) ? name : "Unknown",
                TotalMarks = (int)(t.TotalMarks ?? 0),
                ObtainedMarks = (int)(t.ObtainedMarks ?? 0),
                Dated = t.Dated ?? new DateTime(1900, 1, 1),
                TestTime = t.TestTime.HasValue ? TimeSpan.FromSeconds(t.TestTime.Value) : null
            }).ToList();

            return top15Dtos;
        }





        //public async Task<List<TestTopScorerDto>> GetTopScorersForTestAsync(int testId)
        //{
        //    var twelveMonthsAgo = DateTime.Now.AddMonths(-12);

        //    var result = await (
        //        from t in _context.TblTests
        //        join m in _context.TblDefMemberInfo2s
        //            on (decimal?)t.MemberId equals m.MemberId
        //        join c in _context.TblDefCities
        //            on m.CityId equals (decimal?)c.CityId into cityGroup
        //        from c in cityGroup.DefaultIfEmpty()

        //        where t.TestId == testId
        //              && t.IsPassed == true

        //              && t.Dated >= twelveMonthsAgo   // ✅ last 12 months filter

        //        orderby t.ObtainedMarks descending, t.TestTime ascending

        //        select new TestTopScorerDto
        //        {
        //            MemberId = (int)m.MemberId,
        //            MemberName = m.MemberName,
        //            TestId = t.Id,
        //            TotalMarks = (int)(t.TotalMarks ?? 0),
        //            ObtainedMarks = (int)(t.ObtainedMarks ?? 0),
        //            Dated = t.Dated ?? new DateTime(1900, 1, 1),
        //            CityName = c != null ? c.CityName : "Unknown",
        //            ImageName = m.ImageName,
        //            TestTime = t.TestTime.HasValue
        //                ? TimeSpan.FromSeconds(t.TestTime.Value)
        //                : null,
        //        }
        //    )
        //    .Take(15) // ✅ top 15 scorers
        //    .ToListAsync();

        //    return result;

        //}




        public static string ExtractStreamFromUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return "";

            var lowerUrl = url.ToLower();

            // check for start, middle, or end match
            if (lowerUrl.StartsWith("fsc") || lowerUrl.Contains("/fsc") || lowerUrl.Contains("-fsc"))
                return "fsc";
            else if (lowerUrl.StartsWith("ics") || lowerUrl.Contains("/ics") || lowerUrl.Contains("-ics"))
                return "ics";
            else if (lowerUrl.StartsWith("icom") || lowerUrl.Contains("/icom") || lowerUrl.Contains("-icom"))
                return "icom";
            else if (lowerUrl.StartsWith("fa") || lowerUrl.Contains("/fa") || lowerUrl.Contains("-fa"))
                return "fa";
            else if (lowerUrl.Contains("pre-engineering"))
                return "pre-engineering";
            else if (lowerUrl.Contains("pre-general-science") || lowerUrl.Contains("general-science"))
                return "general-science";

            return "";
        }





        public async Task<IActionResult> GetClasses(string url)
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/online-test/{url}");
            if (cmsData == null)
            {
                return NotFound();
            }
            ViewBag.CmsData = cmsData;
            return View("GetClasses");
        }


        [Route("online-test/{subjectUrl}-quiz")]
        public async Task<IActionResult> CustomTestsHome(string subjectUrl)
        {
            // Step 1: Get SubjectId (used as CategoryID)
            var subject = await _context.TblOtssubjects
                .FirstOrDefaultAsync(s => s.OtsSubjectUrl == subjectUrl && s.OtsClassId == 51);

            if (subject == null)
            {
                return await CustomTestsDetail(subjectUrl);
            }

            var categoryId = subject.Id;


            // Step 1: Get all tests for this subject/class
            var testInfoList = await (
                from d in _context.TblOtsTestCriteriaDetails
                join c in _context.TblOtsTestCriteria
                    on d.TestId equals c.Id
                where d.SubjectId == categoryId && d.ClassId == 51
                select new
                {
                    c.Id,
                    c.TestName,
                    c.Url,
                    c.ImageName
                }
            ).ToListAsync();

            // Step 2: Get all TestIds
            var testIds = testInfoList.Select(x => (int)x.Id).ToList();

            // Step 3: Query TblTest only once, grouped and filtered
            var attemptCounts = await _context.TblTests
                .Where(t => t.TestId.HasValue && testIds.Contains(t.TestId.Value))
                .GroupBy(t => t.TestId)
                .Select(g => new { TestId = g.Key.Value, Count = g.Count() })
                .ToDictionaryAsync(x => x.TestId, x => x.Count);

            // Step 4: Combine both
            var tests = testInfoList.Select(t => new TestListItemViewModel
            {
                TestId = t.Id,
                TestName = t.TestName,
                Url = t.Url,
                ImageName = t.ImageName,
                AttemptedCount = attemptCounts.ContainsKey(t.Id) ? attemptCounts[t.Id] : 0
            }).ToList();


            ViewBag.SubjectName = subject.OtsSubjectName;
            ViewBag.Banners = await _bannerService.GetBannersAsync();
            //var cmsData = await _context.TblCms
            //                            .FirstOrDefaultAsync(c => c.Url == $"https://www.ilmkidunya.com/online-test/{subjectUrl}-quiz");
            var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/online-test/{subjectUrl}-quiz");


            ViewBag.CmsData = cmsData;

            return View(tests);
        }

        public async Task<IActionResult> CustomTestsDetail(string subjectUrl)
        {
            var test = await _context.TblOtsTestCriteria.FirstOrDefaultAsync(x => x.Url == subjectUrl);
            if (test == null)
                return NotFound();

            var testDetail = await _context.TblOtsTestCriteriaDetails.FirstOrDefaultAsync(td => td.TestId == test.Id);
            if (testDetail == null)
                return NotFound();

            // ✅ Step 1: Prepare Related Tests
            int? categoryId = testDetail?.SubjectId;
            List<TestListItemViewModel> relatedTests = new();

            if (categoryId != null)
            {

                // Step 1: Get all tests for this subject/class
                var testInfoList = await (
                    from d in _context.TblOtsTestCriteriaDetails
                    join c in _context.TblOtsTestCriteria
                        on d.TestId equals c.Id
                    where d.SubjectId == categoryId && d.ClassId == 51 && c.Id != test.Id
                    select new
                    {
                        c.Id,
                        c.TestName,
                        c.Url,
                        c.ImageName
                    }
                ).ToListAsync();

                // Step 2: Get all TestIds
                var testIds = testInfoList.Select(x => (int)x.Id).ToList();

                // Step 3: Query TblTest only once, grouped and filtered
                var attemptCounts = await _context.TblTests
                    .Where(t => t.TestId.HasValue && testIds.Contains(t.TestId.Value))
                    .GroupBy(t => t.TestId)
                    .Select(g => new { TestId = g.Key.Value, Count = g.Count() })
                    .ToDictionaryAsync(x => x.TestId, x => x.Count);

                // Step 4: Combine both
                relatedTests = testInfoList.Select(t => new TestListItemViewModel
                {
                    TestId = t.Id,
                    TestName = t.TestName,
                    Url = t.Url,
                    ImageName = t.ImageName,
                    AttemptedCount = attemptCounts.ContainsKey(t.Id) ? attemptCounts[t.Id] : 0
                }).ToList();

            }

            // ✅ Step 2: Fetch Questions
            var chapterIds = testDetail.ChapterIds?
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => int.TryParse(id.Trim(), out var cid) ? cid : -1)
                .Where(cid => cid > 0)
                .ToList() ?? new List<int>();

            List<TblOtsTestMcq> questions;
            int? otschNo = null;
            string otschName = null;

            if (chapterIds.Count == 1)
            {
                // ✅ Single Chapter Test
                int chapterId = chapterIds.First();
                questions = await _context.TblOtsTestMcqs
                    .Where(q => q.ChapterId == chapterId)
                    .OrderBy(q => Guid.NewGuid())
                    .Take(testDetail.TotalQuestions)
                    .ToListAsync();

                var chapter = await _context.TblOtsChapters
                    .Where(c => c.Id == chapterId)
                    .Select(c => new { c.OtschNo, c.OtschName })
                    .FirstOrDefaultAsync();

                if (chapter != null)
                {
                    otschNo = chapter.OtschNo;
                    otschName = chapter.OtschName;
                }
            }
            else
            {
                // ✅ Full Book Test
                var subjectId = testDetail.SubjectId;
                var subjectChapterIds = await _context.TblOtsChapters
                    .Where(c => c.OtssubjectId == subjectId)
                    .Select(c => c.Id)
                    .ToListAsync();

                questions = await _context.TblOtsTestMcqs
                    .Where(q => q.ChapterId.HasValue && subjectChapterIds.Contains(q.ChapterId.Value))
                    .OrderBy(q => Guid.NewGuid())
                    .Take(testDetail.TotalQuestions)
                    .ToListAsync();
            }

            // ✅ Step 3: Map to ViewModel
            var questionViewModels = questions.Select((q, index) => new TestQuestionViewModel
            {
                QuestionNumber = index + 1,
                QuestionText = q.Question,
                QuestionImageUrl = string.IsNullOrEmpty(q.QuestionImage)
                    ? null
                    : $"{q.Id}_{q.QuestionImage}",
                Id = q.Id,
                CorrectOption = q.CorrectAnswer,
                Options = new List<TestOptionViewModel>
        {
            new() { OptionText = q.Choice1, OptionLabel = "A", OptionImageUrl = string.IsNullOrEmpty(q.Choice1img) ? null : $"{q.Id}_{q.Choice1img}" },
            new() { OptionText = q.Choice2, OptionLabel = "B", OptionImageUrl = string.IsNullOrEmpty(q.Choice2img) ? null : $"{q.Id}_{q.Choice2img}" },
            new() { OptionText = q.Choice3, OptionLabel = "C", OptionImageUrl = string.IsNullOrEmpty(q.Choice3img) ? null : $"{q.Id}_{q.Choice3img}" },
            new() { OptionText = q.Choice4, OptionLabel = "D", OptionImageUrl = string.IsNullOrEmpty(q.Choice4img) ? null : $"{q.Id}_{q.Choice4img}" },
            new() { OptionText = q.Choice5, OptionLabel = "E", OptionImageUrl = string.IsNullOrEmpty(q.Choice5img) ? null : $"{q.Id}_{q.Choice5img}" }
        }.Where(o => !string.IsNullOrWhiteSpace(o.OptionText) || !string.IsNullOrWhiteSpace(o.OptionImageUrl)).ToList()
            }).ToList();

            // ✅ Step 4: Prepare Model
            var model = new OnlineTestChapterListViewModel
            {
                Test = test,
                TestDetail = testDetail,
                RelatedTests = relatedTests,
                TotalMarks = testDetail.Marks,
                Questions = questionViewModels,
                OtschNo = otschNo,
                OtschName = otschName,
                TotalQuestions = testDetail.TotalQuestions,
                TimeAllowed = testDetail.Time,
                IsLoggedIn = User.Identity.IsAuthenticated,
                TestUrl = test.Url,
                TopScorers = await GetTopScorersForTestAsync(test.Id)
            };

            // ✅ Step 5: Add TestSets or Chapter Tests
            if (chapterIds.Count == 1)
            {
                int actualQuestionCount = await _context.TblOtsTestMcqs
                    .CountAsync(q => q.ChapterId.HasValue && chapterIds.Contains(q.ChapterId.Value));

                model.TestSets = GenerateTestSets(subjectUrl, actualQuestionCount, testDetail.TotalQuestions);
            }
            else
            {
                model.Chapters = await GetChapterTestsBySubject(testDetail.SubjectId, test.Url);
            }

            // ✅ Step 6: Load CMS + Banners
            ViewBag.Banners = await _bannerService.GetBannersAsync();

            //var cmsData = await _context.TblCms
            //    .FirstOrDefaultAsync(c => c.Url == $"https://www.ilmkidunya.com/online-test/{test.Url}");
            var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/online-test/{test.Url}");
            ViewBag.CmsData = cmsData;



            return View("CustomTestsDetail", model);
        }


        //public async Task<IActionResult> CustomTestsDetail(string subjectUrl)
        //{

        //    var test = await _context.TblOtsTestCriteria.FirstOrDefaultAsync(x => x.Url == subjectUrl);
        //    if (test == null)
        //        return NotFound();

        //    var testDetail = await _context.TblOtsTestCriteriaDetails.FirstOrDefaultAsync(td => td.TestId == test.Id);


        //    int? categoryId = testDetail?.SubjectId;
        //    List<TestListItemViewModel> relatedTests = new();

        //    if (categoryId != null)
        //    {
        //        relatedTests = await (
        //            from c in _context.CategoryTests
        //            join o in _context.TblOtsTestCriteria on c.TestId equals o.Id
        //            where c.CategoryId == categoryId
        //            select new TestListItemViewModel
        //            {
        //                TestId = o.Id,
        //                TestName = o.TestName,
        //                Url = o.Url,
        //                ImageName = o.ImageName
        //            }).ToListAsync();
        //    }


        //    var model = new OnlineTestChapterListViewModel
        //    {
        //        Test = test,
        //        TestDetail = testDetail,
        //        RelatedTests = relatedTests
        //    };

        //    ViewBag.Banners = await _bannerService.GetBannersAsync();

        //    var cmsData = await _context.TblCms
        //                                .FirstOrDefaultAsync(c => c.Url == $"https://www.ilmkidunya.com/online-test/{test.Url}");
        //    ViewBag.CmsData = cmsData;
        //    if (cmsData != null)
        //    {
        //        ViewBag.Heading1 = ExtractFirstTagContentFromBoth(cmsData.Desc1 ?? "", cmsData.Desc2 ?? "", "h1");
        //        ViewBag.Heading2 = ExtractFirstTagContentFromBoth(cmsData.Desc1 ?? "", cmsData.Desc2 ?? "", "h2");
        //        ViewBag.Heading3 = ExtractFirstTagContentFromBoth(cmsData.Desc1 ?? "", cmsData.Desc2 ?? "", "h3");

        //        var desc1Paras = ExtractParagraphs(cmsData.Desc1 ?? "");
        //        var desc2Paras = ExtractParagraphs(cmsData.Desc2 ?? "");

        //        for (int i = 0; i < desc1Paras.Count; i++)
        //            ViewData[$"Desc1Paragraph{i + 1}"] = desc1Paras[i];

        //        for (int i = 0; i < desc2Paras.Count; i++)
        //            ViewData[$"Desc2Paragraph{i + 1}"] = desc2Paras[i];

        //    }
        //    return View("CustomTestsDetail", model);
        //}


        [Route("online-test/{testUrl}-test-home")]
        public async Task<IActionResult> CustomTestsHome2(string testUrl)
        {
            var test = await _context.TblOtsTestCriteria.FirstOrDefaultAsync(x => x.Url.Replace(".aspx", "") == testUrl);
            if (test == null)
                return NotFound();

            var testDetail = await _context.TblOtsTestCriteriaDetails.FirstOrDefaultAsync(x => x.TestId == test.Id);
            if (testDetail == null)
                return NotFound();

            // Get class name
            var className = await _context.TblOtsclasses
                .Where(c => c.Id == testDetail.ClassId)
                .Select(c => c.OtsclassName)
                .FirstOrDefaultAsync();

            // Get subject name
            var subjectName = await _context.TblOtssubjects
                .Where(s => s.Id == testDetail.SubjectId)
                .Select(s => s.OtsSubjectName)
                .FirstOrDefaultAsync();

            // Get first chapter name (you can enhance this if multiple needed)
            string chapterName = "";
            if (!string.IsNullOrEmpty(testDetail.ChapterIds))
            {
                var firstChapterId = testDetail.ChapterIds
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => int.TryParse(id.Trim(), out var cid) ? cid : -1)
                    .FirstOrDefault(cid => cid > 0);

                chapterName = await _context.TblOtsChapters
                    .Where(c => c.Id == firstChapterId)
                    .Select(c => c.OtschName)
                    .FirstOrDefaultAsync();
            }

            // SEO Meta Tags
            ViewBag.Title = $"Online MCQ Test for {className} {subjectName} Unit {chapterName}";
            ViewBag.Description = $"Practice objective type MCQ questions for {subjectName} {className} Unit {chapterName} prepare online. {subjectName} {className} MCQs with answers PDF download.";
            ViewBag.Keywords = $"{subjectName} {className} mcqs, {className} {subjectName} mcq questions with answers, online test {className} {subjectName}";

            // Question count and sets
            var chapterIds = testDetail.ChapterIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => int.TryParse(id.Trim(), out var cid) ? cid : -1)
                .Where(cid => cid > 0)
                .ToList();

            int actualQuestionCount = await _context.TblOtsTestMcqs
                .CountAsync(q => q.ChapterId.HasValue && chapterIds.Contains(q.ChapterId.Value));

            int questionsPerSet = testDetail.TotalQuestions;
            int totalSets = (int)Math.Ceiling((double)actualQuestionCount / questionsPerSet);

            var model = new CustomTestHomeViewModel
            {
                TestTitle = test.TestName,
                TestUrl = test.Url,
                TotalMarks = testDetail.Marks,
                TotalQuestions = testDetail.TotalQuestions,
                DurationMinutes = int.TryParse(testDetail.Time, out var minutes) ? minutes : 0,
                TestSets = new List<TestSetViewModel>()
            };

            for (int i = 1; i <= totalSets; i++)
            {
                model.TestSets.Add(new TestSetViewModel
                {
                    SetName = $"Set {i}",
                    TotalQuestions = questionsPerSet,
                    StartUrl = $"/online-test/start/{testUrl}?set={i}"
                });
            }

            ViewBag.Banners = await _bannerService.GetBannersAsync();
            //var cmsData = await _context.TblCms
            //    .FirstOrDefaultAsync(c => c.Url == $"https://www.ilmkidunya.com/online-test/{testUrl}-test-home");
            var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/online-test/{testUrl}-test-home");
            ViewBag.CmsData = cmsData;

            return View("CustomTestsHome2", model);
        }


        [Route("online-test/{testUrl}-test-form")]
        public async Task<IActionResult> TestStart(string testUrl)
        {
            // Get Test by URL
            var test = await _context.TblOtsTestCriteria.FirstOrDefaultAsync(x => x.Url.Replace(".aspx", "") == testUrl);
            if (test == null)
                return NotFound();

            // Get Test Details
            var testDetail = await _context.TblOtsTestCriteriaDetails.FirstOrDefaultAsync(x => x.TestId == test.Id);
            if (testDetail == null)
                return NotFound();

            // Parse ChapterIds from comma-separated string
            var chapterIds = string.IsNullOrEmpty(testDetail.ChapterIds)
                ? new List<int>()
                : testDetail.ChapterIds.Split(',').Select(int.Parse).ToList();
            var className = await _context.TblOtsclasses
                .Where(c => c.Id == testDetail.ClassId)
                .Select(c => c.OtsclassName)
                .FirstOrDefaultAsync();

            // Get subject name
            var subjectName = await _context.TblOtssubjects
                .Where(s => s.Id == testDetail.SubjectId)
                .Select(s => s.OtsSubjectName)
                .FirstOrDefaultAsync();

            // Get first chapter name (you can enhance this if multiple needed)
            string chapterName = "";
            if (!string.IsNullOrEmpty(testDetail.ChapterIds))
            {
                var firstChapterId = testDetail.ChapterIds
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => int.TryParse(id.Trim(), out var cid) ? cid : -1)
                    .FirstOrDefault(cid => cid > 0);

                chapterName = await _context.TblOtsChapters
                    .Where(c => c.Id == firstChapterId)
                    .Select(c => c.OtschName)
                    .FirstOrDefaultAsync();
            }

            ViewBag.ChName = chapterName;
            ViewBag.Title = $"Online MCQ Test for {className} {subjectName} Unit {chapterName}";
            ViewBag.Description = $"Practice objective type MCQ questions for {subjectName} {className} Unit {chapterName} prepare online. {subjectName} {className} MCQs with answers PDF download.";
            ViewBag.Keywords = $"{subjectName} {className} mcqs, {className} {subjectName} mcq questions with answers, online test {className} {subjectName}";

            var totalAvailableQuestions = await _context.TblOtsTestMcqs
                          .CountAsync(q => q.ChapterId.HasValue && chapterIds.Contains(q.ChapterId.Value));

            ViewBag.TotalAvailableQuestions = totalAvailableQuestions;

            var questions = await _context.TblOtsTestMcqs
                 .Where(q => q.ChapterId.HasValue && chapterIds.Contains(q.ChapterId.Value))
                                .OrderBy(q => Guid.NewGuid())
                                .Take(testDetail.TotalQuestions)
                                .ToListAsync();


            var questionViewModels = questions.Select((q, index) => new TestQuestionViewModel
            {
                QuestionNumber = index + 1,
                QuestionText = q.Question,
                QuestionImageUrl = q.QuestionImage, // NEW

                CorrectOption = q.CorrectAnswer,

                Options = new List<TestOptionViewModel>
                                            {
                                                new TestOptionViewModel { OptionText = q.Choice1, OptionLabel = "A", OptionImageUrl = q.Choice1img },
                                                new TestOptionViewModel { OptionText = q.Choice2, OptionLabel = "B", OptionImageUrl = q.Choice2img },
                                                new TestOptionViewModel { OptionText = q.Choice3, OptionLabel = "C", OptionImageUrl = q.Choice3img },
                                                new TestOptionViewModel { OptionText = q.Choice4, OptionLabel = "D", OptionImageUrl = q.Choice4img },
                                                new TestOptionViewModel { OptionText = q.Choice5, OptionLabel = "E", OptionImageUrl = q.Choice5img }
                                            }
                                              .Where(o => !string.IsNullOrWhiteSpace(o.OptionText) || !string.IsNullOrWhiteSpace(o.OptionImageUrl)) // At least one present
                                              .ToList()
            }).ToList();




            var model = new CustomTestHomeViewModel
            {
                TestTitle = test.TestName,
                TestId = test.Id,
                TestUrl = test.Url,
                TotalMarks = testDetail.Marks,
                TotalQuestions = testDetail.TotalQuestions,
                DurationMinutes = int.TryParse(testDetail.Time, out var minutes) ? minutes : 0,
                Questions = questionViewModels
            };

            ViewBag.Banners = await _bannerService.GetBannersAsync();

            return View("TestStart", model);
        }

        [HttpPost]
        [Route("online-test/save-result")]
        public async Task<IActionResult> SaveResult([FromBody] TestResultViewModel input)
        {

            var userIdClaim = User.FindFirst("UserId")?.Value;
            int? userId = null;

            if (int.TryParse(userIdClaim, out int parsedUserId))
            {
                userId = parsedUserId;
            }


            decimal marksPerQuestion = (decimal)input.TotalMarks / input.TotalQuestions;
            decimal obtainedMarks = input.CorrectAnswers * marksPerQuestion;

            bool isPassed = obtainedMarks >= (input.TotalMarks / 2);

            var testResult = new TblTest
            {
                TestType = 1,
                ObtainedMarks = Math.Round(obtainedMarks, 2),
                TotalMarks = input.TotalMarks,
                Dated = DateTime.Now,
                IsPassed = isPassed,
                Url = input.TestUrl,
                TestId = input.TestId,
                MemberId = userId ?? 0,
                TestTime = input.Duration,
                IsBrowser = true,
            };

            _context.TblTests.Add(testResult);
            await _context.SaveChangesAsync();

            return Json(new { success = true, testResultId = testResult.Id });
        }


        [Route("online-test/api/get-test-result/{id}")]
        public async Task<IActionResult> GetTestResultJson(int id)
        {
            var result = await _context.TblTests.FirstOrDefaultAsync(t => t.Id == id);
            if (result == null)
                return NotFound();

            var userName = User.Identity?.Name ?? "Guest";
            string testTitle = "Untitled Test";

            if (!string.IsNullOrEmpty(result.Url))
            {
                string cleanUrl = Path.GetFileNameWithoutExtension(result.Url);
                cleanUrl = cleanUrl.Replace("-", " ");
                testTitle = char.ToUpper(cleanUrl[0]) + cleanUrl.Substring(1);
            }

            var resultData = new
            {
                TestTitle = testTitle,
                UserName = userName,
                TotalQuestions = (int)(result.TotalMarks ?? 0),
                CorrectAnswers = (int)(result.ObtainedMarks ?? 0),
                IsPassed = result.IsPassed ?? false,
                Dated = result.Dated?.ToString("dd MMM yyyy") ?? DateTime.Now.ToString("dd MMM yyyy"),
                PassBgImage = "https://resources.ilmkidunya.com/online-test/assets/images/bg-certificates.jpg",
                FailBgImage = "https://images.ishallwin.com/certi/bg-certificates-fail.jpg",
                LogoUrl = "https://cdn.ilmkidunya.com/logo.svg",
                PassImage = "https://images.ishallwin.com/certi/high-scorrers.png",
                FailImage = "https://images.ishallwin.com/certi/img-fail.png"
            };

            return Json(resultData);
        }






        public async Task<int> GetTestAttemptCountAsync(int testId)
        {
            var count = await _context.TblTests
                .Where(t => t.Id == testId)
                .CountAsync();

            return count;
        }

        [HttpGet]


        [Route("online-test/{testUrl}-mcqs-with-answers")]
        public async Task<IActionResult> TestPreparationWithAnswers(string testUrl, int startfrom = 0, int last = 10)
        {
            if (last <= 0)
                return BadRequest("The 'last' parameter must be greater than zero.");

            var test = await _context.TblOtsTestCriteria
                .FirstOrDefaultAsync(x => x.Url.Replace(".aspx", "") == testUrl.Replace(".aspx", ""));
            if (test == null)
                return NotFound();

            var testDetail = await _context.TblOtsTestCriteriaDetails
                .FirstOrDefaultAsync(x => x.TestId == test.Id);
            if (testDetail == null)
                return NotFound();

            var chapterIds = testDetail.ChapterIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => int.TryParse(id.Trim(), out var cid) ? cid : -1)
                .Where(cid => cid > 0)
                .ToList();

            int takeCount = last - startfrom;
            if (takeCount <= 0)
                return BadRequest("Invalid range: 'last' must be greater than 'startfrom'");

            var questions = await _context.TblOtsTestMcqs
       .Where(q => q.ChapterId.HasValue && chapterIds.Contains(q.ChapterId.Value))
       .OrderByDescending(q => q.Id)
       .Skip(startfrom)
       .Take(takeCount)
       .Select(q => new
       {
           q.Id,
           q.Question,
           QuestionImage = !string.IsNullOrEmpty(q.QuestionImage)
               ? $"{q.Id}_{q.QuestionImage}"
               : null,

           Choices = new[] {
            new {
                Text = q.Choice1,
                Img = string.IsNullOrEmpty(q.Choice1) && !string.IsNullOrEmpty(q.Choice1img)
                    ? $"{q.Id}_{q.Choice1img}"
                    : null,
                Value = "A"
            },
            new {
                Text = q.Choice2,
                Img = string.IsNullOrEmpty(q.Choice2) && !string.IsNullOrEmpty(q.Choice2img)
                    ? $"{q.Id}_{q.Choice2img}"
                    : null,
                Value = "B"
            },
            new {
                Text = q.Choice3,
                Img = string.IsNullOrEmpty(q.Choice3) && !string.IsNullOrEmpty(q.Choice3img)
                    ? $"{q.Id}_{q.Choice3img}"
                    : null,
                Value = "C"
            },
            new {
                Text = q.Choice4,
                Img = string.IsNullOrEmpty(q.Choice4) && !string.IsNullOrEmpty(q.Choice4img)
                    ? $"{q.Id}_{q.Choice4img}"
                    : null,
                Value = "D"
            },
            new {
                Text = q.Choice5,
                Img = string.IsNullOrEmpty(q.Choice5) && !string.IsNullOrEmpty(q.Choice5img)
                    ? $"{q.Id}_{q.Choice5img}"
                    : null,
                Value = "E"
            }
           },
           q.CorrectAnswer
       })
       .ToListAsync();



			return Ok(questions);

		}

		[Route("online-test/board/{testUrl}-mcqs-with-answers")]
		public async Task<IActionResult> BoardTestPreparationWithAnswers(string testUrl, int year = 0, int board = 0, int myclass = 0, int subject = 0)
		{
			var questions = await _contextCollege.BoardOtsmcqs
	   .Where(q => q.BoardId==board && q.YearId==year && q.SubjectId==subject && q.ClassId==myclass)
	   .OrderByDescending(q => q.BoardOtsid)
	   .Select(q => new
	   {
		   q.BoardOtsid,
		   q.Question,
		   QuestionImage = !string.IsNullOrEmpty(q.QuestionImage)
			   ? $"{q.QuestionImage}"
			   : null,

		   Choices = new[] {
			new {
				Text = q.Choice1,
				Img = string.IsNullOrEmpty(q.Choice1) && !string.IsNullOrEmpty(q.Choice1img)
					? $"{q.Choice1img}"
					: null,
				Value = "A"
			},
			new {
				Text = q.Choice2,
				Img = string.IsNullOrEmpty(q.Choice2) && !string.IsNullOrEmpty(q.Choice2img)
					? $"{q.Choice2img}"
					: null,
				Value = "B"
			},
			new {
				Text = q.Choice3,
				Img = string.IsNullOrEmpty(q.Choice3) && !string.IsNullOrEmpty(q.Choice3img)
					? $"{q.Choice3img}"
					: null,
				Value = "C"
			},
			new {
				Text = q.Choice4,
				Img = string.IsNullOrEmpty(q.Choice4) && !string.IsNullOrEmpty(q.Choice4img)
					? $"{q.Choice4img}"
					: null,
				Value = "D"
			},
			new {
				Text = q.Choice5,
				Img = string.IsNullOrEmpty(q.Choice5) && !string.IsNullOrEmpty(q.Choice5img)
					? $"{q.Choice5img}"
					: null,
				Value = "E"
			}
		   },
		   q.CorrectAnswer
	   })
	   .ToListAsync();



			return Ok(questions);

		}


		[Route("online-test/{testUrl}-mcqs-with-answersold")]
        public async Task<IActionResult> TestPreparationWithAnswersold(string testUrl, int startfrom = 0, int last = 10)
        {
            var test = await _context.TblOtsTestCriteria.FirstOrDefaultAsync(x => x.Url.Replace(".aspx", "") == testUrl.Replace(".aspx", ""));
            if (test == null)
                return NotFound();
            ViewBag.TestName = test.TestName;
            ViewBag.TestUrl = test.Url;
            var testDetail = await _context.TblOtsTestCriteriaDetails.FirstOrDefaultAsync(x => x.TestId == test.Id);
            if (testDetail == null)
                return NotFound();

            var chapterIds = testDetail.ChapterIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => int.TryParse(id.Trim(), out var cid) ? cid : -1)
                .Where(cid => cid > 0)
                .ToList();

            var questions = await _context.TblOtsTestMcqs
                .Where(q => q.ChapterId.HasValue && chapterIds.Contains(q.ChapterId.Value))
                .OrderByDescending(q => q.Id)
                .Skip(startfrom)
                .Take(last - startfrom)
                .ToListAsync();

            //var cmsData = await _context.TblCms
            //                        .FirstOrDefaultAsync(c => c.Url == $"https://www.ilmkidunya.com/online-test/{test.Url}");
            var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/online-test/{test.Url}");
            ViewBag.CmsData = cmsData;
            ViewBag.Banners = await _bannerService.GetBannersAsync();
            return View("TestPreparationWithAnswersold", questions);
        }

        [HttpGet("download-questions-pdf/{testUrl}")]
        public async Task<IActionResult> DownloadQuestionsPdf(string testUrl, int startfrom = 0, int last = 10)
        {
            var test = await _context.TblOtsTestCriteria.FirstOrDefaultAsync(x => x.Url.Replace(".aspx", "") == testUrl.Replace(".aspx", ""));
            if (test == null)
                return NotFound();

            var testDetail = await _context.TblOtsTestCriteriaDetails.FirstOrDefaultAsync(x => x.TestId == test.Id);
            if (testDetail == null)
                return NotFound();
            ViewBag.TestName = test.TestName;
            var chapterIds = testDetail.ChapterIds?
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse).ToList() ?? new List<int>();

            var questions = await _context.TblOtsTestMcqs
                              .Where(q => q.ChapterId.HasValue && chapterIds.Contains(q.ChapterId.Value))
                              .OrderBy(q => Guid.NewGuid()) // random order
                              .Take(20) // take max 20
                              .ToListAsync();


            var htmlContent = await this.RenderViewAsync("TestPreparationWithAnswersPdf", questions, true); // Make a minimal version of the view

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
            PaperSize = PaperKind.A4,
            Orientation = Orientation.Portrait,
            DocumentTitle = "MCQs With Answers"
        },
                Objects = {
            new ObjectSettings() {
                HtmlContent = htmlContent,
                WebSettings = { DefaultEncoding = "utf-8" }
            }
        }
            };

            var pdf = _converter.Convert(doc);
            return File(pdf, "application/pdf", $"{testUrl}-mcqs.pdf");
        }


        [HttpGet("download-all-questions/{testUrl}")]
        public async Task<IActionResult> DownloadAllQuestionsPdf(string testUrl)
        {
            var test = await _context.TblOtsTestCriteria.FirstOrDefaultAsync(x => x.Url.Replace(".aspx", "") == testUrl.Replace(".aspx", ""));
            if (test == null)
                return NotFound();

            var testDetail = await _context.TblOtsTestCriteriaDetails.FirstOrDefaultAsync(x => x.TestId == test.Id);
            if (testDetail == null)
                return NotFound();

            ViewBag.TestName = test.TestName;

            var chapterIds = testDetail.ChapterIds?
       .Split(',', StringSplitOptions.RemoveEmptyEntries)
       .Select(id => int.TryParse(id.Trim(), out var cid) ? cid : -1)
       .Where(cid => cid > 0)
       .ToList() ?? new List<int>();

            List<TblOtsTestMcq> questions;

            if (chapterIds.Count == 1) // ✅ Single Chapter Test
            {
                int chapterId = chapterIds.First();

                questions = await _context.TblOtsTestMcqs
                    .Where(q => q.ChapterId == chapterId)
                    .OrderBy(q => Guid.NewGuid()) // randomize
                    .Take(20) // take max 20
                    .ToListAsync();
            }
            else // ✅ Multiple or NULL ChapterIds → Full Subject Questions
            {
                var subjectId = testDetail.SubjectId;

                var subjectChapterIds = await _context.TblOtsChapters
                    .Where(c => c.OtssubjectId == subjectId)
                    .Select(c => c.Id)
                    .ToListAsync();

                questions = await _context.TblOtsTestMcqs
                    .Where(q => q.ChapterId.HasValue && subjectChapterIds.Contains(q.ChapterId.Value))
                    .OrderBy(q => Guid.NewGuid()) // randomize
                    .Take(20) // take max 20
                    .ToListAsync();
            }


            var htmlContent = await this.RenderViewAsync("TestPreparationWithAnswersPdf", questions, true);

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
            PaperSize = PaperKind.A4,
            Orientation = Orientation.Portrait,
            DocumentTitle = "All MCQs With Answers"
        },
                Objects = {
            new ObjectSettings()
            {
                HtmlContent = htmlContent,
                WebSettings = {
                    DefaultEncoding = "utf-8",
                    LoadImages = true
                }
            }
        }
            };

            var pdf = _converter.Convert(doc);
            return File(pdf, "application/pdf", $"{testUrl}-all-mcqs.pdf");
        }




    }
}