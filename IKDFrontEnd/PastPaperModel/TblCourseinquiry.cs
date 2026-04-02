using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblCourseinquiry
{
    public long Id { get; set; }

    public string Email { get; set; } = null!;

    public string MobileNo { get; set; } = null!;

    public int GraduationYear { get; set; }

    public string ExamDescription { get; set; } = null!;

    public string CourseName { get; set; } = null!;

    public DateOnly Inquirydate { get; set; }

    public string Name { get; set; } = null!;

    public int? EducationLevel { get; set; }

    public int? CollegeId { get; set; }

    public string? Extra { get; set; }

    public bool? Isactive { get; set; }

    public bool? PhoneVerified { get; set; }

    public int? InquiryType { get; set; }

    public string? Marks { get; set; }

    public bool? TestTaken { get; set; }

    public int? DivisionId { get; set; }

    public int? Attempts { get; set; }

    public DateOnly? DateofBirth { get; set; }

    public bool? CallMe { get; set; }

    public bool? IsAgreed { get; set; }

    public string? Url { get; set; }

    public string? Source { get; set; }

    public int? GuideId { get; set; }

    public int? Status { get; set; }

    public int? CityId { get; set; }

    public string? CurrentDegree { get; set; }

    public int? CategoryId { get; set; }
}
