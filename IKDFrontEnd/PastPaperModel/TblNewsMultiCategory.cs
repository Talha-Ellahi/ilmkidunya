using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblNewsMultiCategory
{
    public int Id { get; set; }

    public decimal? NewsCategoryId { get; set; }

    public decimal? NewsId { get; set; }
}
