using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class Topic
{
    public int TopicId { get; set; }

    public int? ClassId { get; set; }

    public int? ChapterId { get; set; }

    public string? TopicNumber { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public int? SortOrder { get; set; }

    public bool? IsEnable { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? Date { get; set; }
}
