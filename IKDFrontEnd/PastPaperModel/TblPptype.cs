using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblPptype
{
    public int Id { get; set; }

    public string PptypeName { get; set; } = null!;

    public int? SortOrder { get; set; }

    public int? UserId { get; set; }

    public bool? IsDelete { get; set; }
}
