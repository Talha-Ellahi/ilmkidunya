// Controllers/CommentController.cs
using IKDFrontEnd.Models;
using IKDFrontEnd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace IKDFrontEnd.Controllers
{
    [Route("Comment")]
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost("AddComment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment([FromBody] CommentRequest request)
        {
            try
            {
                // Simplified: always check authentication
                if (!(User.Identity?.IsAuthenticated ?? false))
                {
                    return Json(new { success = false, message = "Please sign in to comment" });
                }

                var userPrincipal = HttpContext.User as ClaimsPrincipal;

                var claimsJson = JsonSerializer.Serialize(
                    userPrincipal?.Claims.Select(c => new
                    {
                        c.Type,
                        c.Value,
                        c.Issuer
                    }),
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    }
                );

                Console.WriteLine($"UserPrincipal Claims:\n{claimsJson}");


                var userIdClaim = userPrincipal?.FindFirstValue("UserId");

                if (string.IsNullOrEmpty(userIdClaim) || !decimal.TryParse(userIdClaim, out decimal userId))
                {
                    return Json(new { success = false, message = "Invalid user session" });
                }

                // Get user info from claims (not from request)
                var userName = userPrincipal?.FindFirst(ClaimTypes.Name)?.Value ?? "User";
                var userEmail = userPrincipal?.FindFirst(ClaimTypes.Email)?.Value ?? "";
                var userPicture = userPrincipal?.FindFirst("picture")?.Value ?? "https://resources.ilmkidunya.com/images/avatar.png";

                var comment = new TblDefComment
                {
                    PageUrl = request?.PageUrl ?? "",
                    PageTitle = request?.PageTitle ?? "",
                    UserId = userId,
                    UserName = userName,
                    UserEmail = userEmail,
                    CommentText = request?.CommentText?.Trim() ?? "",
                    UserProfilePicture = userPicture,
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    IsApproved = true
                };

                var addedComment = await _commentService.AddCommentAsync(comment);

                return Json(new
                {
                    success = true,
                    comment = new
                    {
                        commentId = addedComment.CommentId,
                        userName = addedComment.UserName,
                        userId = addedComment.UserId,
                        userPicture = addedComment.UserProfilePicture,
                        commentText = addedComment.CommentText,
                        createdDate = addedComment.CreatedDate.ToString("dd MMM yyyy")
                    }
                });
            }
            catch (Exception ex)
            {
                // Simple error response
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpPost("AddReply")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReply([FromBody] CommentRequest request)
        {
            try
            {
                if (!(User.Identity?.IsAuthenticated ?? false))
                {
                    return Json(new { success = false, message = "Please sign in to reply" });
                }

                var userPrincipal = HttpContext.User as ClaimsPrincipal;
                var userIdClaim = userPrincipal?.FindFirstValue("UserId");

                if (string.IsNullOrEmpty(userIdClaim) || !decimal.TryParse(userIdClaim, out decimal userId))
                {
                    return Json(new { success = false, message = "Invalid user session" });
                }

                var userName = userPrincipal?.FindFirst(ClaimTypes.Name)?.Value ?? "User";
                var userEmail = userPrincipal?.FindFirst(ClaimTypes.Email)?.Value ?? "";
                var userPicture = userPrincipal?.FindFirst("picture")?.Value ?? "https://resources.ilmkidunya.com/images/avatar.png";

                var comment = new TblDefComment
                {
                    PageUrl = request?.PageUrl ?? "",
                    PageTitle = request?.PageTitle ?? "",
                    UserId = userId,
                    UserName = userName,
                    UserEmail = userEmail,
                    CommentText = request?.CommentText?.Trim() ?? "",
                    UserProfilePicture = userPicture,
                    ParentCommentId = request?.ParentCommentId,
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    IsApproved = true
                };

                var addedComment = await _commentService.AddCommentAsync(comment);

                return Json(new
                {
                    success = true,
                    comment = new
                    {
                        commentId = addedComment.CommentId,
                        userName = addedComment.UserName,
                        userId = addedComment.UserId,
                        userPicture = addedComment.UserProfilePicture,
                        commentText = addedComment.CommentText,
                        createdDate = addedComment.CreatedDate.ToString("dd MMM yyyy")
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpPost("LikeComment/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LikeComment(int id)
        {
            try
            {
                if (!(User.Identity?.IsAuthenticated ?? false))
                {
                    return Json(new { success = false, message = "Please sign in to like comments" });
                }

                var userPrincipal = HttpContext.User as ClaimsPrincipal;
                var userIdClaim = userPrincipal?.FindFirstValue("UserId");

                if (string.IsNullOrEmpty(userIdClaim) || !decimal.TryParse(userIdClaim, out decimal userId))
                {
                    return Json(new { success = false, message = "Invalid user session" });
                }

                // Check if comment exists
                var comment = await _commentService.GetCommentAsync(id);
                if (comment == null)
                {
                    return Json(new { success = false, message = "Comment not found" });
                }

                // Toggle like
                var liked = await _commentService.ToggleLikeAsync(id, userId);
                var likeCount = await _commentService.GetLikeCountAsync(id);

                return Json(new
                {
                    success = true,
                    liked = liked,
                    likeCount = likeCount
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }


        [HttpGet("GetReplies/{parentCommentId}")]
        public async Task<IActionResult> GetReplies(int parentCommentId)
        {
            try
            {
                var replies = await _commentService.GetRepliesAsync(parentCommentId);

                // Get like counts for each reply
                foreach (var reply in replies)
                {
                    reply.LikeCount = await _commentService.GetLikeCountAsync(reply.CommentId);

                    if (User.Identity?.IsAuthenticated ?? false)
                    {
                        var userPrincipal = HttpContext.User as ClaimsPrincipal;
                        var userIdClaim = userPrincipal?.FindFirstValue("UserId");
                        if (!string.IsNullOrEmpty(userIdClaim) && decimal.TryParse(userIdClaim, out decimal userId))
                        {
                            reply.HasLiked = await _commentService.HasUserLikedAsync(reply.CommentId, userId);
                        }
                    }
                }

                return Json(replies.Select(r => new
                {
                    commentId = r.CommentId,
                    userId = r.UserId,
                    userName = r.UserName,
                    userPicture = r.UserProfilePicture,
                    commentText = r.CommentText,
                    createdDate = r.CreatedDate.ToString("dd MMM yyyy HH:mm"),
                    likeCount = r.LikeCount,
                    hasLiked = r.HasLiked
                }));
            }
            catch (Exception ex)
            {
                return Json(new List<object>());
            }
        }
        // In CommentController.cs, update the LoadMoreComments method:
        [HttpGet("LoadMoreComments")]
        public async Task<IActionResult> LoadMoreComments(string pageUrl, int skip = 0)
        {
            try
            {
                // Load 10 more comments
                var comments = await _commentService.GetCommentsAsync(pageUrl, skip, 10);
                return PartialView("_CommentListPartial", comments);
            }
            catch
            {
                return Content("");
            }
        }

        [HttpDelete("DeleteComment/{id}")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int id)
        {
            try
            {
                var userPrincipal = HttpContext.User as ClaimsPrincipal;
                var userIdClaim = userPrincipal?.FindFirstValue("UserId");

                if (string.IsNullOrEmpty(userIdClaim) || !decimal.TryParse(userIdClaim, out decimal userId))
                {
                    return Json(new { success = false, message = "Invalid user session" });
                }

                var success = await _commentService.DeleteCommentAsync(id, userId);
                return Json(new { success = success });
            }
            catch
            {
                return Json(new { success = false, message = "Error deleting comment" });
            }
        }
    }

    public class CommentRequest
    {
        public string? PageUrl { get; set; }
        public string? PageTitle { get; set; }
        public string? CommentText { get; set; }
        public int? ParentCommentId { get; set; }
    }
}