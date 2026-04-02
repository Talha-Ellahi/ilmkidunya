using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblChapter
{
    public int ChapterId { get; set; }

    public decimal SubjectId { get; set; }

    public string ChapterName { get; set; } = null!;

    public string? MetaTitle { get; set; }

    public string? MetaKey { get; set; }

    public string? Description { get; set; }

    public string? ChapterContent { get; set; }

    public string? Url { get; set; }

    public int? SortOrder { get; set; }

    public bool? IsActive { get; set; }

    public int? ChapterNumber { get; set; }
}
