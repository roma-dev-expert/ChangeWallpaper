using Serilog;
using System.IO;

namespace ChangeWallpaper.Services
{
    public class DirectoryService : BaseService
    {
        private string WallpaperPath;

        public DirectoryService(ILogger logger, string wallpaperPath) : base(logger)
        {
            WallpaperPath = wallpaperPath;
        }

        public void CreateWallpapersDirectory()
        {
            var wallpapersDirectory = Path.GetDirectoryName(WallpaperPath);

            try
            {
                if (!Directory.Exists(wallpapersDirectory))
                {
                    Directory.CreateDirectory(wallpapersDirectory);
                    Logger.Information("Wallpapers directory created successfully!");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("An error occurred while creating the wallpapers directory: {ErrorMessage}", ex.Message);
            }
        }
    }
}
