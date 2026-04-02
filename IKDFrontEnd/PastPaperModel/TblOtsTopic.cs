using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblOtsTopic
{
    public int TopicId { get; set; }

    public int ClassId { get; set; }

    public int SubjectId { get; set; }

    public int ChapterId { get; set; }

    public string TopicName { get; set; } = null!;

    public decimal? TopicNumber { get; set; }

    public string? Description { get; set; }
}
