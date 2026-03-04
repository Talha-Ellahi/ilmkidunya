namespace IKDFrontEnd.ViewModels
{
	public class PastPaperPageViewModel
	{
		public string PageTitle2 { get; set; }
		public string PageDescription { get; set; }
		public string PageContent { get; set; }



		public string MetaTitle { get; set; }
		public string MetaDesc { get; set; }
		public string MetaTags { get; set; }
		public string MetaKeywords { get; set; }
		public string Heading { get; set; }
		public string Detail { get; set; }
		public string DetailShort { get; set; }
		public string? Description { get; set; }
		public List<PastPaperViewModel> PastPapers { get; set; }
        public string Detail2 { get; internal set; }
    }
}
