using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class Country
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Url { get; set; }

    public DateTime? Dated { get; set; }

    public string? Detail { get; set; }

    public string? ImageName { get; set; }

    public int? SortOrder { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaDesc { get; set; }

    public string? MetaKeyword { get; set; }

    public bool? IsActive { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual ICollection<TblSchTop> TblSchTops { get; set; } = new List<TblSchTop>();

    public virtual ICollection<TblSch> TblSches { get; set; } = new List<TblSch>();

    public virtual ICollection<TblStudyAbroadGuide> TblStudyAbroadGuides { get; set; } = new List<TblStudyAbroadGuide>();
}
