using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class Subscriber
{
    public int Id { get; set; }

    public string? Email { get; set; }

    public DateTime? Date { get; set; }
}
