using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class User
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public string? Email { get; set; }

    public string? PasswordHash { get; set; }

    public virtual ICollection<FriendRequest> FriendRequestReceivers { get; set; } = new List<FriendRequest>();

    public virtual ICollection<FriendRequest> FriendRequestSenders { get; set; } = new List<FriendRequest>();

    public virtual ICollection<PrivateMessage> PrivateMessageReceivers { get; set; } = new List<PrivateMessage>();

    public virtual ICollection<PrivateMessage> PrivateMessageSenders { get; set; } = new List<PrivateMessage>();
}
