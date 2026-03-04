using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class User
{
    public int Id { get; set; }

    public string? FullName { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public bool IsActive { get; set; }

    public string? Image { get; set; }

    public bool IsAdmin { get; set; }

    public string? Address { get; set; }

    public string? ContactNo { get; set; }

    public bool IsMasterAdmin { get; set; }

    public virtual ICollection<ContentPage> ContentPages { get; set; } = new List<ContentPage>();

    public virtual ICollection<ContentUnit> ContentUnits { get; set; } = new List<ContentUnit>();

    public virtual ICollection<PanelMenuUserRelation> PanelMenuUserRelations { get; set; } = new List<PanelMenuUserRelation>();

    public virtual ICollection<UrlbasedCmspage> UrlbasedCmspages { get; set; } = new List<UrlbasedCmspage>();
}
