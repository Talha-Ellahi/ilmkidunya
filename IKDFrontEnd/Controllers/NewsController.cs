using IKDFrontEnd.Models;
using IKDFrontEnd.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using IKDFrontEnd.Services;
using System.Data;
using System.Security.Policy;
using IKDFrontEnd.ViewModels.Common;

namespace IKDFrontEnd.Controllers
	{
		public class NewsController : Controller
		{
        private readonly DbikdContext _context;
        private readonly BannerService _bannerService;
        private readonly CmsRepository _cmsRepo;

        public NewsController(DbikdContext context, BannerService bannerService, CmsRepository cmsRepo)
        {
            _context = context;
            _bannerService = bannerService;
            _cmsRepo = cmsRepo;
        }

        [Route("edunews")]
        public async Task<IActionResult> Home(int page = 1)
        {
            const int pageSize = 50;

            ViewBag.Banners = await _bannerService.GetBannersAsync();

            var newsList = await _context.TblMainNews
                .AsNoTracking()
                .Where(n => n.Approve == true)
                .OrderByDescending(n => n.Dated)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(n => new TblMainNews
                {
                    NewsId = n.NewsId,
                    MainHeading = n.MainHeading,
                    RewriteUrl = n.RewriteUrl,
                    Dated = n.Dated,
                    PictureThumbnail = string.IsNullOrEmpty(n.PictureThumbnail)
                        ? "https://images.ishallwin.com/news/default-thumb.webp"
                        : $"https://cdn.ilmkidunya.com/news/newsImages/{n.Dated.Value.Year}/{n.Dated.Value.Month}/thumb/{n.PictureThumbnail}"
                })
                .ToListAsync();

            // Check if request is AJAX for JSON response
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    news = newsList,
                    hasMore = newsList.Count >= pageSize,
                    nextPage = page + 1
                });
            }

            var newsModel = new NewsPageViewModel
            {
                LatestNews = newsList,
                MetaTitle = "Latest News - IKD",
                MetaDescription = "Get the latest educational news, updates, and announcements.",
                MetaKeywords = "news, education, results, announcements",
                MetaImage = "https://images.ishallwin.com/news/default-news-image.jpg",
            };

            return View(newsModel);
        }

        [Route("edunews/news-categories/{categorySlug}.aspx")]
		public async Task<IActionResult> NewsByCategory(string categorySlug, int page = 1)
		{
			const int pageSize = 50;
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;
            var category = await _context.TblNewsCategories
				.FirstOrDefaultAsync(c => c.RewriteUrl == categorySlug.Replace(" ","-"));

			if (category == null)
			{
				return NotFound();
			}

			var newsIds = await _context.TblNewsMultiCategories
				.Where(nc => nc.NewsCategoryId == category.NewsCategoryId)
				.Select(nc => nc.NewsId)
				.ToListAsync();

			var newsList = await _context.TblMainNews
				.Where(n => newsIds.Contains(n.NewsId) && n.Approve == true)
				.OrderByDescending(n => n.Dated)
				.Skip((page - 1) * pageSize) 
				.Take(pageSize)
                  .Select(n => new TblMainNews
                  {
                      NewsId = n.NewsId,
                      MainHeading = n.MainHeading,
                      RewriteUrl = n.RewriteUrl,
                      Dated = n.Dated,
                      PictureThumbnail = string.IsNullOrEmpty(n.PictureThumbnail)
                        ? "https://images.ishallwin.com/news/default-thumb.webp"
                        : $"https://cdn.ilmkidunya.com/news/newsImages/{n.Dated.Value.Year}/{n.Dated.Value.Month}/thumb/{n.PictureThumbnail}"
                  })
                .ToListAsync();

			

			var allComments = await _context.TblNewsComments
				.Where(c => newsIds.Contains(c.NewsId.Value))
				.OrderByDescending(c => c.DatePosted)
				.ToListAsync();
            
			//var cmsData = await _context.TblCms
			//	.Where(c => c.Url.Contains($"/news-categories/{categorySlug}"))
			//	.FirstOrDefaultAsync();
            var cmsData = await _cmsRepo.GetByUrlAsync($"/edunews/news-categories/{categorySlug}.aspx");
            cmsData = cmsData ?? new TblCmsDto();

			var model = new NewsPageViewModel
			{
				NewsCategoryName = category.NewsCategoryName,
				LatestNews = newsList,
				MainHeading = cmsData.Heading,
				MetaTitle = cmsData.MetaTitle,
                Desc1=cmsData.Desc1,

                MetaDescription = cmsData.MetaDesc,
				MetaKeywords = cmsData.MetaKeys,
				NewsComments = allComments
			};

			return View(model);
		}


        //[Route("edunews/{newsslug}")]
        //public async Task<IActionResult> NewsDetails(string newsslug)
        //{
        //    var model = new NewsPageViewModel();
        //    var relatedNews = new List<NewsRelated>();

        //    using (var connection = _context.Database.GetDbConnection())
        //    {
        //        await connection.OpenAsync();

        //        using (var command = connection.CreateCommand())
        //        {
        //            command.CommandText = "GetNewsDetails";
        //            command.CommandType = CommandType.StoredProcedure;

        //            var param = command.CreateParameter();
        //            param.ParameterName = "@NewsSlug";
        //            param.Value = newsslug;
        //            command.Parameters.Add(param);

        //            using (var reader = await command.ExecuteReaderAsync())
        //            {
        //                // 1. Main News Item
        //                if (await reader.ReadAsync())
        //                {
        //                    model.MainHeading = reader["Main_Heading"].ToString();
        //                    model.NewsDetails = reader["News_Details"].ToString();
        //                    model.Dated = reader["Dated"] as DateTime?;
        //                    model.NewsUrl = reader["Rewrite_Url"].ToString();
        //                    model.MetaTitle = reader["Meta_Title"]?.ToString();
        //                    model.MetaDescription = reader["MetaDescription"]?.ToString();
        //                    model.MetaKeywords = reader["MetaKeywords"]?.ToString();
        //                    model.MetaImage = reader["Picture_Thumbnail"]?.ToString() ?? "default-image-url";
        //                    model.Picture1 = reader["Picture_1"]?.ToString();
        //                    model.NewsViews = reader["Views"]?.ToString() ?? "N/A";
        //                }

        //                // 2. Related News (Next Result Set)
        //                if (await reader.NextResultAsync())
        //                {
        //                    while (await reader.ReadAsync())
        //                    {
        //                        var newsItem = new NewsRelated
        //                        {
        //                            MainHeading = reader["Main_Heading"].ToString(),
        //                            PictureThumbnail = reader["Picture_Thumbnail"].ToString(),
        //                            NewsUrl = reader["NewsUrl"].ToString(),
        //                            Dated = reader["Dated"] as DateTime?
        //                        };

        //                        // ✅ build thumbnail path
        //                        var year = newsItem.Dated?.Year.ToString();
        //                        var month = newsItem.Dated?.Month.ToString();
        //                        newsItem.PictureThumbnail = $"https://cdn.ilmkidunya.com/news/newsImages/{year}/{month}/thumb/{newsItem.PictureThumbnail}";

        //                        relatedNews.Add(newsItem);
        //                    }
        //                }

        //                // 3. Banners (Next Result Set)
        //                await reader.NextResultAsync();
        //                var banners = new List<Banner>();
        //                while (await reader.ReadAsync())
        //                {
        //                    banners.Add(new Banner
        //                    {
        //                        Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
        //                        Url = reader["Url"]?.ToString(),
        //                        Image = reader["Image"]?.ToString(),
        //                        SortOrder = reader["SortOrder"] != DBNull.Value ? Convert.ToInt32(reader["SortOrder"]) : 0
        //                    });
        //                }
        //                ViewBag.Banners = banners;
        //            }
        //        }
        //    }

        //    // Add related news to model
        //    model.RelatedNews = relatedNews;

        //    return View(model);
        //}

        [Route("edunews/{newsslug}")]
        public async Task<IActionResult> NewsDetails(string newsslug)
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            var news = await _context.TblMainNews
                .Where(n => n.RewriteUrl.Contains(newsslug) && n.Approve == true)
                .FirstOrDefaultAsync();

            if (news == null)
                return NotFound();

            // ✅ Increment views
            news.Views = (news.Views ?? 0) + 1;
            await _context.SaveChangesAsync();

            // ✅ Extract words from Title + Tags
            var titleWords = (news.MainHeading ?? "")
                .Split(new[] { ' ', ',', '.', '-', '_', '/' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(w => w.Trim().ToLower())
                .ToList();

            var tagWords = (news.NewsTags ?? "")
                .Split(new[] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim().ToLower())
                .ToList();

            var keywords = titleWords.Concat(tagWords).Distinct().ToList();

            // ✅ Fetch all approved news except current
            var allNews = await _context.TblMainNews
                .Where(n => n.NewsId != news.NewsId && n.Approve == true)
                .Select(n => new
                {
                    n.NewsId,
                    n.MainHeading,
                    n.NewsTags,
                    n.PictureThumbnail,
                    n.RewriteUrl,
                    n.Dated
                })
                .ToListAsync();

            // ✅ Calculate match score for each news
            var relatedNewsData = allNews
                .Select(n =>
                {
                    var nTitleWords = (n.MainHeading ?? "")
                        .Split(new[] { ' ', ',', '.', '-', '_', '/' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(w => w.Trim().ToLower())
                        .ToList();

                    var nTagWords = (n.NewsTags ?? "")
                        .Split(new[] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(t => t.Trim().ToLower())
                        .ToList();

                    var nWords = nTitleWords.Concat(nTagWords).Distinct().ToList();

                    int matchCount = keywords.Intersect(nWords).Count();

                    return new
                    {
                        n.NewsId,
                        n.MainHeading,
                        n.PictureThumbnail,
                        n.RewriteUrl,
                        n.Dated,
                        MatchScore = matchCount
                    };
                })
                .Where(n => n.MatchScore > 0)  // only keep if some words match
                .OrderByDescending(n => n.Dated) // most relevant first
                .ThenByDescending(n => n.MatchScore)       // tie-breaker by date
                .Take(6)
                .Select(n => new NewsRelated
                {
                    MainHeading = n.MainHeading,
                    PictureThumbnail = n.PictureThumbnail,
                    NewsUrl = n.RewriteUrl,
                    Dated = n.Dated
                })
                .ToList();

            // ✅ Fix thumbnail path
            foreach (var newsItem in relatedNewsData)
            {
                var year = newsItem.Dated?.Year.ToString(); 
                var month = newsItem.Dated?.Month.ToString();

                newsItem.PictureThumbnail = $"https://cdn.ilmkidunya.com/news/newsImages/{year}/{month}/thumb/{newsItem.PictureThumbnail}";
            }

            // ✅ Final model
            var model = new NewsPageViewModel
            {
                MainHeading = news.MainHeading,
                NewsDetails = news.NewsDetails,
                Dated = news.Dated,
                NewsUrl = news.RewriteUrl,
                RelatedNews = relatedNewsData,

                MetaTitle = news.MetaTitle,
                MetaDescription = news.NewsDescription,
                MetaKeywords = news.NewsKeywords,
                MetaImage = news.PictureThumbnail ?? "default-image-url",
                Picture1 = news.Picture1,
                NewsViews = news.Views?.ToString() ?? "N/A"
            };

            return View(model);
        }





        //	[HttpPost]
        //	public async Task<IActionResult> PostComment(string newsslug, TblNewsComment comment, IFormFile CommentImage)
        //      {
        //          var banners = await _bannerService.GetBannersAsync();
        //          ViewBag.Banners = banners;
        //          if (comment == null || string.IsNullOrEmpty(comment.Comments))
        //		{
        //			return RedirectToAction("NewsDetails", new { newsslug = newsslug });
        //		}

        //		var news = await _context.TblMainNews
        //			.Where(n => n.RewriteUrl.Contains(newsslug) && n.Approve == true)
        //			.FirstOrDefaultAsync();

        //		if (news == null)
        //		{
        //			return NotFound();
        //		}

        //		if (CommentImage != null && CommentImage.Length > 0)
        //		{
        //			var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/comments", CommentImage.FileName);

        //			using (var stream = new FileStream(filePath, FileMode.Create))
        //			{
        //				await CommentImage.CopyToAsync(stream);
        //			}
        //			comment.MemberImage = "/images/comments/" + CommentImage.FileName;
        //		}

        //		comment.NewsId = news.NewsId;
        //		comment.DatePosted = DateTime.Now;
        //		comment.IsApproved = false;
        //		comment.PageUrl = "https://www.ilmkidunya.com/edunews/" + newsslug;

        //		_context.TblNewsComments.Add(comment);
        //		await _context.SaveChangesAsync();

        //		return RedirectToAction("NewsDetails", new { newsslug = newsslug });
        //	}
        //[HttpPost]
        //public async Task<IActionResult> PostReply([FromBody] PostReplyDto reply)
        //{
        //	if (reply == null || string.IsNullOrWhiteSpace(reply.Comment))
        //		return BadRequest("Invalid data.");

        //	var newReply = new TblCommentsChild
        //	{
        //		CId = reply.ParentCommentId,
        //		Comment = reply.Comment,
        //		Username = reply.Username,
        //		Email = reply.Email,
        //		Posteddate = DateTime.Now.ToString("dd MMM yyyy"),
        //		IsAdminReply = false,
        //		Abused = 0,
        //		Source = reply.Source,
        //		Ipaddress = reply.IpAddress
        //	};

        //	_context.TblCommentsChildren.Add(newReply);
        //	await _context.SaveChangesAsync();

        //	return Ok();
        //}



    }
}
