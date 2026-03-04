using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblArticle
{
    public decimal ArticleId { get; set; }

    public decimal? ArticleTypeId { get; set; }

    public string? Title { get; set; }

    public DateTime? Dated { get; set; }

    public string? SenderEmail { get; set; }

    public string? SenderName { get; set; }

    public string? Source { get; set; }

    public decimal? Viewed { get; set; }

    public decimal? MemberId { get; set; }

    public bool? Approve { get; set; }

    public string? ArticleDetails { get; set; }

    public string? OtherFile { get; set; }

    public string? RewriteUrl { get; set; }

    public string? MetaTitle { get; set; }

    public string? ArticleDescription { get; set; }

    public string? ArticleKeywords { get; set; }

    public string? NewsTags { get; set; }

    public string? PictureThumbnail { get; set; }

    public string? Picture1 { get; set; }

    public string? Filename { get; set; }
}
