using System;
using System.Text;

namespace IKDFrontEnd.Helpers
{
    public static class UrlDecoder
    {
        public static string Decode(string base64Url)
        {
            if (string.IsNullOrEmpty(base64Url))
                return string.Empty;

            try
            {
                var bytes = Convert.FromBase64String(base64Url);
                return Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                return base64Url; // agar galti ho to as-it-is return
            }
        }
    }
}
