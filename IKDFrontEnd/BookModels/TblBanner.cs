using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class TblBanner
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Banner { get; set; }

    public string? Url { get; set; }

    public string? AddSone { get; set; }

    public bool IsActive { get; set; }

    public bool IsDelete { get; set; }

    public DateOnly CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public byte[]? Timestamp { get; set; }

    public DateOnly? EndingDate { get; set; }

    public byte? BannerRank { get; set; }

    public byte? BannerType { get; set; }

    public int? Hits { get; set; }
}
