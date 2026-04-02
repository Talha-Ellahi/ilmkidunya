using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class Collegeformdatum
{
    public long Id { get; set; }

    public int CollegeId { get; set; }

    public string Name { get; set; } = null!;

    public string? Mobile { get; set; }

    public int CityId { get; set; }

    public string? Qualification { get; set; }

    public DateOnly Dated { get; set; }

    public int UserId { get; set; }

    public string? Email { get; set; }
}
