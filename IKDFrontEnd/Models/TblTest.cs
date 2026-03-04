using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblTest
{
    public int Id { get; set; }

    public byte? TestType { get; set; }

    public decimal? ObtainedMarks { get; set; }

    public decimal? TotalMarks { get; set; }

    public DateTime? Dated { get; set; }

    public bool? IsPassed { get; set; }

    public string? Url { get; set; }

    public int? TestId { get; set; }

    public int? MemberId { get; set; }

    public short? TestTime { get; set; }

    public bool? IsBrowser { get; set; }
}
