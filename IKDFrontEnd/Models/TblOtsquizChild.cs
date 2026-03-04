using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblOtsquizChild
{
    public int Id { get; set; }

    public int? OtsQuizId { get; set; }

    public string? Question { get; set; }

    public string? QuestionImg { get; set; }

    public string? Choice1 { get; set; }

    public string? Choice1Img { get; set; }

    public string? Choice2 { get; set; }

    public string? Choice2Img { get; set; }

    public string? Choice3 { get; set; }

    public string? Choice3Img { get; set; }

    public string? Choice4 { get; set; }

    public string? Choice4Img { get; set; }

    public int? CorrectAnswer { get; set; }

    public int? UserId { get; set; }

    public DateTime? Dated { get; set; }
}
