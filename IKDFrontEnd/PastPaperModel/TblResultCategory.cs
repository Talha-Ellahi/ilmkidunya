using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblResultCategory
{
    public int Id { get; set; }

    public string CategoryName { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public bool IsActive { get; set; }
}
