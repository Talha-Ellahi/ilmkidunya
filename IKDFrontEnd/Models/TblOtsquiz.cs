using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblOtsquiz
{
    public int Id { get; set; }

    public int? OtsquizCatId { get; set; }

    public string? QuizName { get; set; }

    public string? Url { get; set; }

    public int? TotalQuestions { get; set; }

    public int? MarksPerQuestion { get; set; }

    public int? TimeAllowed { get; set; }

    public int? SortOrder { get; set; }

    public bool? IsActive { get; set; }

    public string? Image { get; set; }

    public DateTime? Date { get; set; }

    public int? UserId { get; set; }
}
