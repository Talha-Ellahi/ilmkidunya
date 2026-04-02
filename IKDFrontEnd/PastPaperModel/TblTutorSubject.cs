using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblTutorSubject
{
    public int Id { get; set; }

    public int? TlTutorId { get; set; }

    public int? SubjectId { get; set; }
}
