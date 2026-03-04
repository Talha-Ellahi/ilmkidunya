using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblHostelFacility
{
    public int Id { get; set; }

    public string? FacilityName { get; set; }

    public string? FacilityImage { get; set; }

    public int? SortOrder { get; set; }

    public bool? IsActive { get; set; }
}
