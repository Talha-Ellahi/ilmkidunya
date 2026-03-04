using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblResultCriteriaChild
{
    public int Id { get; set; }

    public int ResultCriteriaId { get; set; }

    public int Year { get; set; }

    public string FileName { get; set; } = null!;

    public string ViewOnline { get; set; } = null!;

    public string Note { get; set; } = null!;

    public string ExpectedDate { get; set; } = null!;

    public short Status { get; set; }

    public short SortOrder { get; set; }

    public byte ResultType { get; set; }

    public string? Subheading { get; set; }

    public bool? IsActive { get; set; }

    public string? PositionHolder { get; set; }

    public string? ShortDesc { get; set; }

    public string? RewardHtml { get; set; }

    public string? ShortNote { get; set; }

    public string? WebsiteSource { get; set; }

    public string? ResultGazette { get; set; }

    public string? DateText { get; set; }
}
