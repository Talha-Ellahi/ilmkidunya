using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class CompanyFollow
{
    public int Id { get; set; }

    public int? CompanyId { get; set; }

    public int? MemberId { get; set; }
}
