using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblClassesChild
{
    public int Id { get; set; }

    public string MainClassName { get; set; } = null!;

    public int ChildClassId { get; set; }
}
