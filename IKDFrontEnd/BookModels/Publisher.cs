using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class Publisher
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public int? SortOrder { get; set; }
}
