using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class Faq
{
    public short Id { get; set; }

    public string? Title { get; set; }

    public string? Decsription { get; set; }

    public short? SortOrder { get; set; }
}
