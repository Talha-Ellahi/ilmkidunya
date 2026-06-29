using System;
using System.Collections.Generic;

namespace IKDFrontEnd.DBCollege;

public partial class UpdatedHistory
{
    public int UpdatedHistoryId { get; set; }

    public short HistoryListId { get; set; }

    public int UserId { get; set; }

    public string Url { get; set; } = null!;

    public string Heading { get; set; } = null!;

    public DateTime UpdatedDate { get; set; }

    public byte Count { get; set; }

    public virtual HistoryList HistoryList { get; set; } = null!;

    public virtual ICollection<UpdatedHistoryDetail> UpdatedHistoryDetails { get; set; } = new List<UpdatedHistoryDetail>();
}
