using DinkToPdf;
using DinkToPdf.Contracts;
using IKDFrontEnd.BackupModel1;
using IKDFrontEnd.BackupModel2;
using IKDFrontEnd.BackupModel3;
using IKDFrontEnd.BookModels;
using IKDFrontEnd.DBCollege;
using IKDFrontEnd.DictionaryModels;
using IKDFrontEnd.Helpers;
using IKDFrontEnd.Interfaces;
using IKDFrontEnd.JobModels;
using IKDFrontEnd.Models;
using IKDFrontEnd.PastPaperModels;
using IKDFrontEnd.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders; // ADD THIS
using Serilog;
using StackExchange.Redis;
using System;
using System.IO.Compression;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// ---------------- Services ----------------
builder.Services.AddSignalR();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<BannerService>();
builder.Services.AddScoped<RandomCmsService>();
builder.Services.AddScoped<CmsRepository>();
builder.Services.AddSingleton<ICompositeViewEngine, CompositeViewEngine>();
builder.Services.AddHttpClient();

// ---------------- Logging ----------------
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddEventLog(); // Windows Event Log
    logging.SetMinimumLevel(LogLevel.Warning);
});

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)          // read from appsettings.json
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Custom exception logging
//builder.Services.AddSingleton<ILogger>(provider =>
//    provider.GetService<ILoggerFactory>().CreateLogger("GlobalLogger"));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
    });

// ---------------- Compression ----------------
// SINGLE ResponseCompression configuration - REMOVED DUPLICATE
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
    options.Providers.Add<BrotliCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
    {
        "image/svg+xml",
        "image/webp",
        "image/jpeg",
        "image/png",
        "image/gif"
    });
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Optimal;
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Optimal;
});
builder.Services.AddScoped<ICommentService, CommentService>();

// ---------------- Output Cache ----------------
builder.Services.AddOutputCache();

// ---------------- Memory Management ----------------
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 100_000_000; // 100MB
});

// ---------------- Controllers ----------------
builder.Services.AddControllersWithViews();

// ---------------- Resource Version Service ----------------
// FIXED: Added IWebHostEnvironment dependency
// ---------------- Resource Version Service ----------------
builder.Services.AddSingleton<IResourceVersionService, ResourceVersionService>();
builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
builder.Services.AddHttpClient<ITezMateService, TezMateService>();

// ---------------- DinkToPdf ----------------
var contextLoad = new CustomAssemblyLoadContext();
contextLoad.LoadUnmanagedLibrary(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "lib", "wkhtmltox", "win", "x64", "libwkhtmltox.dll"));
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

// ---------------- DbContexts ----------------
builder.Services.AddDbContext<DbikdContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    sqlOptions => {
        sqlOptions.CommandTimeout(60); // 60 seconds timeout
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorNumbersToAdd: null);
    }));
builder.Services.AddDbContext<JobsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("JobsDbConnectionString"),
    sqlOptions => {
        sqlOptions.CommandTimeout(60); // 60 seconds timeout
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorNumbersToAdd: null);
    }));
builder.Services.AddDbContext<BookDbikdContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BookStoreConnection"),
    sqlOptions => {
        sqlOptions.CommandTimeout(60);
        sqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null);
    }));
builder.Services.AddDbContext<Dbikd1Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Dbikd1"),
    sqlOptions => {
        sqlOptions.CommandTimeout(60);
        sqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null);
    }));
builder.Services.AddDbContext<Dbikd2Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Dbikd2"),
    sqlOptions => {
        sqlOptions.CommandTimeout(60);
        sqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null);
    }));
builder.Services.AddDbContext<DbCollegeContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DbCollege")));
builder.Services.AddDbContext<PastPaperDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("PastPaperDbConnectionString")));
//builder.Services.AddDbContext<Dbikd3Context>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("Dbikd3"),
//    sqlOptions => {
//        sqlOptions.CommandTimeout(60);
//        sqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null);
//    }));
//builder.Services.AddDbContext<Dbikd4Context>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("Dbikd4"),
//    sqlOptions => {
//        sqlOptions.CommandTimeout(60);
//        sqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null);
//    }));
builder.Services.AddDbContext<DictionaryContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DictionaryDb"),
    sqlOptions => {
        sqlOptions.CommandTimeout(60);
        sqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null);
    }));
builder.Services.AddSingleton<IFtpService>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<FtpService>>();

    return new FtpService(
        ftpServer: "ftp://plesk8100.is.cc",
        ftpUsername: "ftpadmissionsikd",
        ftpPassword: "n9~88hyC6",
        cdnBaseUrl: "https://admissions.ilmkidunya.com",
        logger
    );
});


// With this corrected block:
builder.Services.AddStackExchangeRedisCache(options =>
{
    var config = new ConfigurationOptions
    {
        AbortOnConnectFail = true,
        ConnectTimeout = 15000,
        Password = "1F4AaDVPCswf86E4js8o0JJT8ZbypgDk",
        User = "default",
        Ssl = false,
        SslProtocols = System.Security.Authentication.SslProtocols.Tls12
    };
    config.EndPoints.Add("redis-15065.c265.us-east-1-2.ec2.cloud.redislabs.com", 15065);
    options.ConfigurationOptions = config;

    // Test connection (optional - you can remove this in production)
    try
    {
        var connection = ConnectionMultiplexer.Connect(config);
        Console.WriteLine("✅ Redis connected successfully!");
        connection.Close();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ Redis connection warning: {ex.Message}");
        // Don't throw - let the app continue even if Redis fails
    }
});

// ---------------- Authentication ----------------
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    options.CallbackPath = "/signin-google";
    options.Scope.Add("profile");
    options.ClaimActions.MapJsonKey("picture", "picture", "url");
})
.AddFacebook(options =>
{
    options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
    options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
    options.CallbackPath = "/external-login-callback";
    options.Fields.Add("picture");
    options.ClaimActions.MapCustomJson("picture", json =>
        json.GetProperty("picture").GetProperty("data").GetProperty("url").GetString()
    );
});

var app = builder.Build();

// ---------------- Error Handling ----------------
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler(new ExceptionHandlerOptions
    {
        ExceptionHandlingPath = "/Error/ServerError",
        AllowStatusCode404Response = true
    });
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Error/{0}");

// ---------------- Middlewares ----------------
app.UseHttpsRedirection();
app.UseResponseCompression(); // Compression middleware

// Security headers middleware
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

    await next();
});


// ---------------- Static Files with PROPER Caching Headers ----------------
if (app.Environment.IsDevelopment())
{
    app.UseStaticFiles(new StaticFileOptions
    {
        OnPrepareResponse = ctx =>
        {
            ctx.Context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            ctx.Context.Response.Headers["Pragma"] = "no-cache";
            ctx.Context.Response.Headers["Expires"] = "0";
        }
    });
}
else
{
    app.UseStaticFiles(new StaticFileOptions
    {
        OnPrepareResponse = ctx =>
        {
            var path = ctx.File.PhysicalPath?.ToLower() ?? "";
            var extension = Path.GetExtension(path);
            var requestHost = ctx.Context.Request.Host.ToString();

            // Apply aggressive caching for CDN requests AND local images
            if (requestHost.Contains("cdn.ilmkidunya.com") ||
                extension == ".png" || extension == ".jpg" || extension == ".jpeg" ||
                extension == ".gif" || extension == ".webp" || extension == ".svg" ||
                extension == ".ico" || path.Contains("/banners/") || path.Contains("/icons/"))
            {
                ctx.Context.Response.Headers["Cache-Control"] = "public, max-age=31536000, immutable";
            }
            else if (extension == ".css" || extension == ".js")
            {
                ctx.Context.Response.Headers["Cache-Control"] = "public, max-age=31536000";
            }
            else if (extension == ".woff" || extension == ".woff2" || extension == ".ttf" || extension == ".eot")
            {
                ctx.Context.Response.Headers["Cache-Control"] = "public, max-age=31536000, immutable";
            }
            else
            {
                ctx.Context.Response.Headers["Cache-Control"] = "public, max-age=2592000";
            }

            ctx.Context.Response.Headers.Remove("Pragma");
            ctx.Context.Response.Headers["Vary"] = "Accept-Encoding";
        }
    });
}

app.UseRouting();

// ---------------- Global Exception Handler ----------------
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex,
            "Unhandled exception occurred. Path: {Path}, Method: {Method}, User: {User}",
            context.Request.Path,
            context.Request.Method,
            context.User?.Identity?.Name ?? "Anonymous");


        context.Response.StatusCode = 500;
        context.Response.ContentType = "text/html";
        await context.Response.WriteAsync(@"
            <html><body>
            <h2>Service Temporarily Unavailable</h2>
            <p>Please try again in a few moments.</p>
            <script>setTimeout(function(){ window.location.reload(); }, 3000);</script>
            </body></html>");
    }
});

// ---------------- Dart client block ----------------
app.Use(async (context, next) =>
{
    var userAgent = context.Request.Headers["User-Agent"].ToString();
    if (!string.IsNullOrEmpty(userAgent) && userAgent.Contains("Dart", StringComparison.OrdinalIgnoreCase))
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Unauthorized client (Dart requests are blocked).");
        return;
    }

    await next();
});

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.UseOutputCache();

// ---------------- Routes ----------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();