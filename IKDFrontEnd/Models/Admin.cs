using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class Admin
{
    public int Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? LoginId { get; set; }

    public string? Password { get; set; }

    public bool? IsSuper { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsLogin { get; set; }

    public string? MacAddress { get; set; }

    public DateTime? Dated { get; set; }

    public int? UpdatedBy { get; set; }

    public string? Pictures { get; set; }
}
