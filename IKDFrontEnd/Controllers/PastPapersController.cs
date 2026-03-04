using IKDFrontEnd.Models;
using IKDFrontEnd.Services;
using IKDFrontEnd.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using static NuGet.Packaging.PackagingConstants;


namespace IKDFrontEnd.Controllers
{

    public class PastPapersController : Controller
    {
        private readonly DbikdContext _context;
        private readonly BannerService _bannerService;

        public PastPapersController(DbikdContext context, BannerService bannerService)
        {
            _context = context;
            _bannerService = bannerService;
        }


        [Route("past_papers")]
        public async Task<IActionResult> Home()
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            var sectionType = _context.SectionTypeImports
                .FirstOrDefault(c => c.Url == "past_papers");

            var sectionContent = await _context.SectionContentImports
                .Where(c => c.ContentId == sectionType.Id && c.IsActive == true)
                .Select(c => new TblCm
                {
                    Id = c.Id,
                    Url = sectionType.Url,
                    Heading = c.Heading,
                    Desc1 = c.Detail,
                    Desc2 = c.DetailShort,
                    MetaDesc = c.MetaDesc,
                    MetaKeys = c.MetaKeyword,
                    MetaTitle = c.MetaTitle,
                }).FirstOrDefaultAsync();

            ViewBag.CmsData = sectionContent;

            // Get initial boards data
            var initialBoards = await _context.Boards
                .Where(b => b.IsActive == true)
                .Select(b => new { id = b.Id, name = b.Name })
                .OrderBy(b => b.name)
                .ToListAsync();

            ViewBag.InitialBoards = initialBoards;

            return View();
        }


        [HttpGet]
        [Route("/past_papers/search_papers.aspx")]
        public async Task<IActionResult> GetPageContentByFilters(
            int classId = 0,
            int subjectId = 0,
            int boardId = 0)
        {
            try
            {
                // Start with the base query
                var query = _context.TblPastPapers
                    .Include(p => p.Board)
                    .Include(p => p.Ppsubject)
                    .Include(p => p.Ppclass)
                    .AsQueryable();

                // Apply filters only if parameters are provided
                if (classId > 0)
                    query = query.Where(p => p.PpclassId == classId);

                if (subjectId > 0)
                    query = query.Where(p => p.PpsubjectId == subjectId);


                if (boardId > 0)
                    query = query.Where(p => p.BoardId == boardId);

                // Execute query and fetch data
                var papers = await query.ToListAsync();

                var paperViewModels = papers
                    .Select(p => new PastPaperViewModel
                    {
                        Id = p.Id,
                        PaperName = p.Pnname,
                        ImageName = p.Image,
                        Dated = p.Date ?? DateTime.MinValue,
                        Heading = p.Pnname,
                        SubjectName = p.Ppsubject?.PpsubjectName ?? "N/A",
                        BoardName = p.Board?.Name ?? "N/A",
                        Year = p.Year,
                        Month = p.Date?.Month.ToString("00") ?? "01" // Default to January if no date                        
                    }).OrderByDescending(p => p.Year)
                    .ToList();


                var viewModel = new PastPaperPageViewModel
                {
                    Heading = "Search Results",
                    Detail = $"Found {paperViewModels.Count} past papers",
                    Description = "",
                    MetaTitle = "Past Papers Search Results",
                    MetaDesc = "Search past papers by board, class, year and subject",
                    MetaKeywords = "past papers, search, filter",
                    MetaTags = "",
                    PastPapers = paperViewModels
                };

                var banners = await _bannerService.GetBannersAsync();
                ViewBag.Banners = banners;

                return View("GetPageContentByUrl", viewModel);
            }
            catch (Exception ex)
            {
                // Log error
                return StatusCode(500, "An error occurred while searching past papers");
            }
        }




        [Route("past_papers/ba/{urlSlug}")]
        [Route("past_papers/{urlSlug}")]
        public async Task<IActionResult> GetPageContentByUrl(string urlSlug)
        {
            // --- 301 Redirect for parentheses in URL ---
            string currentPath = Request.Path.Value;
            int lastSlash = currentPath.LastIndexOf('/');
            if (lastSlash >= 0 && lastSlash < currentPath.Length - 1)
            {
                string basePath = currentPath.Substring(0, lastSlash + 1); // includes trailing '/'
                string currentSlug = currentPath.Substring(lastSlash + 1);
                string newSlug = currentSlug.Replace("(", "-").Replace(")", "-");
                if (newSlug != currentSlug)
                {
                    string newUrl = basePath + newSlug;
                    return RedirectPermanent(newUrl); // 301 redirect
                }
            }

            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            var content = await (from sc in _context.SectionContentImports
                                 join st in _context.SectionTypeImports
                                 on sc.ContentId equals st.Id
                                 //where st.Url.Contains(urlSlug.Replace("-urdu-medium", "").Replace("-english-medium", ""))
                                 where st.Url == urlSlug
                                 select new
                                 {
                                     st.Id,
                                     st.Name,
                                     st.ClassId,
                                     st.SubjectId,
                                     st.Url,
                                     st.InstituteTypeId,
                                     st.InstituteId,
                                     sc.ContentId,
                                     sc.Heading,
                                     sc.Detail,
                                     sc.Detail2,
                                     sc.DetailShort,
                                     sc.MetaDesc,
                                     sc.MetaTitle,
                                     sc.MetaTags,
                                     sc.MetaKeyword,
                                     sc.PanelDescriptionId,
                                 })
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync();

            if (content == null)
            {
                content = await (from sc in _context.SectionContentImports
                                 join st in _context.SectionTypeImports
                                 on sc.ContentId equals st.Id
                                 //where st.Url.Contains(urlSlug.Replace("-urdu-medium", "").Replace("-english-medium", ""))
                                 where st.Url == urlSlug.Replace("-urdu-medium", "").Replace("-english-medium", "")
                                 select new
                                 {
                                     st.Id,
                                     st.Name,
                                     st.ClassId,
                                     st.SubjectId,
                                     st.Url,
                                     st.InstituteTypeId,
                                     st.InstituteId,
                                     sc.ContentId,
                                     sc.Heading,
                                     sc.Detail,
                                     sc.Detail2,
                                     sc.DetailShort,
                                     sc.MetaDesc,
                                     sc.MetaTitle,
                                     sc.MetaTags,
                                     sc.MetaKeyword,
                                     sc.PanelDescriptionId,
                                 })
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync();

            }

            if (content == null)
            {
                var redirectSlug = $"{urlSlug}".ToLower().Replace(" ", "-");
                return await PaperDetails(redirectSlug);
            }
            var PanelDescription = await (from pd in _context.PastPaperPageDescriptions
                                          join sc in _context.SectionContentImports
                                          on pd.Id equals sc.PanelDescriptionId
                                          where pd.Id == content.PanelDescriptionId
                                          select new
                                          {
                                              pd.Id,
                                              pd.Description,
                                          })
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync();

            ViewBag.PanelDescription = PanelDescription;



            IQueryable<TblPastPaper> papersQuery = _context.TblPastPapers
                        .Where(p => p.IsDelete == true || p.IsDelete == null);


            var InstituteId = content.InstituteId;
            var ClassId = content.ClassId;
            if (content.InstituteId == 43 && urlSlug.Contains("5th"))
            {
                InstituteId = 51;
                ClassId = 11;
            }
            if (content.InstituteId == 44 && urlSlug.Contains("8th"))
            {
                InstituteId = 51;
                ClassId = 12;
            }

            if (InstituteId != null && InstituteId != 0 &&
                ClassId != null && ClassId != 0 &&
                content.SubjectId != null && content.SubjectId != 0)
            {

                papersQuery = papersQuery
                    .Where(p => p.BoardId == InstituteId.Value &&
                                p.PpclassId == ClassId.Value &&
                                p.PpsubjectId == content.SubjectId.Value);
            }
            else if (InstituteId != null && InstituteId != 0 &&
                     ClassId != null && ClassId != 0)
            {

                papersQuery = papersQuery
                    .Where(p => p.BoardId == InstituteId.Value &&
                                p.PpclassId == ClassId.Value);
            }
            else if (InstituteId != null && InstituteId != 0)
            {

                papersQuery = papersQuery
                    .Where(p => p.BoardId == InstituteId.Value);
            }

            if (urlSlug.Contains("english-medium"))
            {
                papersQuery = papersQuery.Where(p => p.Language == "English");
            }
            else if (urlSlug.Contains("urdu-mediumn"))
            {
                papersQuery = papersQuery.Where(p => p.Language == "Urdu");
            }


            List<PastPaperViewModel> papers = null;
            if (InstituteId != null && InstituteId != 0)
            {
                papers = await papersQuery
                    .OrderByDescending(p => p.Date)
                    .Select(p => new PastPaperViewModel
                    {
                        Id = p.Id,
                        PaperName = p.Pnname,
                        Year = p.Date.HasValue ? p.Date.Value.Year : DateTime.Now.Year,
                        Month = p.Date.HasValue ? p.Date.Value.ToString("MM").TrimStart('0') : DateTime.Now.ToString("MM"),
                        BoardName = _context.Boards.Where(b => b.Id == p.BoardId).Select(b => b.Name).FirstOrDefault(),
                        ClassName = _context.TblPpclasses.Where(c => c.Id == p.PpclassId).Select(c => c.PpclassName).FirstOrDefault(),
                        SubjectName = _context.TblPpsubjects.Where(s => s.Id == p.PpsubjectId).Select(s => s.PpsubjectName).FirstOrDefault(),
                        QuestionType = p.QuestionType,
                        Language = p.Language,
                        Description = p.Description,
                        PdfUrl = p.Pdf,
                        ImageName = p.Image,
                        year = p.Year
                    })
                    .Take(50).OrderByDescending(p => p.year)
                    .ToListAsync();
            }


            var viewModel = new PastPaperPageViewModel
            {
                Heading = content.Name,
                Detail = content.Detail,
                Detail2 = content.Detail2,
                Description = content.DetailShort,
                MetaTitle = content.MetaTitle,
                MetaDesc = content.MetaDesc,
                MetaKeywords = content.MetaKeyword,
                MetaTags = content.MetaTags,
                PastPapers = papers
            };


            return View("GetPageContentByUrl", viewModel);
        }



        public async Task<IActionResult> PaperDetails(string redirectSlug)
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            if (string.IsNullOrEmpty(redirectSlug) || !redirectSlug.Contains('-'))
                return NotFound();

            int lastHyphenIndex = redirectSlug.LastIndexOf('-');
            var searchBase = redirectSlug.Substring(0, lastHyphenIndex).Replace("-", " ");
            var normalizedSlug = NormalizeStringDetail(searchBase);
            var lowerSearchBase = searchBase.ToLower();

            // Try database search first
            var paper = await _context.TblPastPapers
                .AsNoTracking()
                .Where(p => p.Pnname.ToLower().Replace("(", " ").Replace(")", " ").Contains(lowerSearchBase))
                .Select(p => new PastPaperViewModel
                {
                    Id = p.Id,
                    PaperName = p.Pnname,
                    ImageName = p.Image,
                    Dated = p.Date ?? DateTime.MinValue,
                    Heading = p.Pnname,
                    Description = p.Description
                })
                .FirstOrDefaultAsync();

            if (paper != null)
            {
                var viewModel = new PastPaperPageViewModel
                {
                    PageTitle2 = paper.PaperName,
                    PastPapers = new List<PastPaperViewModel> { paper }
                };
                return View("PaperDetails", viewModel);
            }

            // Fallback to recent papers search
            var recentPapers = await _context.TblPastPapers
                .AsNoTracking()
                .OrderByDescending(p => p.Date)
                .Take(300)
                .Select(p => new
                {
                    p.Id,
                    p.Pnname,
                    p.Image,
                    p.Date,
                    p.Description
                })
                .ToListAsync();

            foreach (var p in recentPapers)
            {
                if (NormalizeStringDetail(p.Pnname).Contains(normalizedSlug))
                {
                    var resultPaper = new PastPaperViewModel
                    {
                        Id = p.Id,
                        PaperName = p.Pnname,
                        ImageName = p.Image,
                        Dated = p.Date ?? DateTime.MinValue,
                        Heading = p.Pnname,
                        Description = p.Description
                    };

                    var resultViewModel = new PastPaperPageViewModel
                    {
                        PageTitle2 = resultPaper.PaperName,
                        PastPapers = new List<PastPaperViewModel> { resultPaper }
                    };
                    return View("PaperDetails", resultViewModel);
                }
            }

            return NotFound();
        }

        private static string NormalizeStringDetail(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            var sb = new StringBuilder(input.Length);
            var lowerInput = input.ToLowerInvariant();

            for (int i = 0; i < lowerInput.Length; i++)
            {
                char c = lowerInput[i];
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                    sb.Append(c);
            }

            return sb.ToString();
        }


        // Normalizer function
        private static string NormalizeString(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            // To lowercase
            var normalized = input.ToLowerInvariant();

            // Remove all non-alphanumeric characters (anything not a–z, 0–9)
            normalized = Regex.Replace(normalized, @"[^a-z0-9]", "");

            return normalized;
        }


        [HttpGet]
        [Route("/past_papers/get-boards")]
        public async Task<IActionResult> GetBoards()
        {
            try
            {
                var boards = await _context.Boards
                    .Where(b => b.IsActive == true)
                    .Select(b => new { id = b.Id, name = b.Name })
                    .OrderBy(b => b.name)
                    .ToListAsync();

                return Json(boards);
            }
            catch (Exception ex)
            {
                return Json(new List<object>());
            }
        }

        [HttpGet]
        [Route("/past_papers/get-classes")]
        public async Task<IActionResult> GetClasses(int boardId = 0)
        {
            try
            {
                var query = _context.TblPpclasses
                    .Where(c => !c.IsDelete.HasValue || !c.IsDelete.Value);

                // If boardId is provided, filter classes by board
                if (boardId > 0)
                {
                    query = query.Where(c => c.TblPastPapers.Any(p => p.BoardId == boardId));
                }

                var classes = await query
                    .Select(c => new { id = c.Id, name = c.PpclassName })
                    .OrderBy(c => c.name)
                    .ToListAsync();

                return Json(classes);
            }
            catch (Exception ex)
            {
                return Json(new List<object>());
            }
        }

        [HttpGet]
        [Route("/past_papers/get-subjects")]
        public async Task<IActionResult> GetSubjects(int boardId = 0, int classId = 0)
        {
            try
            {
                var query = _context.TblPpsubjects
                    .Where(s => !s.IsDelete.HasValue || !s.IsDelete.Value);

                // If boardId and classId are provided, filter subjects
                if (boardId > 0 && classId > 0)
                {
                    query = query.Where(s => s.TblPastPapers.Any(p => p.BoardId == boardId && p.PpclassId == classId));
                }

                var subjects = await query
                    .Select(s => new { id = s.Id, name = s.PpsubjectName })
                    .OrderBy(s => s.name)
                    .ToListAsync();

                return Json(subjects);
            }
            catch (Exception ex)
            {
                return Json(new List<object>());
            }
        }



    }
}
