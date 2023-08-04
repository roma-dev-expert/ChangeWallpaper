using AngleSharp;
using AngleSharp.Dom;

namespace ChangeWallpaper.WallpapersAPI
{
    public class Wallpaper
    {

        public string? Preview { get; set; }
        public string? Link { get; set; }
        public string? Info { get; set; }
        public string? Rating { get; set; }

        public async Task<string?> GetDownloadLinkAsync(string resolution)
        {
            string link = $"{Link?.Replace("/wallpaper/", "/download/")}";

            if (!link.EndsWith(resolution)) link += $"/{resolution}";

            var document = await GetAsync(link);
            var imgNode = document.DocumentElement.QuerySelector("img.wallpaper__image");
            return imgNode?.GetAttribute("src");
        }

        private async Task<IDocument> GetAsync(string query)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(query);
            return document;
        }
    }
}
