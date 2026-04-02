using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblDefComment
{
    public int CommentId { get; set; }

    public string PageUrl { get; set; } = null!;

    public string? PageTitle { get; set; }

    public decimal UserId { get; set; }

    public string? UserName { get; set; }

    public string? UserEmail { get; set; }

    public string CommentText { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public bool IsActive { get; set; }

    public bool IsApproved { get; set; }

    public int? ParentCommentId { get; set; }

    public string? UserProfilePicture { get; set; }

    public string? PageUrlNoSlash { get; set; }

    public virtual ICollection<TblDefCommentLike> TblDefCommentLikes { get; set; } = new List<TblDefCommentLike>();
}
