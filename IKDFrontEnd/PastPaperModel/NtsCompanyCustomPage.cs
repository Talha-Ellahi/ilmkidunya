using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class NtsCompanyCustomPage
{
    public long Id { get; set; }

    public int PageTypeId { get; set; }

    public long NtscompanyId { get; set; }

    public long NtsjobId { get; set; }

    public string PageHeading { get; set; } = null!;

    public string PageDesc { get; set; } = null!;

    public string? Thumbnail { get; set; }

    public string Url { get; set; } = null!;

    public string MetaTitle { get; set; } = null!;

    public string MetaKeyWord { get; set; } = null!;

    public string MetaDesc { get; set; } = null!;

    public DateOnly LastUpdatedDate { get; set; }

    public int UpdateBy { get; set; }

    public string? Title { get; set; }

    public string? CustomUrl { get; set; }

    public bool? IsNewWindow { get; set; }

    public short? SamplePaperCategory { get; set; }

    public bool? IsFeatured { get; set; }

    public int? SectionId { get; set; }

    public int? JobId { get; set; }
}
