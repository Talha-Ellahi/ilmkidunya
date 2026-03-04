using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class ShortQuestionCriterion
{
    public short Id { get; set; }

    public string? TestName { get; set; }

    public string? Url { get; set; }

    public bool IsActive { get; set; }

    public byte? UpdatedBy { get; set; }

    public DateTime? LastUpdated { get; set; }

    public bool? Tabs { get; set; }

    public bool? MixTabs { get; set; }

    public string? ImageName { get; set; }

    public string? AppName { get; set; }
}
