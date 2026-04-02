using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblNewsLink
{
    public decimal LinkId { get; set; }

    public decimal? NewsId { get; set; }

    public string? Description { get; set; }

    public string? Url { get; set; }
}
