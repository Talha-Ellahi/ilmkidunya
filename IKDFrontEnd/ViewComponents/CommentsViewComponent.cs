// ViewComponents/CommentsViewComponent.cs
using IKDFrontEnd.Models;
using IKDFrontEnd.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IKDFrontEnd.ViewComponents
{
    public class CommentsViewComponent : ViewComponent
    {
        private readonly ICommentService _commentService;

        public CommentsViewComponent(ICommentService commentService)
        {
            _commentService = commentService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var pageUrl = $"{HttpContext.Request.Path}{HttpContext.Request.QueryString}";
            var pageTitle = ViewContext.ViewData["Title"]?.ToString();

            // Load initial comments
            var comments = await _commentService.GetCommentsAsync(pageUrl, 0, 5);
            var totalCount = await _commentService.GetCommentsCountAsync(pageUrl);

            var userPrincipal = HttpContext.User as ClaimsPrincipal;
            var isAuthenticated = userPrincipal?.Identity?.IsAuthenticated ?? false;

            decimal userId = 0;
            string userName = "";
            string userEmail = "";
            string userPicture = "https://resources.ilmkidunya.com/images/avatar.png";

            if (isAuthenticated && userPrincipal != null)
            {
                userId = decimal.TryParse(userPrincipal.FindFirstValue("UserId"), out var uid) ? uid : 0;
                userName = userPrincipal.FindFirst(ClaimTypes.Name)?.Value ?? "";
                userEmail = userPrincipal.FindFirst(ClaimTypes.Email)?.Value ?? "";
                userPicture = userPrincipal.FindFirst("picture")?.Value ?? "https://resources.ilmkidunya.com/images/avatar.png";
            }

            // Get all comment IDs for batch processing
            var commentIds = comments.Select(c => c.CommentId).ToList();

            // Batch query for like counts
            var likeCounts = await _commentService.GetLikeCountsAsync(commentIds);

            // Batch query for user like statuses
            Dictionary<int, bool> userLikeStatuses = new();
            if (isAuthenticated)
            {
                userLikeStatuses = await _commentService.GetUserLikeStatusesAsync(commentIds, userId);
            }

            // Batch query for replies
            var repliesDict = await _commentService.GetRepliesForCommentsAsync(commentIds);

            // Get all reply IDs for batch processing
            var replyIds = repliesDict.Values.SelectMany(r => r).Select(r => r.CommentId).ToList();

            // Batch query for reply like counts and statuses
            var replyLikeCounts = await _commentService.GetLikeCountsAsync(replyIds);
            Dictionary<int, bool> replyUserLikeStatuses = new();
            if (isAuthenticated)
            {
                replyUserLikeStatuses = await _commentService.GetUserLikeStatusesAsync(replyIds, userId);
            }

            // Apply the data to comments
            foreach (var comment in comments)
            {
                comment.LikeCount = likeCounts.ContainsKey(comment.CommentId) ? likeCounts[comment.CommentId] : 0;

                if (isAuthenticated)
                {
                    comment.HasLiked = userLikeStatuses.ContainsKey(comment.CommentId) && userLikeStatuses[comment.CommentId];
                }

                // Get and set replies
                if (repliesDict.TryGetValue(comment.CommentId, out var replies))
                {
                    comment.Replies = replies;

                    // Set like data for replies
                    foreach (var reply in replies)
                    {
                        reply.LikeCount = replyLikeCounts.ContainsKey(reply.CommentId) ? replyLikeCounts[reply.CommentId] : 0;

                        if (isAuthenticated)
                        {
                            reply.HasLiked = replyUserLikeStatuses.ContainsKey(reply.CommentId) && replyUserLikeStatuses[reply.CommentId];
                        }
                    }
                }
                else
                {
                    comment.Replies = new List<TblDefComment>();
                }
            }

            var model = new CommentViewModel
            {
                PageUrl = pageUrl,
                PageTitle = pageTitle,
                Comments = comments,
                TotalCount = totalCount,
                IsAuthenticated = isAuthenticated,
                UserId = userId,
                UserName = userName,
                UserEmail = userEmail,
                UserPicture = userPicture,
                Skip = 5  // Changed from 10 to match the initial take of 5
            };

            return View(model);
        }
    }

    public class CommentViewModel
    {
        public string PageUrl { get; set; } = null!;
        public string? PageTitle { get; set; }
        public List<TblDefComment> Comments { get; set; } = new();
        public int TotalCount { get; set; }
        public bool IsAuthenticated { get; set; }
        public decimal UserId { get; set; }
        public string UserName { get; set; } = "";
        public string UserEmail { get; set; } = "";
        public string UserPicture { get; set; } = "";
        public int Skip { get; set; }
        public string SortOrder { get; set; } = "newest";
    }
}