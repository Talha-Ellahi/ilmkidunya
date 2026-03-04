using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class TblTeamMember
{
    public int Id { get; set; }

    public string? FullName { get; set; }

    public string? Designation { get; set; }

    public string? FacbookLink { get; set; }

    public string? Pinterest { get; set; }

    public string? TwitterLink { get; set; }

    public string? InstagramLink { get; set; }

    public string? ShortDescription { get; set; }

    public string? ImageName { get; set; }
}
