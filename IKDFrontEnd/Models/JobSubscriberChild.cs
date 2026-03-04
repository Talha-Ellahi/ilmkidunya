using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class JobSubscriberChild
{
    public long Id { get; set; }

    public long JobSubsCriberId { get; set; }

    public int JobTypeId { get; set; }
}
