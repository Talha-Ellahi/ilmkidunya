using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblTutorLevel
{
    public int Id { get; set; }

    public int? TlTutorId { get; set; }

    public int? EducationLevelId { get; set; }
}
