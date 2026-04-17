using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class UserRight
{
    public int UserRightId { get; set; }

    public int MenuId { get; set; }

    public int SubMenuId { get; set; }

    public int AdminId { get; set; }

    public bool IsAllowed { get; set; }

    public virtual Admin Admin { get; set; } = null!;

    public virtual Menu Menu { get; set; } = null!;

    public virtual SubMenu SubMenu { get; set; } = null!;
}
