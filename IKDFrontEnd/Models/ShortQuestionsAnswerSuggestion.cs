using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class ShortQuestionsAnswerSuggestion
{
    public int Id { get; set; }

    public int MemberId { get; set; }

    public int QuestionId { get; set; }

    public string? Suggestion { get; set; }

    public DateTime Dated { get; set; }

    public string? QuestionNo { get; set; }

    public int? UpDatedBy { get; set; }
}
