using System.Runtime.InteropServices;
using ChangeWallpaper.WallpapersAPI;
using System.IO;
using System.Net.Http;
using Serilog;

public class Program
{
    private const int SPI_SETDESKWALLPAPER = 0x0014;
    private const int SPIF_UPDATEINIFILE = 0x01;
    private const int SPIF_SENDCHANGE = 0x02;

    private static readonly string wallpaperPath = Path.Combine(Directory.GetCurrentDirectory(), "Wallpapers\\wallpaper.jpg");
    private static readonly HttpClient httpClient = new HttpClient();
    private static readonly Random random = new Random();
    private static readonly ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

    public static async Task Main()
    {
        try
        {
            var resolution = GetResolution();
            var category = GetRandomItem(WallpaperSettings.Categories);
            await DownloadRandomWallpaper(category, resolution);

            UpdateDesktopWallpaper();
            logger.Information("Desktop wallpaper updated!");
        }
        catch (HttpRequestException httpEx)
        {
            logger.Error("HTTP request error: {ErrorMessage}", httpEx.Message);
        }
        catch (Exception ex)
        {
            logger.Error("An error occurred: {ErrorMessage}", ex.Message);
        }
    }

    private static string GetScreenResolution()
    {
        var screens = Screen.AllScreens;
        var primaryScreen = Screen.PrimaryScreen;
        var screenWithHighestResolution = screens.Aggregate(primaryScreen, (maxScreen, nextScreen) =>
            nextScreen.Bounds.Width * nextScreen.Bounds.Height > maxScreen.Bounds.Width * maxScreen.Bounds.Height
                ? nextScreen
                : maxScreen);
        return $"{screenWithHighestResolution.Bounds.Width}x{screenWithHighestResolution.Bounds.Height}";
    }

    private static string GetResolution()
    {
        try
        {
            var screenResolution = GetScreenResolution();
            var availableWallpaperResolutions = WallpaperSettings.GetResolutionsGreaterThan(screenResolution);
            return GetRandomItem(availableWallpaperResolutions);
        }
        catch (Exception ex)
        {
            logger.Warning("An error occurred: {ErrorMessage}. Used Default resolution: {DefaultResolution}", ex.Message, WallpaperSettings.DefaultResolution);
            return WallpaperSettings.DefaultResolution;
        }
    }

    private static T GetRandomItem<T>(IList<T> items)
    {
        if (items == null || items.Count == 0)
        {
            throw new ArgumentException("The list is empty or null.");
        }

        var randomIndex = random.Next(0, items.Count);
        return items[randomIndex];
    }

    private static void UpdateDesktopWallpaper()
    {
        SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, wallpaperPath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
    }

    private static void CreateWallpapersDirectory()
    {
        var wallpapersDirectory = Path.GetDirectoryName(wallpaperPath);

        try
        {
            if (!Directory.Exists(wallpapersDirectory))
            {
                Directory.CreateDirectory(wallpapersDirectory);
                logger.Information("Wallpapers directory created successfully!");
            }
        }
        catch (Exception ex)
        {
            logger.Error("An error occurred while creating the wallpapers directory: {ErrorMessage}", ex.Message);
            return;
        }
    }

    private static async Task DownloadWallpaper(string wallpaperUrl)
    {
        if (string.IsNullOrEmpty(wallpaperUrl))
        {
            throw new ArgumentException("The wallpaper URL is null or empty.");
        }

        using var response = await httpClient.GetAsync(wallpaperUrl).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        if (!response.Content.Headers.ContentType.MediaType.StartsWith("image/"))
        {
            throw new Exception("The provided URL does not point to an image.");
        }

        await using var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        await using var fileStream = File.Create(wallpaperPath);
        await contentStream.CopyToAsync(fileStream).ConfigureAwait(false);
    }

    private static async Task DownloadRandomWallpaper(string category, string resolution)
    {
        CreateWallpapersDirectory();

        var api = new WallpapersCraftAPI();
        var wallpapers = await api.GetByCatalog(category, resolution);
        var randomWallpaper = GetRandomItem(wallpapers);
        var downloadLink = await randomWallpaper.GetDownloadLinkAsync(resolution);
        await DownloadWallpaper(downloadLink);
        logger.Information("Wallpaper downloaded successfully!");
    }
}
