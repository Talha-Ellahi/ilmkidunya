using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class JobAdsLatest
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Url { get; set; } = null!;

    public string? Detail { get; set; }

    public int JobNature { get; set; }

    public int JobTimeStatus { get; set; }

    public string? JobSkills { get; set; }

    public DateTime Dated { get; set; }

    public DateTime? LastDate { get; set; }

    public string? ImageName { get; set; }

    public string? AbroadCityName { get; set; }

    public int? AbroadCity { get; set; }

    public int? JobViews { get; set; }

    public string? MetaTitle { get; set; }

    public string MetaKeyword { get; set; } = null!;

    public string? MetaDesc { get; set; }

    public string? CompanyName { get; set; }

    public int? CompanyId { get; set; }

    public string? ContactPerson { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public string? ApplyOnlineUrl { get; set; }

    public string? JobAdsTypes { get; set; }

    public string? JobAdsNewsPapers { get; set; }

    public string? JobAdsCities { get; set; }

    public string? JobAdsTypesIds { get; set; }

    public string? JobAdsNewsPapersIds { get; set; }

    public string? JobAdsCitiesIds { get; set; }

    public int? MemberId { get; set; }

    public bool IsActive { get; set; }

    public int UpdatedBy { get; set; }

    public int? NoofJobs { get; set; }

    public decimal? MinSalary { get; set; }

    public decimal? MaxSalary { get; set; }

    public string? Otstest { get; set; }

    public string? TestSpid { get; set; }

    public DateTime? TestDate { get; set; }

    public string? VideoEmbedUrl { get; set; }

    public bool? IsHotspot { get; set; }

    public int? ParentJobId { get; set; }

    public int? ParentJobIdNew { get; set; }

    public int? QualificationId { get; set; }

    public int? JobScaleId { get; set; }

    public string? Experience { get; set; }

    public byte? IsMale { get; set; }

    public int? Salary { get; set; }

    public string? AgeLimit { get; set; }

    public int? ParentCompanyId { get; set; }
}
