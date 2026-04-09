using System;
using System.Collections.Generic;

namespace IKDFrontEnd.DBCollege;

public partial class CourseCategory
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? SortOrder { get; set; }

    public string? Image { get; set; }

    public string? Url { get; set; }
}
