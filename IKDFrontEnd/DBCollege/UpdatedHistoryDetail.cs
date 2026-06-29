using System;
using System.Collections.Generic;

namespace IKDFrontEnd.DBCollege;

public partial class UpdatedHistoryDetail
{
    public int UpdatedHistoryDetailId { get; set; }

    public int UpdatedHistoryId { get; set; }

    public string WordCount { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public virtual UpdatedHistory UpdatedHistory { get; set; } = null!;
}
