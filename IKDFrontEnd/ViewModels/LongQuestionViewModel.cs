using IKDFrontEnd.Models;

namespace IKDFrontEnd.ViewModels
{
    public class LongQuestionViewModel
    {
    }

    public class CriteriaWithLongQuestionSetsVM
    {
        public int ParentCriteriaID { get; set; }
        public bool? Tabs { get; set; }
        public string SubjectName { get; set; }
        public string TestName { get; set; }
        public string Url { get; set; }
        public string ImageName { get; set; }
        public bool? MixTabs { get; set; }
        public LongQuestionCriteriaDetail CriteriaDetail { get; set; }
        public List<QuestionSetVM> QuestionSets { get; set; } = new List<QuestionSetVM>();
        public List<ChapterInfoVM> Chapters { get; set; } = new List<ChapterInfoVM>();
        public byte TotalQuestions { get; internal set; }
        public object AvailableQuestions { get; internal set; }
        public bool CanCreateSets { get; internal set; }
        public int ActualNumberOfSets { get; internal set; }
    }



    public class LongQuestionSubjectViewModel
    {
        public bool HasMultipleChapters { get; set; }
        public CriteriaViewModel OriginalCriteria { get; set; }
        public List<RelatedCriteriaViewModel> RelatedCriteria { get; set; } = new();
        public QuestionSetViewModel QuestionSet { get; set; }
    }


    public class LongQuestionsPdfViewModel
    {
        public string SubjectName { get; set; }
        public string TestName { get; set; }
        public List<QuestionAnswerVM> Questions { get; set; }
    }

}
