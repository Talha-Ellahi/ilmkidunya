using System;
using System.Collections.Generic;

namespace IKDFrontEnd.DBCollege;

public partial class Course
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? CollegeId { get; set; }

    public int? EducationLevelId { get; set; }

    public string? Duration { get; set; }

    public int? SortOrder { get; set; }

    public string? FeeDescription { get; set; }

    public string? TotalFee { get; set; }

    public string? Coursetags { get; set; }

    public string? CourseIntroduction { get; set; }

    public string? AdmissionRequirement { get; set; }

    public string? Syllabus { get; set; }

    public string? Syllabusfile { get; set; }

    public string? SemesterPlan { get; set; }

    public string? SemesterPlanFile { get; set; }

    public string? Specializations { get; set; }

    public string? SpecializationsFile { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public string? Fee { get; set; }
}
