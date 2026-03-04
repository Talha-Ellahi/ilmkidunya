
using IKDFrontEnd.Models;
using System;
using System.Collections.Generic;

namespace IKDFrontEnd.ViewModels
{
	public class CollegesViewModel
	{
		public int Id { get; set; }
        public int CollegeId { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string? CollegeName { get; set; }
		public string? EstablishedYear { get; set; }
		public string? StudentsCount { get; set; }
		public string? Recognition { get; set; }
		public string? ContactNumber { get; set; }
		public string? Address { get; set; }
		public string? CollegeImage { get; set; }
		public string? Website { get; set; }
		public string? IsGovt { get; set; }
		public string? AffiliationStatus { get; set; }
		public string? MetaTitle { get; set; }
		public string? MetaDesc { get; set; }
		public string? MetaKeyWords { get; set; }
		public string? InstituteDetails { get; set; }
		public string? InstituteOtherDetails { get; set; }
		public int? GoogleRanking { get; set; }
		public int? FacebokkRanking { get; set; }
		public string? IlmkiDuniyaRanking { get; set; }
		public string? OveraAlRanking { get; set; }
		public string? City { get; set; }
		public string? Email { get; set; }
		public string? Latitude { get; set; }
		public string? CourseFee { get; set; }
		public string? CourseTotalFee { get; set; }
        public string? AdministrationDetails { get; set; }
        public string? Longitude { get; set; }
		public string? LevelName { get; set; }
		public string? Zoomlevel { get; set; }
		public DateTime? StartDate { get; set; }
		public int? CourseId { get; set; }
		public int? NoticeId { get; set; }
		public DateTime? LastDate { get; set; }
		public string? MeritListName { get; set; }
		public string MeritTypeName { get; set; }
		public TblAdmission? Addmission { get; set; }
		public TblCollege College { get; set; }
		public List<TblAdmission> Addmissions { get; set; }
		public List<TblCollegereview>? Reviews { get; set; }
		public int TotalCourseCount { get; set; }
		public List<string>? DistinctStudyLevels { get; set; }  // For distinct study levels
		public List<Course>? AllCourses { get; set; }
		public List<CourseGroupedData> GroupedCourses { get; set; }
		public List<MeritListResult> MeritList { get; set; }
		public List<TblMeritList>? MeritListType { get; set; }
        public int CourseCount { get; set; }

        // Additional properties for dynamically getting courses grouped by course levels
        public List<CourseLevelData> CourseLevels { get; set; }  // List of course levels with their courses
		public List<MeritListDetails> CourseMeritLists { get; set; } // Store merit lists for each course
        public string CollegeShortName { get; internal set; }
        public string AddmissionLogo { get; internal set; }
    }

	public class CourseLevelData
	{
		public int CourseLevelId { get; set; }
		public string CourseLevelName { get; set; }
		public List<CourseDetails> Courses { get; set; }  // List of courses for this specific course level
	}

	public class CourseDetails
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int CourseLevelId { get; set; }
		public List<MeritListDetails> MeritLists { get; set; }
	}

	public class MeritListDetails
	{
		public DateTime? AddedDate { get; set; }
		public string CourseName { get; set; }
		public string MeritListTypeName { get; set; }
		public string MeritListName { get; set; }
        public string FileName { get; set; }
        public int CourseId { get; set; }
	}


    public class CollegeCourseDetailsViewModel
    {
        public TblCollege College { get; set; }
        public Course Course { get; set; }
        public string Level { get; set; }
        public string CityName { get; set; }

        public List<TblAdmission> Admissions { get; set; }
        public List<TblMeritList> MeritLists { get; set; }

    }


    public class MeritListResult
	{
		public DateTime? AddedDate { get; set; }
		public string CourseName { get; set; }
		public string MeritListTypeName { get; set; }
		public string MeritListName { get; set; }
        public string FileName { get; set; }
    }
	public class CourseGroupedData
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Duration { get; set; }
		public string Fee { get; set; }
		public string CourseLevel { get; set; }
		public string CollegeName { get; set; }
		public DateTime? AdmissionDate { get; set; }
		public DateTime? LastDate { get; set; }
		public string AdmissionImage { get; set; }
		public string AdmissionTitle { get; set; }
		public List<Course> Courses { get; set; }
        public List<string> NoticeImageThumb { get; internal set; }
        public List<DateTime?> Dated { get; internal set; }
        public List<string> AddmissionLogoList { get; internal set; }
    }
	public class CityInstitutionsViewModel
	{
		public dynamic CityInfo { get; set; }
		public List<CollegeWithCourseCountViewModel> Colleges { get; set; }
		public List<CollegeWithCourseCountViewModel> Universities { get; set; }
		public List<Models.Course> Courses { get; set; }
		public List<TblDefCity> Cities { get; set; }
		public List<TblXcourseLevel> CourseLevels { get; set; }
		public string CityName { get; set; }
        public string Url { get; set; }
        public List<CollegeWithCourseNamesViewModel> CollegesWithCourseNames { get; set; }
        public List<Course> TopCourses { get; set; }

    }
	public class CollegeWithCourseCountViewModel
	{
		public TblCollege College { get; set; }
		public int CourseCount { get; set; }
        public List<string> CourseNames { get; set; }
        public double AvgRating { get; set; }
        public List<string> CourseDurations { get; set; }
        public List<CourseInfoVm> Courses { get; set; }
        public string CourseNameSummary { get; internal set; }
    }
    public class CourseInfoVm
    {
        public string Name { get; set; }
        public string Duration { get; set; }
    }
    public class StudyLevelViewModel
	{
		public int EducationLevelId { get; set; }
		public string LevelName { get; set; }
		public int InstituteCount { get; set; }
	}
	public class GroupedCategoryViewModel
	{
		public int CategoryId { get; set; }
		public string CategoryName { get; set; }
		public List<CourseDetailViewModel> Courses { get; set; }
	}

	public class CourseDetailViewModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int? CollegeId { get; set; }
		public string CollegeName { get; set; }
		public string CourseLevelName { get; set; }
	}




    public class SearchViewModel
    {
        public string Keyword { get; set; }
        public int? CityId { get; set; }
        public int? CategoryId { get; set; }
        public int? LevelId { get; set; }
        public List<CollegeSearchResult> Results { get; set; }
    }

    public class CollegeSearchResult
    {
        public string Name { get; set; }
        public string Logo { get; set; }
        public string Url { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public string? IsGovt { get; set; }
        public int? Qsranking { get; set; }
        public decimal? PS { get; set; }
        public int? GoogleRanking { get; set; }
        public int TotalCourses { get; set; }
        public double Rating { get; set; }
    }
    public class SuggestionViewModel
    {
        public string Name { get; set; }
        public string Mobile { get; set; }
        public int? EducationLevel { get; set; }
        public string? DesiredStudyLevel { get; set; }
        public int CityId { get; set; }
        public int? FieldId { get; set; }
        public bool? AgreeToTerms { get; set; }
        public string Description { get; set; }
        public string CurrentDegree { get; set; }
        public string PageUrl { get; set; }
        public int CollegeId { get; set; }
    }
    public class CollegeWithCourseNamesViewModel
    {
        public TblCollege College { get; set; }
        public List<string> CourseNames { get; set; }
        public double AvgRating { get; set; }
    }

    public class CollegeReviewsViewModel
    {
        public List<ReviewViewModel> Reviews { get; set; }
        public string CollegeName { get; set; }
        public int? CollegeId { get; set; }
        public string CollegeUrl { get; set; }
        public string CmsContent { get; set; }
        public List<Banner> Banners { get; set; }
    }

    public class ReviewViewModel
    {
        public string CollegeName { get; set; }
        public int? CollegeId { get; set; }
        public int ReviewId { get; set; }
        public string ReviewName { get; set; }
        public string ReviewRating { get; set; }
        public string ReviewComment { get; set; }
        public DateTime? ReviewDate { get; set; }
        public string ReviewerEmail { get; set; }
        public byte? Afford { get; set; }
        public byte? Academics { get; set; }
        public byte? JobPlacement { get; set; }
        public byte? Facilities { get; set; }
        public bool? ReAdmission { get; set; }
    }

    public class CollegeRating
    {
        public int CollegeId { get; set; }
        public double AvgRating { get; set; }
    }

    public class CustomPagesCollegeViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string Url { get; set; }
        public string? Address { get; set; }
        public string? ContactNumber { get; set; }
        public int TotalCourses { get; set; }
        public string? Sector { get; set; }
        public int PopularityScore { get; set; }
        public decimal Rating { get; set; }
        public string? LogoUrl { get; set; }
    }







    // View Models
    public class CollegeCourseDetailViewModel
    {
        public CollegeBasicInfo College { get; set; }
        public CourseDetailInfo Course { get; set; }
        public TblAdmission AdmissionInfo { get; set; }
        public FeeStructureViewModel FeeStructure { get; set; }
        public List<MeritListViewModel> MeritLists { get; set; }
        public List<RelatedColleges> RelatedColleges { get; set; }
        public string AdmissionLogo { get; internal set; }
    }
    public class RelatedColleges
    {
        public TblCollege College { get; set; }
        public List<CourseInfo> Courses { get; set; }

    }

    public class CourseInfo
    {
        public string Name { get; set; }
        public string TotalFee { get; set; }
        public string Duration { get; set; }
    }

    public class CollegeBasicInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Logo { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
        public decimal OverallRating { get; set; }
        public decimal ReviewScore { get; set; }
        public string IsGovt { get; set; }
        public int? Fbranking { get; internal set; }
        public int? GoogleRanking { get; internal set; }
    }

    public class CourseDetailInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        public string Fee { get; set; }
        public string EligibilityCriteria { get; set; }
        public string EducationLevel { get; set; }
        public List<string> Categories { get; set; }
        public string Syllabus { get; set; }
        public string CareerOpportunities { get; set; }
        public string? TotalFees { get; internal set; }
    }

    public class AdmissionInfoViewModel
    {
        public int AdmissionId { get; set; }
        public string Title { get; set; }
        public DateTime? Dated { get; set; }
        public DateTime? LastDate { get; set; }
        public string NoticeImage { get; set; }
        public string Description { get; set; }
        public string EligibilityCriteria { get; set; }
    }

    public class FeeStructureViewModel
    {
        public string Year { get; set; }
        public decimal? AdmissionFee { get; set; }
        public decimal? TuitionFee { get; set; }
        public decimal? ExamFee { get; set; }
        public decimal? LibraryFee { get; set; }
        public decimal? LabFee { get; set; }
        public decimal? OtherCharges { get; set; }
        public decimal? TotalFee { get; set; }
    }

    public class MeritListViewModel
    {
        public string MeritListName { get; set; }
        public string MeritListType { get; set; }
        public DateTime? AddedDate { get; set; }
        public string FileName { get; set; }
        public string CourseName { get; internal set; }
    }

    public class RelatedCollegeViewModel
    {
        public int CollegeId { get; set; }
        public string CollegeName { get; set; }
        public string CollegeUrl { get; set; }
        public string CityName { get; set; }
        public string IsGovt { get; set; }
        public string Logo { get; set; }
        public string CourseFee { get; set; }
        public string CourseDuration { get; set; }
        public SimpleAdmissionInfo LatestAdmission { get; set; }
    }

    public class SimpleAdmissionInfo
    {
        public DateTime? Dated { get; set; }
        public DateTime? LastDate { get; set; }
    }





    public class CollegeCityDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public int? EstablishedYear { get; set; }
        public int? StudentsCount { get; set; }
        public string ShortName { get; set; }
        public string AffiliationStatus { get; set; }
        public string ContactNumber { get; set; }
        public string Address { get; set; }
        public string Website { get; set; }
        public string IsGovt { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDesc { get; set; }
        public string MetaKeyWords { get; set; }
        public string InstituteDetails { get; set; }
        public string InstituteOtherDetails { get; set; }
        public int? Fbranking { get; set; }
        public int? GoogleRanking { get; set; }
        public string Email { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Zoomlevel { get; set; }
        public string Url { get; set; }
        public string AdministrationDetails { get; set; }
        public string CityName { get; set; }
    }

}
