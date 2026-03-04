namespace IKDFrontEnd.ViewModels
{
    public class UniversityDetailsViewModel
    {
        public string UniversityName { get; set; }
        public string UniversityLogo { get; set; }  // URL or path to the logo image
        public DateTime? AdmissionOpenDate { get; set; }
        public DateTime? AppliedDate { get; set; }
        public string UniversityUrl { get; set; }  // URL for the university
    }
}
