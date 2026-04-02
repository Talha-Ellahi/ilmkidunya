using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblDefConsultant
{
    public decimal CompanyId { get; set; }

    public string? CompanyName { get; set; }

    public string? ContactPerson { get; set; }

    public string? Phone { get; set; }

    public string? Fax { get; set; }

    public string? Email { get; set; }

    public string? Website { get; set; }

    public string? MaillingAddress { get; set; }

    public string? YearEstablished { get; set; }

    public string? Serviceprovide { get; set; }

    public decimal? CityId { get; set; }

    public decimal? MemberId { get; set; }

    public bool? Approve { get; set; }

    public string? Logo { get; set; }

    public string? PremiumDetails { get; set; }

    public string? Photo1 { get; set; }

    public string? Photo2 { get; set; }

    public string? Photo3 { get; set; }

    public bool? PremiumMember { get; set; }

    public decimal? Views { get; set; }

    public bool? IsInquiry { get; set; }

    public string? RewriteUrl { get; set; }

    public DateTime? Dated { get; set; }

    public byte? Zoom { get; set; }

    public string? Latitude { get; set; }

    public string? Longitude { get; set; }

    public byte? SortOrder { get; set; }
}
