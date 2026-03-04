using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblStudyAbroadGuide
{
    public int Id { get; set; }

    public int? CountryId { get; set; }

    public string? Url { get; set; }

    public int? SortOrder { get; set; }

    public string? Image { get; set; }

    public int? UserId { get; set; }

    public virtual Country? Country { get; set; }
}
