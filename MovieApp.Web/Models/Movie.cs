using MovieCatalogue;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.Web.Models;

[Table("Movie")]
public class Movie : IMovie
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public DateTime AddedOn { get; set; } = DateTime.Now;

    [InverseProperty(nameof(MovieContent.Movie))]
    public MovieContent? Content { get; set; }
}
