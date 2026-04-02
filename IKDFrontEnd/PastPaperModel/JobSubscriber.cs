using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class JobSubscriber
{
    public long Id { get; set; }

    public int? MemberId { get; set; }

    public string? JobTypeIds { get; set; }

    public int? CityId { get; set; }

    public string? Dated { get; set; }

    public bool? IsActive { get; set; }
}
