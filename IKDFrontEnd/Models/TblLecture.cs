using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblLecture
{
    public int Id { get; set; }

    public int LectureClassId { get; set; }

    public int LectureSubjectId { get; set; }

    public int LectureChId { get; set; }

    public int LectureTopicId { get; set; }

    public int LectureTeacherId { get; set; }

    public string LectureName { get; set; } = null!;

    public string? Description { get; set; }

    public string? Duration { get; set; }

    public string? Url { get; set; }

    public string? EmbedCode { get; set; }

    public string? Tags { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaDesc { get; set; }

    public string? MetaKeys { get; set; }

    public string? LectureThumb { get; set; }

    public int? SortOrder { get; set; }

    public bool IsDelete { get; set; }

    public int? UserId { get; set; }

    public virtual TblLectureChapter LectureCh { get; set; } = null!;

    public virtual TblLectureClass LectureClass { get; set; } = null!;

    public virtual TblLectureSubject LectureSubject { get; set; } = null!;

    public virtual TblLectureTeacher LectureTeacher { get; set; } = null!;

    public virtual TblLectureTopic LectureTopic { get; set; } = null!;
}
