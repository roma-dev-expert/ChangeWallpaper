using System.Runtime.InteropServices;
using ChangeWallpaper.WallpapersAPI;
using System.IO;
using System.Net.Http;

public class Program
{

    private const int SPI_SETDESKWALLPAPER = 0x0014;
    private const int SPIF_UPDATEINIFILE = 0x01;
    private const int SPIF_SENDCHANGE = 0x02;


    // Import the SystemParametersInfo function from user32.dll
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);


    public static async Task Main(string[] args)
    {
        var api = new WallpapersCraftAPI();
        var resolution = GetResolution();

        var pictures = await api.GetByCatalog(GetRandomItem(WallpaperSettings.Categories), resolution);
        //var pictures = await api.Search("bmw");

        var randomPicture = GetRandomItem(pictures);
        var downloadLink = await randomPicture.GetDownloadLinkAsync(resolution);
        string relativePath = "Pictures\\picture.jpg";

        CreateFolder();

        try
        {
            string fullPath = await DownloadPicture(downloadLink, relativePath);
            Console.WriteLine("Picture downloaded successfully!");

            // Update the desktop wallpaper with the downloaded picture
            UpdateDesktopWallpaper(fullPath);

            Console.WriteLine("Desktop wallpaper updated!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    public static string GetScreenResolution()
    {
        var screens = Screen.AllScreens;
        Screen primaryScreen = Screen.PrimaryScreen;
        Screen screenWithHighestResolution = screens.Aggregate(primaryScreen, (maxScreen, nextScreen) => nextScreen.Bounds.Width * nextScreen.Bounds.Height > maxScreen.Bounds.Width * maxScreen.Bounds.Height ? nextScreen : maxScreen);
        return $"{screenWithHighestResolution.Bounds.Width}x{screenWithHighestResolution.Bounds.Height}";
    }

    public static string GetResolution()
    {
        try
        {
            var screenResolution = GetScreenResolution();
            var availablePictureResolutions = WallpaperSettings.GetResolutionsGreaterThan(screenResolution);
            return GetRandomItem(availablePictureResolutions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}. Used Default resolution: {WallpaperSettings.DefaultResolution}");
            return WallpaperSettings.DefaultResolution;
        }
    }

    private static T GetRandomItem<T>(IList<T> item)
    {
        if (item == null || item.Count == 0)
        {
            throw new ArgumentException("The list is empty or null.");
        }

        Random random = new Random();
        int randomIndex = random.Next(0, item.Count);
        return item[randomIndex];
    }

    public static void UpdateDesktopWallpaper(string picturePath)
    {
        SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, picturePath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
    }

    static async Task<string> DownloadPicture(string? pictureUrl, string relativePath)
    {
        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);
        using (var httpClient = new HttpClient())
        {
            using (var response = await httpClient.GetAsync(pictureUrl))
            {
                response.EnsureSuccessStatusCode();

                using (var contentStream = await response.Content.ReadAsStreamAsync())
                {
                    using (var fileStream = File.Create(fullPath))
                    {
                        await contentStream.CopyToAsync(fileStream);
                    }
                }
            }
        }

        return fullPath;
    }

    public static void CreateFolder()
    {
        string picturesDirectory = "Pictures";

        try
        {
            if (!Directory.Exists(picturesDirectory))
            {
                Directory.CreateDirectory(picturesDirectory);
                Console.WriteLine("Pictures directory created successfully!");
            }
            else
            {
                Console.WriteLine("Pictures directory already exists.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
