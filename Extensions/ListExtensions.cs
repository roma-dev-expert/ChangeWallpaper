namespace ChangeWallpaper.Extensions
{
    public static class ListExtensions
    {
        private static readonly Random random = new Random();

        public static T GetRandomItem<T>(this List<T>? list)
        {
            if (list.IsNullOrEmpty())
            {
                throw new ArgumentException("The list is empty or null.");
            }

            var randomIndex = random.Next(0, list.Count);
            return list[randomIndex];
        }

        public static bool IsNullOrEmpty<T>(this List<T>? list)
        {
            return list == null || list.Count == 0;
        }
    }
}
