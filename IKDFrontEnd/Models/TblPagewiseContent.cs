using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class Tblpagewisecontent
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Url { get; set; }

    public string? Heading { get; set; }

    public string? Detail { get; set; }

    public string? DetailShort { get; set; }

    public string? ImageName { get; set; }

    public string? PageLink { get; set; }

    public string? FbHeader { get; set; }

    public int? SectionId { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaKeyword { get; set; }

    public string? MetaDesc { get; set; }

    public string? MetaTags { get; set; }

    public int? Views { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? Dated { get; set; }

    public int? UpdatedBy { get; set; }

    public string? PageShortDetail { get; set; }

    public string? TxtDetailPopuler { get; set; }
}
