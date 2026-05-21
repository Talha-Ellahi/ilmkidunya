using System;
using System.Collections.Generic;

namespace IKDFrontEnd.DBComment2;

public partial class TblComments2
{
    public decimal CommentId { get; set; }

    public string? SectionName { get; set; }

    public int? PageNo { get; set; }

    public decimal? ItemId { get; set; }

    public DateTime? DatePosted { get; set; }

    public string? Comments { get; set; }

    public bool? IsApproved { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public decimal? MemberId { get; set; }

    public string? PageUrl { get; set; }

    public int? LikeCmt { get; set; }

    public int? DislikeCmt { get; set; }

    public int? AbusedCmt { get; set; }

    public byte? CommentType { get; set; }

    public string? OtherStuff { get; set; }

    public string? Ipaddress { get; set; }

    public string? Source { get; set; }
}
