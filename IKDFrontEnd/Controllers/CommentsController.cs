
//using IKDFrontEnd.DBComment;
//using IKDFrontEnd.DBComment2;
using IKDFrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class CommentsController : Controller
{
    private readonly DbikdContext _context;
    //private readonly DbComment2Context _context;

    public CommentsController(DbikdContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> PostCommentAjax([FromForm] TblComments2 comment)
    {
        comment.DatePosted = DateTime.UtcNow;
        comment.IsApproved = true;
        comment.Ipaddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        if (User.Identity.IsAuthenticated)
        {
            comment.MemberId = Convert.ToDecimal(User.FindFirst("UserId")?.Value);

            var member = await _context.TblDefMemberInfo2s
                                       .FirstOrDefaultAsync(m => m.MemberId == comment.MemberId);

            if (member != null)
            {
                comment.Name = member.MemberName;
                comment.Email = member.Email; 
            }
        }
        _context.TblComments2s.Add(comment);
        await _context.SaveChangesAsync();

        return Json(new
        {
            success = true,
            comment = new
            {
                comment.CommentId,
                comment.Name,
                comment.Comments,
                DatePosted = comment.DatePosted?.ToString("dd MMM yyyy")
            }
        });
    }

    [HttpPost]
    public async Task<IActionResult> PostReply([FromForm] TblCommentsChild reply)
    {
        reply.Posteddate = DateTime.UtcNow.ToString("dd MMM yyyy");
        if (User.Identity.IsAuthenticated)
        {
            reply.MemberId = Convert.ToDecimal(User.FindFirst("UserId")?.Value);
            var member = await _context.TblDefMemberInfo2s
                                      .FirstOrDefaultAsync(m => m.MemberId == reply.MemberId);

            if (member != null)
            {
                reply.Username = member.MemberName;
                reply.Email = member.Email;
            }
        }
        _context.TblCommentsChildren.Add(reply);
        await _context.SaveChangesAsync();

        return Json(new
        {
            success = true,
            reply = new
            {
                reply.ChildCmtId,
                reply.Username,
                Comment = reply.Comment,
                Posteddate = reply.Posteddate,
                Likes = reply.Likes ?? 0
            }
        });
    }


    [HttpPost]
    public async Task<IActionResult> ToggleLike([FromBody] ToggleLikeRequest request)
    {
        if (request == null || request.CommentId <= 0)
            return BadRequest();

        var comment = await _context.TblComments2s.FindAsync(request.CommentId);
        if (comment == null)
            return NotFound();

        // Update like count
        if (request.Liked)
            comment.LikeCmt = (comment.LikeCmt ?? 0) + 1;
        else
            comment.LikeCmt = Math.Max((comment.LikeCmt ?? 0) - 1, 0);

        await _context.SaveChangesAsync();

        // Return updated like count
        return Ok(new { likes = comment.LikeCmt });
    }

    [HttpPost]
    public async Task<IActionResult> ToggleReplyLike([FromBody] ToggleLikeRequest request)
    {
        var reply = await _context.TblCommentsChildren.FindAsync(request.ReplyId);
        if (reply == null)
            return NotFound();

        if (request.Liked)
            reply.Likes = (reply.Likes ?? 0) + 1;
        else
            reply.Likes = Math.Max((reply.Likes ?? 0) - 1, 0);

        await _context.SaveChangesAsync();
        return Ok(new { likes = reply.Likes });
    }

    public class ToggleLikeRequest
    {
        public decimal CommentId { get; set; }
        public int ReplyId { get; set; }
        public bool Liked { get; set; }
    }

}
