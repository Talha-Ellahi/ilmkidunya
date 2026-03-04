using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblLectureChapter
{
    public int Id { get; set; }

    public int LectureClassId { get; set; }

    public int LectureSubjectId { get; set; }

    public int ChNumber { get; set; }

    public string ChName { get; set; } = null!;

    public string? ChImage { get; set; }

    public int? SortOrder { get; set; }

    public bool? IsDelete { get; set; }

    public int? UserId { get; set; }

    public string? Chtitle { get; set; }

    public string? Metadesc { get; set; }

    public virtual TblLectureClass LectureClass { get; set; } = null!;

    public virtual TblLectureSubject LectureSubject { get; set; } = null!;

    public virtual ICollection<TblLectureTopic> TblLectureTopics { get; set; } = new List<TblLectureTopic>();

    public virtual ICollection<TblLecture> TblLectures { get; set; } = new List<TblLecture>();
}
