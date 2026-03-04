using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TutorInquiry
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? QualificationId { get; set; }

    public int? CityId { get; set; }

    public string? Email { get; set; }

    public bool? Gender { get; set; }

    public string? Contact { get; set; }

    public string? Subjects { get; set; }

    public int? LevelId { get; set; }

    public string? TutoringOptions { get; set; }

    public int MemberId { get; set; }

    public DateTime? Date { get; set; }

    public string? Detail { get; set; }

    public bool? IsShared { get; set; }

    public bool? RequestType { get; set; }

    public string? TutorLocation { get; set; }
}
