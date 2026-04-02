using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblSchTop
{
    public int Id { get; set; }

    public string TopSchName { get; set; } = null!;

    public string? Url { get; set; }

    public bool? IsActive { get; set; }

    public int? SortOrder { get; set; }

    public int? CountryId { get; set; }

    public string? Image { get; set; }

    public int? UserId { get; set; }

    public string? Keyword { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Country? Country { get; set; }

    public virtual ICollection<TblSchTopLinking> TblSchTopLinkings { get; set; } = new List<TblSchTopLinking>();
}
