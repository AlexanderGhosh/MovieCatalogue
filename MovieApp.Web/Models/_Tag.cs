using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieCatalogue.Models;

public class _Tag
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Movie> Movies { get; set; } = new HashSet<Movie>();
}
