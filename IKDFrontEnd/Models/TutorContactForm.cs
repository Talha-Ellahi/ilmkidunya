using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TutorContactForm
{
    public int Id { get; set; }

    public int MemberId { get; set; }

    public string Name { get; set; } = null!;

    public string Contact { get; set; } = null!;

    public string Subjects { get; set; } = null!;

    public DateTime Dated { get; set; }
}
