using IKDFrontEnd.Models;
using System;

namespace IKDFrontEnd.ViewModels
{
    public class PastPaperViewModel
    {
        public int Id { get; set; }
        public string PaperName { get; set; }
        public int Year { get; set; }
        public string Month { get; set; }
        public string BoardName { get; set; }
        public string ClassName { get; set; }
        public string SubjectName { get; set; }
        public string? QuestionType { get; set; }
        public string? Language { get; set; }
        public string? PdfUrl { get; set; }
        public string? ImageName { get; set; } 
        public DateTime Dated { get; set; } 

        List<SectionTypeImport> SectionTypeImports { get; set; }
        List<SectionContentImport> SectionContentImports { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDesc { get; set; }
        public string MetaTags { get; set; }
        public string MetaKeywords { get; set; }
        public string Heading { get; set; }
        public string Detail { get; set; }
        public string DetailShort { get; set; }
        public string? Description { get; set; }



        // ✅ Generate Image URL based on Year and Month
        public string ImageUrl
        {
            get
            {
                string year = Dated.ToString("yyyy");
                string month = Dated.ToString("MM").TrimStart('0'); // Remove leading zero
                return $"https://pastpapers.ilmkidunya.com/past_papers/Images/{year}/{month}/large/{ImageName}";
            }
        }

        public int year { get; internal set; }
    }
}
