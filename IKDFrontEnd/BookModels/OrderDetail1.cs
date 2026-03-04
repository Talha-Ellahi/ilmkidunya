using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class OrderDetail1
{
    public int Id { get; set; }

    public int? OrderId { get; set; }

    public int? ProductId { get; set; }

    public decimal? Price { get; set; }

    public short? Qty { get; set; }

    public int? ColorId { get; set; }

    public virtual TblColor? Color { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }
}
