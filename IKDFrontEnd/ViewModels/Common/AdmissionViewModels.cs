using IKDFrontEnd.Models;

namespace IKDFrontEnd.ViewModels.Common
{
    public class HomePageViewModel
    {
        public List<CityWiseAdmissionViewModel> Admissions { get; set; }
        public List<CityWiseAdmissionViewModel>? SearchResults { get; set; }
        public List<TblDefCity> Cities { get; set; }
        public List<TblXcourseLevel> CourseLevels { get; set; }
    }

    public class CityWiseAdmissionViewModel
    {
        public int? AdmissionId { get; set; }
        public string? AdmissionTitle { get; set; }
        public string? Url { get; set; }
        public string? AdmissionLogo { get; set; }
        public DateTime? Dated { get; set; }
        public DateTime? LastDate { get; set; }
        public string? CollegeName { get; set; }
        public string? CollageLogo { get; set; }
        public string? CollageUrl { get; set; }
        public string? CityName { get; set; }
        public string? CourseNames { get; set; }
        public int? AdmissionLogoYear { get; set; }
        public int? AdmissionLogoMonth { get; set; }


    }
    public class CourseCategoryViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Url { get; set; }
        public string? Image { get; set; }
        public int? TotalAdmission { get; set; }
    }
    public class AdmissionDetailViewModel
    {
        public int Id { get; set; }
        public string AdmissionTitle { get; set; }
        public string? Url { get; set; }
        public string Detail { get; set; }
        public string NoticeImageThumb { get; set; }
        public string NoticeImageLarge { get; set; }
        public DateTime? Dated { get; set; }
        public DateTime? LastDate { get; set; }
        public string CollegeName { get; set; }
        public string ColUrl { get; set; }
        public string CollageLogo { get; set; }
        public string CityName { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDesc { get; set; }
        public string MetaKeys { get; set; }

    }


}
