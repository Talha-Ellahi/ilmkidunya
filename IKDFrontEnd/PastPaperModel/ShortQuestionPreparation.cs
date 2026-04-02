using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class ShortQuestionPreparation
{
    public int Id { get; set; }

    public DateTime Dated { get; set; }

    public string? Url { get; set; }

    public int? PreparationId { get; set; }

    public int? MemberId { get; set; }

    public short? PreparationTime { get; set; }

    public bool? IsBrowser { get; set; }
}
