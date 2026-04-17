using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblAdmission
{
    public int Id { get; set; }

    public DateTime? Dated { get; set; }

    public string? AdmissionTitle { get; set; }

    public int? CollegeId { get; set; }

    public DateTime? LastDate { get; set; }

    public int? SourceId { get; set; }

    public int? CityId { get; set; }

    public string? NoticeImageThumb { get; set; }

    public string? NoticeImageLarge { get; set; }

    public string? Details { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaDesc { get; set; }

    public string? MetaKeywords { get; set; }

    public string? Url { get; set; }

    public int? UserId { get; set; }

    public DateTime? Updated { get; set; }

    public virtual Admin? User { get; set; }
}
