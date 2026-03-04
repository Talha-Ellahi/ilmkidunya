using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class ProductPhoto
{
    public int PhotoId { get; set; }

    public int ProductId { get; set; }

    public string? LargePhoto { get; set; }

    public string? MedPhoto { get; set; }

    public string? Thumbnail { get; set; }

    public virtual Product Product { get; set; } = null!;
}
