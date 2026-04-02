using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblTutorFavourite
{
    public int FavId { get; set; }

    public int? MemberId { get; set; }

    public int? TutorId { get; set; }
}
