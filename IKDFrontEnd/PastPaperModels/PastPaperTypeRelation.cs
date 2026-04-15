using System;
using System.Collections.Generic;

namespace IKDFrontEnd.PastPaperModels;

public partial class PastPaperTypeRelation
{
    public int Id { get; set; }

    public int PastPaperId { get; set; }

    public byte PastPaperTypeId { get; set; }
}
