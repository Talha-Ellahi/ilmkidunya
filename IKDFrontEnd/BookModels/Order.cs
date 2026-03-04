using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class Order
{
    public int Id { get; set; }

    public int? MemberId { get; set; }

    public byte? DliveryStatusId { get; set; }

    public byte? PaymentModeId { get; set; }

    public byte? EmirateId { get; set; }

    public string? Address { get; set; }

    public string? Area { get; set; }

    public DateTime? OrderDate { get; set; }

    public bool? IsDelete { get; set; }

    public int? TotalPrice { get; set; }

    public decimal ShippingPrice { get; set; }

    public virtual DliveryStatus? DliveryStatus { get; set; }

    public virtual TblMember? Member { get; set; }

    public virtual ICollection<OrderDetail1> OrderDetail1s { get; set; } = new List<OrderDetail1>();

    public virtual PaymentMode? PaymentMode { get; set; }
}
