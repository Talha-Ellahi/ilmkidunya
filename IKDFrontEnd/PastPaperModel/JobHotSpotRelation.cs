using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class JobHotSpotRelation
{
    public int Id { get; set; }

    public int? HotSpotId { get; set; }

    public int? CompanyId { get; set; }
}
