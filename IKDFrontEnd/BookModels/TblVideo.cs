using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class TblVideo
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? CategoryId { get; set; }

    public string VideoUrl { get; set; } = null!;

    public string? EmbededCode { get; set; }

    public string? Image { get; set; }

    public string? Url { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaDesc { get; set; }

    public string? MetaKeyword { get; set; }

    public bool IsActive { get; set; }

    public DateTime? Dated { get; set; }

    public string? Detail { get; set; }

    public bool? IsTop { get; set; }

    public bool? IsDeleted { get; set; }

    public string? AltTag { get; set; }
}
