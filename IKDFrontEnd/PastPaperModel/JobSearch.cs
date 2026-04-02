using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class JobSearch
{
    public long Id { get; set; }

    public DateOnly SearchDate { get; set; }

    public string? Keywords { get; set; }

    public int? JobCategoryId { get; set; }

    public int? CityId { get; set; }

    public int? NewsPaperId { get; set; }

    public string? DateFrom { get; set; }

    public string? DateTo { get; set; }

    public long? UserId { get; set; }
}
