using MovieApp.Web.Models;

namespace MovieCatalogue.Models
{
    public class Thumbnail : IThumbnail
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public Guid BlobId { get; set; }
        public Movie Movie { get; set; } = null!;
    }
}
