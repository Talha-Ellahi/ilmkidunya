using IKDFrontEnd.Models;
using Microsoft.Identity.Client;
using System.Diagnostics;

namespace IKDFrontEnd.ViewModels
{
	public class ArticlesPageViewModel
	{
		public string PageTitle { get; set; }
		public string PageDescription { get; set; }
		public string MetaTitle { get; set; }
		public string MetaDescription { get; set; }
		public string MetaKeywords { get; set; }
		public string MetaImage { get; set; }
		public string ArticleImage { get; set; }
		public string ArticleDetail { get; set; }

		public string ArticleSenderName { get; set; }
		public string PostDate { get; set; }
		public string articleslug { get; set; }
		public string SearchTerm { get; set; }
		public string ArticleViews { get; set; }
		public int CurrentPage { get; set; }

		public List<TblArticle> Articles { get; set; }
		public List<TblArticleType> ArticleTypes { get; set; }

		public List<TblArticle> RelatedArticles { get; set; }
	}
}
