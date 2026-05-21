using System;
using System.Collections.Generic;

namespace IKDFrontEnd.DBComment;

public partial class TblDefMemberInfo2
{
    public decimal MemberId { get; set; }

    public string? Id { get; set; }

    public int? MemberTypeId { get; set; }

    public string? Passwordold { get; set; }

    public string? MemberFirstName { get; set; }

    public string? MemberLastName { get; set; }

    public string? Email { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public bool? Gender { get; set; }

    public decimal? CityId { get; set; }

    public decimal? InstId { get; set; }

    public int? QualificationId { get; set; }

    public decimal? ProgramOfStudy { get; set; }

    public string? AboutMe { get; set; }

    public string? ImageName { get; set; }

    public bool? MatchMaking { get; set; }

    public DateTime? Dated { get; set; }

    public short? Age { get; set; }

    public int? AsStudent { get; set; }

    public string MemberName { get; set; } = null!;

    public string? Thumbnail { get; set; }

    public bool? Verified { get; set; }

    public string? VerificationCode { get; set; }

    public DateTime? LastLogin { get; set; }

    public decimal? Views { get; set; }

    public string? MobileNumber { get; set; }

    public string? Password { get; set; }

    public string? RewriteUrl { get; set; }

    public string? Address { get; set; }

    public string? MemberSource { get; set; }

    public string? ArticleAboutMe { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? Updated { get; set; }

    public int? MemberSourceId { get; set; }

    public int? CountryId { get; set; }

    public byte? Rights { get; set; }

    public bool? JoinRewards { get; set; }

    public string? Fburl { get; set; }

    public string? Fbimage { get; set; }

    public bool? IsNewsApproved { get; set; }

    public bool? IsArticleApproved { get; set; }

    public bool? IsDelete { get; set; }
}
