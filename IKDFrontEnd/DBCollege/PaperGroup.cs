using System;
using System.Collections.Generic;

namespace IKDFrontEnd.DBCollege;

public partial class PaperGroup
{
    public byte PaperGroupId { get; set; }

    public string PaperGroupName { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<BoardOtsmcq> BoardOtsmcqs { get; set; } = new List<BoardOtsmcq>();
}
