using Humanizer;
using IKDFrontEnd.DictionaryModels;
using IKDFrontEnd.DictionaryViewModels;
using IKDFrontEnd.Services;
using IKDFrontEnd.ViewModels.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // <-- Add this
using System.Text;
using System.Text.RegularExpressions;

namespace IKDFrontEnd.Controllers
{
    [Route("dictionary")]
    public class DictionaryController : Controller
    {
        private readonly DictionaryContext _context;
        private readonly BannerService _bannerService;
        private readonly CmsRepository _cmsRepo;
        private readonly IWebHostEnvironment _environment;
        private ICompositeViewEngine _viewEngine;
        private readonly ILogger<DictionaryController> _logger; // <-- Add logger
        private readonly IFtpService _ftpService;

        public DictionaryController(
            DictionaryContext context,
            BannerService bannerService,
            CmsRepository cmsRepo,
            IWebHostEnvironment environment,
            ICompositeViewEngine viewEngine,
            ILogger<DictionaryController> logger,
            IFtpService ftpService) // <-- Inject logger
        {
            _context = context;
            _bannerService = bannerService;
            _cmsRepo = cmsRepo;
            _environment = environment;
            _viewEngine = viewEngine;
            _logger = logger; // <-- Assign
            _ftpService = ftpService;

        }

        protected ICompositeViewEngine ViewEngine => _viewEngine;


        [HttpGet("")]
        public async Task<IActionResult> Home()
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            var cmsData = await _context.TblCms
                .Where(u => u.Url == "https://www.ilmlkidunya.com/dictionary").FirstOrDefaultAsync();

            if (cmsData == null)
            {

                cmsData = new TblCm
                {
                    Url = "https://www.ilmlkidunya.com/dictionary",
                    MetaTitle = "Dictionary for all Languages",
                    MetaDesc = "",
                    MetaKeys = "",

                    Date = DateTime.Now
                };
            }

            ViewBag.CmsData = cmsData;

            int englishLangId = 21;
            int urduLangId = 88;

            // ✅ Fetch words from DB
            var englishWords = _context.Words
                .Where(w => w.LanguageId == englishLangId)
                .OrderByDescending(x => x.SearchCount)
                .Take(5)
                .ToList();

            var urduWords = _context.Words
                .Where(w => w.LanguageId == urduLangId)
                .OrderByDescending(x => x.SearchCount)
                .Take(5)
                .ToList();

            // Get existing meanings for English to Urdu
            var englishWordIds = englishWords.Select(w => w.WordId).ToList();
            var existingEnToUrMeanings = await (from m in _context.Meanings
                                                join sw in _context.Words on m.SourceWordId equals sw.WordId
                                                join tw in _context.Words on m.TargetWordId equals tw.WordId
                                                where englishWordIds.Contains(m.SourceWordId) &&
                                                      tw.LanguageId == urduLangId
                                                select new
                                                {
                                                    SourceWord = sw.Word1,
                                                    TargetWord = tw.Word1
                                                }).ToListAsync();

            ViewBag.ExistingEnToUrMeanings = existingEnToUrMeanings.ToDictionary(
                m => m.SourceWord.ToLower(),
                m => m.TargetWord
            );

            // Get existing meanings for Urdu to English
            var urduWordIds = urduWords.Select(w => w.WordId).ToList();
            var existingUrToEnMeanings = await (from m in _context.Meanings
                                                join sw in _context.Words on m.SourceWordId equals sw.WordId
                                                join tw in _context.Words on m.TargetWordId equals tw.WordId
                                                where urduWordIds.Contains(m.SourceWordId) &&
                                                      tw.LanguageId == englishLangId
                                                select new
                                                {
                                                    SourceWord = sw.Word1,
                                                    TargetWord = tw.Word1
                                                }).ToListAsync();

            try
            {
                ViewBag.ExistingUrToEnMeanings = existingUrToEnMeanings.ToDictionary(
                m => m.SourceWord.ToLower(),
                m => m.TargetWord
                );
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: ", e);
            }

            var englishToUrdu = englishWords
                .Select((w, index) => new WordPair
                {
                    SrNo = index + 1,
                    English = w.Word1,
                    Urdu = "" // will be filled from existing meanings or API later
                })
                .ToList();

            var urduToEnglish = urduWords
                .Select((w, index) => new WordPair
                {
                    SrNo = index + 1,
                    Urdu = w.Word1,
                    English = "" // will be filled from existing meanings or API later
                })
                .ToList();

            ViewBag.EnglishToUrdu = englishToUrdu;
            ViewBag.UrduToEnglish = urduToEnglish;

            return View();
        }



        [HttpGet("{lang1}-to-{lang2}-dictionary")]
        public async Task<IActionResult> Dictionary(string lang1, string lang2)
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            var cmsData = await _context.TblCms
                .Where(u => u.Url == $"https://www.ilmkidunya.com/dictionary/{lang1}-to-{lang2}-dictionary").FirstOrDefaultAsync();

            ViewBag.CmsData = cmsData;

            var model = getLanguagesFromName(lang1, lang2);

            //if (model.lang1.LanguageId == 21)
            //{
            //    model.lang1.LanguageId = 22;
            //}


            // Fetch words excluding "symbol" and then filter out words with digits or special characters
            model.words = (await _context.Words
                .Where(l => l.LanguageId == model.lang1.LanguageId && l.PartOfSpeech != "symbol")
                .Take(200) // fetch a few more, we’ll filter below
                .ToListAsync())
                .Where(w => Regex.IsMatch(w.Word1, @"^[A-Za-z]+$")) // only letters allowed
                .Take(10)
                .ToList();


            foreach (var word in model.words)
            {
                if (!string.IsNullOrEmpty(word.Definition))
                {
                    // Take first part before comma, trim spaces
                    word.Definition = word.Definition.Split(',')[0].Trim();
                }
            }

            // Get existing meanings from database to avoid API calls
            if (model.words.Any() && model.lang1.LanguageId != model.lang2.LanguageId)
            {
                var sourceWordIds = model.words.Select(w => w.WordId).ToList();

                // Get meanings with manual joins instead of navigation properties
                var existingMeanings = await (from m in _context.Meanings
                                              join sw in _context.Words on m.SourceWordId equals sw.WordId
                                              join tw in _context.Words on m.TargetWordId equals tw.WordId
                                              where sourceWordIds.Contains(m.SourceWordId) &&
                                                    tw.LanguageId == model.lang2.LanguageId
                                              select new
                                              {
                                                  SourceWord = sw.Word1,
                                                  TargetWord = tw.Word1
                                              }).ToListAsync();

                ViewBag.ExistingMeanings = existingMeanings
                    .GroupBy(m => m.SourceWord.ToLower())
                    .ToDictionary(
                        g => g.Key,
                        g => g.First().TargetWord
                    );

            }

            return View(model);
        }


        [HttpGet("{lang1}-to-{lang2}-dictionary/loadmore")]
        public async Task<IActionResult> LoadMoreWords(string lang1, string lang2, int page = 1)
        {
            try
            {
                var model = getLanguagesFromName(lang1, lang2);

                if (model.lang1 == null || model.lang2 == null)
                {
                    return Json(new { success = false, error = "Language not found" });
                }

                int pageSize = 10;
                int skip = (page - 1) * pageSize;

                // IMPORTANT: Skip the first 10 words that were already shown on the main page
                // Since main page shows first 10, loadmore should start from 11th word
                skip += 10; // Add 10 to skip the initial words

                var words = await _context.Words
                    .Where(l => l.LanguageId == model.lang1.LanguageId && l.PartOfSpeech != "symbol")
                    .OrderBy(w => w.WordId) // Add ordering for consistent pagination
                    .Skip(skip)
                    .Take(pageSize)
                    .ToListAsync();

                // If no more words, return empty
                if (!words.Any())
                {
                    return Json(new
                    {
                        success = true,
                        words = new List<object>(),
                        existingMeanings = new Dictionary<string, string>(),
                        noMoreWords = true
                    });
                }

                Dictionary<string, string> existingMeaningsDict = new Dictionary<string, string>();

                if (words.Any() && model.lang1.LanguageId != model.lang2.LanguageId)
                {
                    var sourceWordIds = words.Select(w => w.WordId).ToList();

                    var existingMeanings = await (from m in _context.Meanings
                                                  join sw in _context.Words on m.SourceWordId equals sw.WordId
                                                  join tw in _context.Words on m.TargetWordId equals tw.WordId
                                                  where sourceWordIds.Contains(m.SourceWordId) &&
                                                        tw.LanguageId == model.lang2.LanguageId
                                                  select new
                                                  {
                                                      SourceWord = sw.Word1,
                                                      TargetWord = tw.Word1
                                                  }).ToListAsync();

                    existingMeaningsDict = existingMeanings.ToDictionary(
                        m => m.SourceWord.ToLower(),
                        m => m.TargetWord
                    );
                }

                return Json(new
                {
                    success = true,
                    words = words.Select(w => new
                    {
                        word1 = w.Word1,
                        definition = w.Definition,
                        wordId = w.WordId
                    }),
                    existingMeanings = existingMeaningsDict,
                    hasMore = words.Count == pageSize // Indicate if there might be more words
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }



        [HttpGet("{lang1}-to-{lang2}-translator")]
        public async Task<IActionResult> Translate(string lang1, string lang2)
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;
            TblCm cmsData = await _context.TblCms
                .Where(u => u.Url == $"https://www.ilmlkidunya.com/dictionary/{lang1}-to-{lang2}-translator")
                .FirstOrDefaultAsync();
            if (cmsData == null)
            {
                cmsData = new TblCm
                {
                    MetaTitle = "Dictionary for all Languages",
                    MetaDesc = "",
                    MetaKeys = "",
                    Date = DateTime.Now
                };
            }

            ViewBag.CmsData = cmsData;
            var model = getLanguagesFromName(lang1, lang2);
            return View(model);
        }




        [HttpGet("{lang}-words-with-{alphabet}")]
        public async Task<IActionResult> AlphabetWords(string lang, string alphabet)
        {
            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;

            var encodedAlphabet = Uri.EscapeDataString(alphabet);

            var cmsData = await _context.TblCms
                .Where(u => u.Url == $"https://www.ilmlkidunya.com/dictionary/{lang}-words-with-{alphabet}").FirstOrDefaultAsync();

            if (cmsData == null)
            {
                cmsData = new TblCm
                {
                    MetaTitle = "Dictionary for all Languages",
                    MetaDesc = "",
                    MetaKeys = "",
                    Heading = $"Words That Start With '{alphabet.ToUpper()}' in {char.ToUpper(lang[0]) + lang.Substring(1)}"
                };
            }

            ViewBag.CmsData = cmsData;
            var model = new DictionaryPage();
            model = getLanguagesFromName(lang, null);

            if (model.lang1.LanguageId == 21)
                model.lang1.LanguageId = 22;

            if (model.lang1.Name == "English")
            {
                model.lang2.Code = "ur";
                model.lang2.Name = "Urdu";
                model.lang2.LanguageId = 88;

            }
            else
            {
                model.lang2.Code = "en";
                model.lang2.Name = "English";
                model.lang2.LanguageId = 22;
            }

            model.words = await _context.Words
                .Where(l => l.LanguageId == model.lang1.LanguageId &&
                            l.Word1.StartsWith(alphabet) &&
                            l.PartOfSpeech != "symbol")
                .ToListAsync();

            foreach (var word in model.words)
            {
                if (!string.IsNullOrEmpty(word.Definition))
                    word.Definition = word.Definition.Split(',')[0].Trim();
            }

            ViewBag.Alphabet = alphabet.ToUpperInvariant();
            return View(model);
        }




        [Route("{word}-meaning-in-{translatedLanguage}")]
        [HttpGet("detail/word/{word}/from/{currentLanguage}/to/{translatedLanguage}")]
        public async Task<IActionResult> Detail(string word, string currentLanguage, string translatedLanguage)
        {
            string htmlFileName = $"{word}-meaning-in-{translatedLanguage}.html";
            string remoteFilePath = $"files/dictionary/{htmlFileName}"; // relative path on FTP

            _logger.LogInformation("Detail action called for word '{Word}', translatedLanguage '{TranslatedLanguage}'. Remote path: {RemotePath}", word, translatedLanguage, remoteFilePath);

            // Check if file already exists on FTP
            bool fileExists = await _ftpService.FileExistsAsync(remoteFilePath);
            if (fileExists)
            {
                _logger.LogInformation("File already exists on FTP. Downloading from {RemotePath}", remoteFilePath);
                try
                {
                    string existingHtml = await _ftpService.DownloadFileAsync(remoteFilePath);
                    return Content(existingHtml, "text/html");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to download existing file from FTP. Will regenerate.");
                    // fall through to regeneration
                }
            }

            // --- Generate HTML (same as before) ---
            word = word.Replace("-", " ");
            TblCm cmsData = await _context.TblCms
                .Where(u => u.Url == "https://www.ilmlkidunya.com/dictionary")
                .FirstOrDefaultAsync();

            if (cmsData == null)
            {
                cmsData = new TblCm
                {
                    MetaTitle = $"{word} Meaning in {translatedLanguage}",
                    MetaDesc = "",
                    MetaKeys = "",
                    Date = DateTime.Now
                };
            }
            ViewBag.CmsData = cmsData;

            var banners = await _bannerService.GetBannersAsync();
            ViewBag.Banners = banners;
            var translatedLanguageCode = getLanguagesFromName(null, translatedLanguage);

            var normalizedWord = word.Trim().ToLower();
            var foundWord = await _context.Words
                .FirstOrDefaultAsync(w => w.Word1.ToLower() == normalizedWord);

            if (foundWord == null)
            {
                foundWord = new Word();
                foundWord.Word1 = word;
                foundWord.LanguageId = 21;
            }
            else
            {
                foundWord.SearchCount++;
                await _context.SaveChangesAsync();
            }

            var wordText = foundWord?.Word1;
            var completeLanguage = translatedLanguageCode.lang2.Name;
            var completeCurrentLanguage = _context.Languages
                .Where(l => l.LanguageId == foundWord.LanguageId)
                .FirstOrDefault();

            var currentLangCode = currentLanguage != null ? currentLanguage : completeCurrentLanguage.Code;
            var translatedLangCode = translatedLanguage == "english" ? "en-gb" : translatedLanguageCode.lang2.Code;

            // Get existing meaning from database
            string existingMeaning = null;
            string existingDefinition = null;
            string existingSynonyms = null;
            string existingAntonyms = null;
            string existingDescription = null;
            string existingPartOfSpeech = null;

            if (foundWord.WordId > 0)
            {
                // Get existing translation meaning
                var existingMeaningRecord = await (from m in _context.Meanings
                                                   join sw in _context.Words on m.SourceWordId equals sw.WordId
                                                   join tw in _context.Words on m.TargetWordId equals tw.WordId
                                                   where m.SourceWordId == foundWord.WordId &&
                                                         tw.LanguageId == translatedLanguageCode.lang2.LanguageId
                                                   select new
                                                   {
                                                       TargetWord = tw.Word1
                                                   }).FirstOrDefaultAsync();

                if (existingMeaningRecord != null)
                {
                    existingMeaning = existingMeaningRecord.TargetWord;
                    if (!string.IsNullOrEmpty(existingMeaning) && existingMeaning.Contains("&#x"))
                    {
                        existingMeaning = System.Net.WebUtility.HtmlDecode(existingMeaning);
                    }
                }

                // Get existing dictionary data for the word itself
                var englishLanguage = await _context.Languages.FirstOrDefaultAsync(l => l.Code == "en");
                if (englishLanguage != null)
                {
                    var englishWord = await _context.Words
                        .FirstOrDefaultAsync(w => w.Word1.ToLower() == normalizedWord && w.LanguageId == englishLanguage.LanguageId);

                    if (englishWord != null)
                    {
                        existingDefinition = englishWord.Definition;
                        existingSynonyms = englishWord.Synonyms;
                        existingAntonyms = englishWord.Antonyms;
                        existingDescription = englishWord.Description;
                        existingPartOfSpeech = englishWord.PartOfSpeech;

                        // Decode any HTML entities
                        if (!string.IsNullOrEmpty(existingDefinition) && existingDefinition.Contains("&#x"))
                            existingDefinition = System.Net.WebUtility.HtmlDecode(existingDefinition);
                        if (!string.IsNullOrEmpty(existingSynonyms) && existingSynonyms.Contains("&#x"))
                            existingSynonyms = System.Net.WebUtility.HtmlDecode(existingSynonyms);
                        if (!string.IsNullOrEmpty(existingAntonyms) && existingAntonyms.Contains("&#x"))
                            existingAntonyms = System.Net.WebUtility.HtmlDecode(existingAntonyms);
                        if (!string.IsNullOrEmpty(existingDescription) && existingDescription.Contains("&#x"))
                            existingDescription = System.Net.WebUtility.HtmlDecode(existingDescription);
                    }
                }
            }

            var model = new DictionaryViewModel
            {
                Word = char.ToUpper(wordText[0]) + wordText.Substring(1).ToLower(),
                CurrentLanguage = currentLangCode,
                CompleteLanguage = completeLanguage,
                CompleteCurrentLanguage = completeCurrentLanguage.Name,
                TranslatedLanguageCode = translatedLangCode,
                TranslatedLanguage = translatedLanguage,
                ExistingMeaning = existingMeaning,
                ExistingDefinition = existingDefinition,
                ExistingSynonyms = existingSynonyms,
                ExistingAntonyms = existingAntonyms,
                ExistingDescription = existingDescription,
                ExistingPartOfSpeech = existingPartOfSpeech
            };

            foreach (var language in LanguageData.Languages)
            {
                if (language.Code == translatedLanguage)
                {
                    model.CompleteLanguage = language.Name;
                }
            }

            model.AvailableLanguages = await _context.Languages
                .Select(l => new LanguageOption { Code = l.Code, name = l.Name })
                .ToListAsync();

            var viewResult = View(model);
            _logger.LogInformation("Rendering view to string...");
            string htmlContent = await RenderViewToStringAsync(viewResult);
            _logger.LogInformation("View rendered to string successfully, length: {Length} characters", htmlContent.Length);

            // --- Upload to FTP (background, but we wait to ensure it's saved for next time) ---
            try
            {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(htmlContent)))
                {
                    await _ftpService.UploadFileAsync(remoteFilePath, stream);
                }
                _logger.LogInformation("File uploaded successfully to FTP: {RemotePath}", remoteFilePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload file to FTP: {RemotePath}", remoteFilePath);
                // Show error on page (as before)
                string errorDiv = $@"
            <div style='background:#ffdddd; border:3px solid #cc0000; border-radius:8px; 
                        padding:20px; margin:20px; font-family:Consolas, monospace; 
                        white-space:pre-wrap; box-shadow:0 4px 8px rgba(0,0,0,0.1);'>
                <h3 style='color:#cc0000; margin-top:0;'>⚠️ Static File Upload Failed</h3>
                <p><strong>Error:</strong> {System.Net.WebUtility.HtmlEncode(ex.Message)}</p>
                <p><strong>Remote path:</strong> {System.Net.WebUtility.HtmlEncode(remoteFilePath)}</p>
                <p><em>The page content is still shown below.</em></p>
            </div>";

                int bodyIndex = htmlContent.IndexOf("<body", StringComparison.OrdinalIgnoreCase);
                if (bodyIndex >= 0)
                {
                    int bodyEndIndex = htmlContent.IndexOf('>', bodyIndex);
                    if (bodyEndIndex >= 0)
                        htmlContent = htmlContent.Insert(bodyEndIndex + 1, errorDiv);
                    else
                        htmlContent = errorDiv + htmlContent;
                }
                else
                {
                    htmlContent = errorDiv + htmlContent;
                }
            }

            return Content(htmlContent, "text/html");
        }

        private async Task<string?> SaveHtmlToFile(string filePath, string htmlContent)
        {
            try
            {
                var directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    _logger.LogInformation("Directory does not exist. Creating: {Directory}", directory);
                    Directory.CreateDirectory(directory);
                }

                await System.IO.File.WriteAllTextAsync(filePath, htmlContent);
                _logger.LogInformation("HTML file saved successfully to {FilePath}", filePath);
                return null; // success
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving HTML file to {FilePath}", filePath);
                return ex.Message; // return error message
            }
        }

        private async Task<string> RenderViewToStringAsync(ViewResult viewResult)
        {
            using (var sw = new StringWriter())
            {
                viewResult.ViewData = ViewData;
                viewResult.TempData = TempData;

                var viewEngineResult = viewResult.ViewName != null
                    ? ViewEngine?.FindView(ControllerContext, viewResult.ViewName, true)
                    : ViewEngine?.FindView(ControllerContext, ControllerContext.ActionDescriptor.ActionName, true);

                if (viewEngineResult?.View == null)
                {
                    throw new Exception($"View '{viewResult.ViewName ?? ControllerContext.ActionDescriptor.ActionName}' not found.");
                }

                var view = viewEngineResult.View;

                var viewContext = new ViewContext(
                    ControllerContext,
                    view,
                    ViewData,
                    TempData,
                    sw,
                    new HtmlHelperOptions()
                );

                await view.RenderAsync(viewContext);
                return sw.ToString();
            }
        }

        [HttpGet("check-existing-meaning")]
        public async Task<IActionResult> CheckExistingMeaning(string word, string sourceLang, string targetLang)
        {
            try
            {
                var sourceLanguage = await _context.Languages
                    .FirstOrDefaultAsync(l => l.Code.ToLower() == sourceLang.ToLower());
                var targetLanguage = await _context.Languages
                    .FirstOrDefaultAsync(l => l.Code.ToLower() == targetLang.ToLower());

                if (sourceLanguage == null || targetLanguage == null)
                    return Json(new { exists = false });

                var sourceWord = await _context.Words
                    .FirstOrDefaultAsync(w => w.LanguageId == sourceLanguage.LanguageId &&
                                              w.Word1.ToLower() == word.ToLower());

                if (sourceWord == null)
                    return Json(new { exists = false });

                var existingMeaning = await (from m in _context.Meanings
                                             join tw in _context.Words on m.TargetWordId equals tw.WordId
                                             where m.SourceWordId == sourceWord.WordId &&
                                                   tw.LanguageId == targetLanguage.LanguageId
                                             select tw.Word1).FirstOrDefaultAsync();

                if (existingMeaning != null)
                    return Json(new { exists = true, meaning = existingMeaning });

                return Json(new { exists = false });
            }
            catch (Exception ex)
            {
                return Json(new { exists = false, error = ex.Message });
            }
        }

        [HttpPost("check-existing-meanings-batch")]
        public async Task<IActionResult> CheckExistingMeaningsBatch([FromBody] BatchMeaningRequest request)
        {
            try
            {
                var normalizedWord = request.Word.Trim().ToLower();
                var results = new Dictionary<string, object>();

                var sourceLanguage = await _context.Languages
                    .FirstOrDefaultAsync(l => l.Code.ToLower() == request.SourceLang.ToLower());
                if (sourceLanguage == null)
                    return Json(new { error = "Source language not found" });

                var sourceWord = await _context.Words
                    .FirstOrDefaultAsync(w => w.LanguageId == sourceLanguage.LanguageId &&
                                              w.Word1.ToLower() == normalizedWord);
                if (sourceWord == null)
                {
                    foreach (var targetLang in request.TargetLangs.Distinct())
                        results[targetLang] = new { exists = false };
                    return Json(results);
                }

                var distinctTargetLangs = request.TargetLangs.Distinct().ToList();

                // Get target languages by code (case-insensitive)
                var targetLanguages = await _context.Languages
                    .Where(l => distinctTargetLangs.Select(t => t.ToLower()).Contains(l.Code.ToLower()))
                    .ToDictionaryAsync(l => l.Code, l => l.LanguageId);

                var existingMeanings = await (from m in _context.Meanings
                                              join tw in _context.Words on m.TargetWordId equals tw.WordId
                                              where m.SourceWordId == sourceWord.WordId &&
                                                    targetLanguages.Values.Contains(tw.LanguageId)
                                              select new { tw.Word1, tw.LanguageId }).ToListAsync();

                var meaningDict = existingMeanings
                    .GroupBy(x => x.LanguageId)
                    .ToDictionary(g => g.Key, g => g.First().Word1);

                foreach (var targetLang in distinctTargetLangs)
                {
                    var langCodeLower = targetLang.ToLower();
                    var langEntry = targetLanguages.FirstOrDefault(x => x.Key.ToLower() == langCodeLower);
                    if (langEntry.Key != null && meaningDict.TryGetValue(langEntry.Value, out var meaning))
                        results[targetLang] = new { exists = true, meaning = meaning };
                    else
                        results[targetLang] = new { exists = false };
                }

                return Json(results);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public class BatchMeaningRequest
        {
            public string Word { get; set; }
            public string SourceLang { get; set; }
            public List<string> TargetLangs { get; set; }
        }

        [HttpPost("update-word-data")]
        public async Task<IActionResult> UpdateWordData([FromBody] WordDataRequest request)
        {
            try
            {
                // Find the language
                var language = await _context.Languages
                    .FirstOrDefaultAsync(l => l.Code == request.LanguageCode);

                if (language == null)
                {
                    return BadRequest(new { success = false, error = "Language not found" });
                }

                // Find existing word (case insensitive search)
                var existingWord = await _context.Words
                    .FirstOrDefaultAsync(w =>
                        w.LanguageId == language.LanguageId &&
                        w.Word1.ToLower() == request.CleanWord.ToLower());

                if (existingWord != null)
                {
                    // Update only if current values are null or empty
                    if (string.IsNullOrEmpty(existingWord.PartOfSpeech) && !string.IsNullOrEmpty(request.PartOfSpeech))
                        existingWord.PartOfSpeech = request.PartOfSpeech;

                    if (string.IsNullOrEmpty(existingWord.Definition) && !string.IsNullOrEmpty(request.Definition))
                        existingWord.Definition = request.Definition;

                    if (string.IsNullOrEmpty(existingWord.Description) && !string.IsNullOrEmpty(request.Description))
                        existingWord.Description = request.Description;

                    if (string.IsNullOrEmpty(existingWord.Synonyms) && !string.IsNullOrEmpty(request.Synonyms))
                        existingWord.Synonyms = request.Synonyms;

                    if (string.IsNullOrEmpty(existingWord.Antonyms) && !string.IsNullOrEmpty(request.Antonyms))
                        existingWord.Antonyms = request.Antonyms;

                    _context.Words.Update(existingWord);
                }
                else
                {
                    // Create new word
                    var newWord = new Word
                    {
                        LanguageId = language.LanguageId,
                        Word1 = request.Word,
                        BucketKey = language.Code.ToUpper() + "-" + request.Word[0].ToString().ToUpper(),
                        PartOfSpeech = request.PartOfSpeech,
                        Definition = request.Definition,
                        Description = request.Description,
                        Example = request.Example,
                        Synonyms = request.Synonyms,
                        Antonyms = request.Antonyms
                    };

                    await _context.Words.AddAsync(newWord);
                }

                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Word data updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }



        [HttpPost("update-bulk-word-data")]
        public async Task<IActionResult> UpdateBulkWordData([FromBody] BulkWordDataRequest request)
        {
            try
            {
                // Find the language
                var language = await _context.Languages
                    .FirstOrDefaultAsync(l => l.Code == request.LanguageCode);

                if (language == null)
                {
                    return BadRequest(new { success = false, error = "Language not found" });
                }

                var results = new List<object>();

                foreach (var wordData in request.Words)
                {
                    // Find existing word
                    var existingWord = await _context.Words
                        .FirstOrDefaultAsync(w =>
                            w.LanguageId == language.LanguageId &&
                            w.Word1.ToLower() == wordData.CleanWord.ToLower());

                    if (existingWord != null)
                    {
                        // Update only if current values are null or empty
                        bool wasUpdated = false;

                        if (string.IsNullOrEmpty(existingWord.PartOfSpeech) && !string.IsNullOrEmpty(wordData.PartOfSpeech))
                        {
                            existingWord.PartOfSpeech = wordData.PartOfSpeech;
                            wasUpdated = true;
                        }

                        if (string.IsNullOrEmpty(existingWord.Definition) && !string.IsNullOrEmpty(wordData.Definition))
                        {
                            existingWord.Definition = wordData.Definition;
                            wasUpdated = true;
                        }

                        if (string.IsNullOrEmpty(existingWord.Description) && !string.IsNullOrEmpty(wordData.Description))
                        {
                            existingWord.Description = wordData.Description;
                            wasUpdated = true;
                        }

                        if (string.IsNullOrEmpty(existingWord.Synonyms) && !string.IsNullOrEmpty(wordData.Synonyms))
                        {
                            existingWord.Synonyms = wordData.Synonyms;
                            wasUpdated = true;
                        }

                        if (string.IsNullOrEmpty(existingWord.Antonyms) && !string.IsNullOrEmpty(wordData.Antonyms))
                        {
                            existingWord.Antonyms = wordData.Antonyms;
                            wasUpdated = true;
                        }

                        if (wasUpdated)
                        {
                            _context.Words.Update(existingWord);
                            results.Add(new { word = wordData.CleanWord, action = "updated" });
                        }
                        else
                        {
                            results.Add(new { word = wordData.CleanWord, action = "no_changes" });
                        }
                    }
                    else
                    {
                        // Create new word
                        var newWord = new Word
                        {
                            LanguageId = language.LanguageId,
                            Word1 = wordData.Word,
                            BucketKey = wordData.CleanWord.ToLower(),
                            PartOfSpeech = wordData.PartOfSpeech,
                            Definition = wordData.Definition,
                            Description = wordData.Description,
                            Example = wordData.Example,
                            Synonyms = wordData.Synonyms,
                            Antonyms = wordData.Antonyms
                        };

                        await _context.Words.AddAsync(newWord);
                        results.Add(new { word = wordData.CleanWord, action = "created" });
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new { success = true, results = results });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }


        [HttpPost("save-word-with-meaning")]
        public async Task<IActionResult> SaveWordWithMeaning([FromBody] SaveWordRequest request)
        {
            try
            {
                // Case-insensitive language lookup by Code
                var sourceLanguage = await _context.Languages
                    .FirstOrDefaultAsync(l => l.Code.ToLower() == request.SourceLanguageCode.ToLower());
                if (sourceLanguage == null)
                {
                    Console.WriteLine($"Source language code '{request.SourceLanguageCode}' not found.");
                    return Json(new { success = false, message = "Source language not found" });
                }

                var targetLanguage = await _context.Languages
                    .FirstOrDefaultAsync(l => l.Code.ToLower() == request.TargetLanguageCode.ToLower());
                if (targetLanguage == null)
                {
                    Console.WriteLine($"Target language code '{request.TargetLanguageCode}' not found.");
                    return Json(new { success = false, message = "Target language not found" });
                }

                // Use the execution strategy to handle retries
                var strategy = _context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    // Begin a transaction inside the execution strategy
                    using var transaction = await _context.Database.BeginTransactionAsync();

                    // Case-insensitive word lookup for source word
                    var sourceWord = await _context.Words
                        .FirstOrDefaultAsync(w => w.LanguageId == sourceLanguage.LanguageId &&
                                                  w.Word1.ToLower() == request.SourceWord.ToLower());
                    if (sourceWord == null)
                    {
                        sourceWord = new Word
                        {
                            Word1 = request.SourceWord,
                            LanguageId = sourceLanguage.LanguageId,
                            BucketKey = GenerateBucketKey(request.SourceWord, request.SourceLanguageCode),
                            SearchCount = 1
                        };
                        _context.Words.Add(sourceWord);
                        await _context.SaveChangesAsync(); // Save to get WordId
                    }
                    else
                    {
                        sourceWord.SearchCount++;
                        _context.Words.Update(sourceWord);
                    }

                    // Case-insensitive word lookup for target word
                    var targetWord = await _context.Words
                        .FirstOrDefaultAsync(w => w.LanguageId == targetLanguage.LanguageId &&
                                                  w.Word1.ToLower() == request.TargetWord.ToLower());
                    if (targetWord == null)
                    {
                        targetWord = new Word
                        {
                            Word1 = request.TargetWord,
                            LanguageId = targetLanguage.LanguageId,
                            BucketKey = GenerateBucketKey(request.TargetWord, request.TargetLanguageCode),
                            SearchCount = 0
                        };
                        _context.Words.Add(targetWord);
                        await _context.SaveChangesAsync(); // Save to get WordId
                    }

                    // Check if meaning already exists
                    var existingMeaning = await _context.Meanings
                        .FirstOrDefaultAsync(m => m.SourceWordId == sourceWord.WordId && m.TargetWordId == targetWord.WordId);
                    if (existingMeaning == null)
                    {
                        var meaning = new DictionaryModels.Meaning
                        {
                            SourceWordId = sourceWord.WordId,
                            TargetWordId = targetWord.WordId
                        };
                        _context.Meanings.Add(meaning);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                });

                return Json(new { success = true, message = "Word and meaning saved successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SaveWordWithMeaning: {ex}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        private string GenerateBucketKey(string word, string languageCode)
        {
            if (string.IsNullOrEmpty(word) || string.IsNullOrEmpty(languageCode))
                return "default";

            // Extract language part (first 2 characters in uppercase)
            string langPart = languageCode.Length >= 2 ?
                languageCode.Substring(0, 2).ToUpper() :
                languageCode.ToUpper();

            // Extract first letter of the word (in uppercase)
            string wordPart = word.Length > 0 ?
                word.Substring(0, 1).ToUpper() :
                "A";

            return $"{langPart}-{wordPart}";
        }

        // Request model
        public class SaveWordRequest
        {
            public string SourceWord { get; set; }
            public string SourceLanguageCode { get; set; }
            public string TargetWord { get; set; }
            public string TargetLanguageCode { get; set; }
        }



        public DictionaryPage getLanguagesFromName(string? lang1, string? lang2)
        {
            var model = new DictionaryPage();
            if (lang1 != null)
            {
                model.lang1 = _context.Languages.Where(l => l.Name.ToLower() == lang1).FirstOrDefault();

            }
            if (lang2 != null)
            {
                model.lang2 = _context.Languages.Where(l => l.Name.ToLower() == lang2).FirstOrDefault();

            }
            else if (model.lang2 == null)
            {
                model.lang2 = new Language { Code = "en", Name = "English", LanguageId = 22 };
            }

            return model;
        }



        public static class LanguageData
        {
            public static readonly List<Language> Languages = new List<Language>
    {
        new Language { Code = "af", Name = "Afrikaans" },
        new Language { Code = "sq", Name = "Albanian" },
        new Language { Code = "am", Name = "Amharic" },
        new Language { Code = "ar", Name = "Arabic" },
        new Language { Code = "hy", Name = "Armenian" },
        new Language { Code = "az", Name = "Azerbaijani" },
        new Language { Code = "eu", Name = "Basque" },
        new Language { Code = "be", Name = "Belarusian" },
        new Language { Code = "bn", Name = "Bengali" },
        new Language { Code = "bs", Name = "Bosnian" },
        new Language { Code = "bg", Name = "Bulgarian" },
        new Language { Code = "ca", Name = "Catalan" },
        new Language { Code = "ceb", Name = "Cebuano" },
        new Language { Code = "zh", Name = "Chinese (Simplified)" },
        new Language { Code = "zh-cn", Name = "Chinese (China)" },
        new Language { Code = "zh-tw", Name = "Chinese (Taiwan)" },
        new Language { Code = "hr", Name = "Croatian" },
        new Language { Code = "cs", Name = "Czech" },
        new Language { Code = "da", Name = "Danish" },
        new Language { Code = "nl", Name = "Dutch" },
        new Language { Code = "en", Name = "English" },
        new Language { Code = "en-gb", Name = "English (UK)" },
        new Language { Code = "en-us", Name = "English (US)" },
        new Language { Code = "eo", Name = "Esperanto" },
        new Language { Code = "et", Name = "Estonian" },
        new Language { Code = "fi", Name = "Finnish" },
        new Language { Code = "fr", Name = "French" },
        new Language { Code = "gl", Name = "Galician" },
        new Language { Code = "ka", Name = "Georgian" },
        new Language { Code = "de", Name = "German" },
        new Language { Code = "el", Name = "Greek" },
        new Language { Code = "gu", Name = "Gujarati" },
        new Language { Code = "ht", Name = "Haitian Creole" },
        new Language { Code = "ha", Name = "Hausa" },
        new Language { Code = "he", Name = "Hebrew" },
        new Language { Code = "hi", Name = "Hindi" },
        new Language { Code = "hu", Name = "Hungarian" },
        new Language { Code = "is", Name = "Icelandic" },
        new Language { Code = "id", Name = "Indonesian" },
        new Language { Code = "ga", Name = "Irish" },
        new Language { Code = "it", Name = "Italian" },
        new Language { Code = "ja", Name = "Japanese" },
        new Language { Code = "jv", Name = "Javanese" },
        new Language { Code = "kn", Name = "Kannada" },
        new Language { Code = "kk", Name = "Kazakh" },
        new Language { Code = "km", Name = "Khmer" },
        new Language { Code = "ko", Name = "Korean" },
        new Language { Code = "ku", Name = "Kurdish" },
        new Language { Code = "ky", Name = "Kyrgyz" },
        new Language { Code = "lo", Name = "Lao" },
        new Language { Code = "la", Name = "Latin" },
        new Language { Code = "lv", Name = "Latvian" },
        new Language { Code = "lt", Name = "Lithuanian" },
        new Language { Code = "mk", Name = "Macedonian" },
        new Language { Code = "mg", Name = "Malagasy" },
        new Language { Code = "ms", Name = "Malay" },
        new Language { Code = "ml", Name = "Malayalam" },
        new Language { Code = "mt", Name = "Maltese" },
        new Language { Code = "mi", Name = "Maori" },
        new Language { Code = "mr", Name = "Marathi" },
        new Language { Code = "mn", Name = "Mongolian" },
        new Language { Code = "my", Name = "Burmese" },
        new Language { Code = "ne", Name = "Nepali" },
        new Language { Code = "no", Name = "Norwegian" },
        new Language { Code = "fa", Name = "Persian (Farsi)" },
        new Language { Code = "pl", Name = "Polish" },
        new Language { Code = "pt", Name = "Portuguese" },
        new Language { Code = "pt-br", Name = "Portuguese (Brazil)" },
        new Language { Code = "pa", Name = "Punjabi" },
        new Language { Code = "ro", Name = "Romanian" },
        new Language { Code = "ru", Name = "Russian" },
        new Language { Code = "sr", Name = "Serbian" },
        new Language { Code = "si", Name = "Sinhala" },
        new Language { Code = "sk", Name = "Slovak" },
        new Language { Code = "sl", Name = "Slovenian" },
        new Language { Code = "so", Name = "Somali" },
        new Language { Code = "es", Name = "Spanish" },
        new Language { Code = "su", Name = "Sundanese" },
        new Language { Code = "sw", Name = "Swahili" },
        new Language { Code = "sv", Name = "Swedish" },
        new Language { Code = "tl", Name = "Tagalog" },
        new Language { Code = "tg", Name = "Tajik" },
        new Language { Code = "ta", Name = "Tamil" },
        new Language { Code = "te", Name = "Telugu" },
        new Language { Code = "th", Name = "Thai" },
        new Language { Code = "tr", Name = "Turkish" },
        new Language { Code = "uk", Name = "Ukrainian" },
        new Language { Code = "ur", Name = "Urdu" },
        new Language { Code = "ur-pk", Name = "Urdu (Pakistan)" },
        new Language { Code = "uz", Name = "Uzbek" },
        new Language { Code = "vi", Name = "Vietnamese" },
        new Language { Code = "cy", Name = "Welsh" },
        new Language { Code = "xh", Name = "Xhosa" },
        new Language { Code = "yi", Name = "Yiddish" },
        new Language { Code = "yo", Name = "Yoruba" },
        new Language { Code = "zu", Name = "Zulu" }
    };
        }

        // Request models
        public class WordDataRequest
        {
            public string Word { get; set; }
            public string CleanWord { get; set; }
            public string PartOfSpeech { get; set; }
            public string Definition { get; set; }
            public string Description { get; set; }
            public string Example { get; set; }
            public string Synonyms { get; set; }
            public string Antonyms { get; set; }
            public string LanguageCode { get; set; }
        }

        public class BulkWordDataRequest
        {
            public List<WordDataRequest> Words { get; set; }
            public string LanguageCode { get; set; }
        }


    }
}