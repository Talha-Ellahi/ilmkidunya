using System;
using System.Collections.Generic;

namespace IKDFrontEnd.DBCollege;

public partial class TblAdmissionCourse
{
    public int Id { get; set; }

    public int? NoticeId { get; set; }

    public int? CourseId { get; set; }
}
