using System;
using System.Collections.Generic;

namespace IKDFrontEnd.DBCollege;

public partial class TblXcourseLevel
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Heading { get; set; }

    public string? Url { get; set; }

    public string? ImageName { get; set; }

    public int? SortOrder { get; set; }

    public DateTime? Dated { get; set; }

    public bool? IsActive { get; set; }

    public int? UpdatedBy { get; set; }
}
