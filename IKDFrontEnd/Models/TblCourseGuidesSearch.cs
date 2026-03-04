using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblCourseGuidesSearch
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public int LevelId { get; set; }

    public string Url { get; set; } = null!;

    public string? GuideName { get; set; }

    public bool IsActive { get; set; }

    public DateTime DateTime { get; set; }

    public bool IsHomePage { get; set; }

    public string? GuideIds { get; set; }
}
