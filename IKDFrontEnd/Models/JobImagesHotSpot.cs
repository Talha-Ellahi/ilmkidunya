using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class JobImagesHotSpot
{
    public int Id { get; set; }

    public int? JobId { get; set; }

    public int? NoOfVacancies { get; set; }

    public DateTime? PostedDate { get; set; }

    public DateTime? LastDate { get; set; }

    public int? X { get; set; }

    public int? Y { get; set; }

    public int? Z { get; set; }

    public int? Width { get; set; }

    public int? Height { get; set; }

    public int? AreaId { get; set; }

    public int? X1 { get; set; }

    public int? Y1 { get; set; }

    public int? CityId { get; set; }
}
