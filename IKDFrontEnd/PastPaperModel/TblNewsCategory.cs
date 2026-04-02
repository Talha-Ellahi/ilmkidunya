using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblNewsCategory
{
    public int NewsCategoryId { get; set; }

    public string? NewsCategoryName { get; set; }

    public string? Description { get; set; }

    public string? Picture { get; set; }

    public int? CategoryOrder { get; set; }

    public string? RewriteUrl { get; set; }
}
