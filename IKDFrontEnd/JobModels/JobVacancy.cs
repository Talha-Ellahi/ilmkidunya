using System;
using System.Collections.Generic;

namespace IKDFrontEnd.JobModels;

public partial class JobVacancy
{
    public int Id { get; set; }

    public int JobAdId { get; set; }

    public int? CompanyId { get; set; }

    public int? ProfessionId { get; set; }

    public int? JobCategoryId { get; set; }

    public int NoOfVacancies { get; set; }

    public int? AgeLimit { get; set; }

    public string? Experience { get; set; }

    public string? Qualification { get; set; }

    public string? Gender { get; set; }

    public string? Description { get; set; }
}
