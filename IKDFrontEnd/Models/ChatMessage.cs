using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models;

public partial class ChatMessage
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public string Message { get; set; } = null!;

    public DateTime SentAt { get; set; }
}
