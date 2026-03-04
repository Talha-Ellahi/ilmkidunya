using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblHostelImage
{
    public int Id { get; set; }

    public int HostelId { get; set; }

    public string? Thumbnail { get; set; }

    public string? ImageName { get; set; }

    public int? SortOrder { get; set; }

    public DateTime? Dated { get; set; }

    public virtual TblHostel Hostel { get; set; } = null!;
}
