using IKDFrontEnd.Models;
using IKDFrontEnd.Services;
using IKDFrontEnd.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace IKDFrontEnd.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly DbikdContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly BannerService _bannerService;
        private readonly IDistributedCache _distributedCache;

        public ArticlesController(
            DbikdContext context,
            IWebHostEnvironment env,
            BannerService bannerService,
            IDistributedCache distributedCache)  // Added distributed cache parameter
        {
            _context = context;
            _env = env;
            _bannerService = bannerService;
            _distributedCache = distributedCache;
        }

        [Route("/articles/")]
        public async Task<IActionResult> Home(int page = 1)
        {
            const int pageSize = 48;

            // Get banners (unchanged)
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            // Try to get from Redis cache
            string cacheKey = $"articles_home_page_{page}";
            ArticlesPageViewModel ArticleModel = null;

            try
            {
                var cachedData = await _distributedCache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cachedData))
                {
                    ArticleModel = JsonSerializer.Deserialize<ArticlesPageViewModel>(cachedData);
                }
            }
            catch
            {
                // If Redis fails, just continue to database
            }

            // If not in cache, get from database
            if (ArticleModel == null)
            {
                var articles = await _context.TblArticles
                              .AsNoTracking()
                              .Where(n => n.Approve == true)
                              .OrderByDescending(n => n.Dated)
                              .Skip((page - 1) * pageSize)
                              .Take(pageSize)
                              .Select(n => new TblArticle
                              {
                                  ArticleId = n.ArticleId,
                                  Title = n.Title,
                                  RewriteUrl = n.RewriteUrl,
                                  Dated = n.Dated,
                                  SenderName = n.SenderName,
                                  PictureThumbnail = n.PictureThumbnail
                              })
                              .ToListAsync();

                ArticleModel = new ArticlesPageViewModel
                {
                    Articles = articles,
                    CurrentPage = page
                };

                // Save to Redis cache
                try
                {
                    await _distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(ArticleModel), new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) // Cache for 1 hour
                    });
                }
                catch
                {
                    // If Redis fails, just continue
                }
            }

            return View("Home", ArticleModel);
        }



        [Route("articles/article-categories/{categorySlug}.aspx")]
        public async Task<IActionResult> ArticlesByCategory(string categorySlug, string searchTerm, int page = 1)
        {
            const int pageSize = 48;

            ViewBag.Banners = await _bannerService.GetBannersAsync();

            var category = await _context.TblArticleTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.RewriteUrl == categorySlug.Replace(" ", "-"));

            if (category == null)
                return NotFound();

            var metadata = await _context.Tblarticlestypemetadata
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.CatId == category.ArticleTypeId);

            var articlesQuery = _context.TblArticles
                .AsNoTracking()
                .Where(a => a.ArticleTypeId == category.ArticleTypeId && a.Approve == true);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                articlesQuery = articlesQuery.Where(a => a.Title.Contains(searchTerm));
            }

            var articles = await articlesQuery
                .OrderByDescending(a => a.Dated)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new TblArticle
                {
                    ArticleId = a.ArticleId,
                    Title = a.Title,
                    RewriteUrl = a.RewriteUrl,
                    Dated = a.Dated,
                    SenderName = a.SenderName,
                    PictureThumbnail = a.PictureThumbnail
                })
                .ToListAsync();

            // ✅ model fill
            var model = new ArticlesPageViewModel
            {
                MetaTitle = metadata?.Title,
                MetaDescription = metadata?.MetaDescription,
                MetaKeywords = metadata?.MetaKeywords,
                PageTitle = category.ArticleTypeName,
                Articles = articles,
                articleslug = categorySlug,
                CurrentPage = page,
                SearchTerm = searchTerm
            };

            return View(model);
        }







        [Route("articles/{articlesslug}")]
        public async Task<IActionResult> ArticlesDetails(string articlesslug)
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            var articles = await _context.TblArticles
                .Where(n => n.RewriteUrl == articlesslug && n.Approve == true)
                .FirstOrDefaultAsync();

            if (articles == null) return NotFound();

            articles.Viewed = (articles.Viewed ?? 0) + 1;
            await _context.SaveChangesAsync();

            var relatedArticles = await _context.TblArticles
                .AsNoTracking()
                .Where(n => n.Approve == true &&
                            n.ArticleId != articles.ArticleId &&
                            n.ArticleTypeId == articles.ArticleTypeId)
                .OrderByDescending(n => n.Dated)
                .Take(6)
                .Select(n => new TblArticle
                {
                    ArticleId = n.ArticleId,
                    Title = n.Title,
                    RewriteUrl = n.RewriteUrl,
                    Dated = n.Dated,
                    SenderName = n.SenderName,
                    PictureThumbnail = n.PictureThumbnail,
                })
                .ToListAsync();


            var model = new ArticlesPageViewModel
            {
                PageTitle = articles.Title,
                MetaTitle = articles.MetaTitle,
                MetaDescription = articles.ArticleDescription,
                MetaKeywords = articles.ArticleKeywords,
                MetaImage = articles.PictureThumbnail ?? "default-image-url",
                ArticleViews = articles.Viewed?.ToString() ?? "N/A",
                ArticleDetail = articles.ArticleDetails,
                PostDate = articles.Dated?.ToString("dd-MMM-yyyy"),
                ArticleSenderName = articles.SenderName,
                RelatedArticles = relatedArticles,
                ArticleImage = articles.Picture1,
            };

            return View("ArticlesDetails", model);
        }


    }
}
