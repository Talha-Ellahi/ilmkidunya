using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblDateSheetCriteriaChild
{
    public int Id { get; set; }

    public int DateSheetHeadingId { get; set; }

    public int Year { get; set; }

    public string FileName { get; set; } = null!;

    public string ViewOnline { get; set; } = null!;

    public string? Note { get; set; }

    public string? ExpectedDate { get; set; }

    public short? Status { get; set; }

    public short? SortOrder { get; set; }

    public byte? DateSheetType { get; set; }

    public string? Subheading { get; set; }

    public DateTime? Dated { get; set; }

    public string? ShortDesc { get; set; }

    public string? ExamDate { get; set; }

    public string? FileName1 { get; set; }

    public string? FileName2 { get; set; }

    public string? Note2 { get; set; }

    public string? Note3 { get; set; }
}
