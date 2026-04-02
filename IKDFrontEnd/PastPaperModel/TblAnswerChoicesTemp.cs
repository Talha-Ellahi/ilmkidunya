using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblAnswerChoicesTemp
{
    public int ChoiceId { get; set; }

    public int? QuestionId { get; set; }

    public string? ChoiceDescription { get; set; }

    public string? ChoiceImage { get; set; }

    public bool? IsTrueChoice { get; set; }
}
