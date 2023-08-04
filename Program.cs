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

    private static readonly string picturePath = Path.Combine(Directory.GetCurrentDirectory(), "Pictures\\picture.jpg");
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
            await DownloadRandomPicture(category, resolution);

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
            var availablePictureResolutions = WallpaperSettings.GetResolutionsGreaterThan(screenResolution);
            return GetRandomItem(availablePictureResolutions);
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
        SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, picturePath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
    }

    private static async Task DownloadPicture(string pictureUrl)
    {
        if (string.IsNullOrEmpty(pictureUrl))
        {
            throw new ArgumentException("The picture URL is null or empty.");
        }

        using var response = await httpClient.GetAsync(pictureUrl).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        if (!response.Content.Headers.ContentType.MediaType.StartsWith("image/"))
        {
            throw new Exception("The provided URL does not point to an image.");
        }

        await using var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        await using var fileStream = File.Create(picturePath);
        await contentStream.CopyToAsync(fileStream).ConfigureAwait(false);
    }

    private static async Task DownloadRandomPicture(string category, string resolution)
    {
        var api = new WallpapersCraftAPI();
        var pictures = await api.GetByCatalog(category, resolution);
        var randomPicture = GetRandomItem(pictures);
        var downloadLink = await randomPicture.GetDownloadLinkAsync(resolution);
        await DownloadPicture(downloadLink);
        logger.Information("Picture downloaded successfully!");
    }
}
