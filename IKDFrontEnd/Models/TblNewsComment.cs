using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblNewsComment
{
    public decimal CommentId { get; set; }

    public decimal? NewsId { get; set; }

    public DateTime? DatePosted { get; set; }

    public string? Comments { get; set; }

    public bool? IsApproved { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? MemberImage { get; set; }

    public string? PageUrl { get; set; }
}
