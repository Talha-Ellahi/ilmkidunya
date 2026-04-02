using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblSlider
{
    public int Id { get; set; }

    public string? Image { get; set; }

    public string? Thumbnail { get; set; }

    public string? Url { get; set; }

    public int? Sortorder { get; set; }

    public bool? Isactive { get; set; }

    public bool? Isbanned { get; set; }

    public int? Userid { get; set; }
}
