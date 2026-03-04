using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class BookDetail
{
    public int Id { get; set; }

    public int? BookId { get; set; }

    public int? StoreId { get; set; }

    public int? NewPrice { get; set; }

    public int? OldPrice { get; set; }

    public string? Description { get; set; }

    public string? PublicationDate { get; set; }

    public string? NumofPages { get; set; }

    public string? Binding { get; set; }

    public string? Isbn { get; set; }

    public bool? Active { get; set; }

    public DateTime? Date { get; set; }

    public string? Image { get; set; }

    public int? Year { get; set; }

    public int? PublisherId { get; set; }

    public int? AuthorId { get; set; }

    public string? Url { get; set; }
}
