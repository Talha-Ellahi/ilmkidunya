using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblGuide
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Url { get; set; }

    public string? Logo { get; set; }

    public string? AppUrl { get; set; }

    public int? SortOrder { get; set; }

    public string? Title { get; set; }

    public string? ClassId { get; set; }

    public string? LevelId { get; set; }

    public string? QualificationId { get; set; }

    public bool? IsActive { get; set; }

    public string? Title2 { get; set; }

    public string? CategoryId { get; set; }
}
