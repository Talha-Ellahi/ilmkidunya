using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class WebStorySlider
{
    public int Id { get; set; }

    public DateTime? Date { get; set; }

    public string? SliderName { get; set; }

    public int? SlierCategoryId { get; set; }

    public string? AuthorInfo { get; set; }

    public string? MainImage { get; set; }

    public virtual TblSliderCategory? SlierCategory { get; set; }
}
