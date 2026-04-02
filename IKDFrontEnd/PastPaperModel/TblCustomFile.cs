using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblCustomFile
{
    public int Id { get; set; }

    public string FileName { get; set; } = null!;

    public int CustomPageId { get; set; }

    public string? Title { get; set; }

    public DateOnly? Dated { get; set; }
}
