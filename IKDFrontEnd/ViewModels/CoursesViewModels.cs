using IKDFrontEnd.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IKDFrontEnd.ViewModels
{
    public class CoursesViewModels
    {
        public List<TblDefCity> Cities { get; set; }
        public List<CourseCategory> Categories { get; set; }
        public List<TblXcourseLevel> Levels { get; set; }
        public List<CollegeWithCourseCountViewModel> SearchResults { get; set; } // ✅ sahi type
        public int? TotalFilteredCollegeCount { get; set; }
        public int? TotalFilteredCourseCount { get; set; }
        public int? CoursesCount { get; set; }
        public List<CollegeSelectionViewModel> Colleges { get; set; }

    }

    public class HomePageViewModel2
    {
        public List<CityInstitutionsViewModel> Colleges { get; set; }
        public List<CollegeWithCourseCountViewModel>? SearchResults { get; set; }
       

        public List<TblDefCity> Cities { get; set; }
        public List<TblXcourseLevel> CourseLevels { get; set; }
        public List<CourseCategory> Categories { get; set; }
    }

    public class CollegeSelectionViewModel
    {
        public List<SelectListItem> Colleges { get; set; } = new();
        public int SelectedCollegId { get; set; }
    }


}
