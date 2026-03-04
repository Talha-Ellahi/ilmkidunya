using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class News
{
    public int Id { get; set; }

    public string? Heading { get; set; }

    public string? ShortDescription { get; set; }

    public string? Description { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaKeyword { get; set; }

    public string? MetaDescription { get; set; }

    public string? Image { get; set; }

    public string? Url { get; set; }

    public DateTime? DateTime { get; set; }

    public string? Thumbnail { get; set; }
}
