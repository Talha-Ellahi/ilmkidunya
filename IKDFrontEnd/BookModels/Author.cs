using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class Author
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Image { get; set; }

    public int? SortOrder { get; set; }

    public bool? IsTop { get; set; }

    public int? Active { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaDesc { get; set; }

    public string? MetaKeywords { get; set; }
}
