using System;
using System.Collections.Generic;

namespace IKDFrontEnd.PastPaperModels;

public partial class TblPpclass
{
    public int Id { get; set; }

    public int PpqualificationId { get; set; }

    public string? PpclassName { get; set; }

    public string? PpclassUrl { get; set; }

    public string? Description { get; set; }

    public string? Image { get; set; }

    public int? SortOrder { get; set; }

    public int? UserId { get; set; }

    public bool? IsDelete { get; set; }

    public virtual TblPpqualification Ppqualification { get; set; } = null!;

    public virtual ICollection<TblPastPaper> TblPastPapers { get; set; } = new List<TblPastPaper>();

    public virtual ICollection<TblPpboardClassSubject> TblPpboardClassSubjects { get; set; } = new List<TblPpboardClassSubject>();
}
