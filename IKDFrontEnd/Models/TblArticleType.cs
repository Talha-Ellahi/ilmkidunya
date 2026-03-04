using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblArticleType
{
    public decimal ArticleTypeId { get; set; }

    public string? ArticleTypeName { get; set; }

    public int? SortOrder { get; set; }

    public string? RewriteUrl { get; set; }

    public string? Icon { get; set; }
}
