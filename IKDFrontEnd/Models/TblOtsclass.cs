using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblOtsclass
{
    public int Id { get; set; }

    public int TutorClassId { get; set; }

    public string OtsclassName { get; set; } = null!;

    public string? OtsclassUrl { get; set; }

    public string? Description { get; set; }

    public string? Image { get; set; }

    public bool IsActive { get; set; }

    public int SortOrder { get; set; }

    public int UserId { get; set; }
}
