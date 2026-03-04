using System.Collections.Concurrent;
using Microsoft.Extensions.Hosting;

public interface IResourceVersionService
{
    string GetVersionedPath(string filePath);
    Task<string> GenerateFileHashAsync(string filePath);
    void ClearCache(); // Optional: for cache invalidation
}

public class ResourceVersionService : IResourceVersionService
{
    private readonly IWebHostEnvironment _env;
    private readonly ConcurrentDictionary<string, string> _fileHashes = new();
    private readonly ILogger<ResourceVersionService> _logger;

    public ResourceVersionService(IWebHostEnvironment env, ILogger<ResourceVersionService> logger = null)
    {
        _env = env;
        _logger = logger;
    }

    public string GetVersionedPath(string filePath)
    {
        try
        {
            // Remove leading ~/ if present and ensure it starts from wwwroot
            var cleanPath = filePath.TrimStart('~', '/');

            var hash = GetFileHash(cleanPath);
            return $"/{cleanPath}?v={hash}";
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Failed to get versioned path for {FilePath}, using unversioned", filePath);
            return filePath;
        }
    }

    public async Task<string> GenerateFileHashAsync(string filePath)
    {
        var physicalPath = Path.Combine(_env.WebRootPath, filePath.TrimStart('~', '/'));

        if (!File.Exists(physicalPath))
        {
            _logger?.LogWarning("File not found: {PhysicalPath}", physicalPath);
            return DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        }

        try
        {
            using var stream = File.OpenRead(physicalPath);
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashBytes = await sha256.ComputeHashAsync(stream);
            return Convert.ToBase64String(hashBytes)[..8]
                .Replace("/", "_")
                .Replace("+", "-")
                .Replace("=", "");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to generate hash for file: {FilePath}", physicalPath);
            return DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        }
    }

    private string GetFileHash(string filePath)
    {
        return _fileHashes.GetOrAdd(filePath, path =>
        {
            var task = GenerateFileHashAsync(path);
            return task.ConfigureAwait(false).GetAwaiter().GetResult();
        });
    }

    public void ClearCache()
    {
        _fileHashes.Clear();
        _logger?.LogInformation("Resource version cache cleared");
    }
}