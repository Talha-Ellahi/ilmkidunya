using System;
using System.Collections.Generic;

namespace IKDFrontEnd.DBCollege;

public partial class CreatedHistory
{
    public int CreatedHistoryId { get; set; }

    public short HistoryListId { get; set; }

    public int UserId { get; set; }

    public string Url { get; set; } = null!;

    public string Heading { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string? WordCount { get; set; }

    public virtual HistoryList HistoryList { get; set; } = null!;
}
