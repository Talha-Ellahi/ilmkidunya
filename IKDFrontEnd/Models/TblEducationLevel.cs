using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblEducationLevel
{
    public int EducationLevelId { get; set; }

    public string EducationLevelName { get; set; } = null!;

    public string? EducationLevelNo { get; set; }

    public short? SortOrder { get; set; }

    public string? SearchTags { get; set; }

    public bool? Isacademic { get; set; }

    public string? LevelImage { get; set; }

    public string? Url { get; set; }

    public short? Courselevel { get; set; }

    public virtual ICollection<TblStudyGuide> TblStudyGuides { get; set; } = new List<TblStudyGuide>();
}
