using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class UrlbasedCmspage
{
    public int Id { get; set; }

    public string? PageName { get; set; }

    public string? Heading { get; set; }

    public string? Pageimage { get; set; }

    public string? Url { get; set; }

    public string? Description { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaKeyword { get; set; }

    public string? MetaDescription { get; set; }

    public DateTime? Date { get; set; }

    public int UserId { get; set; }

    public string? IsGuide { get; set; }

    public virtual User User { get; set; } = null!;
}
