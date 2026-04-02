using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblMainNews
{
    public decimal NewsId { get; set; }

    public string? MainHeading { get; set; }

    public string? SubHeading { get; set; }

    public string? NewsSource { get; set; }

    public string? PictureThumbnail { get; set; }

    public string? Picture1 { get; set; }

    public string? Picture1Desc { get; set; }

    public string? Picture2 { get; set; }

    public string? Picture2Desc { get; set; }

    public string? Picture3 { get; set; }

    public string? Picture3Desc { get; set; }

    public string? NewsDetails { get; set; }

    public string? NewsQuote { get; set; }

    public DateTime? Dated { get; set; }

    public string? ShortHeading { get; set; }

    public string? ShortDesc { get; set; }

    public decimal? MemberId { get; set; }

    public bool? Approve { get; set; }

    public string? OtherFile { get; set; }

    public decimal? Views { get; set; }

    public string? RewriteUrl { get; set; }

    public string? NewsTags { get; set; }

    public string? MetaTitle { get; set; }

    public string? NewsDescription { get; set; }

    public string? NewsKeywords { get; set; }

    public int? PictureStoryId { get; set; }

    public DateTime? Updated { get; set; }

    public string? ResultKeywords { get; set; }

    public bool ShowinSlider { get; set; }

    public int? UserId { get; set; }

    public virtual Admin? User { get; set; }
}
