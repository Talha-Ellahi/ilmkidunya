using System.ComponentModel.DataAnnotations;

namespace IKDFrontEnd.ViewModels
{
    public class SubjectWithTutorCountViewModel
    {
        public short SubjectId { get; set; }
        public string? SubjectName { get; set; }
        public string? SubjectIcon { get; set; }
        public int TutorCount { get; set; }
    }

    public class CityWithTutorCountViewModel
    {
        public int CityId { get; set; }
        public string? CityName { get; set; }
        public string? ImageName { get; set; } // You can map it to any placeholder image
        public int TutorCount { get; set; }
    }

    public class EducationLevelWithTutorCountViewModel
    {
        public int EducationLevelId { get; set; }
        public string EducationLevelName { get; set; }

        public string Url { get; set; }
        public string? LevelImage { get; set; }
        public int TutorCount { get; set; }
    }
    public class SkillWithTutorCountViewModel
    {
        public short SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string SubjectIcon { get; set; }
        public int TutorCount { get; set; }
    }


    public class TutorCardViewModel
    {
        public decimal TutorId { get; set; }

        public decimal MemberId { get; set; }
        public string TutorName { get; set; }
        public string CityName { get; set; }
        public string ProfileImage { get; set; }
        public bool Gender { get; set; }
        public string TeachingSummary { get; set; }
        public string? Experience { get; set; }
        public string? Availability { get; set; }
        public string? CourseName { get; set; }

        public string UrlSlug =>
    $"{TutorName?.ToLower().Replace(" ", "-")}-home-tutor-{MemberId}";

        public string? TutoringOptions { get; set; }
        public List<string> Subjects { get; set; }
        public string ShortSubjects =>
    string.Join(", ", Subjects).Length > 95
    ? string.Join(", ", Subjects).Substring(0, 92) + "..."
    : string.Join(", ", Subjects);
    }



    public class TutorDetailViewModel
    {
        public int TutorId { get; set; }
        public int MemberId { get; set; }
        public string TutorName { get; set; }

        public string Email { get; set; }
        public string CityName { get; set; }
        public string ProfileImage { get; set; }
        public bool Gender { get; set; }
        public string TeachingSummary { get; set; }
        public string Experience { get; set; }
        public string Availability { get; set; }
        public string CourseName { get; set; }
        public string TutoringOptions { get; set; }
        public List<string> Subjects { get; set; }
        public List<string> EducationLevels { get; set; }

        public List<TutorCardViewModel> SimilarTutors { get; set; }

    }


    public class TutorSearchFormViewModel
    {
        public int? CityId { get; set; }
        public int? SubjectId { get; set; }
        public int? EducationLevelId { get; set; }
    }

    public class TutorForCityViewModel
    {
        public string TutorQualification { get; set; }
        public bool? Gender { get; set; }
        public string ImageName { get; set; }
        public int MemberId { get; set; }
        public string MemberName { get; set; }
        public string Subjects { get; set; }
        public string CityName { get; set; }
        public string TeachingLevels { get; set; }
        public string Availability { get; set; }
        public string Experience { get; set; }
        public string AboutTutoring { get; set; }
        public int? ProgramId { get; set; }
        public string CourseName { get; set; }
        public string Phone { get; set; }
    }



    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "City")]
        public int CityId { get; set; }

        //[Required]
        //[Display(Name = "Country Code")]
        //public string CountryCode { get; set; } // e.g., "+92"

        [Required]
        [Display(Name = "Mobile Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Education Level")]
        public int EducationLevelId { get; set; }

        [Required]
        [Display(Name = "Subjects")]
        public int SelectedSubjectIds { get; set; }

        [Required]
        [Display(Name = "Tutoring Options")]
        public List<int> SelectedTutoringOptionIds { get; set; }


        [Display(Name = "Details about your request")]
        public string Detail { get; set; }

        [MustBeTrue(ErrorMessage = "You must agree to share your information with relevant tutors.")]
        [Display(Name = "Information Sharing Consent")]
        public bool IsShared { get; set; }

        [Display(Name = "Request Type")]
        public bool RequestType { get; set; } // 0 for Parent, 1 for student etc.


    }


    public class MustBeTrueAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is bool b)
            {
                return b;
            }
            return false;
        }
    }

    public class TutorContactViewModel
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Subject { get; set; }
        public string TutorName { get; set; }
        public string TutorEmail { get; set; }
    }


    public class CombinedSubjectCityLevelViewModel
    {
        public List<SubjectWithTutorCountViewModel> Subjects { get; set; }
        public List<CityWithTutorCountViewModel> Cities { get; set; }
        public List<EducationLevelWithTutorCountViewModel> EducationLevels { get; set; }
        public List<SkillWithTutorCountViewModel> Skills { get; set; }
        public List<TutorCardViewModel> LatestTutors { get; set; }


        public int CityId { get; set; } // ✅ Add this
        public string cityName { get; set; }
        public int educationLevelId { get; set; }
        public string educationLevelName { get; set; }

        public int subjectId { get; set; }
        public string subjectName { get; set; }
    }



}
