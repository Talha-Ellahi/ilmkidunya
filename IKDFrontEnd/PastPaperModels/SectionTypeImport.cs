using System;
using System.Collections.Generic;

namespace IKDFrontEnd.PastPaperModels;

public partial class SectionTypeImport
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Url { get; set; }

    public int? SectionId { get; set; }

    public int? InstituteTypeId { get; set; }

    public int? InstituteId { get; set; }

    public int? ClassId { get; set; }

    public int? SubjectId { get; set; }
}
