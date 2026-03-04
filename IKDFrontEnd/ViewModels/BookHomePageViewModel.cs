using IKDFrontEnd.BookModels;
namespace IKDFrontEnd.ViewModels
{
    public class BookHomePageViewModel
    {
        public List<Book>? HomePageBooks { get; set; }
        public List<Book>? LatestReleases { get; set; }
        public List<Author>? Authors { get; set; }
        public List<Category>? Categories { get; set; }
        public int TotalBooksCount { get; set; } = 0;
        public Book? FirstBook { get; internal set; }
    }
    public class BookStoreViewModel
    {
        public int Val { get; set; } // cart count

        public string? AuthorName { get; set; }
        public Book Book { get; set; }
        public int BookID { get; set; }
        public List<BookReview> BooksReviews { get; set; }
        public List<Book> RelatedStores { get; set; }
        public List<Category> BooksCategories { get; set; }
        public List<Book> RelatedBooks { get; set; }
        public Publisher? Publisher { get; internal set; }
        public List<Author> Authors { get; internal set; }
    }
    public class BookStoreDetailViewModel
    {
        public int Val { get; set; } // cart count

        public string? AuthorName { get; set; }
        public Book Book { get; set; }
        public int BookID { get; set; }
        public List<BookReview> BooksReviews { get; set; }
        public List<Book> RelatedStores { get; set; }
        public List<Category> Categories { get; set; }
        public List<BookCategory> BooksCategories { get; set; }
        public List<Book> RelatedBooks { get; set; }
        public Publisher? Publisher { get; internal set; }
        public List<Author> Authors { get; internal set; }
    }


    public class AllBooksViewModel
    {
        public int BookID { get; set; }
        public int Views { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public int? BookStoreID { get; set; }
        public string Binding { get; set; }
        public int? AuthorID { get; set; }
        public DateTime? Date { get; set; }
        public string Description { get; set; }
        public decimal? NewPrice { get; set; }
        public decimal? OldPrice { get; set; }
        public int? Year { get; set; }
        public string PublicationDate { get; set; }
        public string NumofPages { get; set; }
        public string ISBN { get; set; }
        public string Image { get; set; }

    }
    public class BookStoreAuthor
    {
        public int _authorID { get; set; }
        public Author aut { get; set; }   // Author entity (from EF)
        public List<Book> Books { get; set; } = new List<Book>();
    }

    public class BookStoreCategory
    {
        public int _categoryID { get; set; }
        public Category cat { get; set; }   // Category entity (from EF)
        public List<Book> Books { get; set; } = new List<Book>();
        public Category BookCategory { get; internal set; }
        public List<Book> LatestRelease { get; internal set; }
        public List<Author> Authors { get; internal set; }
        public List<Category> RelatedCategories { get; internal set; }
    }
}
