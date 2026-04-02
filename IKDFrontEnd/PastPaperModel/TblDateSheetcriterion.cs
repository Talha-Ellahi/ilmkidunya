using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblDateSheetcriterion
{
    public int Id { get; set; }

    public int LevelId { get; set; }

    public int ClassId { get; set; }

    public int BoardId { get; set; }

    public DateTime? Dated { get; set; }

    public string? Heading { get; set; }

    public bool? IsActive { get; set; }

    public short? UpdatedBy { get; set; }

    public string? DatesheetCode { get; set; }
}
