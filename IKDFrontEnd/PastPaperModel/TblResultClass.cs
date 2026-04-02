using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblResultClass
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public string ClassName { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public bool IsActive { get; set; }
}
