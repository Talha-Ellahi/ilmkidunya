using System;
using System.Collections.Generic;

namespace IKDFrontEnd.PastPaperModels;

public partial class TblPptype
{
    public int Id { get; set; }

    public string PptypeName { get; set; } = null!;

    public int? SortOrder { get; set; }

    public int? UserId { get; set; }

    public bool? IsDelete { get; set; }
}
