using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class Testimonial
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Designation { get; set; }

    public string? Comment { get; set; }

    public string? Image { get; set; }

    public int? SortOrder { get; set; }
}
