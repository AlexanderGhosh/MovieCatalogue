using System;
using System.Collections.Generic;
using System.Text;

namespace MovieCatalogue
{
    public interface IMovieContent
    {
        TimeSpan Length { get; }
        TimeSpan? LastTimeStamp { get; }
    }
}
