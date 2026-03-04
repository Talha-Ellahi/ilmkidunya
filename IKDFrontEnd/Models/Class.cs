using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class Class
{
    public int Id { get; set; }

    public int? QualificationId { get; set; }

    public string? Name { get; set; }

    public string? Heading { get; set; }

    public string? Url { get; set; }

    public string? Detail { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaKeyword { get; set; }

    public string? MetaDesc { get; set; }

    public int? SortOrder { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? Dated { get; set; }

    public int? UpdatedBy { get; set; }

    public bool? ShowResultApp { get; set; }

    public string? ImageName { get; set; }

    public string? ResultAppName { get; set; }
}
