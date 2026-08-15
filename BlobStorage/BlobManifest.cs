using System.Diagnostics.CodeAnalysis;

namespace BlobStorage
{
    public class BlobManifest
    {
        public required string BlobFiles { get; set; }
        public ICollection<BlobMetaData> MetaData { get; set; } = new HashSet<BlobMetaData>();

        public bool Exists(Guid id)
        {
            return MetaData.Any(m => m.Id == id);
        }

        public BlobMetaData? GetMetaData(Guid id)
        {
            return MetaData.FirstOrDefault(m => m.Id == id);
        }

        public bool TryGetMetaData(Guid id, [NotNullWhen(true)] out BlobMetaData? metaData)
        {
            if (!Exists(id))
            {
                metaData = null;
                return false;
            }
            metaData = GetMetaData(id);
            return metaData is not null;
        }

        public void DeleteMetaData(Guid id)
        {
            var metaData = GetMetaData(id);
            if (metaData is null) return;
            MetaData.Remove(metaData);
        }
    }
}
