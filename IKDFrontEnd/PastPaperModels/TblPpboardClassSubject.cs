using System;
using System.Collections.Generic;

namespace IKDFrontEnd.PastPaperModels;

public partial class TblPpboardClassSubject
{
    public int Id { get; set; }

    public int BoardId { get; set; }

    public int PpclassId { get; set; }

    public int PpsubjectId { get; set; }

    public bool? IsDelete { get; set; }

    public virtual Board Board { get; set; } = null!;

    public virtual TblPpclass Ppclass { get; set; } = null!;

    public virtual TblPpsubject Ppsubject { get; set; } = null!;
}
