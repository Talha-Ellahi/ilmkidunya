using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class ProductBrand
{
    public int Id { get; set; }

    public string? BrandName { get; set; }

    public string? Description { get; set; }

    public string? BrandImage { get; set; }

    public bool? IsShowHome { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
