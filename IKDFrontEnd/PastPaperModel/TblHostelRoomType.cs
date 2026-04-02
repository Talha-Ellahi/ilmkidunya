using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblHostelRoomType
{
    public int RoomTypeId { get; set; }

    public string? RoomType { get; set; }

    public string? DetailsRoomType { get; set; }

    public decimal? Cost { get; set; }

    public int HostelId { get; set; }

    public virtual TblHostel Hostel { get; set; } = null!;
}
