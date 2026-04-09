using System;
using System.Collections.Generic;

namespace IKDFrontEnd.DBCollege;

public partial class TblCollegereview
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? Cid { get; set; }

    public string? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? Date { get; set; }

    public int? InstId { get; set; }

    public string? Email { get; set; }

    public byte? Afford { get; set; }

    public byte? Academics { get; set; }

    public byte? JobPlacement { get; set; }

    public byte? Facilities { get; set; }

    public bool? ReAdmission { get; set; }
}
