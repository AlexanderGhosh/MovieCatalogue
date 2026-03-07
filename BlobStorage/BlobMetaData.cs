namespace BlobStorage
{
    public class BlobMetaData
    {
        public required Guid Id { get; set; }
        public required string ContentType
        {
            get => field;
            set
            {
                ArgumentNullException.ThrowIfNullOrEmpty(value);
                field = value;
            }
        }
        public string? FileName { get; set; }
        public string? Description { get; set; }
    }
}
