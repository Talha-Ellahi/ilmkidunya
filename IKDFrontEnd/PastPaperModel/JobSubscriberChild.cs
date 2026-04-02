using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class JobSubscriberChild
{
    public long Id { get; set; }

    public long JobSubsCriberId { get; set; }

    public int JobTypeId { get; set; }
}
