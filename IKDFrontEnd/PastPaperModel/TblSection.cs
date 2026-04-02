using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblSection
{
    public int Id { get; set; }

    public string SectionName { get; set; } = null!;

    public string? Description { get; set; }

    public string? Url { get; set; }

    public string? Cmsurlpattern { get; set; }

    public string? Image { get; set; }

    public virtual ICollection<TblFile> TblFiles { get; set; } = new List<TblFile>();
}
