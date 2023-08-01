using System.Runtime.InteropServices;

public class Program
{
    // Константы для SystemParametersInfo
    private const int SPI_SETDESKWALLPAPER = 0x0014;
    private const int SPIF_UPDATEINIFILE = 0x01;
    private const int SPIF_SENDCHANGE = 0x02;

    // Импортируем функцию SystemParametersInfo из библиотеки user32.dll
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

    public static void Main()
    {
        // Путь к новой заставке рабочего стола
        string wallpaperPath = @"C:\Users\rkhma\Pictures\Wallpapers\bmw_e30_m3_97062_1920x1080.jpg";

        // Вызываем функцию для установки новой заставки рабочего стола
        bool result = SetWallpaper(wallpaperPath);

        if (result)
        {
            Console.WriteLine("Заставка рабочего стола изменена успешно.");
        }
        else
        {
            Console.WriteLine("Не удалось изменить заставку рабочего стола.");
        }
    }

    // Функция для изменения заставки рабочего стола
    public static bool SetWallpaper(string wallpaperPath)
    {
        // Вызываем SystemParametersInfo для установки новой заставки
        int result = SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, wallpaperPath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);

        return result != 0;
    }
}