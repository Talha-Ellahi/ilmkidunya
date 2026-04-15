using System;
using System.Collections.Generic;

namespace IKDFrontEnd.PastPaperModels;

public partial class PastPaperPageDescription
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public int? Sortorder { get; set; }
}
