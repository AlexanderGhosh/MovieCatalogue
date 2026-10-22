using MovieApp.Web.Models;

namespace MovieApp.Web.Helpers
{
    public static class SortingOperations
    {
        public static int OrderByEpisode(Movie movie) =>
            OrderByEpisode(movie, int.MinValue);
        public static int OrderByEpisode(Movie movie, int defaultOrdering)
        {
            var s = movie.Name.Split(" ");
            var a = s.FirstOrDefault();
            if (a is null) return defaultOrdering;
            var nums = a[1..];
            return int.TryParse(nums, out var n) ? n : defaultOrdering;
        }
    }
}
