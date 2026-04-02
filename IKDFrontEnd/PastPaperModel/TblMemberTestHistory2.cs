using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblMemberTestHistory2
{
    public int TestId { get; set; }

    public int SubjectId { get; set; }

    public string QuestionsList { get; set; } = null!;

    public string AttemptedList { get; set; } = null!;

    public string? TestTitle { get; set; }

    public decimal TotalMarks { get; set; }

    public decimal ObtainedMarks { get; set; }

    public int ClassId { get; set; }

    public string TestDate { get; set; } = null!;

    public decimal MemberId { get; set; }

    public string ChaptersList { get; set; } = null!;

    public string TestGrade { get; set; } = null!;

    public int TotalQuestions { get; set; }

    public decimal TestDuration { get; set; }

    public int? SpecialTestId { get; set; }

    public string? TestType { get; set; }

    public string? TestUrl { get; set; }

    public byte? TestType1 { get; set; }
}
