using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblOtsmcsq
{
    public int Id { get; set; }

    public int OtsclassId { get; set; }

    public int OtssubjectId { get; set; }

    public int OtschId { get; set; }

    public DateTime Date { get; set; }

    public int UserId { get; set; }

    public string? Url { get; set; }

    public string? TestName { get; set; }
}
