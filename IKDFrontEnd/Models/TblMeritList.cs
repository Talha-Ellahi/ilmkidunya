using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblMeritList
{
    public int Id { get; set; }

    public int? CollegeId { get; set; }

    public int? CourseId { get; set; }

    public int? MeritListTypeId { get; set; }

    public string? MeritListName { get; set; }

    public string? Year { get; set; }

    public DateTime? AddedDate { get; set; }

    public string? MeritValue { get; set; }

    public string? Notes { get; set; }

    public string? DateOfIssue { get; set; }

    public string? FileName { get; set; }

    public int? UserId { get; set; }

    public int? CourseLevelId { get; set; }
}
