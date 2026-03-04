using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblSchFieldsofStudy
{
    public int Id { get; set; }

    public int SchId { get; set; }

    public int SchFieldofStudyId { get; set; }

    public virtual TblSch Sch { get; set; } = null!;

    public virtual TblSchFieldofStudy SchFieldofStudy { get; set; } = null!;
}
