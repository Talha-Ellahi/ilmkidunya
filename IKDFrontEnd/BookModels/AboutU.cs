using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class AboutU
{
    public int Id { get; set; }

    public string? Descriptions { get; set; }

    public string? BannerImage { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaKeyword { get; set; }

    public string? MetaDescription { get; set; }
}
