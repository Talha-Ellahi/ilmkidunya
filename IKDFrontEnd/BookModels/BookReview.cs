using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class BookReview
{
    public int Id { get; set; }

    public int? BookId { get; set; }

    public string? Name { get; set; }

    public string? Message { get; set; }

    public int? Review { get; set; }
}
