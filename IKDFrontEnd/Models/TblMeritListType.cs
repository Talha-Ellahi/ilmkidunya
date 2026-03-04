using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblMeritListType
{
    public int Id { get; set; }

    public string MeritListTypeName { get; set; } = null!;

    public int SortOrder { get; set; }
}
