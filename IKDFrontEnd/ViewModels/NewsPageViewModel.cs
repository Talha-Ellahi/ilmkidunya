using IKDFrontEnd.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;

namespace IKDFrontEnd.ViewModels
{
	public class NewsPageViewModel
	{
		// Existing fields
		public string PageTitle { get; set; }
		public string PageDescription { get; set; }
		public string MetaTitle { get; set; }
        public string Desc1 { get; set; }
        public string MetaDescription { get; set; }
		public string MetaKeywords { get; set; }
		public string MetaImage { get; set; }
		public string NewsImage { get; set; }

		// New fields for news details
		public string NewsCategoryName { get; set; }

		// Adding fields for news content
		public string MainHeading { get; set; }
		public string NewsDetails { get; set; }
		public string Picture1 { get; set; }
		public string Picture1Desc { get; set; }
		public string Picture2 { get; set; }
		public string Picture2Desc { get; set; }
		public string Picture3 { get; set; }
		public string Picture3Desc { get; set; }
		public DateTime? Dated { get; set; }
		public string NewsUrl { get; set; }
		public string NewsViews { get; set; }


		// Collection of lists
		public List<TblMainNews> LatestNews { get; set; }
		public List<TblNewsCategory> NewsCategories { get; set; }
		public List<TblNewsComment> NewsComments { get; set; }

		public List<TblCommentsChild> NewsChildComments { get; set; }

		public List<NewsRelated> RelatedNews { get; set; }
	}

	public class NewsRelated
	{
		public string MainHeading { get; set; }
		public string PictureThumbnail { get; set; }
		public string NewsUrl { get; set; }
		public DateTime? Dated { get; set; }
	}

}
