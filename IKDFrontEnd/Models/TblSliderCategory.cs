using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblSliderCategory
{
    public int Id { get; set; }

    public string? SliderCategoryName { get; set; }

    public int? SortOrder { get; set; }

    public string? IconImage { get; set; }

    public virtual ICollection<WebStorySlider> WebStorySliders { get; set; } = new List<WebStorySlider>();
}
