using System;
using System.Collections.Generic;

namespace IKDFrontEnd.PastPaperModels;

public partial class PastPaperQuestionType
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public bool? IsActive { get; set; }
}
