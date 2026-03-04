using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblPastPaperType
{
    public int Id { get; set; }

    public int? PastPaperId { get; set; }

    public int? PastPaperTypeId { get; set; }
}
