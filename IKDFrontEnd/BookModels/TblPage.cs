using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class TblPage
{
    public long Id { get; set; }

    public string? ArticleContent { get; set; }

    public string Title { get; set; } = null!;

    public string? MetaKeywords { get; set; }

    public string? MetaDescription { get; set; }

    public string? MetaTitle { get; set; }

    public int IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public string SeoUrl { get; set; } = null!;

    public string Thumbnail { get; set; } = null!;

    public string ShortDescription { get; set; } = null!;

    public int? MemberId { get; set; }

    public string RelatedLink { get; set; } = null!;

    public string Content2 { get; set; } = null!;

    public string Content3 { get; set; } = null!;

    public string Content4 { get; set; } = null!;

    public int? PanelId { get; set; }

    public int? QualificationId { get; set; }

    public int? InsituteId { get; set; }

    public int? ClassId { get; set; }

    public int? SubjId { get; set; }

    public int? YearId { get; set; }

    public int? PastpaperId { get; set; }
}
