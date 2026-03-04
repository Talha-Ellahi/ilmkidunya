using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TutorDocument
{
    public int Id { get; set; }

    public int MemberId { get; set; }

    public string Name { get; set; } = null!;

    public string ShowName { get; set; } = null!;

    public byte Sortorder { get; set; }

    public bool IsActive { get; set; }

    public DateTime? Dated { get; set; }
}
