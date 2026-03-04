using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblDefCity
{
    public int CityId { get; set; }

    public string? CityName { get; set; }

    public int CountryId { get; set; }

    public string? Url { get; set; }

    public string? Heading { get; set; }

    public string? Detail { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaKeyword { get; set; }

    public string? MetaDesc { get; set; }

    public int? SortOrder { get; set; }

    public string? ImageName { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? Dated { get; set; }

    public int? UpdatedBy { get; set; }

    public double? Lat { get; set; }

    public double? Lng { get; set; }

    public int? ProvinceId { get; set; }

    public string? PostalCode { get; set; }

    public bool? IsImageAvailable { get; set; }
}
