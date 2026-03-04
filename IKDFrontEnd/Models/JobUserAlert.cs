using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class JobUserAlert
{
    public long Id { get; set; }

    public long MemberId { get; set; }

    public int MaxjobId { get; set; }

    public DateOnly LastAlertDate { get; set; }
}
