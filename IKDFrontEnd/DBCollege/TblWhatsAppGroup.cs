using System;
using System.Collections.Generic;

namespace IKDFrontEnd.DBCollege;

public partial class TblWhatsAppGroup
{
    public int Id { get; set; }

    public string GuideName { get; set; } = null!;

    public string GroupLink { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }
}
