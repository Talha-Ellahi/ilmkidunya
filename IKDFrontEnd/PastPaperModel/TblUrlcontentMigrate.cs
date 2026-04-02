using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblUrlcontentMigrate
{
    public int Id { get; set; }

    public string? PageName { get; set; }

    public string? Url { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaDescription { get; set; }

    public string? MetaKeywords { get; set; }

    public string? Content1 { get; set; }

    public string? Content2 { get; set; }

    public string? Content3 { get; set; }

    public string? MainImage { get; set; }

    public string? LastEdited { get; set; }

    public DateTime? Date { get; set; }
}
