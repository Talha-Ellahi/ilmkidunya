using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblLectureClass
{
    public int Id { get; set; }

    public string ClassName { get; set; } = null!;

    public int? GuideCourseId { get; set; }

    public int? SortOrder { get; set; }

    public bool? IsDelete { get; set; }

    public int? UserId { get; set; }

    public string? Image { get; set; }

    public string? ClassUrl { get; set; }

    public virtual ICollection<TblLectureChapter> TblLectureChapters { get; set; } = new List<TblLectureChapter>();

    public virtual ICollection<TblLectureSubject> TblLectureSubjects { get; set; } = new List<TblLectureSubject>();

    public virtual ICollection<TblLectureTopic> TblLectureTopics { get; set; } = new List<TblLectureTopic>();

    public virtual ICollection<TblLecture> TblLectures { get; set; } = new List<TblLecture>();
}
