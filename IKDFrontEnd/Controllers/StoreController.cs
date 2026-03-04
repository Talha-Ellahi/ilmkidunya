using IKDFrontEnd.BookModels;
using IKDFrontEnd.Models;
using IKDFrontEnd.Services;
using IKDFrontEnd.ViewModels;
using IKDFrontEnd.ViewModels.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace IKDFrontEnd.Controllers
{
    [Route("store")]
    public class StoreController : Controller
    {
        private readonly BookDbikdContext _context;
        private readonly DbikdContext _dbikdContext;
        private readonly BannerService _bannerService;
        private readonly CmsRepository _cmsRepo;
        public StoreController(BookDbikdContext context, BannerService bannerService, DbikdContext dbikdContext, CmsRepository cmsRepo)
        {
            _context = context;
            _bannerService = bannerService;
            _dbikdContext = dbikdContext;
            _cmsRepo = cmsRepo;
        }

        [HttpGet("")]
        public async Task<IActionResult> Home()
        {
            ViewBag.Banners = await _bannerService.GetBannersAsync();
            var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/store/");

            ViewBag.CmsData = cmsData;

            var homePageBooks = await _context.Books
                .OrderByDescending(b => b.Views)
                .Take(13)
                .Select(b => new Book
                {
                    BookId = b.BookId,
                    Views = b.Views ?? 0,
                    Url = b.Url,
                    Name = b.Name != null ? b.Name.Replace("&amp;", "") : "",
                    StoreId = b.StoreId,
                    Binding = b.Binding,
                    AuthorId = b.AuthorId,
                    Date = b.Date,
                    Description = b.Description,
                    NewPrice = b.NewPrice,
                    OldPrice = b.OldPrice,
                    Year = b.Year,
                    PublicationDate = b.PublicationDate,
                    NumofPages = b.NumofPages,
                    Isbn = b.Isbn,
                    Image = b.Image
                })
                .ToListAsync();

            var latestReleases = await _context.Books
                .OrderByDescending(b => b.BookId)
                .Take(16)
                .Select(b => new Book
                {
                    BookId = b.BookId,
                    Views = b.Views ?? 0,
                    Url = b.Url,
                    Name = b.Name != null ? b.Name.Replace("&amp;", "") : "",
                    StoreId = b.StoreId,
                    Binding = b.Binding,
                    AuthorId = b.AuthorId,
                    Date = b.Date,
                    Description = b.Description,
                    NewPrice = b.NewPrice,
                    OldPrice = b.OldPrice,
                    Year = b.Year,
                    PublicationDate = b.PublicationDate,
                    NumofPages = b.NumofPages,
                    Isbn = b.Isbn,
                    Image = b.Image
                })
                .ToListAsync();

            var authors = await _context.Authors
                .Select(a => new Author
                {
                    Id = a.Id,
                    Image = "https://bookstore.campus.pk/Resources/Authors/" + a.Image,
                    Name = a.Name,
                    Description = a.Description,
                    MetaTitle = a.MetaTitle,
                    MetaDesc = a.MetaDesc,
                    MetaKeywords = a.MetaKeywords,
                    SortOrder = a.SortOrder ?? 0
                }).OrderByDescending(s => s.SortOrder).Take(16)
                .ToListAsync();

            var categories = await _context.Categories
                .Select(c => new Category
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName,
                    Description = c.Description,
                    MetaTitle = c.MetaTitle,
                    MetaKeyWords = c.MetaKeyWords,
                    MetaDescription = c.MetaDescription,
                    ParentCategoryId = c.ParentCategoryId,
                    SortOrder = c.SortOrder
                })
                .ToListAsync();

            var totalBooksCount = await _context.Books.CountAsync();
            var firstBook = homePageBooks.OrderByDescending(b => b.Views).FirstOrDefault();

            // Remove it from the list so it doesn't appear in HomePageBooks
            if (firstBook != null)
            {
                homePageBooks.Remove(firstBook);
            }
            var vm = new BookHomePageViewModel
            {
                HomePageBooks = homePageBooks,
                FirstBook = firstBook,
                LatestReleases = latestReleases,
                Authors = authors,
                Categories = categories,
                TotalBooksCount = totalBooksCount
            };
            try
            {
                return View(vm);
            }
            catch (Exception ex)
            {
                // You can log this instead of returning it directly in production
                return Content($"An error occurred: {ex.Message}\n\n{ex.StackTrace}");
            }
        }

        [HttpGet("all-books")]
        public async Task<IActionResult> AllBooks(int skip = 0, int take = 102)
        {
            // Total books count
            if (skip == 0)
            {
                ViewBag.TotalBooksCount = await _context.Books.CountAsync();
                ViewBag.Banners = await _bannerService.GetBannersAsync();
                var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/store/all-books");

                ViewBag.CmsData = cmsData;
            }

            // Get books dynamically
            var latestReleases = await _context.Books
                .OrderByDescending(b => b.BookId)
                .Skip(skip)
                .Take(take)
                .Select(b => new AllBooksViewModel
                {
                    BookID = b.BookId,
                    Views = b.Views ?? 0,
                    Url = b.Url,
                    Name = b.Name != null ? b.Name.Replace("&amp;", "") : "",
                    BookStoreID = b.StoreId,
                    Binding = b.Binding,
                    AuthorID = b.AuthorId,
                    Date = b.Date,
                    Description = b.Description,
                    NewPrice = b.NewPrice,
                    OldPrice = b.OldPrice,
                    Year = b.Year,
                    PublicationDate = b.PublicationDate,
                    NumofPages = b.NumofPages,
                    ISBN = b.Isbn,
                    Image = b.Image
                })
                .ToListAsync();

            // If it's an AJAX request, return partial HTML (books list only)
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_BooksListPartial", latestReleases);
            }

            // Otherwise return full page
            return View(latestReleases);
        }



        [HttpGet("search")]
        public async Task<IActionResult> Search(string keyword, string category)
        {
            ViewBag.TotalBooksCount = await _context.Books.CountAsync();
            ViewBag.Banners = await _bannerService.GetBannersAsync();
            var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/store/all-books");

            ViewBag.CmsData = cmsData;

            keyword = keyword?.Trim() ?? string.Empty;
            category = category?.Trim() ?? string.Empty;

            var booksQuery = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(keyword) && !string.IsNullOrEmpty(category))
            {
                booksQuery = booksQuery.Where(b =>
                    EF.Functions.Like(b.Keyword, $"%{keyword}%") &&
                    EF.Functions.Like(b.Keyword, $"%{category}%")
                );
            }
            else if (!string.IsNullOrEmpty(keyword))
            {
                booksQuery = booksQuery.Where(b =>
                    EF.Functions.Like(b.Keyword, $"%{keyword}%")
                );
            }

            // Load results
            var booksList = booksQuery.ToList();

            // Keyword splitting & relevance ordering (Orders count)
            var keywordParts = (string.IsNullOrEmpty(keyword) ? string.Empty : keyword)
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var categoryParts = (string.IsNullOrEmpty(category) ? string.Empty : category)
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var allSearchParts = keywordParts.Concat(categoryParts).Distinct().ToList();

            var finalList = booksList
                .GroupBy(b => b.BookId)
                .Select(g =>
                {
                    var lastBook = g.Last();

                    return lastBook;
                })

                .ToList();


            var model = new BookHomePageViewModel
            {
                HomePageBooks = finalList,
            };


            return View(model);
        }


        [HttpGet("allauthors")]
        public async Task<IActionResult> AllAuthors()
        {
            ViewBag.AllAuthorCounts = await _context.Authors.CountAsync();
            ViewBag.Banners = await _bannerService.GetBannersAsync();
            var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/store/all-authors");

            ViewBag.CmsData = cmsData;

            var authors = await _context.Authors
                .Select(a => new Author
                {
                    Id = a.Id,
                    Image = a.Image != null && a.Image != "" ? "https://bookstore.campus.pk/Resources/Authors/" + a.Image : "https://resources.ilmkidunya.com/images/imgpsh_fullsize_anim.jpg",
                    Name = a.Name,
                    Description = a.Description,
                    MetaTitle = a.MetaTitle,
                    MetaDesc = a.MetaDesc,
                    MetaKeywords = a.MetaKeywords,
                    SortOrder = a.SortOrder ?? 0
                }).OrderByDescending(s => s.SortOrder).ToListAsync();
            return View(authors);
        }

        [HttpGet("author/{url}")]
        public async Task<IActionResult> AuthorDetail(string Url)
        {
            ViewBag.Banners = await _bannerService.GetBannersAsync();

            var aut = _context.Authors
                .FirstOrDefault(u => u.Name.Trim().Replace(" ", "-").ToLower() == Url);

            if (aut == null)
            {
                return NotFound(); // 404 if no author found
            }

            var book = new BookStoreAuthor
            {
                _authorID = aut.Id,
                aut = aut,
                Books = _context.Books
                                .Where(b => b.AuthorId == aut.Id)
                                .ToList(),
            };

            var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/store/author/{Url}");

            if (cmsData == null)
            {
                cmsData = new  TblCmsDto
                {
                    Url = "https://www.ilmkidunya.com/store/author/" + Url,
                    Desc1 = $"Find all books by {aut.Name} online in Pakistan. Get best prices and PDF download of selected books.",
                    MetaTitle = $"Books by {aut.Name} - Online Bookstore",
                    MetaDesc = aut.MetaDesc != null ? aut.MetaDesc : $"View online books catalog by {aut.Name}. " +
                               $"There are {book.Books.Count()} books by this author. " +
                               $"Get best prices and PDF download of selected books of {aut.Name}.",
                    MetaKeys = !string.IsNullOrEmpty(aut.MetaKeywords)
                               ? aut.MetaKeywords
                               : $"books by {aut.Name}, {aut.Name} books in Pakistan, " +
                                $"buy online books of {aut.Name}, {aut.Name} books pdf download",
                    Heading = !string.IsNullOrEmpty(aut.MetaTitle)
                              ? aut.MetaTitle
                              : $"Find all books by {aut.Name} buy online in Pakistan"
                };
            }

            ViewBag.CmsData = cmsData;

            return View(book);
        }
        
        [HttpGet("category/{url}")]
        public async Task<IActionResult> Category(string url)
        {
            ViewBag.Banners = await _bannerService.GetBannersAsync();
            var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/store/category/{url}");
            var viewModel = new BookStoreCategory();

            // ✅ Find Category by URL slug
            var category = _context.Categories
                .FirstOrDefault(c => c.CategoryName.Trim().Replace(" ", "-").ToLower() == url);

            if (category == null)
                return NotFound();

            viewModel.BookCategory = category;

            if (category.ParentCategoryId != null)
            {
                viewModel.RelatedCategories = _context.Categories.Where(c => c.ParentCategoryId == category.ParentCategoryId &&  c.Id != category.Id).ToList();

            }
            else
            {
                viewModel.RelatedCategories = _context.Categories.Where(c => c.Id != category.Id).ToList();

            }

            viewModel.Books = (from a in _context.Books
                               join b in _context.BookDetails on a.BookId equals b.BookId
                               join c in _context.BookCategories on b.Id equals c.BookId
                               where c.CategoryId == category.Id
                               select new Book
                               {
                                   BookId = a.BookId,
                                   Url = a.Url,
                                   Name = a.Name.Replace("&amp;", ""),
                                   StoreId = b.StoreId,
                                   Binding = a.Binding,
                                   AuthorId = b.AuthorId,
                                   Date = a.Date,
                                   Description = a.Description,
                                   NewPrice = a.NewPrice,
                                   OldPrice = a.OldPrice,
                                   Year = a.NewPrice, // ⚠️ double-check if Year should map to NewPrice
                                   PublicationDate = a.PublicationDate,
                                   NumofPages = a.NumofPages,
                                   Isbn = a.Isbn,
                                   Image = a.Image
                               }).ToList();

            if(cmsData == null)
            {
                cmsData = new TblCmsDto
                {
                    Url = "https://www.ilmkidunya.com/store/author/" + Url,
                    Desc1 = !string.IsNullOrEmpty(category.MetaDescription)
                    ? category.MetaDescription
                    : $"Get all the books for {category.CategoryName} at one spot. " +
                      $"Buy online {category.CategoryName} Urdu and English books online from ilmkidunya.com",
                    MetaTitle = !string.IsNullOrEmpty(category.MetaTitle)
                    ? category.MetaTitle
                    : $"{category.CategoryName} books in Urdu & English",

                    MetaDesc = !string.IsNullOrEmpty(category.MetaDescription)
                    ? category.MetaDescription
                    : $"Get all the books for {category.CategoryName} at one spot. " +
                      $"Buy online {category.CategoryName} Urdu and English books online from ilmkidunya.com",
                    MetaKeys = !string.IsNullOrEmpty(category.MetaKeyWords)
                    ? category.MetaKeyWords
                    : $"{category.CategoryName} books, buy online {category.CategoryName} books, " +
                      $"{category.CategoryName} urdu books, {category.CategoryName} english book",
                    Heading = !string.IsNullOrEmpty(category.MetaTitle)
                    ? category.MetaTitle
                    : $"{category.CategoryName} books in Urdu & English",
                };
            }
            ViewBag.CmsData = cmsData;
            viewModel.LatestRelease = _context.Books
                       .OrderByDescending(b => b.BookId)   // latest books first
                       .Take(16)                           
                       .Select(b => new Book
                       {
                           BookId = b.BookId,
                           Views = b.Views ?? 0,
                           Url = b.Url,
                           Name = b.Name.Replace("&amp;", ""),
                           StoreId = b.StoreId,
                           Binding = b.Binding,
                           AuthorId = b.AuthorId,
                           Date = b.Date,
                           Description = b.Description,
                           NewPrice = b.NewPrice,
                           OldPrice = b.OldPrice,
                           Year = b.NewPrice, // check if correct
                           PublicationDate = b.PublicationDate,
                           NumofPages = b.NumofPages,
                           Isbn = b.Isbn,
                           Image = b.Image
                       })
                       .ToList();


            viewModel.Authors = await _context.Authors
                            .Select(a => new Author
                            {
                                Id = a.Id,
                                Image = "https://bookstore.campus.pk/Resources/Authors/" + a.Image,
                                Name = a.Name,
                                Description = a.Description,
                                MetaTitle = a.MetaTitle,
                                MetaDesc = a.MetaDesc,
                                MetaKeywords = a.MetaKeywords,
                                SortOrder = a.SortOrder ?? 0
                            }).OrderByDescending(s => s.SortOrder).Take(16)
                            .ToListAsync();

            return View(viewModel);
        }




        [HttpGet("{url}")]
        public async Task<IActionResult> BookDetail(string url)
        {
            ViewBag.Banners = await _bannerService.GetBannersAsync();
            var cmsData = await _cmsRepo.GetByUrlAsync($"https://www.ilmkidunya.com/store/");

            ViewBag.CmsData = cmsData;

            var bookVm = new BookStoreDetailViewModel();

            // Cart from session
            if (HttpContext.Session.TryGetValue("add2cartpage", out var _))
            {
                var cart = HttpContext.Session.GetObjectFromJson<List<Book>>("add2cartpage");
                bookVm.Val = cart?.Count ?? 0;
            }

            // Get main book (no tracking for read speed)
            var book = await _context.Books
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Url == url);

            var bookDetail = await (
                    from bd in _context.BookDetails.AsNoTracking()
                    join b in _context.Books.AsNoTracking()
                        on bd.BookId equals b.BookId
                    where b.Url == url
                    select new
                    {
                        // From BookDetail
                        BookDetailId = bd.Id,
                        bd.BookId,
                        bd.Description,
                        bd.PublicationDate,
                        bd.NumofPages,
                        bd.Isbn,
                        // Add more BookDetail fields as needed...
                    
                        // From Book
                        b.Views,
                        b.Name,
                        b.MetaTitle,
                        b.MetaDesc,
                        b.MetaKeyword
                    })
                    .FirstOrDefaultAsync();

            if (book == null)
                return NotFound();

            // Update view count separately to avoid tracking issues
            _context.Books
                .Where(b => b.BookId == book.BookId)
                .ExecuteUpdate(b => b.SetProperty(x => x.Views, (book.Views ?? 0) + 1));

            // Fill the view model
            bookVm.BookID = book.BookId;
            bookVm.Book = new Book
            {
                BookId = book.BookId,
                Name = book.Name?.Replace("&amp;", ""),
                StoreId = book.StoreId ?? 0,
                Binding = book.Binding,
                Views = (book.Views ?? 0) + 1,
                AuthorId = book.AuthorId ?? 0,
                Date = book.Date,
                Description = book.Description,
                NewPrice = book.NewPrice ?? 0,
                OldPrice = book.OldPrice ?? 0,
                Year = book.Year ?? 0,
                PublicationDate = book.PublicationDate,
                NumofPages = book.NumofPages,
                Isbn = book.Isbn,
                Image = book.Image,
                PublisherId = book.PublisherId ?? 0,
                MetaKeyword = book.MetaKeyword,
                MetaTitle = book.MetaTitle,
                MetaDesc = book.MetaDesc,
                Url = book.Url
            };

            // Queries (still sequential but AsNoTracking for speed)
            bookVm.AuthorName = await _context.Authors
                .AsNoTracking()
                .Where(a => a.Id == book.AuthorId)
                .Select(a => a.Name)
                .FirstOrDefaultAsync();

            bookVm.Publisher = await _context.Publishers
                .AsNoTracking()
                .Where(p => p.Id == book.PublisherId)
                .Select(p => new Publisher
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                })
                .FirstOrDefaultAsync();

            bookVm.BooksReviews = await _context.BookReviews
                .AsNoTracking()
                .Where(r => r.BookId == book.BookId)
                .Select(r => new BookReview
                {
                    Id = r.Id,
                    Name = r.Name.Replace("&amp;", ""),
                    BookId = r.BookId,
                    Message = r.Message,
                    Review = r.Review
                })
                .ToListAsync();

            if (book.StoreId.HasValue)
            {
                var related = await _context.Books
                    .AsNoTracking()
                    .Where(b => b.StoreId == book.StoreId && b.BookId != book.BookId)
                    .OrderByDescending(b => b.Date) // or whatever makes sense
                    .Select(b => new
                    {
                        b.Url,
                        b.Name,
                        b.StoreId,
                        b.Binding,
                        b.AuthorId,
                        b.Date,
                        b.NewPrice,
                        b.OldPrice,
                        b.Image,
                        b.PublisherId
                    })
                    .Take(12)
                    .ToListAsync();

                bookVm.RelatedStores = related
                    .Select(b => new Book
                    {
                        Url = b.Url,
                        Name = b.Name.Replace("&amp;", ""),
                        StoreId = b.StoreId ?? 0,
                        Binding = b.Binding,
                        AuthorId = b.AuthorId ?? 0,
                        Date = b.Date,
                        NewPrice = b.NewPrice ?? 0,
                        OldPrice = b.OldPrice ?? 0,
                        Image = b.Image,
                        PublisherId = b.PublisherId ?? 0
                    })
                    .ToList();
            }

            bookVm.Categories = await _context.Categories.AsNoTracking().Select(c => new Category { Id = c.Id, CategoryName = c.CategoryName, Description = c.Description, MetaTitle = c.MetaTitle, MetaKeyWords = c.MetaKeyWords, MetaDescription = c.MetaDescription, ParentCategoryId = c.ParentCategoryId, SortOrder = c.SortOrder }).ToListAsync();

            if (bookDetail == null) return NotFound();
            bookVm.BooksCategories = await (
                from bc in _context.BookCategories.AsNoTracking()
                join c in _context.Categories.AsNoTracking()
                    on bc.CategoryId equals c.Id
                where bc.BookId == bookDetail.BookDetailId
                select new BookCategory
                {
                    BookId = bc.BookId,
                    CategoryId = bc.CategoryId,
                    CategoryName = c.CategoryName
                })
                .ToListAsync();


            if (bookVm.BooksCategories.Any())
            {
                var categoryIds = bookVm.BooksCategories
                    .Select(c => c.Id) // keep as int
                    .ToList();

                bookVm.RelatedBooks = await _context.Books
                    .AsNoTracking()
                    .Where(b => _context.BookCategories
                        .Any(c => c.BookId == b.BookId))
                    .Take(24)
                    .Select(b => new Book
                    {
                        BookId = b.BookId,
                        Url = b.Url,
                        Name = b.Name.Replace("&amp;", ""),
                        StoreId = b.StoreId ?? 0,
                        Binding = b.Binding,
                        AuthorId = b.AuthorId ?? 0,
                        Date = b.Date,
                        Description = b.Description,
                        NewPrice = b.NewPrice ?? 0,
                        OldPrice = b.OldPrice ?? 0,
                        Year = b.Year ?? 0,
                        PublicationDate = b.PublicationDate,
                        NumofPages = b.NumofPages,
                        Isbn = b.Isbn,
                        Image = b.Image
                    })
                    .ToListAsync();

            }
            bookVm.Authors = await _context.Authors
            .AsNoTracking()
            .Select(a => new Author
            {
                Id = a.Id,
                Image = "https://bookstore.campus.pk/Resources/Authors/" + a.Image,
                Name = a.Name,
                Description = a.Description,
                MetaTitle = a.MetaTitle,
                MetaDesc = a.MetaDesc,
                MetaKeywords = a.MetaKeywords,
                SortOrder = a.SortOrder ?? 0
            })
            .OrderByDescending(s => s.SortOrder)
            .Take(16)
            .ToListAsync();

            return View(bookVm);
        }


    }
}
//Scaffold - DbContext "Server=69.10.38.62;Database=dbbookstore;User Id=userdbbookstore;Password=Dr56m0d7?;TrustServerCertificate=True;MultipleActiveResultSets=true"Microsoft.EntityFrameworkCore.SqlServer - OutputDir BookModels - Context BookDbikdContext