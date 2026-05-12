using IKDFrontEnd.Models;

namespace IKDFrontEnd.ViewModels
{
    public class OTSViewModel
    {
        public List<SubjectViewModel> Subjects { get; set; }

    }
    public class ChapterTestViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TestUrl { get; set; }
    }

    public class TopScorerViewModel
    {
        public string UserName { get; set; }
        public int Score { get; set; }
        public int TimeTaken { get; set; }
    }
    public class SubjectViewModel
    {
        public string SubjectName { get; set; }
        public string? ImageUrl { get; set; }
        public string? ClassName { get; set; }
    }
    public class OnlineTestChapterListViewModel
    {
        public List<McqQuestionViewModel> SampleMcqs { get; set; }
        public List<TestSetViewModel> TestSets { get; set; }
		public List<BoardTestSetViewModel> BoardTestSets { get; set; }
		public TblOtsTestCriterion? Test { get; set; }
        public TblOtsTestCriteriaDetail? TestDetail { get; set; }
        public List<ChapterListItemViewModel> Chapters { get; set; }
        public List<TestQuestionViewModel> Questions { get; set; } = new();
        public List<TestTopScorerDto> TopScorers { get; set; }
        public List<TestListItemViewModel> RelatedTests { get; set; }
        public int TotalQuestions { get; set; }
        public string TimeAllowed { get; set; }
        public string ClassName { get; set; }
        public bool IsLoggedIn { get; set; }
        public string TestUrl { get; set; }
        public decimal? TotalMarks { get; set; }
        public int? OtschNo { get; set; }

        public string OtschName { get; set; }
        public List<TopicViewModel> Topics { get; set; } = new List<TopicViewModel>();
        public TblOtssubject Subject { get; set; }
    }
    public class ChapterData
    {
        public int Id { get; set; }
        public int? OtschNo { get; set; }
        public string OtschName { get; set; }
    }
    public class ChapterViewModel
    {
        public int Id { get; set; }
        public int OtschNo { get; set; }
        public string OtschName { get; set; }
        public string OtschUrl { get; set; }
        public int OtssubjectId { get; set; }
        public int OtsclassId { get; set; }
    }

    public class ChapterListItemViewModel
    {
        public int Id { get; set; }
        public string TestName { get; set; }
        public string TestUrl { get; set; }
        public string ChNumber { get; set; }
        public int McqCount { get; set; }
    }

    public class McqQuestionViewModel
    {
        public string Question { get; set; }
        public string? QuestionImage { get; set; }
        public string? Choice1 { get; set; }
        public string? Choice2 { get; set; }
        public string? Choice3 { get; set; }
        public string? Choice4 { get; set; }
		public string? Choice5 { get; set; }
		public int? CorrectAnswer { get; set; }
        public string? Choice1Img { get;  set; }
        public string? Choice2Img { get;  set; }
        public string? Choice3Img { get;  set; }
        public string? Choice4Img { get;  set; }
        public string? Choice5Img { get;  set; }
        public int Id { get; internal set; }
    }
    public class HtmlContentElement
    {
        public string TagName { get; set; }  // "h1", "h2", "p", etc.
        public string Content { get; set; }  // inner text or HTML
        public int? ParagraphNumber { get; set; }  // only for <p> tags
    }

    public class TestListItemViewModel
    {
        public int TestId { get; set; }
        public string TestName { get; set; }
        public string Url { get; set; }
        public string ImageName { get; set; }
        public int AttemptedCount { get; set; }
    }

    public class TestSetViewModel
    {
        public string SetName { get; set; } // Example: "Set 1"
        public int TotalQuestions { get; set; }
        public string StartUrl { get; set; }
    }

    //public class BoardTestSetViewModel
    //{
    //    public string SetName { get; set; } // Example: "Set 1"
    //    public int TotalQuestions { get; set; }
    //    public string StartUrl { get; set; }
    //}

    public class BoardTestSetViewModel
	{
		public int Year { get; set; } // Example: "Set 1"
		public string YearName { get; set; }
		public byte? GroupId { get; set; } // Example: "Set 1"
		public string GroupName { get; set; }
		public int board { get; set; }
		public string boardName { get; set; }
		public int myclass { get; set; }
		public string myclassName { get; set; }
		public int subject { get; set; }
		public string subjectName { get; set; }
		public int TotalQuestions { get; set; }
		public string StartUrl { get; set; }
	}

	public class CustomTestHomeViewModel
    {
        public string TestTitle { get; set; }
        public int TestId { get; set; }
        public int TotalMarks { get; set; }
        public string? TestUrl { get; set; }
        public int TotalQuestions { get; set; }
        public int DurationMinutes { get; set; }
        public List<TestSetViewModel> TestSets { get; set; }
        public List<TestQuestionViewModel> Questions { get; set; } = new();
    }
    public class TestQuestionViewModel
    {
        public int Id { get; set; }
        public int QuestionNumber { get; set; }
        public string QuestionText { get; set; }
        public string? QuestionImage { get; set; }
        public string? QuestionImageUrl { get; set; } // NEW
        public List<TestOptionViewModel> Options { get; set; } = new();
        public int? CorrectOption { get; set; }
        public int? TopicId { get; set; }
    }

    public class TestOptionViewModel
    {
        public string OptionLabel { get; set; } // A, B, C, D, E
        public string OptionText { get; set; }
        public string? OptionImage { get; set; }
        public string? OptionImageUrl { get; set; } // NEW
    }

    public class ChapterPdfViewModel
    {
        public string ChapterName { get; set; }
        public string SubjectName { get; set; }
        public List<McqQuestionViewModel> Questions { get; set; }
    }


    public class CustomTestsDetailViewModel
    {
        public TblOtsTestCriterion Test { get; set; }
        public TblOtsTestCriteriaDetail? TestDetail { get; set; }
        public List<TestListItemViewModel> RelatedTests { get; set; }
    }

    public class TestTopScorerDto
    {
        public int MemberId { get; set; }             // Not string
        public string MemberName { get; set; }
        public int TestId { get; set; }
        public int TotalMarks { get; set; }
        public int ObtainedMarks { get; set; }
        public DateTime Dated { get; set; }           // Not nullable
        public string CityName { get; set; }
        public string ImageName { get; set; }
        public TimeSpan? TestTime { get; set; }
    }
    public class CertificateViewModel
    {
        public string UserName { get; set; }
        public string TestName { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public bool IsPassed { get; set; }
    }
    public class TestResultViewModel
    {
        public string UserName { get; set; } = string.Empty;

        public string TestTitle { get; set; } = string.Empty;
        public string TestUrl { get; set; } = string.Empty;

        public int TotalQuestions { get; set; }

        public int CorrectAnswers { get; set; }

        public bool IsPassed { get; set; }

        public DateTime Dated { get; set; }

        public short? Duration { get; set; }

        public int TestId { get; set; }

        public int ResultId { get; set; }
        public int? MemberId { get; set; }
        public decimal? TotalMarks { get; set; }
    }

    public class TopicViewModel
    {
        public int TopicId { get; set; }
        public decimal? TopicNumber { get; set; }
        public string TopicName { get; set; }
        public string Description { get; set; }
        public int McqCount { get; set; }
        public string TestUrl { get; set; } // For launch test link
    }

}
