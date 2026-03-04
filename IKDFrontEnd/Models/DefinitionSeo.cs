using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class DefinitionSeo
{
    public long Id { get; set; }

    public int? SectionId { get; set; }

    public int? DefinitionId { get; set; }

    public int? RelventId { get; set; }

    public string? RelventName { get; set; }

    public string? Heading { get; set; }

    public string? Url { get; set; }

    public string? Detail { get; set; }

    public string? DetailShort { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaKeyword { get; set; }

    public string? MetaDesc { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public int? SortOrder { get; set; }

    public DateTime? Dated { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? ImageName { get; set; }

    public string? Detailw3 { get; set; }
}
