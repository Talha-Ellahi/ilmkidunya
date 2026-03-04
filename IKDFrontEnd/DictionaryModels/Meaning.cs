using System;
using System.Collections.Generic;

namespace IKDFrontEnd.DictionaryModels;

public partial class Meaning
{
    public long MeaningId { get; set; }

    public long SourceWordId { get; set; }

    public long TargetWordId { get; set; }
}
