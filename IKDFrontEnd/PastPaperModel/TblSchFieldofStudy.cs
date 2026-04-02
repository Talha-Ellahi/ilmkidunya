using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblSchFieldofStudy
{
    public int Id { get; set; }

    public string SchFieldofStudyName { get; set; } = null!;

    public string? Url { get; set; }

    public bool? IsActive { get; set; }

    public int? SortOrder { get; set; }

    public int? UserId { get; set; }
}
