using Microsoft.AspNetCore.Components.Forms;
using MovieCatalogue;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace MovieApp.Web.Models
{
    [Table("MovieContent")]
    public class MovieContent() : IMovieContent
    {
        private readonly IBrowserFile? _file;
        private Stream? _stream;

        public MovieContent(IBrowserFile file) : this()
        {
            _file = file;
        }

        public int Id { get; set; }
        public int MovieId { get; set; }
        public Guid BlobGuid { get; set; }
        public TimeSpan Length { get; } = default;
        public TimeSpan? LastTimeStamp { get; }
        [NotMapped, AllowNull, NotNull]
        public Stream Stream
        {
            get
            {
                if (_stream is not null) return _stream;
                if (_file is null)
                {
                    _stream = null;
                    return Stream.Null;
                }
                _stream ??= _file.OpenReadStream(_file.Size);
                return _stream;
            }
            set => _stream = value;
        }
        public Movie Movie { get; set; } = default!;
    }
}
