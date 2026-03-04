using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class JobsCustomCompanyCriterion
{
    public int Id { get; set; }

    public string? PageName { get; set; }

    public string? Url { get; set; }

    public string? Keywords { get; set; }

    public string? IsGovt { get; set; }

    public int? CityId { get; set; }

    public int? CompanyId { get; set; }

    public string? CategoryIds { get; set; }

    public DateTime? Dated { get; set; }

    public int? UpdatedBy { get; set; }
}
