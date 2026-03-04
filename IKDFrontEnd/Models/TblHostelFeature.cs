using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblHostelFeature
{
    public int FeatureId { get; set; }

    public string? FeatureName { get; set; }

    public int? SortOrder { get; set; }

    public string? IconClass { get; set; }
}
