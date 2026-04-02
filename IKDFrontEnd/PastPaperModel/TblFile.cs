using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblFile
{
    public int Id { get; set; }

    public DateTime? Date { get; set; }

    public string? Title { get; set; }

    public int? SectionId { get; set; }

    public string? Url { get; set; }

    public string? FileName { get; set; }

    public int? UserId { get; set; }

    public long? FileSize { get; set; }

    public virtual TblSection? Section { get; set; }
}
