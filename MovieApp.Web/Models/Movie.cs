using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieCatalogue.Models;

[Table("Movie")]
public class Movie : IMovie
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public DateTime AddedOn { get; set; }
}
