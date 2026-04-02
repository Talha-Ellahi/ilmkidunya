using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblSchStudyLevelChild
{
    public int Id { get; set; }

    public int SchId { get; set; }

    public int SchStudyLevelId { get; set; }

    public virtual TblSch Sch { get; set; } = null!;

    public virtual TblSchStudyLevel SchStudyLevel { get; set; } = null!;
}
