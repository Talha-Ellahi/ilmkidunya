using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class SectionContentImport
{
    public int Id { get; set; }

    public string? Heading { get; set; }

    public string? Detail { get; set; }

    public string? DetailShort { get; set; }

    public string? GoogleAdFooter { get; set; }

    public int? ContentId { get; set; }

    public string? ImageName { get; set; }

    public string? PageLink { get; set; }

    public string? FbHeader { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaKeyword { get; set; }

    public string? MetaDesc { get; set; }

    public string? MetaTags { get; set; }

    public int? Views { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? Dated { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public int? CollegeCategoryId { get; set; }

    public int? CityId { get; set; }

    public string? CollegeTypeId { get; set; }

    public string? GenderId { get; set; }

    public string? Keywords { get; set; }

    public short? SortOrder { get; set; }

    public string? IconImage { get; set; }

    public bool? AppActive { get; set; }

    public string? AppHeading { get; set; }

    public bool? ReadFromTextArea { get; set; }

    public string? HeaderText { get; set; }

    public string? MainHeading { get; set; }

    public string? OtherDetail { get; set; }

    public bool? AddSpecificContent { get; set; }

    public int? PanelDescriptionId { get; set; }

    public string? Detail2 { get; set; }
}
