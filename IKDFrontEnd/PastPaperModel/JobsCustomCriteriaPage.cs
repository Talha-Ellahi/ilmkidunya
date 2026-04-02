using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class JobsCustomCriteriaPage
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Keywords { get; set; }

    public string? Url { get; set; }

    public int? JobNature { get; set; }

    public int? JobStatus { get; set; }

    public string? PostDate { get; set; }

    public string? ApplyDate { get; set; }

    public string? TestDate { get; set; }

    public string? Category { get; set; }

    public int? City { get; set; }

    public string? Newspapers { get; set; }

    public int? TestProvider { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? Dated { get; set; }

    public string? CompanyId { get; set; }

    public string? IsGovt { get; set; }
}
