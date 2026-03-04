using IKDFrontEnd.Models;

namespace IKDFrontEnd.ViewModels
{
    public class ShortQuestionsViewModel
    {

    }



    public class QuestionSetVM
    {
        public string SetName { get; set; }
        public int TotalQuestions { get; set; }
        public List<QuestionAnswerVM> Questions { get; set; } = new List<QuestionAnswerVM>();
    }





    // Update your CriteriaWithQuestionSetsVM to include chapters
    public class CriteriaWithQuestionSetsVM
    {
        public int ParentCriteriaID { get; set; }
        public bool? Tabs { get; set; }
        public string SubjectName { get; set; }
        public string TestName { get; set; }
        public string Url { get; set; }
        public string ImageName { get; set; }
        public bool? MixTabs { get; set; }
        public ShortQuestionCriteriaDetail CriteriaDetail { get; set; }
        public List<QuestionSetVM> QuestionSets { get; set; } = new List<QuestionSetVM>();
        public List<ChapterInfoVM> Chapters { get; set; } = new List<ChapterInfoVM>();
        public byte TotalQuestions { get; internal set; }
        public object AvailableQuestions { get; internal set; }
        public bool CanCreateSets { get; internal set; }
        public int ActualNumberOfSets { get; internal set; }
    }
    public class RelatedCriteriaWithChaptersVM
    {
        public int CriteriaId { get; set; }
        public string TestName { get; set; }
        public string Url { get; set; }
        public bool? Tabs { get; set; }
        public string ImageName { get; set; }
        public bool? MixTabs { get; set; }
        public int TotalQuestions { get; set; }
        public int SubjectId { get; set; }
        public string ChapterIds { get; set; }
        public short? ClassId { get; set; }
        public List<ChapterInfoVM> Chapters { get; set; }
        public int AvailableQuestions { get; set; }
        public bool CanCreateSets { get; set; }
    }

    public class ShortQuestionsPdfViewModel
    {
        public string SubjectName { get; set; }
        public string TestName { get; set; }
        public List<QuestionAnswerVM> Questions { get; set; }
    }
    public class PreparationSQ
    {
        public ShortQuestionCriterion Criteria { get; set; }
        public ShortQuestionCriteriaDetail CriteriaDetail { get; set; }

        // Instead of JsonContent (hard to consume in Razor), let's store a C# object
        public List<QuestionAnswerVM> QuestionAnswer { get; set; }

        // Chapter list with both Id + Name (not just names)
        public List<ChapterVM> Chapters { get; set; } = new List<ChapterVM>();

        // Extra fields for View
        public string SubjectName { get; set; }
        public string ClassName { get; set; }
        public string SubjectUrl { get; set; }
        public int TotalQuestions => CriteriaDetail?.TotalQuestions ?? 0;

        public List<string> ChaptersUrls { get; internal set; }
    }

    public class ChapterVM
    {
        public int ChapterId { get; set; }
        public string ChapterName { get; set; }
    }

    public class QuestionAnswerVM
    {
        public int QuestionId { get; set; }
        public string QuestionDescription { get; set; }
        public int ChapterId { get; set; }
        public List<AnswerVM> Answers { get; set; } = new();
        public string? QuestionImage { get; internal set; }
    }

    public class AnswerVM
    {
        public int ChoiceId { get; set; }
        public string ChoiceDescription { get; set; }
        public string? ChoiceImage { get; set; }
    }

    public class ChapterLinkViewModel
    {
        public int ChapterId { get; set; }
        public string ChapterName { get; set; }
    }








        public class ShortQuestionSubjectViewModel
        {
            public bool HasMultipleChapters { get; set; }
            public CriteriaViewModel OriginalCriteria { get; set; }
            public List<RelatedCriteriaViewModel> RelatedCriteria { get; set; } = new();
            public QuestionSetViewModel QuestionSet { get; set; }
        }

        public class CriteriaViewModel
        {
            public int ParentCriteriaID { get; set; }
            public bool? Tabs { get; set; }
            public string SubjectName { get; set; }
            public string TestName { get; set; }
            public string Url { get; set; }
            public string ImageName { get; set; }
            public bool? MixTabs { get; set; }
            public List<ChapterInfoVM> Chapters { get; set; } = new();
        public byte TotalQuestions { get; internal set; }
        public object AvailableQuestions { get; internal set; }
        public bool CanCreateSets { get; internal set; }
    }

        public class RelatedCriteriaViewModel
        {
            public int CriteriaId { get; set; }
            public string TestName { get; set; }
            public string Url { get; set; }
            public string Tabs { get; set; }
            public string ImageName { get; set; }
            public string MixTabs { get; set; }
            public int TotalQuestions { get; set; }
            public int SubjectId { get; set; }
            public string ChapterIds { get; set; }
            public int ClassId { get; set; }
            public List<ChapterInfoVM> Chapters { get; set; } = new();
        public bool CanCreateSets { get; internal set; }
        public int AvailableQuestions { get; internal set; }
    }

        public class ChapterInfoVM
        {
            public int ChapterId { get; set; }
            public string ChapterName { get; set; }
            public string Description { get; set; }
        public int? ChapterNumber { get; internal set; }
        public bool? IsActive { get; internal set; }
    }

        public class QuestionSetViewModel
        {
            public string SetName { get; set; }
            public int TotalQuestions { get; set; }
            public List<QuestionAnswerVM> Questions { get; set; } = new();
        }


    


}
