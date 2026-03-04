using System;
using System.Collections.Generic;

namespace IKDFrontEnd.DictionaryModels;

public partial class Word
{
    public long WordId { get; set; }

    public int LanguageId { get; set; }

    public string Word1 { get; set; } = null!;

    public string BucketKey { get; set; } = null!;

    public string? PartOfSpeech { get; set; }

    public string? Definition { get; set; }

    public string? Description { get; set; }

    public string? Example { get; set; }

    public string? Synonyms { get; set; }

    public string? Antonyms { get; set; }

    public int SearchCount { get; set; }

    public virtual Language Language { get; set; } = null!;
}
