using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class BannerType
{
    public int Id { get; set; }

    public string? BannerTypeName { get; set; }

    public long? Width { get; set; }

    public long? Height { get; set; }

    public string? Description { get; set; }

    public int? SortOrder { get; set; }
}
