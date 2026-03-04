using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblSch
{
    public int Id { get; set; }

    public string? SchName { get; set; }

    public string? Url { get; set; }

    public int? CountryId { get; set; }

    public string? UniLogoImage { get; set; }

    public string? SchImage { get; set; }

    public string? ApplicationLink { get; set; }

    public DateTime? Deadline { get; set; }

    public string? Description1 { get; set; }

    public string? Description2 { get; set; }

    public string? Description3 { get; set; }

    public bool? IsActive { get; set; }

    public int? SortOrder { get; set; }

    public string? Image { get; set; }

    public int? UserId { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaDescription { get; set; }

    public string? MetaKeyword { get; set; }

    public int? Views { get; set; }

    public string? Keywordfortopsch { get; set; }

    public virtual Country? Country { get; set; }

    public virtual ICollection<TblSchFieldsofStudyChild> TblSchFieldsofStudyChildren { get; set; } = new List<TblSchFieldsofStudyChild>();

    public virtual ICollection<TblSchStudyLevelChild> TblSchStudyLevelChildren { get; set; } = new List<TblSchStudyLevelChild>();

    public virtual ICollection<TblSchTopLinking> TblSchTopLinkings { get; set; } = new List<TblSchTopLinking>();
}
