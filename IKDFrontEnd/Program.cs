using DinkToPdf;
using DinkToPdf.Contracts;
using IKDFrontEnd.BackupModel1;
using IKDFrontEnd.BackupModel2;
using IKDFrontEnd.BackupModel3;
using IKDFrontEnd.BookModels;
using IKDFrontEnd.DBCollege;
using IKDFrontEnd.DBComment;
using IKDFrontEnd.DBComment2;
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
using Microsoft.AspNetCore.Rewrite;
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
builder.Services.AddControllers()
	.AddNewtonsoftJson();

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
builder.Services.AddScoped<IErrorLogService, ErrorLogService>();
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

builder.Services.AddDbContext<DbComment2Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbComment2"),
    sqlOptions =>
    {
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

//builder.Services.AddDbContext<DbCommentContext>(options =>
//	options.UseSqlServer(builder.Configuration.GetConnectionString("DbComment")));
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

//// ---------------- Global Exception Handler ----------------
//app.Use(async (context, next) =>
//{
//    try
//    {
//        await next();
//    }
//    catch (Exception ex)
//    {
//        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
//        logger.LogError(ex,
//            "Unhandled exception occurred. Path: {Path}, Method: {Method}, User: {User}",
//            context.Request.Path,
//            context.Request.Method,
//            context.User?.Identity?.Name ?? "Anonymous");


//        context.Response.StatusCode = 500;
//        context.Response.ContentType = "text/html";
//        await context.Response.WriteAsync(@"
//            <html><body>
//            <h2>Service Temporarily Unavailable</h2>
//            <p>Please try again in a few moments.</p>
//            <script>setTimeout(function(){ window.location.reload(); }, 3000);</script>
//            </body></html>");
//    }
//});

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
var redirects = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
{
	["/articles/traditional-education-vs-online-education"] =
		"/articles/traditional-education-vs-online-education-3064",

	["/articles/7-reasons-why-traveling-once-a-year-will-make-your-life-easier"] =
		"/articles/7-reasons-why-traveling-once-a-year-will-make-your-life-easier-1953",

	["/articles/8-study-habits-you-should-be-practicing--1830.aspx"] =
		"/articles/8-study-habits-you-should-be-practicing--1830",
	["/articles/latest-updates-on-matric-result-2022-by-bise-swat-bise-kohat-and-bise-mardan"] =
		"/articles/latest-updates-on-matric-result-2022-by-bise-swat-bise-kohat-and-bise-mardan-3254",
	["/articles/the-role-of-youth-in-shaping-the-future-a-path-to-progress"] =
		"/articles/the-role-of-youth-in-shaping-the-future-a-path-to-progress-3469",

	["/edunews/urdu-language-2000-years-old-interesting-finding-pu-vc-10220.aspx"] =
		"/edunews/urdu-language-2000-years-old-interesting-finding-pu-vc-10220",
	["/edunews/education-minister-announces-important-decision-regarding-syllabus-2022"] =
		"/edunews/education-minister-takes-important-decision-regarding-syllabus-2022-25048",
	["/edunews/bahawalpur-board-10th-class-result-2011-on-tomorrow"] =
		"/edunews/bahawalpur-board-10th-class-result-2011-on-july-21-6104",
	["/EduNews/punjab-government-to-hire-at-least-6-634-teachers-11844.aspx"] =
		"/edunews/punjab-government-to-hire-at-least-6-634-teachers-11844",
	["/edunews/online-9th-class-result-2019-of-all-boards-23044.aspx"] =
		"/edunews/online-9th-class-result-2019-of-all-boards-23044",
	["/EduNews/aiou-announces-results-of-its-matric-program-11353.aspx"] =
		"/edunews/aiou-announces-results-of-its-matric-program-11353",
	["/edunews/gcu-wins-all-pakistan-declamation-contest-8099.aspx"] =
		"/edunews/gcu-wins-all-pakistan-declamation-contest-8099",

	["/past_papers/past-paper-2023-24-karachi-university-ba-part-2-pakistan-studies-subjective-116745.aspx"] =
		"/past_papers/past-paper-2023-24-karachi-university-ba-part-2-pakistan-studies-subjective-urdu-medium-116760.aspx",
	["/past_papers/allama-iqbal-open-university-intermediate-human-rights-2014"] =
		"/past_papers/past-papers-2014-allama-iqbal-open-university-intermediate-human-rights-376-32661.aspx",
	["/past_papers/past-papers-2017-dg-khan-board-9th-class-computer-science-english-medium-group-1-subjective-61812.aspx\r\n"] =
		"/past_papers/past-paper-2017-d.g-khan-board-9th-class-computer-science-group-i-subjective.both-83280.aspx",
	["/ma-political-science-part-i-ideology-dynamics-of-politics-in-pakistan-paper-v-pu-2004-6656.aspx"] =
		"/past_papers/past-paper-ma-msc-part-1-punjab-university-political-science-paper-5-subjective-2020-98300.aspx",
	["/past_papers/past-papers-2016-lahore-board-i-com-part-ii-commercial-geography-group-i-subjective-urdu-medium-73418.aspx"] =
		"/past_papers/past-papers-2016-lahore-board-icom-part-2-commercial-geography-subjective-group-1-61891.aspx",
	["/past_papers/gujranwala-board-10th-chemistry-2016"] =
		"/past_papers/gujranwala-board-10th-chemistry-english-medium.aspx",
	["/past_papers/past-paper-2025-sargodha-board-class-9th-tarjuma-tul-quran-group-i-objective-117159.aspx"] =
		"/past_papers/past-paper-2025-sargodha-board-class-9th-tarjuma-tul-quran-group-i-objective-119016.aspx",
	["/past_papers/past-paper-2025-sargodha-board-class-9th-islamiat-group-ii-subjective-117156.aspx"] =
		"/past_papers/past-paper-2025-sargodha-board-class-9th-islamiat-group-ii-subjective-119017.aspx",
	["/past_papers/past-paper-2025-sargodha-board-class-9th-mathematics-group-i-objective-117169.aspx"] =
		"/past_papers/past-paper-2025-sargodha-board-class-9th-mathematics-group-i-objective-119018.aspx",
	["/past_papers/past-paper-2025-lahore-board-inter-part-ii-math-group-ii-subjective-117144.aspx"] =
		"/past_papers/past-paper-2025-lahore-board-inter-part-ii-math-group-ii-subjective-119019.aspx",

	["/colleges/punjab-vocational-training-council-pvtc-khanewal-jahanian-campus"] =
		"/colleges/punjab-vocational-training-council-pvtc-vehari-ludden-campus.aspx",
	["/admissions/MSc-Botany--admissions"] =
		"/ms-botany/admissions",
	["/store/21-lessons-for-the-21st-century-529"] =
		"/store",
	["/store/amelia-earhart-551"] =
		"/store",
	["/store/author/aadarsh-printers-&-publishers-india"] =
		"/store",
	["/store/author/adams,-emma-&-halford,-katy"] =
		"/store",
	["/store/author/baker-&-taylor"] =
		"/store",
	["/store/author/hasbro-entertainment-&-licensing-(france)"] =
		"/store",
	["/store/author/jack-covert-&-todd-sattersten"] =
		"/store",
	["/store/author/m-s-bhalla-&-co-pvt-ltd"] =
		"/store",
	["/store/author/n/a"] =
		"/store",
	["/store/author/schofield-&-sims"] =
		"/store",
	["/store/brandsplaining-why-marketing-is-still-sexist-and-how-to-fix-it-1168"] =
		"/store",
	["/store/brief-answers-to-the-big-questions-the-final-book-from-stephen-hawking-561"] =
		"/store",
	["/store/category/"] =
		"/store",
	["/store/category/12th civics"] =
		"/store",
	["/store/category/8th-"] =
		"/store",
	["/store/category/animals--"] =
		"/store",
	["/store/category/birds--"] =
		"/store",
	["/store/category/animals--"] =
		"/store",
	["/store/category/children-drama-"] =
		"/store",
	["/store/category/css-"] =
		"/store",
	["/store/category/current-affairs-"] =
		"/store",
	["/store/category/cutter-"] =
		"/store",
	["/store/category/educational-toys-"] =
		"/store",
	["/store/category/entertainments-"] =
		"/store",
	["/store/category/exercise-books-"] =
		"/store",
	["/store/category/govt-exams-"] =
		"/store",
	["/store/category/health-care--"] =
		"/store",
	["/store/category/maps--"] =
		"/store",
	["/store/category/tapes-"] =
		"/store",
	["/store/category/travel--"] =
		"/store",
	["/store/dear-life-a-doctors-story-of-love-loss-and-consolation-566"] =
		"/store",
	["/store/dirty-gold---the-rise-and-fall-of-an-international-smuggling-ring-567"] =
		"/store",
	["/store/dr.-seusss-second-beginner-book-collection---box-289"] =
		"/store",
	["/store/essential-kitchens-the-back-to-basics-guide-to-home-design-decoration--furnishing---hb-245"] =
		"/store",
	["/store/everybody-has-a-plan-until-they-get-punched-in-the-face---12-things-fighting-teaches-you-about-living-578"] =
		"/store",
	["/store/florentines---from-dante-to-galileo-581"] =
		"/store",
	["/store/louis-van-gaal-the-biography-538"] =
		"/store",
	["/store/ludo-marvel-hero-195"] =
		"/store",
	["/store/malala-yousafzai-595"] =
		"/store",
	["/store/marketing-for-competitiveness-asia-to-the-world-in-the-age-of-digital-consumers-1171"] =
		"/store",
	["/store/music-ideas-in-profile-390"] =
		"/store",
	["/store/my-fab-fashion-style-file-paperback-2018---187"] =
		"/store",
	["/store/remarkable-people---extraordinary-stories-of-everyday-lives-602"] =
		"/store",
	["/store/store/hrh-so-many-thoughts-on-royal-style-hardback-2020"] =
		"/store",
	["/store/the-wall-street-journal-essential-guide-to-management-lasting-lesso-from-the-best-leadership-minds-of-our-time-1950"] =
		"/store",
	["/store/to-plant-a-walnut-tree-how-to-create-a-fruitful-legacy-by-using-your-experience-1951"] =
		"/store",
	["/store/who-is-kamala-harris-619"] =
		"/store",
	["/store/world-of-style-hardback-2018---183"] =
		"/store",
	["/merit-list/-m-phil-human-nutrition-dietetics--merit-list-of-bahauddin-zakariya-university-bzu-multan-2440"] =
		"/colleges/bahauddin-zakariya-university-bzu-multan-merit-lists.aspx",
	["/merit-list/b-a-hons-sociology-merit-list-of-government-college-university-lahore-gcu-lahore-3318"] =
		"/colleges/government-college-university-lahore-gcu-lahore-merit-lists.aspx",
	["/merit-list/b-a-hons-sociology-merit-list-of-government-college-university-lahore-gcu-lahore-3373"] =
		"/colleges/government-college-university-lahore-gcu-lahore-merit-lists.aspx",
	["/merit-list/b-a-hons-sociology-merit-list-of-government-college-university-lahore-gcu-lahore-3409"] =
		"/colleges/government-college-university-lahore-gcu-lahore-merit-lists.aspx",
	["/merit-list/b-ed-merit-list-of-pir-mehar-ali-shah-university-of-arid-agriculture-uaar-rawalpindi"] =
		"/colleges/pir-mehar-ali-shah-university-of-arid-agriculture-uaar-rawalpindi-merit-lists.aspx",
	["/merit-list/ba-english-literature-merit-list-of-government-college-university-lahore-gcu-lahore-3307"] =
		"/colleges/government-college-university-lahore-gcu-lahore-merit-lists.aspx",
	["/merit-list/ba-hons-geography-merit-list-of-government-college-university-lahore-gcu-lahore-3347"] =
		"/colleges/government-college-university-lahore-gcu-lahore-merit-lists.aspx",
	["/merit-list/ba-hons-political-sciences-merit-list-of-government-college-university-lahore-gcu-lahore-3315"] =
		"/colleges/government-college-university-lahore-gcu-lahore-merit-lists.aspx",
	["/merit-list/ba-hons-punjabi-merit-list-of-government-college-university-lahore-gcu-lahore-3372"] =
		"/colleges/government-college-university-lahore-gcu-lahore-merit-lists.aspx",
	["/merit-list/bachelor-in-fine-arts-merit-list-of-university-of-sargodha-7436"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/bachelor-of-dental-surgery-bds--merit-list-of-lahore-medical-dental-college-lahore"] =
		"/colleges/lahore-medical-dental-college-lahore-merit-lists.aspx",
	["/merit-list/bachelor-of-dental-surgery-bds-merit-list-of-fmh-college-of-medicine-and-dentistry"] =
		"/colleges/fmh-college-of-medicine-and-dentistry-merit-lists.aspx",
	["/merit-list/bachelor-of-public-administration-bpa-merit-list-of-bahauddin-zakariya-university-bzu-multan-7213"] =
		"/colleges/bahauddin-zakariya-university-bzu-multan-merit-lists.aspx",
	["/merit-list/bachelor-of-public-administration-bpa-merit-list-of-bahauddin-zakariya-university-bzu-multan-7214"] =
		"/colleges/bahauddin-zakariya-university-bzu-multan-merit-lists.aspx",
	["/merit-list/bachelor-of-public-administration-bpa-merit-list-of-bahauddin-zakariya-university-bzu-multan-7650"] =
		"/colleges/bahauddin-zakariya-university-bzu-multan-merit-lists.aspx",
	["/merit-list/bachelors-environmental-science-merit-list-of-fatima-jinnah-women-university-rawalpindi"] =
		"/colleges/fatima-jinnah-women-university-rawalpindi-merit-lists.aspx",
	["/merit-list/bachelors-physics-merit-list-of-fatima-jinnah-women-university-rawalpindi"] =
		"/colleges/fatima-jinnah-women-university-rawalpindi-merit-lists.aspx",
	["/merit-list/bba-bachelor-of-business-administration-merit-list-of-university-of-sahiwal-uoswl-6869"] =
		"/colleges/university-of-sahiwal-uoswl-merit-lists.aspx",
	["/merit-list/bba-hons--merit-list-of-international-islamic-university-islamabad"] =
		"/colleges/international-islamic-university-islamabad-merit-lists.aspx",
	["/merit-list/bba-hons--merit-list-of-university-of-malakand"] =
		"/colleges/university-of-malakand-merit-lists.aspx",
	["/merit-list/bba-in-business-administration--merit-list-of-university-of-sargodha-3004"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/bba-in-business-administration--merit-list-of-university-of-sargodha-7265"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/bcom-merit-list-of-government-college-university-lahore-gcu-lahore-3358"] =
		"/colleges/government-college-university-lahore-gcu-lahore-merit-lists.aspx",
	["/merit-list/bcom-merit-list-of-government-college-university-lahore-gcu-lahore-3393"] =
		"/colleges/government-college-university-lahore-gcu-lahore-merit-lists.aspx",
	["/merit-list/bcom-merit-list-of-queen-mary-college-university-lahore-5147"] =
		"/colleges/queen-mary-college-university-lahore-merit-lists.aspx",
	["/merit-list/bds-merit-list-of-islamabad-medical-and-dental-college-islamabad-9305"] =
		"/bds/merit-list.aspx",
	["/merit-list/bed-in-education--merit-list-of-university-of-sargodha-2815"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/bs-ahs-public-health-lab-sciences-merit-list-of-university-of-sargodha-8634"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/bs-applied-microbiology-merit-list-of-university-of-veterinary-animal-sciences-uvas-lahore-9398"] =
		"/colleges/university-of-veterinary-animal-sciences-uvas-lahore-merit-lists.aspx",
	["/merit-list/bs-applied-psychology-merit-list-of-bahauddin-zakariya-university-bzu-multan-6256"] =
		"/colleges/bahauddin-zakariya-university-bzu-multan-merit-lists.aspx",
	["/merit-list/bs-applied-psychology-merit-list-of-bahauddin-zakariya-university-bzu-multan-7223"] =
		"/colleges/bahauddin-zakariya-university-bzu-multan-merit-lists.aspx",
	["/merit-list/bs-applied-psychology-merit-list-of-bahauddin-zakariya-university-bzu-multan-7939"] =
		"/colleges/bahauddin-zakariya-university-bzu-multan-merit-lists.aspx",
	["/merit-list/bs-arthropology-merit-list-of-bahauddin-zakariya-university-bzu-multan-7365"] =
		"/colleges/bahauddin-zakariya-university-bzu-multan-merit-lists.aspx",
	["/merit-list/bs-biological-sciences-merit-list-of-university-of-veterinary-animal-sciences-uvas-lahore-9405"] =
		"/colleges/university-of-veterinary-animal-sciences-uvas-lahore-merit-lists.aspx",
	["/merit-list/bs-biotechnology-merit-list-of-abdul-wali-khan-university-mardan"] =
		"/colleges/abdul-wali-khan-university-mardan-merit-lists.aspx",
	["/merit-list/bs-chemistry-merit-list-of-bahauddin-zakariya-university-bzu-multan"] =
		"/colleges/bahauddin-zakariya-university-bzu-multan-merit-lists.aspx",
	["/merit-list/bs-chemistry-merit-list-of-mirpur-university-of-science-technology-must-mirpur-azad-kashmir"] =
		"/colleges/mirpur-university-of-science-technology-must-mirpur-azad-kashmir-merit-lists.aspx",
	["/merit-list/bs-chemistry-merit-list-of-university-of-agriculture-faisalabad-burewala-campus"] =
		"/colleges/university-of-agriculture-faisalabad-merit-lists.aspx",
	["/merit-list/bs-chemistry-merit-list-of-university-of-central-punjab-lahore-ucp-lahore"] =
		"/colleges/university-of-central-punjab-lahore-ucp-lahore-merit-lists.aspx",
	["/merit-list/bs-chemistry-merit-list-of-university-of-okara-6705"] =
		"/colleges/university-of-okara-merit-lists.aspx",
	["/merit-list/bs-computer-science-bscs--merit-list-of-benazir-bhutto-shaheed-university-lyari-karachi"] =
		"/colleges/benazir-bhutto-shaheed-university-lyari-karachi-merit-lists.aspx",
	["/merit-list/bs-computer-science-bscs--merit-list-of-govt-sadiq-college-women-university-bahawalpur"] =
		"/colleges/govt-sadiq-college-women-university-bahawalpur-merit-lists.aspx",
	["/merit-list/bs-computer-science-bscs--merit-list-of-university-of-lahore-uol"] =
		"/colleges/university-of-lahore-uol-merit-lists.aspx",
	["/merit-list/bs-computer-science-bscs--merit-list-of-university-of-sargodha-campus-bhakkar"] =
		"/colleges/university-of-sargodha-campus-bhakkar-merit-lists.aspx",
	["/merit-list/bs-computer-science-merit-list-of-muhammad-nawaz-sharif-university-of-agriculture-multan"] =
		"/colleges/muhammad-nawaz-sharif-university-of-agriculture-multan-merit-lists.aspx",
	["/merit-list/bs-computer-science-merit-list-of-sindh-university-mirpurkhas-campus"] =
		"/colleges/sindh-university-mirpurkhas-campus-merit-lists.aspx",
	["/merit-list/bs-computer-science-merit-list-of-university-of-sindh-jamshoro"] =
		"/colleges/university-of-sindh-jamshoro-merit-lists.aspx",
	["/merit-list/bs-education-merit-list-of-bahauddin-zakariya-university-bzu-multan-7134"] =
		"/colleges/bahauddin-zakariya-university-bzu-multan-merit-lists.aspx",
	["/merit-list/bs-gender-studies-merit-list-of-bahauddin-zakariya-university-bzu-multan"] =
		"/colleges/bahauddin-zakariya-university-bzu-multan-merit-lists.aspx",
	["/merit-list/bs-gender-studies-merit-list-of-bahauddin-zakariya-university-bzu-multan-7168"] =
		"/colleges/bahauddin-zakariya-university-bzu-multan-merit-lists.aspx",
	["/merit-list/bs-history-hons--merit-list-of-international-islamic-university-islamabad"] =
		"/colleges/international-islamic-university-islamabad-merit-lists.aspx",
	["/merit-list/bs-in-chemistry-merit-list-of-university-of-sargodha-2637"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/bs-in-chemistry-merit-list-of-university-of-sargodha-8476"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/bs-in-chemistry-merit-list-of-university-of-sargodha-8477"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/bs-in-economic--merit-list-of-university-of-sargodha-2778"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/bs-in-economic--merit-list-of-university-of-sargodha-3141"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/bs-in-education-merit-list-of-university-of-sargodha-8404"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/bs-in-education-merit-list-of-university-of-sargodha-8406"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/bs-in-english-merit-list-of-university-of-sargodha-6755"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/bs-in-geology-merit-list-of-university-of-sargodha-8458"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/bs-in-it-merit-list-of-university-of-sargodha-2669"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/bs-in-it-merit-list-of-university-of-sargodha-6745"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/bs-in-it-merit-list-of-university-of-sargodha-8467"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/bs-in-library-information-sciences-merit-list-of-university-of-sargodha"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/bs-in-library-information-sciences-merit-list-of-university-of-sargodha-7352"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/bs-in-physics-merit-list-of-university-of-sargodha-3233"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/bs-in-physics-merit-list-of-university-of-sargodha-8446"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/bs-in-psychology-merit-list-of-university-of-sargodha-8362"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/bs-in-sociology-merit-list-of-university-of-sargodha-6591"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/bs-in-software-engineering-bs-se--merit-list-of-university-of-sargodha-2897"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/bs-in-software-engineering-bs-se--merit-list-of-university-of-sargodha-2899"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/bs-in-software-engineering-bs-se--merit-list-of-university-of-sargodha-8472"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/bs-in-urdu-merit-list-of-university-of-sargodha-2822"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/bs-in-urdu-merit-list-of-university-of-sargodha-3133"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/bs-in-zoology-merit-list-of-university-of-sargodha-6450"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/bs-in-zoology-merit-list-of-university-of-sargodha-6724"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/bs-in-zoology-merit-list-of-university-of-sargodha-8422"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/bs-information-technology-merit-list-of-university-of-management-and-technology-umt-lahore"] =
		"/colleges/university-of-management-and-technology-umt-lahore-merit-lists.aspx",
	["/merit-list/bs-international-relation-merit-list-of-bahauddin-zakariya-university-bzu-multan-7179"] =
		"/colleges/bahauddin-zakariya-university-bzu-multan-merit-lists.aspx",
	["/merit-list/bs-international-relations-merit-list-of-international-islamic-university-islamabad"] =
		"/colleges/international-islamic-university-islamabad-merit-lists.aspx",
	["/merit-list/bs-mass-communicataion-merit-list-of-the-women-university-multan-539"] =
		"/colleges/the-women-university-multan-merit-lists.aspx",
	["/merit-list/bs-mass-communication-merit-list-of-university-of-sargodha-6527"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/bs-mathematics-merit-list-of-gomal-university-d-i-khan"] =
		"/colleges/gomal-university-d-i-khan-merit-lists.aspx",
	["/merit-list/bs-mathematics-merit-list-of-mirpur-university-of-science-technology-must-mirpur-azad-kashmir"] =
		"/colleges/mirpur-university-of-science-technology-must-mirpur-azad-kashmir-merit-lists.aspx",
	["/merit-list/bs-mathematics-merit-list-of-university-of-education-vehari"] =
		"/colleges/university-of-education-vehari-merit-lists.aspx",
	["/merit-list/bs-mathematics-merit-list-of-university-of-management-and-technology-umt-lahore"] =
		"/colleges/university-of-management-and-technology-umt-lahore-merit-lists.aspx",
	["/merit-list/bs-mathematics-merit-list-of-university-of-sindh-jamshoro"] =
		"/colleges/university-of-sindh-jamshoro-merit-lists.aspx",
	["/merit-list/bs-nursing--merit-list-of-liaquat-university-of-medical-and-health-sciences-jamshoro"] =
		"/bsn/merit-list",
	["/merit-list/bs-physics-merit-list-of-bahauddin-zakariya-university-bzu-multan-7890"] =
		"/colleges/bahauddin-zakariya-university-bzu-multan-merit-lists.aspx",
	["/merit-list/bs-physics-merit-list-of-bahauddin-zakariya-university-bzu-multan-7894"] =
		"/colleges/bahauddin-zakariya-university-bzu-multan-merit-lists.aspx",
	["/merit-list/bs-physics-merit-list-of-the-women-university-multan-7127"] =
		"/colleges/the-women-university-multan-merit-lists.aspx",
	["/merit-list/bs-software-engineering--merit-list-of-national-university-of-modern-languages-islamabad"] =
		"/colleges/national-university-of-modern-languages-islamabad-merit-lists.aspx",
	["/merit-list/bs-software-engineering-merit-list-of-national-textile-university-faisalabad"] =
		"/colleges/national-textile-university-faisalabad-merit-lists.aspx",
	["/merit-list/bs-statistics-merit-list-of-bahauddin-zakariya-university-bzu-multan-7920"] =
		"/colleges/bahauddin-zakariya-university-bzu-multan-merit-lists.aspx",
	["/merit-list/bs-urdu-merit-list-of-bahauddin-zakariya-university-bzu-multan-7369"] =
		"/colleges/bahauddin-zakariya-university-bzu-multan-merit-lists.aspx",
	["/merit-list/bs-urdu-merit-list-of-bahauddin-zakariya-university-bzu-multan-7370"] =
		"/colleges/bahauddin-zakariya-university-bzu-multan-merit-lists.aspx",
	["/merit-list/bs-urdu-merit-list-of-the-women-university-multan-513"] =
		"/colleges/the-women-university-multan-merit-lists.aspx",
	["/merit-list/bsc-botany-hons--merit-list-of-government-college-university-lahore-gcu-lahore-11072"] =
		"/colleges/government-college-university-lahore-gcu-lahore-merit-lists.aspx",
	["/merit-list/bsc-botany-merit-list-of-government-college-university-lahore-gcu-lahore-3413"] =
		"/colleges/government-college-university-lahore-gcu-lahore-merit-lists.aspx",
	["/merit-list/bsc-civil-engineering-technology-merit-list-of-international-islamic-university-islamabad"] =
		"/colleges/international-islamic-university-islamabad-merit-lists.aspx",
	["/merit-list/bsc-economics-merit-list-of-government-college-university-lahore-gcu-lahore-3363"] =
		"/colleges/government-college-university-lahore-gcu-lahore-merit-lists.aspx",
	["/merit-list/bsc-electronics-merit-list-of-government-college-university-lahore-gcu-lahore-3325"] =
		"/colleges/government-college-university-lahore-gcu-lahore-merit-lists.aspx",
	["/merit-list/bsc-in-animal-science-merit-list-of-university-of-sargodha-8629"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/bsc-merit-list-of-queen-mary-college-university-lahore-5564"] =
		"/colleges/queen-mary-college-university-lahore-merit-lists.aspx",
	["/merit-list/bsc-microbiology-merit-list-of-government-college-university-lahore-gcu-lahore-3417"] =
		"/colleges/government-college-university-lahore-gcu-lahore-merit-lists.aspx",
	["/merit-list/bsc-physics-merit-list-of-government-college-university-lahore-gcu-lahore-3334"] =
		"/colleges/government-college-university-lahore-gcu-lahore-merit-lists.aspx",
	["/merit-list/bsc-psychology-merit-list-of-government-college-university-lahore-gcu-lahore-3371"] =
		"/colleges/government-college-university-lahore-gcu-lahore-merit-lists.aspx",
	["/merit-list/certificate-in-bds-merit-list-of-university-of-health-sciences-lahore-7828"] =
		"/colleges/university-of-health-sciences-lahore-merit-lists.aspx",
	["/merit-list/certificate-in-bds-merit-list-of-university-of-health-sciences-lahore-8561"] =
		"/colleges/university-of-health-sciences-lahore-merit-lists.aspx",
	["/merit-list/certificate-in-bds-merit-list-of-university-of-health-sciences-lahore-8563"] =
		"/colleges/university-of-health-sciences-lahore-merit-lists.aspx",
	["/merit-list/certificate-in-mbbs-merit-list-of-university-of-health-sciences-lahore"] =
		"/colleges/university-of-health-sciences-lahore-merit-lists.aspx",
	["/merit-list/certificate-in-mbbs-merit-list-of-university-of-health-sciences-lahore-7803"] =
		"/colleges/university-of-health-sciences-lahore-merit-lists.aspx",
	["/merit-list/certificate-in-mbbs-merit-list-of-university-of-health-sciences-lahore-7833"] =
		"/colleges/university-of-health-sciences-lahore-merit-lists.aspx",
	["/merit-list/doctor-of-physical-therapy-dpt--merit-list-of-university-of-sargodha-3173"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/doctor-of-physical-therapy-merit-list-of-king-edward-medical-university-lahore"] =
		"/colleges/king-edward-medical-university-lahore-merit-lists.aspx",
	["/merit-list/f-a-arts--merit-list-of-government-college-university-lahore-gcu-lahore-4645"] =
		"/colleges/government-college-university-lahore-gcu-lahore-merit-lists.aspx",
	["/merit-list/f-a-arts--merit-list-of-government-college-university-lahore-gcu-lahore-4699"] =
		"/colleges/government-college-university-lahore-gcu-lahore-merit-lists.aspx",
	["/merit-list/f-a-general-science--merit-list-of-government-college-university-lahore-gcu-lahore-4479"] =
		"/colleges/government-college-university-lahore-gcu-lahore-merit-lists.aspx",
	["/merit-list/f-a-home-economics--merit-list-of-lahore-college-for-women-university-lcwu-lahore-10793"] =
		"/colleges/lahore-college-for-women-university-lcwu-lahore-merit-lists.aspx",
	["/merit-list/fsc-pre-medical-merit-list-of-lahore-college-for-women-university-lcwu-lahore-10780"] =
		"/colleges/lahore-college-for-women-university-lcwu-lahore-merit-lists.aspx",
	["/merit-list/fsc-pre-medical-merit-list-of-lahore-college-for-women-university-lcwu-lahore-5161"] =
		"/colleges/lahore-college-for-women-university-lcwu-lahore-merit-lists.aspx",
	["/merit-list/i-com--merit-list-of-government-college-university-lahore-gcu-lahore-5035"] =
		"/colleges/government-college-university-lahore-gcu-lahore-merit-lists.aspx",
	["/merit-list/ics-economics-merit-list-of-lahore-college-for-women-university-lcwu-lahore-5162"] =
		"/colleges/lahore-college-for-women-university-lcwu-lahore-merit-lists.aspx",
	["/merit-list/ics-merit-list-of-muslim-college-city-campus-multan"] =
		"/colleges/muslim-college-city-campus-multan-merit-lists.aspx",
	["/merit-list/ics-physics-merit-list-of-govt-degree-college-for-women-wapda-town-lahore"] =
		"/colleges/govt-degree-college-for-women-wapda-town-lahore-merit-lists.aspx",
	["/merit-list/ics-physics-merit-list-of-lahore-college-for-women-university-lcwu-lahore-10782"] =
		"/colleges/lahore-college-for-women-university-lcwu-lahore-merit-lists.aspx",
	["/merit-list/ics-physics-merit-list-of-lahore-college-for-women-university-lcwu-lahore-10792"] =
		"/colleges/lahore-college-for-women-university-lcwu-lahore-merit-lists.aspx",
	["/merit-list/ics-physics-merit-list-of-lahore-college-for-women-university-lcwu-lahore-5163"] =
		"/colleges/lahore-college-for-women-university-lcwu-lahore-merit-lists.aspx",
	["/merit-list/ics-statistics-merit-list-of-lahore-college-for-women-university-lcwu-lahore-10790"] =
		"/colleges/lahore-college-for-women-university-lcwu-lahore-merit-lists.aspx",
	["/merit-list/intermediate"] =
		"/admissions/1st-year-merit-lists",
	["/merit-list/king-edward-medical-university-kemu-lahore"] =
		"/colleges/king-edward-medical-university-lahore-merit-lists.aspx",
	["/merit-list/llb-merit-list-of-bahauddin-zakariya-university-bzu-multan-2557"] =
		"/colleges/bahauddin-zakariya-university-bzu-multan-merit-lists.aspx",
	["/merit-list/llb-merit-list-of-bahauddin-zakariya-university-bzu-multan-7472"] =
		"/colleges/bahauddin-zakariya-university-bzu-multan-merit-lists.aspx",
	["/merit-list/m-phil-b-ed-secondary--merit-list-of-bahauddin-zakariya-university-bzu-multan-2561"] =
		"/colleges/bahauddin-zakariya-university-bzu-multan-merit-lists.aspx",
	["/merit-list/m-phil-environmental-science--merit-list-of-bahauddin-zakariya-university-bzu-multan-2496"] =
		"/colleges/bahauddin-zakariya-university-bzu-multan-merit-lists.aspx",
	["/merit-list/m-phil-forestry-range-management--merit-list-of-bahauddin-zakariya-university-bzu-multan-2432"] =
		"/colleges/bahauddin-zakariya-university-bzu-multan-merit-lists.aspx",
	["/merit-list/m-phil-islamic-thought-culture--merit-list-of-bahauddin-zakariya-university-bzu-multan-2483"] =
		"/colleges/bahauddin-zakariya-university-bzu-multan-merit-lists.aspx",
	["/merit-list/m-phil-kashmir-studies-merit-list-of-punjab-university-lahore"] =
		"/colleges/punjab-university-lahore-merit-lists.aspx",
	["/merit-list/m-phil-parasitology--merit-list-of-bahauddin-zakariya-university-bzu-multan-2515"] =
		"/colleges/bahauddin-zakariya-university-bzu-multan-merit-lists.aspx",
	["/merit-list/m-sc-hons-agricultural-engineering-merit-list-of-pir-mehar-ali-shah-university-of-arid-agriculture-uaar-rawalpindi-10343"] =
		"/colleges/pir-mehar-ali-shah-university-of-arid-agriculture-uaar-rawalpindi-merit-lists.aspx",
	["/merit-list/ma-anthropology-merit-list-of-bahauddin-zakariya-university-bzu-multan-6382"] =
		"/colleges/bahauddin-zakariya-university-bzu-multan-merit-lists.aspx",
	["/merit-list/ma-in-arabic-merit-list-of-university-of-sargodha-6760"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/ma-in-english-merit-list-of-university-of-sargodha-7358"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/ma-in-english-merit-list-of-university-of-sargodha-8501"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/ma-in-english-merit-list-of-university-of-sargodha-8502"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/ma-in-history-merit-list-of-university-of-sargodha-8618"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/ma-in-mass-communication-merit-list-of-university-of-sargodha-3095"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/ma-in-mass-communication-merit-list-of-university-of-sargodha-7402"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/ma-in-political-science--merit-list-of-university-of-sargodha-2906"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/ma-in-political-science--merit-list-of-university-of-sargodha-2908"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/ma-islamic-studies-merit-list-of-gomal-university-d-i-khan"] =
		"/colleges/gomal-university-d-i-khan-merit-lists.aspx",
	["/merit-list/ma-islamic-studies-merit-list-of-govt-sadiq-college-women-university-bahawalpur"] =
		"/colleges/govt-sadiq-college-women-university-bahawalpur-merit-lists.aspx",
	["/merit-list/ma-telecommunication-engineering-merit-list-of-islamia-university-of-bahawalpur-pakistan"] =
		"/colleges/islamia-university-of-bahawalpur-pakistan-merit-lists.aspx",
	["/merit-list/mbbs-merit-list-of-al-nafees-medical-college-hospital-9270"] =
		"/colleges/al-nafees-medical-college-hospital-merit-lists.aspx",
	["/merit-list/mbbs-merit-list-of-federal-medical-dental-college-fm-dc-islamabad"] =
		"/colleges/federal-medical-dental-college-fm-dc-islamabad-merit-lists.aspx",
	["/merit-list/mbbs-merit-list-of-fmh-college-of-medicine-and-dentistry"] =
		"/colleges/fmh-college-of-medicine-and-dentistry-merit-lists.aspx",
	["/merit-list/mbbs-merit-list-of-islamabad-medical-and-dental-college-islamabad-9304"] =
		"/mbbs/merit-list.aspx",
	["/merit-list/mbbs-merit-list-of-khyber-medical-university-kmu-peshawar"] =
		"/colleges/khyber-medical-university-kmu-peshawar-merit-lists.aspx",
	["/merit-list/mbbs-merit-list-of-khyber-medical-university-kmu-peshawar-9318"] =
		"/colleges/khyber-medical-university-kmu-peshawar-merit-lists.aspx",
	["/merit-list/mbbs-merit-list-of-king-edward-medical-university-lahore"] =
		"/colleges/king-edward-medical-university-lahore-merit-lists.aspx",
	["/merit-list/mbbs-merit-list-of-mohi-ud-din-islamic-medical-college-mirpur-9306"] =
		"/mbbs/merit-list.aspx",
	["/merit-list/mbbs-merit-list-of-national-university-of-medical-sciences-rawalpindi-9221"] =
		"/colleges/national-university-of-medical-sciences-rawalpindi-merit-lists.aspx",
	["/merit-list/mbbs-merit-list-of-rawal-institute-of-health-sciences-islamabad-9307"] =
		"/colleges/rawal-institute-of-health-sciences-islamabad-merit-lists.aspx",
	["/merit-list/mbbs-merit-list-of-shaikh-khalifa-bin-zayed-al-nahyan-medical-dental-college-lahore"] =
		"/colleges/shaikh-khalifa-bin-zayed-al-nahyan-medical-dental-college-lahore-merit-lists.aspx",
	["/merit-list/mbbs-merit-list-of-university-college-of-medicine-dentistry-lahore"] =
		"/colleges/university-college-of-medicine-dentistry-lahore-merit-lists.aspx",
	["/merit-list/mbbs-merit-list-of-university-of-health-sciences-lahore-9283"] =
		"/colleges/university-of-health-sciences-lahore-merit-lists.aspx",
	["/merit-list/mcom-in-commerce--merit-list-of-university-of-sargodha-3032"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/mcom-in-commerce--merit-list-of-university-of-sargodha-7262"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/mphil-botany-merit-list-of-pir-mehar-ali-shah-university-of-arid-agriculture-uaar-rawalpindi"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/mphil-in-biotechnology-merit-list-of-university-of-sargodha-7348"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/mphil-in-botany-merit-list-of-university-of-sargodha-2620"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/mphil-in-education-merit-list-of-university-of-sargodha-6443"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/mphil-in-psychology-merit-list-of-university-of-sargodha-3112"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/mphil-in-psychology-merit-list-of-university-of-sargodha-6713"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/mphil-in-social-work-merit-list-of-university-of-sargodha-2853"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/mphil-in-zoology-merit-list-of-university-of-sargodha-2774"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/mphil-in-zoology-merit-list-of-university-of-sargodha-2775"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/mphil-sindhi-merit-list-of-university-of-karachi-9201"] =
		"/colleges/university-of-karachi-merit-lists.aspx",
	["/merit-list/mphil-teacher-education-merit-list-of-university-of-karachi-9206"] =
		"/colleges/university-of-karachi-merit-lists.aspx",
	["/merit-list/mphil-urdu-merit-list-of-university-of-karachi-9207"] =
		"/colleges/university-of-karachi-merit-lists.aspx",
	["/merit-list/ms-chemistry-merit-list-of-university-of-education-vehari-10467"] =
		"/colleges/university-of-education-vehari-merit-lists.aspx",
	["/merit-list/ms-computer-science-mscs--merit-list-of-information-technology-university-lahore"] =
		"/mscs/merit-lists",
	["/merit-list/ms-computer-science-mscs--merit-list-of-university-of-central-punjab-lahore-ucp-lahore"] =
		"/colleges/university-of-central-punjab-lahore-ucp-lahore-merit-lists.aspx",
	["/merit-list/ms-computer-science-mscs--merit-list-of-university-of-management-and-technology-umt-johar-town-lahore"] =
		"/mscs/merit-lists",
	["/merit-list/ms-english--merit-list-of-international-islamic-university-islamabad"] =
		"/colleges/international-islamic-university-islamabad-merit-lists.aspx",
	["/merit-list/msc-chemistry-merit-list-of-bahauddin-zakariya-university-bzu-multan-6410"] =
		"/colleges/bahauddin-zakariya-university-bzu-multan-merit-lists.aspx",
	["/merit-list/msc-hons-plant-pathology-merit-list-of-islamia-university-of-bahawalpur-pakistan"] =
		"/colleges/islamia-university-of-bahawalpur-pakistan-merit-lists.aspx",
	["/merit-list/msc-hons-soil-science-merit-list-of-islamia-university-of-bahawalpur-pakistan"] =
		"/colleges/islamia-university-of-bahawalpur-pakistan-merit-lists.aspx",
	["/merit-list/msc-in-botany-merit-list-of-university-of-sargodha-7347"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/msc-in-botany-merit-list-of-university-of-sargodha-8486"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/msc-in-chemistry-merit-list-of-university-of-sargodha-7336"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/msc-in-criminology-merit-list-of-university-of-sargodha-8543"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/msc-in-economics-merit-list-of-university-of-sargodha-3137"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/msc-in-geography-merit-list-of-university-of-sargodha-6741"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/msc-in-geography-merit-list-of-university-of-sargodha-8463"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/msc-in-horticulture-merit-list-of-university-of-sargodha-3166"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/msc-in-mathematics--merit-list-of-university-of-sargodha-6733"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/msc-in-mathematics--merit-list-of-university-of-sargodha-7318"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/msc-in-physical-education-merit-list-of-university-of-sargodha-2966"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/msc-in-physics-merit-list-of-university-of-sargodha-7306"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/msc-in-physics-merit-list-of-university-of-sargodha-8451"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/msc-in-sociology-merit-list-of-university-of-sargodha-8547"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/msc-in-statistics-merit-list-of-university-of-sargodha-7299"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/msc-in-statistics-merit-list-of-university-of-sargodha-8443"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/msc-mathematics-merit-list-of-queen-mary-college-university-lahore-5154"] =
		"/colleges/queen-mary-college-university-lahore-merit-lists.aspx",
	["/merit-list/msc-renewable-energy-resource-engineering-merit-list-of-university-of-engineering-and-technology-uet---peshawar"] =
		"/colleges/university-of-engineering-and-technology-uet---peshawar-merit-lists.aspx",
	["/merit-list/msc-sports-science--merit-list-of-bahauddin-zakariya-university-bzu-multan-7027"] =
		"/colleges/bahauddin-zakariya-university-bzu-multan-merit-lists.aspx",
	["/merit-list/msc-statistics-merit-list-of-islamia-university-of-bahawalpur-pakistan"] =
		"/colleges/islamia-university-of-bahawalpur-pakistan-merit-lists.aspx",
	["/merit-list/msc-statistics-merit-list-of-queen-mary-college-university-lahore-5155"] =
		"/colleges/queen-mary-college-university-lahore-merit-lists.aspx",
	["/merit-list/msc-zoology-merit-list-of-bahauddin-zakariya-university-bzu-multan-6413"] =
		"/colleges/bahauddin-zakariya-university-bzu-multan-merit-lists.aspx",
	["/merit-list/mscs-merit-list-of-pir-mehar-ali-shah-university-of-arid-agriculture-uaar-rawalpindi-10851"] =
		"/colleges/pir-mehar-ali-shah-university-of-arid-agriculture-uaar-rawalpindi-merit-lists.aspx",
	["/merit-list/phd-education-merit-list-of-bahauddin-zakariya-university-bzu-multan-2530"] =
		"/colleges/bahauddin-zakariya-university-bzu-multan-merit-lists.aspx",
	["/merit-list/phd-in-library-information-sciences-merit-list-of-university-of-sargodha-6523"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/phd-in-psychology-merit-list-of-university-of-sargodha"] =
		"/colleges/university-of-sargodha-merit-lists.aspx",
	["/merit-list/pu-hailey-college-of-commerce-merit-list.aspx"] =
		"/colleges/hailey-college-of-commerce-lahore-merit-lists.aspx",
	["/merit-list/school-of-nursing-holy-family-hospital-rawalpindi"] =
		"/colleges/school-of-nursing-holy-family-hospital-rawalpindi.aspx",
	["/merit-list/uol-merit-list"] =
		"/colleges/university-of-lahore-uol-merit-lists.aspx",
	["/merit-lists/punjab-college-lahore-merit-list-2026.aspx"] =
		"/colleges/punjab-group-of-colleges-lahore-merit-lists.aspx",
	["/merit_lists/punjab-university-merit-lists.aspx"] =
		"/colleges/punjab-university-lahore-merit-lists.aspx",
};
app.Use(async (context, next) =>
{
	if (redirects.TryGetValue(context.Request.Path, out var newPath))
	{
		context.Response.Redirect(newPath, permanent: true);
		return;
	}

	await next();
});
var rewriteOptions = new RewriteOptions();

rewriteOptions.AddRedirect(
	@"^book_stores(/.*)?$",
	"store",
	StatusCodes.Status301MovedPermanently);
rewriteOptions.AddRedirect(
	@"^tours/.*$",
	"tours",
	StatusCodes.Status301MovedPermanently);
app.UseRewriter(rewriteOptions);
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