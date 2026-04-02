using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblResultCriterion
{
    public int Id { get; set; }

    public int LevelId { get; set; }

    public int ClassId { get; set; }

    public int BoardId { get; set; }

    public DateTime Dated { get; set; }

    public string Heading { get; set; } = null!;

    public bool IsActive { get; set; }

    public short? UpdatedBy { get; set; }

    public string? ResultKeyword { get; set; }

    public string? ResultShortCode { get; set; }

    public bool? Show { get; set; }

    public int? ContentId { get; set; }
}
