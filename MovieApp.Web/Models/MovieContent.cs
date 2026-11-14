using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace MovieApp.Web.Models
{
    public class MovieContent()
    {
        private readonly IBrowserFile? _file;

        public MovieContent(IBrowserFile file) : this()
        {
            _file = file;
        }

        public int Id { get; set; }
        public int MovieId { get; set; }
        public Guid BlobGuid { get; set; }

        [NotMapped, AllowNull, NotNull]
        public Stream Stream
        {
            get
            {
                if (field is not null) return field;
                if (_file is null)
                {
                    field = null;
                    return Stream.Null;
                }
                field ??= _file.OpenReadStream(_file.Size);
                return field;
            }
            set;
        }

        public Movie Movie { get; set; } = default!;
    }
}
