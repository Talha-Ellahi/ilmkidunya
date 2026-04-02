using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblOtsmcqchild
{
    public int Id { get; set; }

    public int Otsmcqid { get; set; }

    public string? Question { get; set; }

    public string? QuestionImg { get; set; }

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
}
