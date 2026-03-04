using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class BookOrder
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? ContactNo { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public int? Price { get; set; }

    public DateTime? Date { get; set; }

    public int? MemberId { get; set; }

    public string? OrderNumber { get; set; }

    public bool? Approve { get; set; }

    public string? Email { get; set; }
}
