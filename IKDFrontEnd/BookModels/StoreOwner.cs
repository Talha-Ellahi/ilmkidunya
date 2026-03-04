using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class StoreOwner
{
    public int Id { get; set; }

    public string? StoreName { get; set; }

    public string? Description { get; set; }

    public string? Website { get; set; }

    public string? Logo { get; set; }

    public string? ListingPage { get; set; }

    public string? SingleProduct { get; set; }

    public string? Prefix { get; set; }

    public string? Prefixs { get; set; }
}
