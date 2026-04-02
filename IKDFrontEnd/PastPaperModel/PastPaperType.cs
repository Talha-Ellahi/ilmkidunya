using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class PastPaperType
{
    public byte Id { get; set; }

    public string Name { get; set; } = null!;

    public int? SortOrder { get; set; }
}
