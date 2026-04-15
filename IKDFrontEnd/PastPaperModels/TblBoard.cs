using System;
using System.Collections.Generic;

namespace IKDFrontEnd.PastPaperModels;

public partial class TblBoard
{
    public int Id { get; set; }

    public string BoardName { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Province { get; set; }

    public bool IsActive { get; set; }
}
