using IKDFrontEnd.Interfaces;
using IKDFrontEnd.ViewModels;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace IKDFrontEnd.Helpers
{
    public class TezMateService : ITezMateService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public TezMateService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;

            // Base URL from appsettings.json
            _httpClient.BaseAddress = new Uri(_config["TezMate:BaseUrl"]);
        }

        public async Task<TezMateResponse?> GetUpdatedContentAsync(TezMateRequest request)
        {
            // Serialize the request to JSON for inspection
            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                WriteIndented = true // makes it easy to read in logs
            });

            // You can log it, or debug print
            Console.WriteLine("TezMate Request JSON:");
            Console.WriteLine(json);

            // Send request
            var response = await _httpClient.PutAsJsonAsync("/api/content/update-single-field", request);

            if (!response.IsSuccessStatusCode)
            {
                // Log error or return null
                Console.WriteLine($"TezMate API Error: {response.StatusCode}");
                return null;
            }

            return await response.Content.ReadFromJsonAsync<TezMateResponse>();
        }

        public async Task<TezMateMultiHtmlResponse?> GetUpdatedHtmlContentsAsync(TezMateMultiHtmlRequest request)
        {
            // Serialize the request to JSON for inspection
            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            Console.WriteLine("TezMate Multi-HTML Request JSON:");
            Console.WriteLine(json);

            // Send request to the new endpoint
            var response = await _httpClient.PutAsJsonAsync("/aiapi-content-generator/add-edit-html-content", request);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"TezMate Multi-HTML API Error: {response.StatusCode}");
                return null;
            }

            return await response.Content.ReadFromJsonAsync<TezMateMultiHtmlResponse>();
        }
    }
}