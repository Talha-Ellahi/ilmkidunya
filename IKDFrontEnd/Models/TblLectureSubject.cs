using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblLectureSubject
{
    public int Id { get; set; }

    public int LectureClassId { get; set; }

    public string Name { get; set; } = null!;

    public string? Image { get; set; }

    public int? SortOrder { get; set; }

    public bool? IsDelete { get; set; }

    public int? UserId { get; set; }

    public virtual TblLectureClass LectureClass { get; set; } = null!;

    public virtual ICollection<TblLectureChapter> TblLectureChapters { get; set; } = new List<TblLectureChapter>();

    public virtual ICollection<TblLectureTopic> TblLectureTopics { get; set; } = new List<TblLectureTopic>();

    public virtual ICollection<TblLecture> TblLectures { get; set; } = new List<TblLecture>();
}
