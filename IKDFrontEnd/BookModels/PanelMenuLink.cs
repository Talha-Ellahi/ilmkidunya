using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class PanelMenuLink
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Link { get; set; }

    public int PanelMenuId { get; set; }

    public virtual PanelMenu PanelMenu { get; set; } = null!;
}
