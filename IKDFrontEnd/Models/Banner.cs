using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class Banner
{
    public int Id { get; set; }

    public int? BannerTypeId { get; set; }

    public string? Url { get; set; }

    public string? Image { get; set; }

    public string? Advertiser { get; set; }

    public DateTime? Deadline { get; set; }

    public int? SortOrder { get; set; }
}
