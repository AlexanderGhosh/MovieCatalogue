using MovieCatalogue;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.Web.Models;

[Table("Movie")]
public class Movie : IMovie
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public DateTime AddedOn { get; } = DateTime.UtcNow;

    [InverseProperty(nameof(MovieContent.Movie))]
    public MovieContent? Content { get; set; }
}
