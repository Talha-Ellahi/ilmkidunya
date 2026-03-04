using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class PanelMenuUserRelation
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int PanelMenuId { get; set; }

    public bool IsActive { get; set; }

    public virtual PanelMenu PanelMenu { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
