using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class DateSheetResultType
{
    public byte Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsDateSheet { get; set; }
}
