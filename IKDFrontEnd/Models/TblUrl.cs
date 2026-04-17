using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblUrl
{
    public int Id { get; set; }

    public string? PageName { get; set; }

    public string? SectionName { get; set; }

    public string? Url { get; set; }

    public bool? IsShowMenu { get; set; }

    public bool? IsActive { get; set; }

    public string? LogoImage { get; set; }

    public int? SortOrder { get; set; }

    public DateTime? Date { get; set; }
}
