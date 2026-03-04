using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class TblMember
{
    public int MemberId { get; set; }

    public string? MemberName { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public int? MemberTypeId { get; set; }

    public int? RegisterType { get; set; }

    public string? FacebookId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? Photo { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public byte[] TimeStamp { get; set; } = null!;

    public string? PhoneNo { get; set; }

    public bool? Gender { get; set; }

    public string? Img { get; set; }

    public int? CityId { get; set; }

    public byte? EmirateId { get; set; }

    public string? Area { get; set; }

    public string? Address { get; set; }

    public string? RecoverPassword { get; set; }

    public virtual TblCity? City { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
