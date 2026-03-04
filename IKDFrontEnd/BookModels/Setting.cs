using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class Setting
{
    public short Id { get; set; }

    public string? Title { get; set; }

    public string? Value { get; set; }

    public string? Email { get; set; }

    public string? Mobile { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? Timing { get; set; }

    public string? HomePageContent { get; set; }

    public string? HomePageMetatTitle { get; set; }

    public string? HomePageMetatKeyword { get; set; }

    public string? HomePageMetatDescription { get; set; }

    public string? WebFavicon { get; set; }

    public string? WebLogo { get; set; }

    public string? FooterLogo { get; set; }

    public string? Client { get; set; }
}
