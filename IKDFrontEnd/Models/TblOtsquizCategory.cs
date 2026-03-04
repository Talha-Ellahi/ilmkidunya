using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblOtsquizCategory
{
    public int Id { get; set; }

    public string OtsquizCategoryName { get; set; } = null!;

    public string Url { get; set; } = null!;

    public string? Description { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; }

    public string? Image { get; set; }

    public int UserId { get; set; }
}
