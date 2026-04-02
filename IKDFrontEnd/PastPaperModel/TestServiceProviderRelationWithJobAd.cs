using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TestServiceProviderRelationWithJobAd
{
    public int Id { get; set; }

    public byte? Tspid { get; set; }

    public long? JobId { get; set; }
}
