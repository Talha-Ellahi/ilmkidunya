using System;
using System.Collections.Generic;

namespace IKDFrontEnd.Models.PastPaperModel;

public partial class TblTutorMessage
{
    public int Id { get; set; }

    public int ConversationId { get; set; }

    public int SenderId { get; set; }

    public int ReceiverId { get; set; }

    public string? Message { get; set; }

    public int? FileType { get; set; }

    public string? AttachmentThumb { get; set; }

    public DateTime? CreatedAt { get; set; }
}
