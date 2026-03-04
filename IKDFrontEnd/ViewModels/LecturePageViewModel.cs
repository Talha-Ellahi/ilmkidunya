using IKDFrontEnd.Models;
using System.Collections.Generic;

namespace IKDFrontEnd.ViewModels
{
    public class LecturePageViewModel
    {
        public LecturePageViewModel()
        {
            // ✅ Default values
            PageTitle2 = "Lectures";
            ClassName = "Unknown Class";
            ChapterName = "N/A";

            SubjectImage = "/images/default-subject.png";
            PageHeading = "Default Heading";
            PageDescription1 = "No description available.";
            PageDescription2 = string.Empty;
            PageDescription3 = string.Empty;

            MetaTitle = "Default Meta Title";
            MetaKeywords = "lectures, study";
            MetaDescription = "Default meta description";
            MetaImage = "/images/default-meta.png";
            Chtitle = "Default Chapter";
            Metadesc = "Default description";

            // ✅ Empty lists instead of null
            LectureClasses = new List<TblLectureClass>();
            LectureSubjects = new List<TblLectureSubject>();
            LectureChapters = new List<TblLectureChapter>();
            Lectures = new List<TblLecture>();
            Topics = new List<TblLectureTopic>();
            Teachers = new List<TblLectureTeacher>();
            RelatedChapters = new List<TblLectureChapter>(); // ✅ new list
        }

        public string PageTitle2 { get; set; }
        public string ClassName { get; set; }
        public string ChapterName { get; set; }
        public int CurrentClassId { get; set; }
        public int CurrentSubjectId { get; set; }
        public int CurrentChapterId { get; set; }
        public string CurrentClassSlug { get; set; }
        public string CurrentSubjectSlug { get; set; }
        public string SubjectImage { get; set; }
        public string? PageHeading { get; set; }
        public string? PageDescription1 { get; set; }
        public string? PageDescription2 { get; set; }
        public string? PageDescription3 { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaKeywords { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaImage { get; set; }
        public string? Chtitle { get; set; }
        public string? Metadesc { get; set; }

        public List<TblLectureClass> LectureClasses { get; set; }
        public List<TblLectureSubject> LectureSubjects { get; set; }
        public List<TblLectureChapter> LectureChapters { get; set; }
        public List<TblLecture> Lectures { get; set; }
        public List<TblLectureTopic> Topics { get; set; }
        public List<TblLectureTeacher> Teachers { get; set; }

        // ✅ New Property for Related Chapters
        public List<TblLectureChapter> RelatedChapters { get; set; }
    }

    public class LectureClassViewModel
    {
        public int Id { get; set; }
        public string ClassName { get; set; }
        public string ClassUrl { get; set; }
    }
}
