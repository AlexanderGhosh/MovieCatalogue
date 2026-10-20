namespace MovieApp.Web.Models;

public class MovieCollection : IMovieItem
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public DateTime AddedOn { get; set; } = DateTime.Now;
    public bool ShowThumbnail { get; set; } = false;
    public Guid? ThumbnailBlobId { get; set; }

    public ICollection<Movie> Movies { get; set; } = new HashSet<Movie>();

    public ICollection<MovieCollection> ParentCollections { get; set; } = new HashSet<MovieCollection>();
    public ICollection<MovieCollection> ChildCollections { get; set; } = new HashSet<MovieCollection>();
}
