using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblPageFeedback
{
    public int Id { get; set; }

    public DateTime? Date { get; set; }

    public string? PageSection { get; set; }

    public string? PageUrl { get; set; }

    public string? Comments { get; set; }
}
