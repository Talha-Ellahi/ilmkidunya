using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class CourseCategoryJoin
{
    public int Id { get; set; }

    public int? CourseId { get; set; }

    public int? CategoryId { get; set; }
}
