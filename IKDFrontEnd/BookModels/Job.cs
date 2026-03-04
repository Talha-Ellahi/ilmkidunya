using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class Job
{
    public int Id { get; set; }

    public string? JobTitle { get; set; }

    public string? Qualification { get; set; }

    public string? Experience { get; set; }

    public string? NoOfPeople { get; set; }

    public DateTime? DeadLine { get; set; }

    public string? Description { get; set; }

    public bool Active { get; set; }

    public int? ApplicationSubmitted { get; set; }

    public DateTime? Date { get; set; }

    public virtual ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
}
