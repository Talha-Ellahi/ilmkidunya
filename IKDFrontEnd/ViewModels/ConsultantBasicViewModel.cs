namespace IKDFrontEnd.ViewModels
{
    public class ConsultantBasicViewModel
    {
        public ConsultantBasicViewModel()
        {
            // ✅ Default values
            Id = 0;
            Name = "Unknown Consultant";
            Url = "javascript:void(0);";

            Image = "/images/default-consultant.png";
            PremiumMember = false;

            Logo = "/images/default-logo.png";
            Views = 0;
            Zoom = 0;

            Latitude = "0.000000";
            Longitude = "0.000000";
            Email = "notprovided@example.com";
            Phone = "N/A";
            Detail = "No details available.";
            Address = "Not available";
        }

        public decimal? Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public bool? PremiumMember { get; set; }

        public string? Logo { get; set; }
        public decimal? Views { get; set; }
        public byte? Zoom { get; set; }

        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Detail { get; set; }
        public string? Address { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
    }

    public class ConsultantDetailViewModel
    {
        public ConsultantBasicViewModel Consultant { get; set; }
        public List<ConsultantCountryViewModel> Countries { get; set; }
       
    }
    public class ConsultantCountryViewModel
    {
        public string CountryName { get; set; }
        public string Flag { get; set; }
    }
}
