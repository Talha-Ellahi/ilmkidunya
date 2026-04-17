using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class Menu
{
    public int MenuId { get; set; }

    public string MenuName { get; set; } = null!;

    public string MenuController { get; set; } = null!;

    public int SortingOrder { get; set; }

    public string? Icon { get; set; }

    public virtual ICollection<SubMenu> SubMenus { get; set; } = new List<SubMenu>();

    public virtual ICollection<UserRight> UserRights { get; set; } = new List<UserRight>();
}
