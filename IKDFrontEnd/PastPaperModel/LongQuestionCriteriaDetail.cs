using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class LongQuestionCriteriaDetail
{
    public int Id { get; set; }

    public short SubjectId { get; set; }

    public string? ChapterIds { get; set; }

    public byte TotalQuestions { get; set; }

    public short Marks { get; set; }

    public string? Time { get; set; }

    public byte NegativeMarks { get; set; }

    public short OtscriteriaId { get; set; }

    public short? SortOrder { get; set; }

    public short? ClassId { get; set; }
}
