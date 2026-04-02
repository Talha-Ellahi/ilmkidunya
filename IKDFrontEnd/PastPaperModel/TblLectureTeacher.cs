using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblLectureTeacher
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Qualification { get; set; }

    public string? Description { get; set; }

    public string? Image { get; set; }

    public int? SortOrder { get; set; }

    public bool? IsActive { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<TblLecture> TblLectures { get; set; } = new List<TblLecture>();
}
