using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class BookCategory
{
    public int Id { get; set; }

    public int? CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public int? BookId { get; set; }
}
