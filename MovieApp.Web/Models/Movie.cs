using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.Web.Models;

public class Movie
{
    public int Id { get; set; }
    public required string Title { get; set; }
    [NotMapped]
    public string? Description { get; set; } =
        "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin ultrices augue ac libero porta posuere euismod a velit.";
    public DateTime AddedOn { get; set; } = DateTime.Now;
    public MovieContent? Content { get; set; }
    public bool ShowThumbnail { get; set; } = false;
    public Guid? ThumbnailBlobId { get; set; }
    public ICollection<MovieCollection> Collections { get; set; } = new HashSet<MovieCollection>();
}
