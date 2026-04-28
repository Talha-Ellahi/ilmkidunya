namespace IKDFrontEnd.ViewModels
{

	public class JobHomeViewModel
	{
		public List<CompanyJobsViewModel> GroupedJobs { get; set; }
		public List<CompanyJobsViewModel> CompanyJobs { get; set; }
	}



	public class CompanyJobsViewModel
    {
        public string CompanyName { get; set; } 
        public List<JobViewModel> Jobs { get; set; } 
		public string? CompanyImage { get; set; }
		public string? JobName { get; set; }
	}

    public class JobViewModel
    {
        public int? Id { get; set; } // Job ID
        public string Name { get; set; } // Job Name
        public string Detail { get; set; } // Job Detail
        public string Detail2 { get; set; } // Additional Detail
        public DateTime? Dated { get; set; } // Job Posting Date
        public DateTime? LastDate { get; set; } // Application Deadline
        public int? NoofJobs { get; set; } // Number of Positions Available
     
    }


	public class JobCategoryViewModel
	{
		public string MainProfession { get; set; }
		public List<string> SubJobs { get; set; }
	}

    public class JobListingPageViewModel
    {
        public List<GroupedJobAdViewModel> GroupedJobAds { get; set; }
        public List<CompanyJobViewModel> TopCompanies { get; set; }
        public List<CityViewModel> CitiesWithImages { get; set; }
    }

    public class CityViewModel
    {
        public string CityName { get; set; }
        public string ImageUrl { get; set; }
        public string Url { get; set; }
    }
    public class GroupedJobAdViewModel
    {
        internal string? cityName;

        public List<long> JobAdIds { get; set; } 
        public DateTime Dated { get; set; }
        public string? CompanyName { get; set; }
		public string? JobNature { get; set; }
		public string? JobStatus { get; set; }
		public string? JobLocation { get; set; }
		public string? JobImageURL { get; set; }
		public string? MetaDesc { get; set; }
		public string? JobDesc { get; set; }
		public List<string> JobTitles { get; set; } = new();
		public List<string> JobVacancy { get; set; } = new();
		public List<int> JobCounts { get; set; } = new();
		public int JobCount { get; set; } = new();
		public DateTime? LastDate { get; set; }
        public string? DetailUrl { get; set; }
        public string? CompanyUrl { get; internal set; }
        public string? Location { get; set; }
        public string? CompanyImage { get; set; }
    }

    public class CompanyJobViewModel
    {
        public string? CompanyName { get; set; }
        public string? LogoUrl { get; set; }
        public string? CompanyUrl { get; set; }
    }


	public class JobDetailViewModel
	{
		public string PostName { get; set; }
		public int? NoOfJobs { get; set; }
		public string QualificationName { get; set; }
		public string JobScaleName { get; set; }
		public bool? IsMale { get; set; }
		public string? AgeLimit { get; set; }
		public string? Experience { get; set; }
		public string? JobCity { get; set; }
		public string? JobType { get; set; }

		public string? Province { get; set; }
		public string? PostalCode { get; set; }
		public decimal? MinSalary { get; set; }
		public decimal? MaxSalary { get; set; }
		public DateTime? LastDate { get; set; }
		public string? CompanyName { get; set; }
		public DateTime? Dated { get; set; }

		// This allows for multiple positions in one ad
		public List<JobPositionViewModel> JobPositions { get; set; } = new();

		public List<RelatedJobViewModel> RelatedJobs { get; set; } = new();
		public string? Detail { get; internal set; }
        public string? CompanyUrl { get; internal set; }
        public string? ImageName { get; internal set; }
    }

	public class JobPositionViewModel
	{
		public string Title { get; set; }
		public int? NoOfJobs { get; set; }
		public string Qualification { get; set; }
		public string JobScale { get; set; }
		public string? AgeLimit { get; set; }
		public bool? IsMale { get; set; }
		public string? Experience { get; set; }
	}

	public class RelatedJobViewModel
	{
		public string PostName { get; set; }
		public int? NoOfJobs { get; set; }
		public DateTime? LastDate { get; set; }
		public string CompanyName { get; set; }
		public DateTime PostedDate { get; internal set; }
        public long PostId { get; internal set; }
        public string? CompanyUrl { get; internal set; }
    }

    public class CityJobViewModel

    {

        public int CityId { get; set; }

        public string CityName { get; set; }

        public string Url { get; set; } // Optional, for dynamic linking

    }

    public class CompanyListItemViewModel

    {

        public int Id { get; set; }

        public string CompanyName { get; set; }
        public string? CompanyUrl { get; internal set; }
		public int? TotalJobs { get; set; }

	}
	public class CompanySearchViewModel
	{
		public List<CompanyListItemViewModel> Companies { get; set; }
		public string Type { get; set; }
		public string? CityId { get; set; }
	}
	public class IndustryViewModel

	{

		public string Name { get; set; }

		public string ImageName { get; set; }
        public string? Url { get; internal set; }
    }

	public class ProfessionGroupViewModel
	{
		public string JobTypeName { get; set; } = "";
		public List<string> SubCategories { get; set; } = new List<string>();
	}
}
