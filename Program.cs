using System.IO;
using System.Net.Http;
using Serilog;
using ChangeWallpaper.Services;

public class Program
{
    private static readonly CategoryFilter CategoryFilter = new CategoryFilter();
    private static readonly string WallpaperPath = Path.Combine(Directory.GetCurrentDirectory(), "Wallpapers\\wallpaper.jpg");
    private static readonly string LogsPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs\\log.txt");
    private static readonly ILogger Logger = new LoggerConfiguration().WriteTo.Console().WriteTo.File(LogsPath).CreateLogger();
    private static readonly WallpaperService WallpaperService = new WallpaperService(Logger, WallpaperPath);
    private static readonly DirectoryService DirectoryService = new DirectoryService(Logger, WallpaperPath);
    private static readonly ScreenService ScreenService = new ScreenService(Logger, WallpaperPath);

    public static async Task Main()
    {
        try
        {
            var category = CategoryFilter.GetRandomCategory();
            var resolution = WallpaperService.GetWallpaperResolution();

            DirectoryService.CreateWallpapersDirectory();
            await WallpaperService.DownloadRandomWallpaperAsync(category, resolution);

            ScreenService.UpdateDesktopWallpaper();
            Logger.Information("Desktop wallpaper updated!");
        }
        catch (HttpRequestException httpEx)
        {
            Logger.Error("HTTP request error: {ErrorMessage}", httpEx.Message);
        }
        catch (Exception ex)
        {
            Logger.Error("An error occurred: {ErrorMessage}", ex.Message);
        }
    }
}
