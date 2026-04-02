using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class JobCustomFile
{
    public int Id { get; set; }

    public long JobId { get; set; }

    public string Name { get; set; } = null!;

    public string FileName { get; set; } = null!;
}
