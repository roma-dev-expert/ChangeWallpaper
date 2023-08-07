using Serilog;
using System.Runtime.InteropServices;

namespace ChangeWallpaper.Services
{
    public class ScreenService : BaseService
    {
        private string WallpaperPath;
        private const int SPI_SETDESKWALLPAPER = 0x0014;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDCHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public ScreenService (ILogger logger, string wallpaperPath) : base (logger)
        {
            WallpaperPath = wallpaperPath;
        }

        public static string GetScreenResolution()  
        {
            var screens = Screen.AllScreens;
            var primaryScreen = Screen.PrimaryScreen;
            var screenWithHighestResolution = screens.Aggregate(primaryScreen, (maxScreen, nextScreen) =>
                nextScreen.Bounds.Width * nextScreen.Bounds.Height > maxScreen.Bounds.Width * maxScreen.Bounds.Height
                    ? nextScreen
                    : maxScreen);
            return $"{screenWithHighestResolution.Bounds.Width}x{screenWithHighestResolution.Bounds.Height}";
        }

        public void UpdateDesktopWallpaper()
        {
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, WallpaperPath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        }
    }
}
