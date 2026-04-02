using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblStudyGuide
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? ShortName { get; set; }

    public string? Type { get; set; }

    public string? Url { get; set; }

    public int? SortOrder { get; set; }

    public int? EduLevelId { get; set; }

    public int? BoardId { get; set; }

    public string? NewsTags { get; set; }

    public string? CollegeTags { get; set; }

    public string? Logo { get; set; }

    public DateTime? Date { get; set; }

    public int? UserId { get; set; }

    public virtual Board? Board { get; set; }

    public virtual TblEducationLevel? EduLevel { get; set; }
}
