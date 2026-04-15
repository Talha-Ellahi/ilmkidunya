using System;
using System.Collections.Generic;

namespace IKDFrontEnd.PastPaperModels;

public partial class TblPastPaper
{
    public int Id { get; set; }

    public string Pnname { get; set; } = null!;

    public int Year { get; set; }

    public int BoardId { get; set; }

    public int PpclassId { get; set; }

    public int PpsubjectId { get; set; }

    public string? QuestionType { get; set; }

    public string? Language { get; set; }

    public string? Description { get; set; }

    public string? Pdf { get; set; }

    public string? Image { get; set; }

    public int? UserId { get; set; }

    public DateTime? Date { get; set; }

    public bool? IsDelete { get; set; }

    public virtual Board Board { get; set; } = null!;

    public virtual TblPpclass Ppclass { get; set; } = null!;

    public virtual TblPpsubject Ppsubject { get; set; } = null!;
}
