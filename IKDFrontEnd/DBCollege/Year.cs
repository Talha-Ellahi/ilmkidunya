using System;
using System.Collections.Generic;

namespace IKDFrontEnd.DBCollege;

public partial class Year
{
    public int YearId { get; set; }

    public string YearName { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<BoardOtsmcq> BoardOtsmcqs { get; set; } = new List<BoardOtsmcq>();
}
