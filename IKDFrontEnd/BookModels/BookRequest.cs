using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class BookRequest
{
    public short Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Contact { get; set; }

    public string? Email { get; set; }

    public string? BookName { get; set; }

    public string? AuthorName { get; set; }

    public string? Edition { get; set; }

    public string? OtherDetail { get; set; }
}
