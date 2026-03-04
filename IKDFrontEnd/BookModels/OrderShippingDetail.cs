using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class OrderShippingDetail
{
    public long Id { get; set; }

    public string? CustomerName { get; set; }

    public string? CustomerEmail { get; set; }

    public string? CustomerContactNo { get; set; }

    public string? CustomerAddress { get; set; }

    public string? CustomerCityCode { get; set; }

    public string? CustomerCity { get; set; }

    public string? OrderShippingPrice { get; set; }

    public string? OrderPaymentType { get; set; }

    public string? ShippingOriginCity { get; set; }

    public string? TotalOrderAmount { get; set; }

    public string? OrderReferenceCode { get; set; }

    public string? BlueExOrderCode { get; set; }

    public string? Cncode { get; set; }

    public string? Status { get; set; }

    public DateTime ShippingDate { get; set; }

    public virtual ICollection<ShippingProductDetail> ShippingProductDetails { get; set; } = new List<ShippingProductDetail>();
}
