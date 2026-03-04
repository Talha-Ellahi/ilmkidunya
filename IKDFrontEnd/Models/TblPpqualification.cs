using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblPpqualification
{
    public int Id { get; set; }

    public string? PpqualificationName { get; set; }

    public string? Image { get; set; }

    public int? SortOrder { get; set; }

    public int? UserId { get; set; }

    public bool? IsDelete { get; set; }

    public virtual ICollection<TblPpclass> TblPpclasses { get; set; } = new List<TblPpclass>();
}
