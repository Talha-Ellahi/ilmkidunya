using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblShortQuestion
{
    public int QuestionId { get; set; }

    public int ChapterId { get; set; }

    public string? QuestionDescription { get; set; }

    public string? QuestionImage { get; set; }

    public decimal? QuestionMarks { get; set; }

    public DateTime? Dated { get; set; }

    public int? MemberId { get; set; }

    public int? TopicId { get; set; }

    public string? QappeardYrs { get; set; }
}
