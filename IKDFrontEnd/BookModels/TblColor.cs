using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class TblColor
{
    public int Id { get; set; }

    public string? ColorName { get; set; }

    public string? ColorValue { get; set; }

    public string? CompanyColorValue { get; set; }

    public bool? Optionional { get; set; }

    public virtual ICollection<OrderDetail1> OrderDetail1s { get; set; } = new List<OrderDetail1>();

    public virtual ICollection<TblProductColor> TblProductColors { get; set; } = new List<TblProductColor>();
}
