using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class MovedCompany
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Url { get; set; }

    public string? Logo { get; set; }

    public int? CityId { get; set; }

    public int? SortOrder { get; set; }

    public int? Views { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaKeyword { get; set; }

    public string? MetaDesc { get; set; }

    public bool? IsActive { get; set; }

    public DateTime Dated { get; set; }

    public int? UpdatedBy { get; set; }

    public string? BannerImage { get; set; }

    public string? Keywords { get; set; }

    public int? ShowTop { get; set; }

    public int? Memberid { get; set; }

    public bool? IsGovt { get; set; }

    public byte? Zoom { get; set; }

    public string? Latitude { get; set; }

    public string? Longitude { get; set; }

    public bool? CssApp { get; set; }

    public int? JobId { get; set; }

    public string? SocialImage { get; set; }

    public string? LiveUrl { get; set; }

    public string? ShortForm { get; set; }

    public string? ShortDetail { get; set; }

    public string? Heading { get; set; }

    public string? Email { get; set; }

    public string? Website { get; set; }

    public string? ShortDesc { get; set; }
}
