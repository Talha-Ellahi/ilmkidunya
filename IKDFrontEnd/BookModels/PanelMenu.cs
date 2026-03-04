using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class PanelMenu
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<PanelMenuLink> PanelMenuLinks { get; set; } = new List<PanelMenuLink>();

    public virtual ICollection<PanelMenuUserRelation> PanelMenuUserRelations { get; set; } = new List<PanelMenuUserRelation>();
}
