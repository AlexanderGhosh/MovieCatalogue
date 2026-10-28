using BlobStorage;

namespace MovieApp.Web.Models
{
    public sealed record class Thumbnail(Stream Stream, BlobContentType ContentType);
}
