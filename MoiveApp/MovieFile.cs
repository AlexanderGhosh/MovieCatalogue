using MovieCatalogue;
using System;
using System.Collections.Generic;
using System.Text;

namespace MovieApp
{
    internal class MovieFile : IMovie
    {
        private static int _idSequence = 0;
        public int Id { get; } = Interlocked.Increment(ref _idSequence);
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required string FilePath { get; init; }
    }
}
