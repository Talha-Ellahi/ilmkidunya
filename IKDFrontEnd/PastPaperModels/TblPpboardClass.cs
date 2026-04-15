using System;
using System.Collections.Generic;

namespace IKDFrontEnd.PastPaperModels;

public partial class TblPpboardClass
{
    public int Id { get; set; }

    public int BoardId { get; set; }

    public int PpclassId { get; set; }

    public bool? IsDelete { get; set; }

    public virtual TblPpclass Ppclass { get; set; } = null!;
}
