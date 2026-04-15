using System;
using System.Collections.Generic;

namespace IKDFrontEnd.PastPaperModels;

public partial class TblPpsubject
{
    public int Id { get; set; }

    public string? PpsubjectName { get; set; }

    public string? Image { get; set; }

    public string? Icon { get; set; }

    public int? SortOrder { get; set; }

    public int? UserId { get; set; }

    public bool? IsDelete { get; set; }

    public virtual ICollection<TblPastPaper> TblPastPapers { get; set; } = new List<TblPastPaper>();

    public virtual ICollection<TblPpboardClassSubject> TblPpboardClassSubjects { get; set; } = new List<TblPpboardClassSubject>();
}
