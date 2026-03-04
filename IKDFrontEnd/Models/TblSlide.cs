using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblSlide
{
    public int Id { get; set; }

    public DateTime? Date { get; set; }

    public int? SliderId { get; set; }

    public string? SliderTitle { get; set; }

    public string? SliderDesc { get; set; }

    public int? SortOrder { get; set; }

    public string? MainImage { get; set; }
}
