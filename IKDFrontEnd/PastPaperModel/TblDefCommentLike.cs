using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblDefCommentLike
{
    public int LikeId { get; set; }

    public int CommentId { get; set; }

    public decimal UserId { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual TblDefComment Comment { get; set; } = null!;
}
