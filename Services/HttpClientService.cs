using ChangeWallpaper.Services;
using Serilog;
using System.IO;
using System.Net.Http;

public class HttpClientService : BaseService
{
    private readonly HttpClient httpClient = new HttpClient();

    public HttpClientService(ILogger logger) : base(logger) { }

    public async Task<Stream> GetStreamAsync(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            throw new ArgumentException("URL cannot be null or empty.", nameof(url));
        }

        try
        {
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStreamAsync();
        }
        catch (HttpRequestException ex)
        {
            Logger.Error("HTTP request error: {ErrorMessage}", ex.Message);
            return Stream.Null;
        }
    }

    public async Task<string> GetStringAsync(string requestUri)
    {
        if (string.IsNullOrEmpty(requestUri))
        {
            throw new ArgumentException("Request URI cannot be null or empty.", nameof(requestUri));
        }

        try
        {
            var response = await httpClient.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException ex)
        {
            Logger.Error("HTTP request error: {ErrorMessage}", ex.Message);
            return string.Empty;
        }
    }
}
