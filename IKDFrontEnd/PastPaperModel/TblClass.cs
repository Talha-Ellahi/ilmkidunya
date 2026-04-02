using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblClass
{
    public int ClassId { get; set; }

    public string ClassName { get; set; } = null!;

    public string? Description { get; set; }

    public int? EducationLevelId { get; set; }
}
