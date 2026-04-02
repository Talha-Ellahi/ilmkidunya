using IKDFrontEnd.Models;
using IKDFrontEnd.Services;
using IKDFrontEnd.ViewModels;
using IKDFrontEnd.ViewModels.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IKDFrontEnd.Controllers
{
    public class NewsController : Controller
    {
        private readonly DbikdContext _context;
        private readonly BannerService _bannerService;
        private readonly CmsRepository _cmsRepo;
        private readonly IDistributedCache _distributedCache;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<NewsController> _logger;

        public NewsController(
            DbikdContext context,
            BannerService bannerService,
            CmsRepository cmsRepo,
            IDistributedCache distributedCache,
            IMemoryCache memoryCache,
            ILogger<NewsController> logger)
        {
            _context = context;
            _bannerService = bannerService;
            _cmsRepo = cmsRepo;
            _distributedCache = distributedCache;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        [Route("edunews")]
        public async Task<IActionResult> Home(int page = 1)
        {
            const int pageSize = 50;
            var stopwatch = Stopwatch.StartNew();

            // Get banners (these can be cached separately if needed)
            ViewBag.Banners = await _bannerService.GetBannersAsync();

            // Create cache key based on page
            string cacheKey = $"news_home_page_{page}";

            // For AJAX requests, handle differently
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return await HandleAjaxRequest(page, pageSize);
            }

            // TRY REDIS CACHE FIRST
            try
            {
                var redisStopwatch = Stopwatch.StartNew();
                var cachedData = await _distributedCache.GetStringAsync(cacheKey);
                redisStopwatch.Stop();

                _logger.LogInformation("News Redis read for page {Page} took {ElapsedMs}ms, Found: {Found}",
                    page, redisStopwatch.ElapsedMilliseconds, !string.IsNullOrEmpty(cachedData));

                if (!string.IsNullOrEmpty(cachedData))
                {
                    var deserializeStopwatch = Stopwatch.StartNew();
                    var cachedModel = JsonSerializer.Deserialize<NewsPageViewModel>(cachedData);
                    deserializeStopwatch.Stop();

                    _logger.LogInformation("News Redis deserialize took {ElapsedMs}ms", deserializeStopwatch.ElapsedMilliseconds);

                    if (cachedModel != null)
                    {
                        ViewBag.CacheSource = "Redis";
                        ViewBag.ResponseTime = stopwatch.ElapsedMilliseconds;
                        _logger.LogInformation("News page {Page} served from Redis in {TotalMs}ms", page, stopwatch.ElapsedMilliseconds);
                        return View(cachedModel);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis cache read failed for news page {Page}, falling back to memory cache", page);
            }

            // TRY MEMORY CACHE NEXT (as fallback)
            string memoryCacheKey = $"news_home_page_memory_{page}";
            if (_memoryCache.TryGetValue(memoryCacheKey, out NewsPageViewModel memoryCachedModel))
            {
                ViewBag.CacheSource = "Memory";
                ViewBag.ResponseTime = stopwatch.ElapsedMilliseconds;
                _logger.LogInformation("News page {Page} served from Memory Cache in {TotalMs}ms", page, stopwatch.ElapsedMilliseconds);
                return View(memoryCachedModel);
            }

            // IF NOT IN ANY CACHE, GET FROM DATABASE
            try
            {
                var dbStopwatch = Stopwatch.StartNew();

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

                dbStopwatch.Stop();
                _logger.LogInformation("News page {Page} database query took {ElapsedMs}ms", page, dbStopwatch.ElapsedMilliseconds);

                var newsModel = new NewsPageViewModel
                {
                    LatestNews = newsList,
                    MetaTitle = "Latest News - IKD",
                    MetaDescription = "Get the latest educational news, updates, and announcements.",
                    MetaKeywords = "news, education, results, announcements",
                    MetaImage = "https://images.ishallwin.com/news/default-news-image.jpg",
                    //CurrentPage = page,
                    //TotalPages = await GetTotalPages(pageSize)
                };

                // SAVE TO BOTH CACHES
                var serializeStopwatch = Stopwatch.StartNew();
                var serializedModel = JsonSerializer.Serialize(newsModel);
                serializeStopwatch.Stop();

                // Save to Redis (with expiration based on page - first page caches longer)
                try
                {
                    var redisSaveStopwatch = Stopwatch.StartNew();

                    // Cache first page for 1 hour, other pages for 30 minutes
                    var cacheDuration = page == 1 ? TimeSpan.FromHours(1) : TimeSpan.FromMinutes(30);

                    await _distributedCache.SetStringAsync(cacheKey, serializedModel, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = cacheDuration
                    });

                    redisSaveStopwatch.Stop();
                    _logger.LogInformation("News Redis save for page {Page} took {ElapsedMs}ms", page, redisSaveStopwatch.ElapsedMilliseconds);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to save news page {Page} to Redis cache", page);
                }

                // Save to Memory Cache (shorter duration)
                _memoryCache.Set(memoryCacheKey, newsModel, new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(page == 1 ? 30 : 15))
                    .SetSize(1)); // Optional: set size if using size limits

                ViewBag.CacheSource = "Database";
                ViewBag.ResponseTime = stopwatch.ElapsedMilliseconds;
                ViewBag.DbTime = dbStopwatch.ElapsedMilliseconds;

                _logger.LogInformation("News page {Page} served from Database in {TotalMs}ms (DB: {DbMs}ms)",
                    page, stopwatch.ElapsedMilliseconds, dbStopwatch.ElapsedMilliseconds);

                return View(newsModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading news page {Page}", page);
                return View("Error", new ErrorViewModel { Message = "Unable to load news. Please try again later." });
            }
        }

        private async Task<IActionResult> HandleAjaxRequest(int page, int pageSize)
        {
            string cacheKey = $"news_ajax_page_{page}";

            // Try Redis for AJAX requests too
            try
            {
                var cachedData = await _distributedCache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cachedData))
                {
                    var results = JsonSerializer.Deserialize<object>(cachedData);
                    return Json(results);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis cache read failed for AJAX news page {Page}", page);
            }

            // If not cached, get from database
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

            var hasMore = newsList.Count >= pageSize;
            var result = new
            {
                news = newsList,
                hasMore = hasMore,
                nextPage = hasMore ? page + 1 : (int?)null
            };

            // Cache AJAX response for 15 minutes
            try
            {
                var serialized = JsonSerializer.Serialize(result);
                await _distributedCache.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to cache AJAX news response for page {Page}", page);
            }

            return Json(result);
        }

        private async Task<int> GetTotalPages(int pageSize)
        {
            string cacheKey = "news_total_count";

            // Try to get total count from cache
            try
            {
                var cachedCount = await _distributedCache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cachedCount) && int.TryParse(cachedCount, out int total))
                {
                    return (int)Math.Ceiling(total / (double)pageSize);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get total news count from cache");
            }

            // Get from database if not cached
            var totalNews = await _context.TblMainNews
                .Where(n => n.Approve == true)
                .CountAsync();

            // Cache the total count for 1 hour
            try
            {
                await _distributedCache.SetStringAsync(cacheKey, totalNews.ToString(), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to cache total news count");
            }

            return (int)Math.Ceiling(totalNews / (double)pageSize);
        }

        // Optional: Add method to clear news cache when new news is added
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearNewsCache()
        {
            try
            {
                // Clear common news cache keys
                for (int i = 1; i <= 5; i++) // Clear first 5 pages
                {
                    await _distributedCache.RemoveAsync($"news_home_page_{i}");
                    await _distributedCache.RemoveAsync($"news_ajax_page_{i}");
                    _memoryCache.Remove($"news_home_page_memory_{i}");
                }
                await _distributedCache.RemoveAsync("news_total_count");

                TempData["Success"] = "News cache cleared successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to clear news cache");
                TempData["Error"] = "Failed to clear cache";
            }

            return RedirectToAction(nameof(Home));
        }


        [Route("edunews/news-categories/{categorySlug}.aspx")]
        public async Task<IActionResult> NewsByCategory(string categorySlug, int page = 1)
        {
            const int pageSize = 50;
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;
            var category = await _context.TblNewsCategories
                .FirstOrDefaultAsync(c => c.RewriteUrl == categorySlug.Replace(" ", "-"));

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
                Desc1 = cmsData.Desc1,

                MetaDescription = cmsData.MetaDesc,
                MetaKeywords = cmsData.MetaKeys,
                NewsComments = allComments
            };

            return View(model);
        }

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
