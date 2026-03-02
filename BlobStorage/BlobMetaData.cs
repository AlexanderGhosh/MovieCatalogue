namespace BlobStorage
{
    public class BlobMetaData
    {
        public required Guid Id { get; set; }
        public string? FileName { get; set; }
        public string? Description { get; set; }
    }
}
