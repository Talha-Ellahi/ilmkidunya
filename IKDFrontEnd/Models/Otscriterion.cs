using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class Otscriterion
{
    public short Id { get; set; }

    public string TestName { get; set; } = null!;

    public string Url { get; set; } = null!;

    public bool IsActive { get; set; }

    public byte? UpdatedBy { get; set; }

    public DateTime? LastUpdated { get; set; }

    public bool? Tabs { get; set; }

    public bool? MixTabs { get; set; }

    public string? ImageName { get; set; }

    public bool? IsCustomTest { get; set; }

    public string? AppName { get; set; }

    public string? ImageName2 { get; set; }

    public virtual ICollection<OtscriterionDetail> OtscriterionDetails { get; set; } = new List<OtscriterionDetail>();
}
