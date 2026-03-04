using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class RequestBook
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? BookName { get; set; }

    public string? Contact { get; set; }

    public string? Author { get; set; }
}
