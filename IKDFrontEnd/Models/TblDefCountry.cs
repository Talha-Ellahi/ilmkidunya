using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblDefCountry
{
    public decimal CountryId { get; set; }

    public string? CountryName { get; set; }

    public string? Url { get; set; }

    public string? ImageUrl { get; set; }

    public bool? IsActive { get; set; }
}
