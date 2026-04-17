using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblCollege
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Url { get; set; }

    public int? CityId { get; set; }

    public string? Address { get; set; }

    public string? Email { get; set; }

    public string? ContactNumber { get; set; }

    public string? Website { get; set; }

    public string? IsGovt { get; set; }

    public int? TypeOfInstituteId { get; set; }

    public int? EstablishedYear { get; set; }

    public string? ProspectusUrl { get; set; }

    public int? StudentsCount { get; set; }

    public string? ShortName { get; set; }

    public string? AffiliationStatus { get; set; }

    public string? AffiliationDescription { get; set; }

    public string? Logo { get; set; }

    public string? TitleImage { get; set; }

    public bool? IsMeritListTop { get; set; }

    public bool? IsFeatured { get; set; }

    public int? Qsranking { get; set; }

    public int? Fbranking { get; set; }

    public int? Hecranking { get; set; }

    public int? GoogleRanking { get; set; }

    public int? DateSheetBoardId { get; set; }

    public string? DateSheetUrl { get; set; }

    public int? ResultBoardId { get; set; }

    public string? ResultUrl { get; set; }

    public string? Hecreview { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaDesc { get; set; }

    public string? MetaKeyWords { get; set; }

    public string? InstituteDetails { get; set; }

    public string? InstituteOtherDetails { get; set; }

    public string? AdministrationDetails { get; set; }

    public string? AdmissionDetails { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public decimal? MaleStudents { get; set; }

    public decimal? FemaleStudents { get; set; }

    public bool? CoEducation { get; set; }

    public string? Latitude { get; set; }

    public string? Longitude { get; set; }

    public string? Zoomlevel { get; set; }

    public byte? Recognition { get; set; }

    public decimal? Views { get; set; }

    public string? GenderType { get; set; }

    public int? SortOrder { get; set; }
}
