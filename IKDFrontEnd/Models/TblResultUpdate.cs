using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblResultUpdate
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int? ClassId { get; set; }

    public int? BoardId { get; set; }

    public DateTime? Dated { get; set; }

    public string? RollNo { get; set; }

    public string? MobileNo { get; set; }

    public string? ClassName { get; set; }

    public string? BoardName { get; set; }
}
