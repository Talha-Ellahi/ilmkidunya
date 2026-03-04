using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblContactMessage
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Subject { get; set; }

    public string Message { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public string? PhoneNo { get; set; }
}
