using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class JobsLastTwoDaysEmailAlert
{
    public int Id { get; set; }

    public long JobId { get; set; }

    public int? CompanyId { get; set; }

    public bool? IsEmailSent { get; set; }
}
