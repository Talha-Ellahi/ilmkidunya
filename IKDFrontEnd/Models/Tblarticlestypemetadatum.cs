using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class Tblarticlestypemetadatum
{
    public decimal CaiId { get; set; }

    public string? Title { get; set; }

    public string? MetaDescription { get; set; }

    public string? MetaKeywords { get; set; }

    public int CatId { get; set; }
}
