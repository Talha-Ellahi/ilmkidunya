using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class TblSlider
{
    public int Id { get; set; }

    public DateTime? CreatedDate { get; set; }

    public byte? SortOrder { get; set; }

    public string? Title { get; set; }

    public string? SubHeading { get; set; }

    public string? SubHeadingTwo { get; set; }

    public string? Link { get; set; }

    public string? Image { get; set; }

    public bool IsActive { get; set; }

    public bool IsExclusive { get; set; }

    public string? Description { get; set; }
}
