using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblLectureTopic
{
    public int Id { get; set; }

    public int LectureClassId { get; set; }

    public int LectureSubjectId { get; set; }

    public int LectureChId { get; set; }

    public string? TopicNumber { get; set; }

    public string TopicName { get; set; } = null!;

    public int? SortOrder { get; set; }

    public bool IsActive { get; set; }

    public int? UserId { get; set; }

    public virtual TblLectureChapter LectureCh { get; set; } = null!;

    public virtual TblLectureClass LectureClass { get; set; } = null!;

    public virtual TblLectureSubject LectureSubject { get; set; } = null!;

    public virtual ICollection<TblLecture> TblLectures { get; set; } = new List<TblLecture>();
}
