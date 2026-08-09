namespace BlobStorage
{
    public class BlobMetaData
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public required BlobContentType ContentType { get; init; }
        public string? FileName { get; set; }
        public string? Description { get; set; }
    }
}
