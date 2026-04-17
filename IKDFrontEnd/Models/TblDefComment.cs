using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IKDFrontEnd.Models;

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

    // Navigation properties for likes (FIXED)
    [InverseProperty("Comment")]
    public virtual ICollection<TblDefCommentLike> TblDefCommentLikes { get; set; } = new List<TblDefCommentLike>();

    // Navigation to user
    [ForeignKey("UserId")]
    public virtual TblDefMemberInfoikd? User { get; set; }

    // Navigation to parent comment (FIXED)
    [ForeignKey("ParentCommentId")]
    [InverseProperty("Replies")]
    public virtual TblDefComment? ParentComment { get; set; }

    // Navigation to replies (FIXED - this is the inverse property)
    [InverseProperty("ParentComment")]
    public virtual ICollection<TblDefComment> Replies { get; set; } = new List<TblDefComment>();
    [NotMapped]
    public int LikeCount { get; set; }

    [NotMapped]
    public bool HasLiked { get; set; }
}