namespace BlobStorage
{
    public interface IBlobStorage
    {
        Task<bool> ExistsAsync(Guid id);
        Task<BlobData> DownloadAsync(Guid id);
        Task<Guid> UploadAsync(Stream dataStream, BlobMetaData metaData);
        Task DeleteAsync(Guid id, bool hard = true);
    }
}
