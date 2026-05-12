using System;
using System.Collections.Generic;

namespace IKDFrontEnd.DBCollege;

public partial class BoardOtsmcq
{
    public int BoardOtsid { get; set; }

    public int YearId { get; set; }

    public int BoardId { get; set; }

    public int ClassId { get; set; }

    public int SubjectId { get; set; }

    public byte? PaperGroupId { get; set; }

    public string? Question { get; set; }

    public string? QuestionImage { get; set; }

    public string? Choice1 { get; set; }

    public string? Choice1img { get; set; }

    public string? Choice2 { get; set; }

    public string? Choice2img { get; set; }

    public string? Choice3 { get; set; }

    public string? Choice3img { get; set; }

    public string? Choice4 { get; set; }

    public string? Choice4img { get; set; }

    public string? Choice5 { get; set; }

    public string? Choice5img { get; set; }

    public byte? CorrectAnswer { get; set; }

    public DateTime? Dated { get; set; }

    public int? UserName { get; set; }

    public DateTime? UpdateDate { get; set; }

    public int? UpDateName { get; set; }

    public virtual PaperGroup? PaperGroup { get; set; }

    public virtual Year Year { get; set; } = null!;
}
