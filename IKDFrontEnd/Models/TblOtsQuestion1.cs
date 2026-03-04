using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblOtsQuestion1
{
    public int Id { get; set; }

    public int TestId { get; set; }

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

    public int? CorrectAnswer { get; set; }

    public int? ChapterId { get; set; }

    public int? ClassId { get; set; }

    public int? SubjectId { get; set; }

    public DateTime? Dated { get; set; }

    public int? UserName { get; set; }
}
