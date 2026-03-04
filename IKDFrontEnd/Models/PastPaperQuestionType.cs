using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class PastPaperQuestionType
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public bool? IsActive { get; set; }
}
