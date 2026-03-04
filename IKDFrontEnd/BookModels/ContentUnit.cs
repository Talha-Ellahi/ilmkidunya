using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class ContentUnit
{
    public int Id { get; set; }

    public string? UnitCode { get; set; }

    public string? UnitName { get; set; }

    public string? Description { get; set; }

    public DateTime? Date { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
