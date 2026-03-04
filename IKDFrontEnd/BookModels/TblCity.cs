using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class TblCity
{
    public int Id { get; set; }

    public string? CityName { get; set; }

    public int? CountryId { get; set; }

    public bool IsActive { get; set; }

    public bool IsDelete { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public byte[] Timestamp { get; set; } = null!;

    public virtual ICollection<TblMember> TblMembers { get; set; } = new List<TblMember>();
}
