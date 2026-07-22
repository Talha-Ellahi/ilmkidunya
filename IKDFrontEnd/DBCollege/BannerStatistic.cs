using System;
using System.Collections.Generic;

namespace IKDFrontEnd.DBCollege;

public partial class BannerStatistic
{
    public int BannerStatId { get; set; }

    public int AdvertisId { get; set; }

    public DateOnly Date { get; set; }

    public int ImpressionCountD { get; set; }

    public int ImpressionCountM { get; set; }

    public int ClickCountD { get; set; }

    public int ClickCountM { get; set; }
}
