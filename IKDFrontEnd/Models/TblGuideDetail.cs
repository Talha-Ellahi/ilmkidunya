using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblGuideDetail
{
    public int Id { get; set; }

    public int? GuideId { get; set; }

    public string? Url { get; set; }

    public string? Icon { get; set; }

    public int? SortOrder { get; set; }

    public string? Heading { get; set; }

    public string? HeadingDesc { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaDesc { get; set; }

    public string? MetaKeyword { get; set; }

    public string? Detail { get; set; }

    public string? Detail2 { get; set; }

    public bool? IsActive { get; set; }

    public bool? AdmissionSlider { get; set; }

    public bool? CollegeSlider { get; set; }

    public string? Name { get; set; }

    public int? PageId { get; set; }

    public bool IsMenu { get; set; }
}
