using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblCommentsChild
{
    public int ChildCmtId { get; set; }

    public decimal? CId { get; set; }

    public int? Likes { get; set; }

    public int? Dislikes { get; set; }

    public string? Comment { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }

    public decimal? MemberId { get; set; }

    public string? Posteddate { get; set; }

    public int? Abused { get; set; }

    public bool? IsAdminReply { get; set; }

    public byte? CommentType { get; set; }

    public string? OtherStuff { get; set; }

    public string? Source { get; set; }

    public string? Ipaddress { get; set; }
}
