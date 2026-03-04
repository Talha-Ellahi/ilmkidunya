using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class OrderDetail
{
    public int Id { get; set; }

    public int? OrderId { get; set; }

    public int? BookId { get; set; }

    public int? Price { get; set; }

    public int? Quantity { get; set; }

    public int? StoreId { get; set; }

    public int? OrderNo { get; set; }

    public string? OrderNumber { get; set; }
}
