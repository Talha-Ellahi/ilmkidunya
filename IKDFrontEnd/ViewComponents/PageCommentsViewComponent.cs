//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using IKDFrontEnd.Models;
//using IKDFrontEnd.ViewModels;
//using IKDFrontEnd.Services;

//public class PageCommentsViewComponent : ViewComponent
//{
//    private readonly DbikdContext _context;
//    private readonly BannerService _bannerService;

//    public PageCommentsViewComponent(DbikdContext context, BannerService bannerService)
//    {
//        _context = context;
//        _bannerService = bannerService;
//    }


//    public async Task<IViewComponentResult> InvokeAsync()
//    {
//        var currentUrl = HttpContext.Request.Path.ToString().ToLower();

//        var comments = await _context.TblComments2s
//            .Where(c => c.PageUrl.ToLower() == currentUrl && c.IsApproved == true)
//            .Take(10)
//            .OrderByDescending(c => c.CommentId)
//            .ToListAsync();

//        var commentIds = comments.Select(c => c.CommentId).ToList();

//        var replies = await _context.TblCommentsChildren
//            .Where(r => r.CId != null && commentIds.Contains((decimal)r.CId))
//            .ToListAsync();

//        var commentViewModels = comments.Select(c => new CommentWithRepliesViewModel
//        {
//            Comment = c,
//            Replies = replies.Where(r => r.CId == c.CommentId).ToList()
//        }).ToList();

//        var viewModel = new PageCommentsViewModel
//        {
//            PageUrl = currentUrl,
//            Comments = commentViewModels,
//            NewComment = new TblComments2 { PageUrl = currentUrl }
//        };

//        return View(viewModel);
//    }

//}
