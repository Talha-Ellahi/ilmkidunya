using IKDFrontEnd.Models;

namespace IKDFrontEnd.ViewModels
{
    public class DateSheetCriteria
    {
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }
        public string? Desc1 { get; set; }
        public string? Desc2 { get; set; }
        public string? Desc3 { get; set; }
        public string? Heading { get; set; }
        public string? PlaceholderCode { get; set; }

        public string? CriteriaHeading { get; set; }
        public string? ArchiveNote { get; set; }
        public string ViewOnline { get; set; } = null!;
        public string? ExpectedDate { get; set; }
        public string? BoardName { get; set; }
        public string? BoardImage { get; set; }
        public bool ShowUpdateForm { get; set; } = false;

        public short? Status { get; set; }

    }

}
