using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class Gallery
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Detail { get; set; }

    public string? Url { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaDesc { get; set; }

    public string? MetaKeyword { get; set; }

    public virtual ICollection<GalleryPhoto> GalleryPhotos { get; set; } = new List<GalleryPhoto>();
}
