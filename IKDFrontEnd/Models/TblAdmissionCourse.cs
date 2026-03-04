using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblAdmissionCourse
{
    public int Id { get; set; }

    public int? NoticeId { get; set; }

    public int? CourseId { get; set; }
}
