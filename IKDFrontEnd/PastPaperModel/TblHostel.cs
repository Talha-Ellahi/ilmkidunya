using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblHostel
{
    public int Id { get; set; }

    public decimal MemberId { get; set; }

    public string HostelName { get; set; } = null!;

    public int CityId { get; set; }

    public int CityAreaId { get; set; }

    public string? FullAddress { get; set; }

    public string? HostelDetails { get; set; }

    public string? Latitude { get; set; }

    public string? Longitude { get; set; }

    public string? Zoomlevel { get; set; }

    public string FeatureIds { get; set; } = null!;

    public string? RewriteUrl { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsApproved { get; set; }

    public bool? IsFeatured { get; set; }

    public bool IsHostel { get; set; }

    public string? RoomHeading { get; set; }

    public decimal? Price { get; set; }

    public bool? IsMale { get; set; }

    public DateTime? Dated { get; set; }

    public string? MainImage { get; set; }

    public string? CityAreaName { get; set; }

    public string? PhoneNumber { get; set; }

    public virtual ICollection<TblHostelImage> TblHostelImages { get; set; } = new List<TblHostelImage>();

    public virtual ICollection<TblHostelRoomType> TblHostelRoomTypes { get; set; } = new List<TblHostelRoomType>();
}
