// FtpService.cs
using FluentFTP;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace IKDFrontEnd.Services
{


    public interface IFtpService
    {
        Task<bool> FileExistsAsync(string remoteFilePath);
        Task UploadFileAsync(string remoteFilePath, Stream content);
        Task<string> DownloadFileAsync(string remoteFilePath); // new
        string GetPublicUrl(string remoteFilePath);
    }



    public class FtpService : IFtpService
    {
        private readonly string _ftpServer;
        private readonly string _ftpUsername;
        private readonly string _ftpPassword;
        private readonly string _cdnBaseUrl;
        private readonly ILogger<FtpService> _logger;

        public FtpService(string ftpServer, string ftpUsername, string ftpPassword, string cdnBaseUrl, ILogger<FtpService> logger)
        {
            _ftpServer = ftpServer;
            _ftpUsername = ftpUsername;
            _ftpPassword = ftpPassword;
            _cdnBaseUrl = cdnBaseUrl.TrimEnd('/');
            _logger = logger;
        }

        public async Task<bool> FileExistsAsync(string remoteFilePath)
        {
            try
            {
                using (var client = new AsyncFtpClient(_ftpServer, _ftpUsername, _ftpPassword))
                {
                    await client.AutoConnect();
                    return await client.FileExists(remoteFilePath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking file existence on FTP: {RemotePath}", remoteFilePath);
                return false;
            }
        }

        public async Task UploadFileAsync(string remoteFilePath, Stream content)
        {
            using (var client = new AsyncFtpClient(_ftpServer, _ftpUsername, _ftpPassword))
            {
                await client.AutoConnect();
                // Ensure the directory exists
                string directory = System.IO.Path.GetDirectoryName(remoteFilePath)?.Replace('\\', '/');
                if (!string.IsNullOrEmpty(directory))
                {
                    await client.CreateDirectory(directory);
                }
                // Upload the stream
                content.Position = 0;
                await client.UploadStream(content, remoteFilePath, FtpRemoteExists.Overwrite);
            }
        }

        // FtpService.cs – add this method
        public async Task<string> DownloadFileAsync(string remoteFilePath)
        {
            using (var client = new AsyncFtpClient(_ftpServer, _ftpUsername, _ftpPassword))
            {
                await client.AutoConnect();
                using (var stream = new MemoryStream())
                {
                    await client.DownloadStream(stream, remoteFilePath);
                    stream.Position = 0;
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        return await reader.ReadToEndAsync();
                    }
                }
            }
        }

        public string GetPublicUrl(string remoteFilePath)
        {
            // remoteFilePath should be like "files/dictionary/filename.html"
            return $"{_cdnBaseUrl}/{remoteFilePath}";
        }
    }
}
