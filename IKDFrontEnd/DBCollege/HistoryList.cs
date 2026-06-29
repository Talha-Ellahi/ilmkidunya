using System;
using System.Collections.Generic;

namespace IKDFrontEnd.DBCollege;

public partial class HistoryList
{
    public short HistoryListId { get; set; }

    public string HistoryListName { get; set; } = null!;

    public virtual ICollection<CreatedHistory> CreatedHistories { get; set; } = new List<CreatedHistory>();

    public virtual ICollection<UpdatedHistory> UpdatedHistories { get; set; } = new List<UpdatedHistory>();
}
