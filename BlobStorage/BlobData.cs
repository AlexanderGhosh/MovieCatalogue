namespace BlobStorage
{
    public sealed class BlobData : IDisposable
    {
        public Stream? Data { get; set; }
        public required BlobMetaData MetaData { get; set; }

        public void Dispose() => Data?.Dispose();
    }
}
