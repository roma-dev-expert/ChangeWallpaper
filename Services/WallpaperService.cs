using ChangeWallpaper.Extensions;
using ChangeWallpaper.Utils;
using ChangeWallpaper.WallpapersAPI;
using Serilog;
using System.IO;

namespace ChangeWallpaper.Services
{
    public class WallpaperService : BaseService
    {
        private string WallpaperPath;
        private HttpClientService HttpClientService;

        public WallpaperService(ILogger logger, string wallpaperPath) : base(logger)
        {
            WallpaperPath = wallpaperPath;
            HttpClientService = new HttpClientService(logger);
        }

        public async Task DownloadRandomWallpaperAsync(string category, string resolution)
        {
            var api = new WallpapersCraftAPI();
            var wallpapers = await api.GetByCatalog(category, resolution);
            var randomWallpaper = wallpapers.GetRandomItem();
            var downloadLink = await api.GetDownloadLinkAsync(randomWallpaper.Link ?? "", resolution);
            await DownloadWallpaperAsync(downloadLink);
            Logger.Information($"Wallpaper for {category} category downloaded successfully!");
        }

        private async Task DownloadWallpaperAsync(string downloadLink)
        {
            if (string.IsNullOrEmpty(downloadLink))
            {
                throw new ArgumentException("The wallpaper download link is null or empty.");
            }

            var stream = await HttpClientService.GetStreamAsync(downloadLink);
            using (var fileStream = File.Create(WallpaperPath))
            {
                await stream.CopyToAsync(fileStream);
            }
        }

        public string GetWallpaperResolution()
        {
            try
            {
                var screenResolution = ScreenService.GetScreenResolution();
                var availableWallpaperResolutions = WallpaperSettings.GetResolutionsGreaterThan(screenResolution);
                return availableWallpaperResolutions.GetRandomItem();
            }
            catch (Exception ex)
            {
                Logger.Warning("An error occurred: {ErrorMessage}. Used Default resolution: {DefaultResolution}", ex.Message, WallpaperSettings.DefaultResolution);
                return WallpaperSettings.DefaultResolution;
            }
        }
    }

}
