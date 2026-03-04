
namespace IKDFrontEnd.ViewModels.AllGuidesViewModel
{


    public class MeritListViewModel
    {
        // Merit list details
        public int MeritListId { get; set; }
        public string? MeritListName { get; set; }
        public string? MeritValue { get; set; }
        public string? Year { get; set; }

        // College details
        public int? CollegeId { get; set; }
        public string? CollegeName { get; set; }
        public string? CollegeUrl { get; set; }
        public string? CollegeLogo { get; set; }
        public string? CollegeAddress { get; set; }
        public string? MeritFileName { get; internal set; }
        public string? MeritAddedDate { get; internal set; }
        public string CourseName { get; internal set; }
    }
    public class CollegeCourseFeeViewModel
    {
        public int CollegeId { get; set; }
        public string CollegeName { get; set; }
        public string CollegeUrl { get; set; }
        public string CollegeLogo { get; set; }
        public string CollegeAddress { get; set; }

        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string Duration { get; set; }  // if you store duration
        public string? Fee { get; set; }      // assuming Fee can be null
    }


    public class CollegeWithCourseViewModel
    {
        public int CollegeId { get; set; }
        public string CollegeName { get; set; }
        public string CollegeUrl { get; set; }
        public string CollegeAddress { get; set; }
        public string CollegeLogo { get; set; }

        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string Duration { get; set; }
        public string Fee { get; set; }
    }

    public class AdmissionWithCollegeViewModel
    {
        // Admission details
        public int AdmissionId { get; set; }
        public string? AdmissionTitle { get; set; }
        public DateTime? Dated { get; set; }
        public DateTime? LastDate { get; set; }
        public string? NoticeImageThumb { get; set; }
        public string? NoticeImageLarge { get; set; }
        public string? Url { get; set; }

        // College details
        public int? CollegeId { get; set; }
        public string? CollegeName { get; set; }
        public string? CollegeUrl { get; set; }
        public string? CollegeLogo { get; set; }
        public string? CollegeAddress { get; set; }
        public string CourseName { get; internal set; }
        public int? courseId { get; internal set; }
        public int? CityId { get; internal set; }
        public string? CityName { get; internal set; }
    }


}
