using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class Service
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Detail { get; set; }

    public string? Image { get; set; }

    public string? Url { get; set; }

    public short CategoryId { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaDesc { get; set; }

    public string? MetaKeyword { get; set; }

    public virtual Category Category { get; set; } = null!;
}
