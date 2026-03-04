using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class Client
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Image { get; set; }

    public string? Link { get; set; }

    public int? SortOrder { get; set; }
}
