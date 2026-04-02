using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblFlag
{
    public int Id { get; set; }

    public string? Image { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaDesc { get; set; }

    public string? MetaKeyword { get; set; }

    public string? Desc1 { get; set; }

    public string? Desc2 { get; set; }

    public string? Desc3 { get; set; }

    public string? Desc4 { get; set; }

    public string? Detail { get; set; }

    public string? File1 { get; set; }

    public string? File2 { get; set; }

    public string? File3 { get; set; }

    public string? File4 { get; set; }

    public string? File5 { get; set; }

    public int? CountryId { get; set; }

    public string? Continent { get; set; }

    public string? Language { get; set; }

    public string? Capital { get; set; }

    public string? Border { get; set; }

    public string? Government { get; set; }

    public string? Neighbour { get; set; }

    public string? Faqs { get; set; }

    public bool? IsActive { get; set; }
}
