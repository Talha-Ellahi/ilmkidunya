using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class Book
{
    public int BookId { get; set; }

    public string? Name { get; set; }

    public int? NewPrice { get; set; }

    public int? OldPrice { get; set; }

    public string? Description { get; set; }

    public string? PublicationDate { get; set; }

    public string? NumofPages { get; set; }

    public string? Binding { get; set; }

    public string Isbn { get; set; } = null!;

    public bool? Active { get; set; }

    public DateTime? Date { get; set; }

    public int? Year { get; set; }

    public string? Image { get; set; }

    public int? PublisherId { get; set; }

    public int? AuthorId { get; set; }

    public int? StoreId { get; set; }

    public string? Image2 { get; set; }

    public string? Keyword { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaKeyword { get; set; }

    public string? MetaDesc { get; set; }

    public int? Views { get; set; }

    public string? Url { get; set; }
}
