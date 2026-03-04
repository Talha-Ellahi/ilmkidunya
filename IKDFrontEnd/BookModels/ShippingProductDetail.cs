using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class ShippingProductDetail
{
    public int Id { get; set; }

    public string? ProductCode { get; set; }

    public string? ProductName { get; set; }

    public string? ProductPrice { get; set; }

    public string? Productquantity { get; set; }

    public string? ProductVariation { get; set; }

    public string? Skucode { get; set; }

    public long ShippingOrderId { get; set; }

    public virtual OrderShippingDetail ShippingOrder { get; set; } = null!;
}
