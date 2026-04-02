using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblDefSubject
{
    public short SubjectId { get; set; }

    public string? SubjectName { get; set; }

    public short? SortOrder { get; set; }

    public string? SearchTags { get; set; }

    public string? SubjectIcon { get; set; }

    public bool? IsGeneral { get; set; }
}
