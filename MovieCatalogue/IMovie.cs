using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieCatalogue;

public interface IMovie
{
    int Id { get; }
    string Title { get; }
    DateTime AddedOn { get; }
}
