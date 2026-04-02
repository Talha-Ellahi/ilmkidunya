using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class PastPaperTypeRelation
{
    public int Id { get; set; }

    public int PastPaperId { get; set; }

    public byte PastPaperTypeId { get; set; }
}
