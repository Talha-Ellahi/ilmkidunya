using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class Board
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? BoardTypeId { get; set; }

    public string? LevelIds { get; set; }

    public string? LiveUrl { get; set; }

    public int? SortOrder { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? Dated { get; set; }

    public int? UpdatedBy { get; set; }

    public string? ImageName { get; set; }

    public string? Url { get; set; }

    public virtual ICollection<TblPastPaper> TblPastPapers { get; set; } = new List<TblPastPaper>();

    public virtual ICollection<TblPpboardClassSubject> TblPpboardClassSubjects { get; set; } = new List<TblPpboardClassSubject>();

    public virtual ICollection<TblPpboardClass> TblPpboardClasses { get; set; } = new List<TblPpboardClass>();

    public virtual ICollection<TblStudyGuide> TblStudyGuides { get; set; } = new List<TblStudyGuide>();
}
