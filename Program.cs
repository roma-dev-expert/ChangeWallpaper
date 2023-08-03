using System.Runtime.InteropServices;
using ChangeWallpaper.WallpapersAPI;

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

        var abstractPictures = await api.GetByCatalog(Catalog.ABSTRACT);
        var pictures = await api.Search("bmw");

        Random random = new Random();
        int randomIndex = random.Next(0, pictures.Count);
        Picture randomPicture = pictures[randomIndex];

        var downloadLink = await randomPicture.GetDownloadLinkAsync();
        string relativePath = "Pictures\\image.jpg";


        CreateFolder();

        try
        {
            string fullPath = await DownloadImage(downloadLink, relativePath);
            Console.WriteLine("Image downloaded successfully!");

            // Update the desktop wallpaper with the downloaded image
            UpdateDesktopWallpaper(fullPath);

            Console.WriteLine("Desktop wallpaper updated!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    public static void UpdateDesktopWallpaper(string imagePath)
    {
        // Call the SystemParametersInfo function to update the desktop wallpaper
        SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, imagePath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
    }

    static async Task<string> DownloadImage(string? imageUrl, string relativePath)
    {
        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);
        using (var httpClient = new HttpClient())
        {
            using (var response = await httpClient.GetAsync(imageUrl))
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
