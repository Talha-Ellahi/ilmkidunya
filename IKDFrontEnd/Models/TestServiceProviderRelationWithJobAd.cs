using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TestServiceProviderRelationWithJobAd
{
    public int Id { get; set; }

    public byte? Tspid { get; set; }

    public long? JobId { get; set; }
}
