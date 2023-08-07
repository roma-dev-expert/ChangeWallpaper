namespace ChangeWallpaper.Utils
{
    public static class WallpaperSettings
    {
        public static readonly List<string> Categories = new List<string>
        {
            "3d",
            "abstract",
            "animals",
            "anime",
            "art",
            "black",
            "black_and_white",
            "cars",
            "city",
            "dark",
            "fantasy",
            "flowers",
            "food",
            "holidays",
            "love",
            "macro",
            "minimalism",
            "motorcycles",
            "music",
            "nature",
            "other",
            "space",
            "sport",
            "technologies",
            "textures",
            "vector",
            "words"
        };

        public static readonly List<string> Resolutions = new List<string>
        {
            "1920x1080", "240x320", "240x400", "320x240", "320x480", "360x640", "480x800", "480x854",
            "540x960", "720x1280", "800x600", "800x1280", "960x544", "1024x600", "1080x1920", "2160x3840", "1366x768", "1440x2560", "800x1200",
            "800x1420", "938x1668", "1280x1280", "1350x2400", "3415x3415", "2780x2780", "1024x768", "1152x864", "1280x960", "1400x1050", "1600x1200",
            "1280x1024", "1280x720", "1280x800", "1440x900", "1680x1050", "1920x1200", "2560x1600", "1600x900", "2560x1440", "2048x1152", "2560x1024",
            "2560x1080", "3840x2400", "3840x2160"
        };

        public static readonly string DefaultResolution = "1920x1080";

        public static List<string> GetResolutionsGreaterThan(string resolution)
        {
            if (resolution == null) resolution = DefaultResolution;

            var selectedResolutions = Resolutions
                .Where(r => CompareResolutions(r, resolution) >= 0)
                .OrderBy(r => r)
                .ToList();

            return selectedResolutions;
        }

        private static int CompareResolutions(string resolution1, string resolution2)
        {
            string[] parts1 = resolution1.Split('x');
            string[] parts2 = resolution2.Split('x');

            int width1 = int.Parse(parts1[0]);
            int height1 = int.Parse(parts1[1]);

            int width2 = int.Parse(parts2[0]);
            int height2 = int.Parse(parts2[1]);

            if (width1 == width2)
            {
                return height1.CompareTo(height2);
            }

            return width1.CompareTo(width2);
        }
    }
}
