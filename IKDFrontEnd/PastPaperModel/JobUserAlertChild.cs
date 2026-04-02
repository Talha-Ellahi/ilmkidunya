using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class JobUserAlertChild
{
    public long Id { get; set; }

    public int JobUserAlertId { get; set; }

    public int JobId { get; set; }
}
