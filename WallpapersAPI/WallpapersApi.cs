using System.Net.Http;

namespace ChangeWallpaper.WallpapersAPI
{
    public class WallpapersCraftAPI
    {
        private const string WEBSITE = "https://wallpaperscraft.com";
        private readonly HttpClient httpClient;

        public WallpapersCraftAPI()
        {
            httpClient = new HttpClient();
        }

        private async Task<HtmlAgilityPack.HtmlDocument> Get(string query = "/")
        {
            if (!query.StartsWith(WEBSITE))
            {
                query = WEBSITE + query;
            }

            var response = await httpClient.GetAsync(query);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var document = new HtmlAgilityPack.HtmlDocument();
            document.LoadHtml(content);
            return document;
        }

        private List<Picture> GetAllPicturesFromPage(HtmlAgilityPack.HtmlDocument page)
        {
            var pictures = new List<Picture>();
            var picNodes = page.DocumentNode.SelectNodes("//li[contains(@class, 'wallpapers__item')]");
            if (picNodes != null)
            {
                foreach (var pic in picNodes)
                {
                    pictures.Add(new Picture
                    {
                        Preview = pic.SelectSingleNode(".//a[contains(@class, 'wallpapers__link')]/span[contains(@class, 'wallpapers__canvas')]/img[contains(@class, 'wallpapers__image')]")
                            ?.GetAttributeValue("src", null),
                        Link = WEBSITE + pic.SelectSingleNode(".//a[contains(@class, 'wallpapers__link')]")?.GetAttributeValue("href", null),
                        Info = pic.SelectSingleNode(".//a[contains(@class, 'wallpapers__link')]/span[contains(@class, 'wallpapers__info')][2]")?.InnerText,
                        Rating = pic.SelectSingleNode(".//a[contains(@class, 'wallpapers__link')]/span[contains(@class, 'wallpapers__info')][1]/span[contains(@class, 'wallpapers__info-rating')]")
                            ?.InnerText?.Trim(),
                        ApiClass = this
                    });
                }
            }
            return pictures;
        }

        public async Task<List<Picture>> GetByCatalog(string catalog, string resolution = "", int page = 1)
        {
            if (!string.IsNullOrEmpty(resolution))
            {
                if (!RESOLUTIONS.Contains(resolution))
                {
                    throw new ArgumentException("Parameter <resolution> isn't a valid screen resolution! Please set a valid resolution.");
                }
                resolution = "/" + resolution;
            }

            var pageDocument = await Get($"/catalog/{catalog}{resolution}/page{page}");
            return GetAllPicturesFromPage(pageDocument);
        }

        public async Task<List<Picture>> Search(string query, int page = 1, string resolution = "")
        {
            if (!string.IsNullOrEmpty(resolution) && !RESOLUTIONS.Contains(resolution))
            {
                throw new ArgumentException("Parameter <resolution> isn't a valid screen resolution! Please set a valid resolution.");
            }

            var pageDocument = await Get("https://wallpaperscraft.com/search/" +
                                         $"?order=&page={page}&query={query.Trim().Replace(" ", "+")}&size={resolution}");
            return GetAllPicturesFromPage(pageDocument);
        }

        private static readonly string[] RESOLUTIONS = { "1920x1080", "240x320", "240x400", "320x240", "320x480", "360x640", "480x800", "480x854", 
            "540x960", "720x1280", "800x600", "800x1280", "960x544", "1024x600", "1080x1920", "2160x3840", "1366x768", "1440x2560", "800x1200", 
            "800x1420", "938x1668", "1280x1280", "1350x2400", "3415x3415", "2780x2780", "1024x768", "1152x864", "1280x960", "1400x1050", "1600x1200", 
            "1280x1024", "1280x720", "1280x800", "1440x900", "1680x1050", "1920x1200", "2560x1600", "1600x900", "2560x1440", "2048x1152", "2560x1024", 
            "2560x1080", "3840x2400", "3840x2160", };
    }
}
