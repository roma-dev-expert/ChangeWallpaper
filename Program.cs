using System;
using System.Runtime.InteropServices;

public class Program
{
    // Constants for SystemParametersInfo
    private const int SPI_SETDESKWALLPAPER = 0x0014;
    private const int SPIF_UPDATEINIFILE = 0x01;
    private const int SPIF_SENDCHANGE = 0x02;

    // Import the SystemParametersInfo function from user32.dll
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

    public static void Main()
    {
        // Path to the new desktop wallpaper
        string wallpaperPath = @"C:\Users\rkhma\Pictures\Wallpapers\bmw_e30_m3_97062_1920x1080.jpg";

        // Call the function to set the new desktop wallpaper
        bool result = SetWallpaper(wallpaperPath);

        if (result)
        {
            Console.WriteLine("Desktop wallpaper changed successfully.");
        }
        else
        {
            Console.WriteLine("Failed to change desktop wallpaper.");
        }
    }

    // Function to change the desktop wallpaper
    public static bool SetWallpaper(string wallpaperPath)
    {
        // Call SystemParametersInfo to set the new wallpaper
        int result = SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, wallpaperPath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);

        return result != 0;
    }
}
