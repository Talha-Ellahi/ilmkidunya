using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblResult
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public int ClassId { get; set; }

    public int BoardId { get; set; }

    public int Year { get; set; }

    public string ResultTitle { get; set; } = null!;

    public string ResultUrl { get; set; } = null!;

    public string ResultStatus { get; set; } = null!;

    public DateTime? AnnounceDate { get; set; }

    public string? Content { get; set; }

    public string? OfficialLink { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaDescription { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; }
}
