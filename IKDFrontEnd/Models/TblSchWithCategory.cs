using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class TblSchWithCategory
{
    public int Id { get; set; }

    public int ScholarshipId { get; set; }

    public int CategoryId { get; set; }
}
