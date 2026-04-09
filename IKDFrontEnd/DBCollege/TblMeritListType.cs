using System;
using System.Collections.Generic;

namespace IKDFrontEnd.DBCollege;

public partial class TblMeritListType
{
    public int Id { get; set; }

    public string MeritListTypeName { get; set; } = null!;

    public int SortOrder { get; set; }
}
