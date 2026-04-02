using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblTltutor
{
    public decimal? MemberId { get; set; }

    public DateTime? Dated { get; set; }

    public decimal TlTutorId { get; set; }

    public string? AboutTutoring { get; set; }

    public string? Experience { get; set; }

    public string? Availability { get; set; }

    /// <summary>
    /// 1 for active alert and 0 for disable alert
    /// </summary>
    public bool ActiveMailAlert { get; set; }

    public string? CourseName { get; set; }

    public string? TutoringOptions { get; set; }

    public bool? IsVerified { get; set; }
}
