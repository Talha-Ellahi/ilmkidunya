using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblSubject
{
    public decimal SubjectId { get; set; }

    public string SubjectName { get; set; } = null!;

    public int ClassId { get; set; }

    public decimal? SubjectDuration { get; set; }

    public decimal? SubjectTotalMarks { get; set; }

    public int? SubjectTotalQuestions { get; set; }

    public string? SubjectUrl { get; set; }

    public bool? IsActive { get; set; }

    public string? ImageName { get; set; }
}
