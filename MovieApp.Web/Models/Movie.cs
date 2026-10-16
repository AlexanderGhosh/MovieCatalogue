namespace MovieApp.Web.Models;

public class Movie
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public DateTime AddedOn { get; set; } = DateTime.Now;
    public MovieContent? Content { get; set; }
    public bool ShowThumbnail { get; set; } = false;
    public Guid? ThumbnailBlobId { get; set; }
    public ICollection<MovieCollection> Collections { get; set; } = new HashSet<MovieCollection>();
}
