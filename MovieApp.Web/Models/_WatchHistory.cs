using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieCatalogue.Models;

public class _WatchHistory
{
    public int Id { get; set; }

    public int MovieId { get; set; }

    public DateTime WatchedOn { get; set; }

    public virtual Movie Movie { get; set; } = null!;
}
