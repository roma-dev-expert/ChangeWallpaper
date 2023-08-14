using ChangeWallpaper.Extensions;
using ChangeWallpaper.Utils;
using System.Configuration;

public class CategoryFilter
{
    private readonly List<string>? AllowedCategories;
    private readonly List<string>? ExcludedCategories;

    public CategoryFilter()
    {
        AllowedCategories = ConfigurationManager.AppSettings["AllowedCategories"]?.Split(", ")?.ToList();
        ExcludedCategories = ConfigurationManager.AppSettings["ExcludedCategories"]?.Split(", ")?.ToList();
    }

    public string GetRandomCategory()
    {
        var filteredCategories = WallpaperSettings.Categories;
        if (!AllowedCategories.IsNullOrEmpty() && !string.IsNullOrEmpty(AllowedCategories?.First())) filteredCategories = AllowedCategories;
        if (!ExcludedCategories.IsNullOrEmpty() && !string.IsNullOrEmpty(ExcludedCategories?.First())) filteredCategories = filteredCategories.Except(ExcludedCategories).ToList();

        return filteredCategories?.ToList()?.GetRandomItem() ?? WallpaperSettings.Categories.GetRandomItem();
    }
}
