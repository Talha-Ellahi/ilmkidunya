using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblOtssubject
{
    public int Id { get; set; }

    public int OtsClassId { get; set; }

    public string OtsSubjectName { get; set; } = null!;

    public string? OtsSubjectUrl { get; set; }

    public string? Description { get; set; }

    public int SortOrder { get; set; }

    public bool ShowFront { get; set; }

    public string? Duration { get; set; }

    public string? NoOfQuestions { get; set; }

    public string? MarksPerQuestion { get; set; }

    public string? NegativeMarkingValue { get; set; }

    public string? Image { get; set; }

    public bool IsActive { get; set; }

    public DateTime Date { get; set; }

    public int UserId { get; set; }
}
