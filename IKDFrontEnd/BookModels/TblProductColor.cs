using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class TblProductColor
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public int? ColorId { get; set; }

    public virtual TblColor? Color { get; set; }

    public virtual Product? Product { get; set; }
}
