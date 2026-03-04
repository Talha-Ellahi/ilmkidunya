using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblSchStudyLevel
{
    public int Id { get; set; }

    public string SchStudyLevelName { get; set; } = null!;

    public string? Url { get; set; }

    public bool? IsActive { get; set; }

    public int? SortOrder { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<TblSchStudyLevelChild> TblSchStudyLevelChildren { get; set; } = new List<TblSchStudyLevelChild>();
}
