using AngleSharp;
using AngleSharp.Dom;
using ChangeWallpaper.Models;
using ChangeWallpaper.Utils;

namespace ChangeWallpaper.WallpapersAPI
{
    public class WallpapersCraftAPI
    {
        private const string WEBSITE = "https://wallpaperscraft.com";
        private readonly IConfiguration DefaultConfig;
        private readonly IBrowsingContext Context;

        public WallpapersCraftAPI()
        {
            DefaultConfig = Configuration.Default.WithDefaultLoader();
            Context = BrowsingContext.New(DefaultConfig);
        }

        private async Task<IDocument> GetDocumentAsync(string query = "/")
        {
            if (!query.StartsWith(WEBSITE))
            {
                query = WEBSITE + query;
            }

            return await Context.OpenAsync(query);
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

            var pageDocument = await GetDocumentAsync($"/catalog/{catalog}{resolution}/page{page}");
            return GetAllWallpapersFromPage(pageDocument);
        }

        public async Task<List<Wallpaper>> Search(string query, string resolution, int page = 1)
        {
            if (!string.IsNullOrEmpty(resolution) && !WallpaperSettings.Resolutions.Contains(resolution))
            {
                throw new ArgumentException("Parameter <resolution> isn't a valid screen resolution! Please set a valid resolution.");
            }

            var pageDocument = await GetDocumentAsync("https://wallpaperscraft.com/search/" +
                                         $"?order=&page={page}&query={query.Trim().Replace(" ", "+")}&size={resolution}");
            return GetAllWallpapersFromPage(pageDocument);
        }

        public async Task<string> GetDownloadLinkAsync(string wallpaperUrl, string resolution)
        {
            if (string.IsNullOrEmpty(wallpaperUrl))
            {
                throw new ArgumentException("The wallpaper URL is null or empty.");
            }
            string link = $"{wallpaperUrl?.Replace("/wallpaper/", "/download/")}";

            if (!link.EndsWith(resolution)) link += $"/{resolution}";

            var document = await Context.OpenAsync(link);
            var imgNode = document.DocumentElement.QuerySelector("img.wallpaper__image");
            return imgNode?.GetAttribute("src") ?? "";
        }

        private List<Wallpaper> GetAllWallpapersFromPage(IDocument page)
        {
            const string previewSelector = ".wallpapers__link .wallpapers__canvas .wallpapers__image";
            const string linkSelector = ".wallpapers__link";
            const string infoSelector = ".wallpapers__link .wallpapers__info:nth-child(3)";
            const string ratingSelector = ".wallpapers__link .wallpapers__info:nth-child(2) .wallpapers__info-rating";

            var wallpapers = new List<Wallpaper>();
            var picNodes = page.QuerySelectorAll(".wallpapers__item");
            if (picNodes != null)
            {
                foreach (var pic in picNodes)
                {
                    wallpapers.Add(new Wallpaper
                    {
                        Preview = pic.QuerySelector(previewSelector)?.GetAttribute("src"),
                        Link = WEBSITE + pic.QuerySelector(linkSelector)?.GetAttribute("href"),
                        Info = pic.QuerySelector(infoSelector)?.TextContent,
                        Rating = pic.QuerySelector(ratingSelector)?.TextContent?.Trim()
                    });
                }
            }
            return wallpapers;
        }
    }
}
