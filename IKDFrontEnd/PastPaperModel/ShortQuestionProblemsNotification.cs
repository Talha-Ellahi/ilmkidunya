using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class ShortQuestionProblemsNotification
{
    public int Id { get; set; }

    public string? TestName { get; set; }

    public string? ChapterName { get; set; }

    public string? Problem { get; set; }

    public bool? IsCompleted { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? Dated { get; set; }

    public string? Name { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Eamil { get; set; }

    public string? GivenPreparationId { get; set; }

    public byte? Type { get; set; }
}
