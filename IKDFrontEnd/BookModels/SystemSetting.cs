using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class SystemSetting
{
    public int Id { get; set; }

    public string? SenderEmail { get; set; }

    public string? SenderPassword { get; set; }

    public string? Smtphost { get; set; }

    public string? Port { get; set; }

    public bool IsSsl { get; set; }

    public string? Email { get; set; }

    public string? Ftpip { get; set; }

    public string? FtpName { get; set; }

    public string? FtpPassword { get; set; }

    public string? Ftppath { get; set; }

    public string? SystemImage { get; set; }
}
