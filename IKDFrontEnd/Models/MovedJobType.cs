using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class MovedJobType
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Detail { get; set; }

    public string? Url { get; set; }

    public string? Heading { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaKeyword { get; set; }

    public string? MetaDesc { get; set; }

    public string? MetaTags { get; set; }

    public int SortOrder { get; set; }

    public string? ImageName { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? Dated { get; set; }

    public int? UpdatedBy { get; set; }

    public string? Detail2 { get; set; }
}
