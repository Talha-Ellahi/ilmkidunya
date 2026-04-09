using System;
using System.Collections.Generic;

namespace IKDFrontEnd.DBCollege;

public partial class TblGuidesDefination
{
    public int Id { get; set; }

    public string GuideName { get; set; } = null!;

    public string GuideMainUrl { get; set; } = null!;

    public string CategoryIds { get; set; } = null!;

    public int EducationLevelId { get; set; }

    public string Tags { get; set; } = null!;

    public string? Icon { get; set; }

    public string Status { get; set; } = null!;

    public string? GuideAdmissionPageUrl { get; set; }

    public string? GuideUniversityListPageUrl { get; set; }

    public string? GuideMeritListPageUrl { get; set; }

    public string? GuideFeeStructurePageUrl { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool ShowInquieryForm { get; set; }

    public string? Abrevation { get; set; }

    public string? CourseUrl { get; set; }
}
