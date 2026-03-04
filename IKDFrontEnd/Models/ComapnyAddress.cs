using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class ComapnyAddress
{
    public int Id { get; set; }

    public int? CompanyId { get; set; }

    public string? CompanyAddress { get; set; }

    public string? ContactNumber { get; set; }

    public int? CityId { get; set; }

    public int? Zoom { get; set; }

    public string? Latitude { get; set; }

    public string? Longitude { get; set; }

    public bool? IsActive { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? Dated { get; set; }

    public DateTime? EstablishedDate { get; set; }

    public int? SortOrder { get; set; }
}
