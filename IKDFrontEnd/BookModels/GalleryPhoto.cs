using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class GalleryPhoto
{
    public int Id { get; set; }

    public int GalleryId { get; set; }

    public string? Image { get; set; }

    public virtual Gallery Gallery { get; set; } = null!;
}
