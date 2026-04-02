using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class City1
{
    public int CityId { get; set; }

    public string CityName { get; set; } = null!;

    public int CountryId { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public bool IsFavourite { get; set; }
}
