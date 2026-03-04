using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblSchTopLinking
{
    public int Id { get; set; }

    public int SchId { get; set; }

    public int TopSchId { get; set; }

    public virtual TblSch Sch { get; set; } = null!;

    public virtual TblSchTop TopSch { get; set; } = null!;
}
