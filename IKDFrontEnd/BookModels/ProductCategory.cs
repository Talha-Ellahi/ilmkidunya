using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class ProductCategory
{
    public int Id { get; set; }

    public short CategoryId { get; set; }

    public int ProductId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
