using IKDFrontEnd.Models;

namespace IKDFrontEnd.ViewModels
{
    public class HomePageViewModel3
    {
        public List<SliderViewModel> Sliders { get; set; }
        public List<NewsViewModel> LatestNews { get; set; }
        public List<ArticleViewModel> Articles { get; set; }
        public List<AdmissionViewModel> Admissions { get; set; }
        public CoursesViewModels Courses { get; set; }
        public List<SliderHome> WebStorySliders { get; set; } = new();
		public List<GroupedJobAdViewModel> Jobs { get; set; }
        public List<FeaturedCollegeViewModel> FeaturedColleges { get; set; }
        public List<NewsViewModel> SliderNews { get; set; }
        public List<CategoryCoursesViewModel> CategoryCourses { get; set; } = new();
        public List<DBCollege.SectionTypeImport> HomeLinks { get; set; }
    }

    public class SliderViewModel
    {
        public string Url { get; set; }
        public string Image { get; set; }
    }

    public class NewsViewModel
    {
        public string RewriteUrl { get; set; }
        public string PictureThumbnail { get; set; }
        public string Picture_1 { get; set; }
        public string MainHeading { get; set; }
        public DateTime? Dated { get; set; }
    }

    public class ArticleViewModel
    {
        public string RewriteUrl { get; set; }
        public string PictureThumbnail { get; set; }
        public string Title { get; set; }
        public DateTime? Dated { get; set; }
    }

    public class AdmissionViewModel
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public DateTime? AdmissionOpenDate { get; set; }
        public DateTime? AppliedDate { get; set; }
    }

    public class FeaturedCollegeViewModel
    {
        public string? Name { get; set; }
        public string? Url { get; set; }
        public string? Logo { get; set; }
        public int? SortOrder { get; set; }
    }
    public class CategoryCoursesViewModel
    {
        public string CategoryName { get; set; } = string.Empty;
        public List<GuideViewModel> Guides { get; set; } = new();
    }


    public class GuideViewModel
    {
        public string GuideName { get; set; }
        public string GuideMainUrl { get; set; }
    }


}
