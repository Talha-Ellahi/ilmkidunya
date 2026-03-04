using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IKDFrontEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace IKDFrontEnd.Services
{
    public interface ICommentService
    {
        Task<List<TblDefComment>> GetCommentsAsync(string pageUrl, int skip = 0, int take = 5);
        Task<int> GetCommentsCountAsync(string pageUrl);
        Task<TblDefComment> AddCommentAsync(TblDefComment comment);
        Task<TblDefComment?> GetCommentAsync(int commentId);
        Task<int> GetLikeCountAsync(int commentId);
        Task<bool> HasUserLikedAsync(int commentId, decimal userId);
        Task<List<TblDefComment>> GetRepliesAsync(int parentCommentId);
        Task<bool> ToggleLikeAsync(int commentId, decimal userId);
        Task<bool> DeleteCommentAsync(int commentId, decimal userId);
        Task<Dictionary<int, int>> GetLikeCountsAsync(IEnumerable<int> commentIds);
        Task<Dictionary<int, bool>> GetUserLikeStatusesAsync(IEnumerable<int> commentIds, decimal userId);
        Task<Dictionary<int, List<TblDefComment>>> GetRepliesForCommentsAsync(IEnumerable<int> parentCommentIds);
    }

    public class CommentService : ICommentService
    {
        private readonly DbikdContext _context;

        public CommentService(DbikdContext context)
        {
            _context = context;
        }

        // FIXED: Added AsNoTracking and optimized query
        public async Task<List<TblDefComment>> GetCommentsAsync(string pageUrl, int skip = 0, int take = 5)
        {
            // FIX: Remove ToLower() from comparison for better performance
            // Assume database is already using case-insensitive collation
            var normalizedRequestUrl = pageUrl.Replace("/", "");

            return await _context.TblDefComments
                .AsNoTracking() // CRITICAL: Reduces memory and performance overhead
                .Where(c => c.PageUrl.Replace("/", "") == normalizedRequestUrl &&
                           c.IsActive &&
                           c.IsApproved &&
                           c.ParentCommentId == null)
                .OrderByDescending(c => c.CreatedDate)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetCommentsCountAsync(string pageUrl)
        {
            var normalizedRequestUrl = pageUrl.Replace("/", "");

            return await _context.TblDefComments
                .AsNoTracking() // CRITICAL
                .CountAsync(c => c.PageUrl.Replace("/", "") == normalizedRequestUrl &&
                                 c.IsActive &&
                                 c.IsApproved &&
                                 c.ParentCommentId == null);
        }

        public async Task<List<TblDefComment>> GetRepliesAsync(int parentCommentId)
        {
            return await _context.TblDefComments
                .AsNoTracking() // CRITICAL
                .Where(c => c.ParentCommentId == parentCommentId &&
                           c.IsActive &&
                           c.IsApproved)
                .OrderBy(c => c.CreatedDate)
                .ToListAsync();
        }

        public async Task<TblDefComment> AddCommentAsync(TblDefComment comment)
        {
            try
            {
                comment.CreatedDate = DateTime.Now;
                comment.IsActive = true;
                comment.IsApproved = true;

                _context.TblDefComments.Add(comment);
                await _context.SaveChangesAsync();

                return comment;
            }
            catch (Exception ex)
            {
                // Log error properly instead of Console.WriteLine
                // Consider using ILogger in production
                throw;
            }
        }

        public async Task<TblDefComment?> GetCommentAsync(int commentId)
        {
            return await _context.TblDefComments
                .AsNoTracking() // CRITICAL
                .FirstOrDefaultAsync(c => c.CommentId == commentId);
        }

        public async Task<int> GetLikeCountAsync(int commentId)
        {
            return await _context.TblDefCommentLikes
                .AsNoTracking() // CRITICAL
                .CountAsync(l => l.CommentId == commentId);
        }

        public async Task<bool> HasUserLikedAsync(int commentId, decimal userId)
        {
            return await _context.TblDefCommentLikes
                .AsNoTracking() // CRITICAL
                .AnyAsync(l => l.CommentId == commentId && l.UserId == userId);
        }

        public async Task<bool> ToggleLikeAsync(int commentId, decimal userId)
        {
            var existingLike = await _context.TblDefCommentLikes
                .FirstOrDefaultAsync(l => l.CommentId == commentId && l.UserId == userId);

            if (existingLike != null)
            {
                _context.TblDefCommentLikes.Remove(existingLike);
                await _context.SaveChangesAsync();
                return false;
            }
            else
            {
                var like = new TblDefCommentLike
                {
                    CommentId = commentId,
                    UserId = userId,
                    CreatedDate = DateTime.Now
                };
                _context.TblDefCommentLikes.Add(like);
                await _context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> DeleteCommentAsync(int commentId, decimal userId)
        {
            var comment = await _context.TblDefComments
                .FirstOrDefaultAsync(c => c.CommentId == commentId && c.UserId == userId);

            if (comment == null) return false;

            comment.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Dictionary<int, int>> GetLikeCountsAsync(IEnumerable<int> commentIds)
        {
            if (!commentIds.Any()) return new Dictionary<int, int>();

            return await _context.TblDefCommentLikes
                .AsNoTracking() // CRITICAL
                .Where(l => commentIds.Contains(l.CommentId))
                .GroupBy(l => l.CommentId)
                .Select(g => new { CommentId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.CommentId, x => x.Count);
        }

        public async Task<Dictionary<int, bool>> GetUserLikeStatusesAsync(IEnumerable<int> commentIds, decimal userId)
        {
            if (!commentIds.Any() || userId == 0) return new Dictionary<int, bool>();

            var userLikes = await _context.TblDefCommentLikes
                .AsNoTracking() // CRITICAL
                .Where(l => commentIds.Contains(l.CommentId) && l.UserId == userId)
                .Select(l => l.CommentId)
                .ToListAsync();

            return commentIds.ToDictionary(id => id, id => userLikes.Contains(id));
        }

        public async Task<Dictionary<int, List<TblDefComment>>> GetRepliesForCommentsAsync(IEnumerable<int> parentCommentIds)
        {
            if (!parentCommentIds.Any()) return new Dictionary<int, List<TblDefComment>>();

            var replies = await _context.TblDefComments
                .AsNoTracking() // CRITICAL
                .Where(c => parentCommentIds.Contains(c.ParentCommentId.Value) &&
                           c.IsActive && c.IsApproved)
                .OrderBy(c => c.CreatedDate)
                .ToListAsync();

            return replies.GroupBy(c => c.ParentCommentId.Value)
                .ToDictionary(g => g.Key, g => g.ToList());
        }
    }
}