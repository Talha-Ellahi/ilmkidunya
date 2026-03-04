using System;
using System.Collections.Generic;

namespace IKDFrontEnd.DictionaryModels;

public partial class Language
{
    public int LanguageId { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<Word> Words { get; set; } = new List<Word>();
}
