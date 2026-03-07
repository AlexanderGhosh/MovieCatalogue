namespace BlobStorage
{
    public sealed class BlobData : IDisposable
    {
        public Stream? DataStream { get; set; }
        public required BlobMetaData MetaData { get; set; }

        public void Dispose() => DataStream?.Dispose();
    }
}
