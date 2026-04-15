using IKDFrontEnd.Controllers;
using IKDFrontEnd.Models;

namespace IKDFrontEnd.ViewModels
{
    public class GuideDetailViewModel
    {
        public int? Id { get; set; }
        public string? Heading { get; set; }
        public string? HeadingDesc { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDesc { get; set; }
        public string? MetaKeyword { get; set; }
        public string? Url { get; set; }
        public string? Detail { get; set; }
        public string? Detail2 { get; set; }
        public List<ConsultantBasicViewModel>? Consultants { get; set; }
    }
    public class MenuItemViewModel
    {
        public string? Url { get; set; }
        public string? HeaderIcon { get; set; }
        public string? HeaderName { get; set; }
    }
    public class GuidePageViewModel
    {
        public GuideDetailViewModel? GuideDetail { get; set; }
        public List<MenuItemViewModel>? MenuItems { get; set; }
        public List<ConsultantBasicViewModel>? Consultants { get; set; }
        public List<AllGuidesController.CitywithAdmissionAndCollegeCountViewModel> CityList { get; internal set; }
        public DBCollege.TblGuidesDefination? Guide { get; internal set; }
    }


    public class ConsultantHomeViewModel
    {
        public GuideDetailViewModel? MetaData { get; set; }
        public List<ConsultantBasicViewModel>? Consultants { get; set; }
    }

}
