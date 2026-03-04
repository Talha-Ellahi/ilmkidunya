using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblOtsChapter
{
    public int Id { get; set; }

    public int OtsclassId { get; set; }

    public int OtssubjectId { get; set; }

    public int OtschNo { get; set; }

    public string OtschName { get; set; } = null!;

    public string? OtschUrl { get; set; }

    public int? SortOrder { get; set; }

    public string? Description { get; set; }

    public DateTime Date { get; set; }

    public int UserId { get; set; }

    public bool IsActive { get; set; }
}
