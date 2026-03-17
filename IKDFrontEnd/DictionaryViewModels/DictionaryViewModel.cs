using IKDFrontEnd.DictionaryModels;

namespace IKDFrontEnd.DictionaryViewModels
{


    public class TranslationResponse
    {
        public ResponseData responseData { get; set; }
    }

    public class ResponseData
    {
        public string translatedText { get; set; }
        public double match { get; set; }
    }


    public class DictionaryApiResponse
    {
        public string word { get; set; }
        public List<Phonetic> phonetics { get; set; }
        public List<Meaning> meanings { get; set; }
        public List<string> sourceUrls { get; set; }
    }

    public class Phonetic
    {
        public string text { get; set; }
        public string audio { get; set; }
    }

    public class Meaning
    {
        public string partOfSpeech { get; set; }
        public List<Definition> definitions { get; set; }
        public List<string> synonyms { get; set; }
        public List<string> antonyms { get; set; }
    }

    public class Definition
    {
        public string definition { get; set; }
        public string example { get; set; }
        public List<string> synonyms { get; set; }
        public List<string> antonyms { get; set; }
    }

    public class WordDetails
    {
        public string Audio { get; set; }
        public Dictionary<string, List<string>> Definitions { get; set; } = new();
        public List<string> Synonyms { get; set; } = new();
        public List<string> Antonyms { get; set; } = new();
        public string Description { get; set; }
    }



    public class DictionaryViewModel
    {
        // 🌍 Main word info
        public string Word { get; set; }
        public string Pronunciation { get; set; }   // e.g. [kom-pli-key-shuh n]
        public string AudioUrl { get; set; }        // speaker icon sound

        // 📖 Translations (Urdu/English/Hindi etc.)
        public string TranslatedText { get; set; }
        public string CurrentLanguage { get; set; }
        public string TranslatedLanguage { get; set; }

        // 📚 Definitions grouped by Part of Speech
        public Dictionary<string, List<string>> Definitions { get; set; } = new();

        // 📝 Extra Description
        public string Description { get; set; }

        // ✅ Synonyms & Antonyms
        public List<string> Synonyms { get; set; } = new();
        public List<string> Antonyms { get; set; } = new();

        // 🔗 Source (dictionary / wiki link)
        public List<string> SourceUrls { get; set; } = new();

        // 📢 For showing dropdown languages (instead of hardcoding in HTML)
        public List<LanguageOption> AvailableLanguages { get; set; } = new();
        public string? EnglishWord { get; internal set; }
        public string CompleteLanguage { get; internal set; }
        public string TranslatedLanguageCode { get; internal set; }
        public string CompleteCurrentLanguage { get; internal set; }
        public string? ExistingMeaning { get; internal set; }
        public string? ExistingDefinition { get; internal set; }
        public string? ExistingSynonyms { get; internal set; }
        public string? ExistingAntonyms { get; internal set; }
        public string? ExistingDescription { get; internal set; }
        public string? ExistingPartOfSpeech { get; internal set; }

        public bool HasExistingData { get; set; }
    }

    public class LanguageOption
    {
        public string Code { get; set; }   // e.g. "en-gb"
        public string Display { get; set; } // e.g. "English"
        public string name { get; internal set; }
    }


    public class WordPair
    {
        public int SrNo { get; set; }
        public string English { get; set; }
        public string Urdu { get; set; }
    }


    public class DictionaryPage
    {
        public Language lang1 { get; set; }
        public Language lang2 { get; set; }

        public List<Word> words { get; set; }
    }

}
