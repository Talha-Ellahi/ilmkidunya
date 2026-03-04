using IKDFrontEnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Text;

namespace IKDFrontEnd.Services
{
    public class BannerService
    {
        private readonly DbikdContext _context;
        private readonly IMemoryCache _cache;
        private static readonly SemaphoreSlim _lock = new(1, 1);
        private const string CacheKey = "BANNERS_CACHE";

        public BannerService(DbikdContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<List<Banner>> GetBannersAsync()
        {
            if (_cache.TryGetValue(CacheKey, out List<Banner> cached))
                return cached;

            await _lock.WaitAsync();
            try
            {
                // Double-check after lock
                if (_cache.TryGetValue(CacheKey, out cached))
                    return cached;

                var banners = await _context.Banners
                    .AsNoTracking()
                    .OrderBy(b => b.SortOrder)
                    .ToListAsync() ?? new List<Banner>();

                foreach (var banner in banners)
                {
                    if (!string.IsNullOrEmpty(banner.Url) &&
                        !string.Equals(banner.Advertiser, "no", StringComparison.OrdinalIgnoreCase))
                    {
                        banner.Url = Convert.ToBase64String(
                            Encoding.UTF8.GetBytes(banner.Url)
                        );
                    }
                }

                _cache.Set(CacheKey, banners, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(5)
                });

                return banners;
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task RefreshBannersAsync()
        {
            _cache.Remove(CacheKey);
            await GetBannersAsync();
        }
    }
}
