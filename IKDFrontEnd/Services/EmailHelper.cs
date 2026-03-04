using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Configuration;

public static class EmailHelper
{
    private static readonly IConfiguration _config;

    static EmailHelper()
    {
        // You can inject IConfiguration in Startup and assign it here if needed.
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json");
        _config = builder.Build();
    }

    public static async Task SendAsync(string toEmail, string subject, string bodyHtml, string pageName, string purpose, List<string>? cc = null)
    {
        string fromEmail = _config["EmailSettings:FromEmail"];
        string displayName = _config["EmailSettings:DisplayName"];
        string smtpHost = _config["EmailSettings:SmtpHost"];
        int smtpPort = int.Parse(_config["EmailSettings:SmtpPort"]);
        string smtpUser = _config["EmailSettings:SmtpUser"];
        string smtpPass = _config["EmailSettings:SmtpPass"];

        using var client = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUser, smtpPass),
            EnableSsl = true
        };

        var mail = new MailMessage
        {
            From = new MailAddress(fromEmail, displayName),
            Subject = subject,
            Body = bodyHtml,
            IsBodyHtml = true,
            BodyEncoding = Encoding.UTF8
        };

        mail.To.Add(toEmail);

        // ✅ Add CCs if available
        if (cc != null)
        {
            foreach (var ccEmail in cc)
            {
                mail.CC.Add(ccEmail);
            }
        }

        await client.SendMailAsync(mail);
    }

    public static async Task SendMassEmailAsync(IEnumerable<string> toEmails, string subject, string bodyHtml)
    {
        foreach (var email in toEmails)
        {
            await SendAsync(email, subject, bodyHtml, "BulkCampaign", "TutorSearchAlert");
        }
    }

    public static string LoadTemplate(string virtualPath)
    {
        string physicalPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            virtualPath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
        );
        return File.Exists(physicalPath) ? File.ReadAllText(physicalPath) : "";
    }

    // ✅ Overloaded method: LoadTemplate("filename.html", "/Html/folder/")
    public static string LoadTemplate(string fileName, string virtualFolder)
    {
        if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(virtualFolder))
            return "";

        string virtualPath = Path.Combine(
            virtualFolder.Trim('/').Replace("/", Path.DirectorySeparatorChar.ToString()),
            fileName
        );

        string fullPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            virtualPath
        );

        return File.Exists(fullPath) ? File.ReadAllText(fullPath) : "";
    }
}
