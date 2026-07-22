using System;
using System.Collections.Generic;

namespace IKDFrontEnd.DBCollege;

public partial class ErrorList
{
    public int ErrorId { get; set; }

    public string ErrorUrl { get; set; } = null!;

    public string? ReferrerUrl { get; set; }

    public string? Ip { get; set; }

    public DateOnly CreatedDate { get; set; }

    public DateOnly? UpdatedDate { get; set; }

    public int ErrorCount { get; set; }
}
