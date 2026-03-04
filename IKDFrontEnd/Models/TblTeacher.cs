using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblTeacher
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Qualification { get; set; }

    public string? Photo { get; set; }

    public bool? IsActive { get; set; }

    public int? SortOrder { get; set; }

    public DateTime? Date { get; set; }
}
