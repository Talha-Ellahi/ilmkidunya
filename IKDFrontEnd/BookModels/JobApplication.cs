using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class JobApplication
{
    public int Id { get; set; }

    public int? JobId { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? Resume { get; set; }

    public bool IsShortListed { get; set; }

    public virtual Job? Job { get; set; }
}
