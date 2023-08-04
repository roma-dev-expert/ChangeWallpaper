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

        private List<Wallpaper> GetAllWallpapersFromPage(HtmlAgilityPack.HtmlDocument page)
        {
            const string previewXPath = ".//a[contains(@class, 'wallpapers__link')]/span[contains(@class, 'wallpapers__canvas')]/img[contains(@class, 'wallpapers__image')]";
            const string linkXPath = ".//a[contains(@class, 'wallpapers__link')]";
            const string infoXPath = ".//a[contains(@class, 'wallpapers__link')]/span[contains(@class, 'wallpapers__info')][2]";
            const string ratingXPath = ".//a[contains(@class, 'wallpapers__link')]/span[contains(@class, 'wallpapers__info')][1]/span[contains(@class, 'wallpapers__info-rating')]";

            var wallpapers = new List<Wallpaper>();
            var picNodes = page.DocumentNode.SelectNodes("//li[contains(@class, 'wallpapers__item')]");
            if (picNodes != null)
            {
                foreach (var pic in picNodes)
                {
                    wallpapers.Add(new Wallpaper
                    {
                        Preview = pic.SelectSingleNode(previewXPath)?.GetAttributeValue("src", null),
                        Link = WEBSITE + pic.SelectSingleNode(linkXPath)?.GetAttributeValue("href", null),
                        Info = pic.SelectSingleNode(infoXPath)?.InnerText,
                        Rating = pic.SelectSingleNode(ratingXPath)?.InnerText?.Trim()
                    });
                }
            }
            return wallpapers;
        }

        public async Task<List<Wallpaper>> GetByCatalog(string catalog, string resolution, int page = 1)
        {
            if (!string.IsNullOrEmpty(resolution))
            {
                if (!WallpaperSettings.Resolutions.Contains(resolution))
                {
                    throw new ArgumentException("Parameter <resolution> isn't a valid screen resolution! Please set a valid resolution.");
                }
                resolution = "/" + resolution;
            }

            var pageDocument = await Get($"/catalog/{catalog}{resolution}/page{page}");
            return GetAllWallpapersFromPage(pageDocument);
        }

        public async Task<List<Wallpaper>> Search(string query, string resolution, int page = 1)
        {
            if (!string.IsNullOrEmpty(resolution) && !WallpaperSettings.Resolutions.Contains(resolution))
            {
                throw new ArgumentException("Parameter <resolution> isn't a valid screen resolution! Please set a valid resolution.");
            }

            var pageDocument = await Get("https://wallpaperscraft.com/search/" +
                                         $"?order=&page={page}&query={query.Trim().Replace(" ", "+")}&size={resolution}");
            return GetAllWallpapersFromPage(pageDocument);
        }
    }
}
