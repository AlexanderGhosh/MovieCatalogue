using BlobStorage;

namespace BlobStorage
{
    public interface IBlobStorage
    {
        Task<bool> ExistsAsync(Guid id);
        Task<BlobData> DownloadAsync(Guid id);
        Task<Guid> UploadAsync(Stream dataStream, string blobName);
    }
}
