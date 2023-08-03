using HtmlAgilityPack;

namespace ChangeWallpaper.WallpapersAPI
{
    public class Picture
    {
        private HttpClient httpClient;

        public string? Preview { get; set; }
        public string? Link { get; set; }
        public string? Info { get; set; }
        public string? Rating { get; set; }
        public WallpapersCraftAPI? ApiClass { get; set; }

        public Picture()
        {

            httpClient = new HttpClient();
        }

        public async Task<string?> GetDownloadLinkAsync(string resolution = "1024x768")
        {
            var link = $"{Link?.Replace("/wallpaper/", "/download/")}/{resolution}";

            var document = await Get(link);
            var imgNode = document.DocumentNode.SelectSingleNode("//img[contains(@class, 'wallpaper__image')]");
            return imgNode?.GetAttributeValue("src", null);
        }

        private async Task<HtmlDocument> Get(string query)
        {
            var response = await httpClient.GetAsync(query);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var document = new HtmlDocument();
            document.LoadHtml(content);
            return document;
        }

    }
}
