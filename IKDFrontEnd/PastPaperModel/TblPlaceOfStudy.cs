using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblPlaceOfStudy
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? SortOrder { get; set; }

    public string? Url { get; set; }

    public string? MetaTile { get; set; }

    public string? MetaDiscription { get; set; }

    public string? MetaKeyword { get; set; }

    public string? ImageName { get; set; }

    public string? Ikdurl { get; set; }
}
