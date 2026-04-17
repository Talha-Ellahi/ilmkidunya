using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class SubMenu
{
    public int SubMenuId { get; set; }

    public int MenuId { get; set; }

    public string SubMenuName { get; set; } = null!;

    public string Controller { get; set; } = null!;

    public string Action { get; set; } = null!;

    public int SortingOrder { get; set; }

    public virtual Menu Menu { get; set; } = null!;

    public virtual ICollection<UserRight> UserRights { get; set; } = new List<UserRight>();
}
