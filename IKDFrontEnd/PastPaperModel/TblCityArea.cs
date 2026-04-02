using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblCityArea
{
    public int Id { get; set; }

    public string? CityArea { get; set; }

    public int CityId { get; set; }
}
