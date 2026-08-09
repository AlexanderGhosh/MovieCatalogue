namespace BlobStorage
{
    public class BlobMetaData
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public required string ContentType
        {
            get => field;
            init
            {
                ArgumentNullException.ThrowIfNullOrEmpty(value);
                field = value;
            }
        }
        public string? FileName { get; set; }
        public string? Description { get; set; }
    }
}
