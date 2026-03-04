using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblInstitute
{
    public int InstId { get; set; }

    public string? InstName { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Fax { get; set; }

    public string? Email { get; set; }

    public string? Website { get; set; }

    public string? InstLogoPath { get; set; }

    public int? CityId { get; set; }

    public string? DegreeStatus { get; set; }

    public string? ScholarshipDetails { get; set; }

    public string? HostelDetails { get; set; }

    public decimal? MaleStudents { get; set; }

    public decimal? FemaleStudents { get; set; }

    public bool? CoEducation { get; set; }

    public string? InstDetails { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public string? AffliationDetail { get; set; }

    public int? InstTypeId { get; set; }

    public string? PageTitle { get; set; }

    public string? PageDesc { get; set; }

    public string? PageKeywords { get; set; }

    public string? RewriteUrl { get; set; }

    public string? Latitude { get; set; }

    public string? Longitude { get; set; }

    public string? Zoomlevel { get; set; }

    public decimal? Views { get; set; }

    public string? OtherContents { get; set; }

    public bool? NoMap { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? Dated { get; set; }

    public int? UpdatedBy { get; set; }

    public int? InstituteId { get; set; }

    public string? InstCategy { get; set; }

    public string? GenderType { get; set; }

    public bool? Feature { get; set; }

    public string? Mobile { get; set; }

    public string? Backgroundimage { get; set; }

    public string? Abbrevation { get; set; }

    public int? SortOrder { get; set; }

    public int? StructureTypeId { get; set; }

    public int? HecRank { get; set; }

    public byte? Recognition { get; set; }

    public string? ProspectusUrl { get; set; }

    public bool? MeritListHome { get; set; }

    public string? Datesheeturl { get; set; }

    public string? Resulturl { get; set; }

    public string? Joburl { get; set; }

    public int? DatesheetboardId { get; set; }

    public int? ResultboardId { get; set; }

    public int? JobcompanyId { get; set; }

    public string? HecReview { get; set; }

    public string? Established { get; set; }

    public decimal? Fbranking { get; set; }

    public decimal? GoogleRanking { get; set; }

    public decimal? Qsranking { get; set; }

    public decimal? Theranking { get; set; }

    public string? AdminInfo { get; set; }

    public string? AdmissionIntro { get; set; }

    public string? Students { get; set; }
}
