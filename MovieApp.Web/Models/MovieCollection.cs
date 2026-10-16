using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.Web.Models;

[Table("MovieCollection")]
public class MovieCollection
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public DateTime AddedOn { get; set; } = DateTime.Now;

    public ICollection<Movie> Movies { get; set; } = [];
}
