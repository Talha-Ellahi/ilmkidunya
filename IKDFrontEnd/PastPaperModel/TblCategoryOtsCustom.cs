using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblCategoryOtsCustom
{
    public int CustomCatId { get; set; }

    public string? CatName { get; set; }

    public string? CatDesc { get; set; }

    public string? DetailedImage { get; set; }

    public bool? IsActive { get; set; }

    public string? ImageIcon { get; set; }

    public int ContentId { get; set; }

    public int? SortOrder { get; set; }
}
