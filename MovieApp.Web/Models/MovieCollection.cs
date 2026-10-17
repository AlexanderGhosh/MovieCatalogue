namespace MovieApp.Web.Models;

public class MovieCollection
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public DateTime AddedOn { get; set; } = DateTime.Now;

    public ICollection<Movie> Movies { get; set; } = new HashSet<Movie>();

    public ICollection<MovieCollection> ParentCollections { get; set; } = new HashSet<MovieCollection>();
    public ICollection<MovieCollection> ChildCollections { get; set; } = new HashSet<MovieCollection>();
}
