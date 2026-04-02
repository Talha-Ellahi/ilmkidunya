using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblSlide1
{
    public int Id { get; set; }

    public string? Title1 { get; set; }

    public string? Title2 { get; set; }

    public string? Price { get; set; }

    public bool? Status { get; set; }

    public string? Link { get; set; }

    public string? SlideName { get; set; }

    public byte[] TimeStamp { get; set; } = null!;
}
