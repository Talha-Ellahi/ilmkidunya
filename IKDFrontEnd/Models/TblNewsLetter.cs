using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblNewsLetter
{
    public decimal NlId { get; set; }

    public string? NlActive { get; set; }

    public string? NlEmail { get; set; }

    public DateTime Dated { get; set; }
}
