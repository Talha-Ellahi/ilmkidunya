using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class PaymentMode
{
    public byte Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
