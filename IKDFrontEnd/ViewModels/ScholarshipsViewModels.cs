using IKDFrontEnd.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IKDFrontEnd.ViewModels
{
    public class ScholarshipsViewModels
    {
    }

    public class HomeScholarshipViewModel
    {
        public List<TblSchStudyLevel> StudyLevels { get; set; } = new();
        public List<TblPlaceOfStudy> Countries { get; set; } = new();
        public List<TblSch> LatestScholarships { get; set; } = new();
       
    }
    public class HomeViewModel
    {
        public int Id { get; set; }
        public string CountryName { get; set; }
        public string CountryUrl { get; set; }
        public string CountryImage { get; set; }

    }

    public class ScholarshipSearchViewModel
    {
        public int? StudyLevelId { get; set; }
        public int? CountryId { get; set; }

        public List<TblSch> Results { get; set; } = new();
    }

    public class ScholarshipSearchResultViewModel
    {
        public List<TblSchStudyLevel> StudyLevels { get; set; } = new();
        public List<TblPlaceOfStudy> Countries { get; set; } = new();
        public List<ScholarshipApiViewModel> Results { get; set; } = new();
        public SelectList StudyLevelList { get; set; }
        public SelectList CountryList { get; set; }

        public string SelectedCountry { get; set; }
        public string SelectedLevel { get; set; }
    }

    public class ScholarshipApiViewModel
    {
        public string SchName { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public DateTime? Deadline { get; set; }

        public List<string> StudyLevels { get; set; } = new();
        public List<string> FieldsOfStudy { get; set; } = new();
    }

    public class CountryScholarshipViewModel
    {
        public string SchName { get; set; }
        public string SchImage { get; set; }
        public string Description1 { get; set; }
        public DateTime? Deadline { get; set; }
        public string Url { get; set; }
        public List<string> StudyLevels { get; set; }
        public List<string> FieldsOfStudy { get; set; }
    }

    public class ScholarshipViewModel
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public DateTime? Deadline { get; set; }
        public List<string> StudyLevels { get; set; }
        public List<string> FieldsOfStudy { get; set; }
    }

    public class ScholarshipsByCountryViewModel
    {
        public string Country { get; set; }
        public List<ScholarshipViewModel> Scholarships { get; set; }
    }


}
