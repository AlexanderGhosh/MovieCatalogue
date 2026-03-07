using System.Text.Json;
using BlobStorage.Extensions;

namespace BlobStorage
{
    public class LocalBlobStorage : IBlobStorage
    {
        const string MANIFEST_FILE_NAME = "manifest.json";
        const string BLOB_EXTENSION = ".blob";
        private readonly string _basePath;
        private string _manifestFilePath { get; }
        private readonly BlobManifest _manifest;
        private readonly JsonSerializerOptions _serializerOptions;
        public LocalBlobStorage(string basePath)
        {
            ArgumentNullException.ThrowIfNull(basePath);
            if (!Directory.Exists(basePath))
                throw new DirectoryNotFoundException("The provided base path does not exist.");
            _basePath = basePath;
            _manifestFilePath = Path.Combine(_basePath, MANIFEST_FILE_NAME);
            _manifest = EnsureManifest(); 
            _serializerOptions = new()
            {
                WriteIndented = true
            };
        }

        public Task<BlobData> DownloadAsync(Guid id)
        {
            if(!_manifest.TryGetMetaData(id, out BlobMetaData? data))
                throw new ArgumentException($"No blob with id: {id} can be found.", nameof(id));

            string blobPath = GetBlobFilePath(id);
            if(!File.Exists(blobPath))
                throw new FileNotFoundException("The blob file cannot be found.", blobPath);

            BlobData result = new() {
                MetaData = data,
                DataStream = File.OpenRead(blobPath)
            };

            return Task.FromResult(result);
        }

        public async Task<Guid> UploadAsync(Stream dataStream, string? blobName)
        {
            if (!dataStream.CanRead)
                throw new ArgumentException("The provided stream is not readable.", nameof(dataStream));
            if (dataStream.CanSeek)
                dataStream.Seek(0, SeekOrigin.Begin);

            BlobMetaData metaData = new()
            {
                Id = Guid.NewGuid(),
                FileName = blobName,
            };

            _manifest.MetaData.Add(metaData);

            string fp = GetBlobFilePath(metaData.Id);
            var fileStream = File.OpenWrite(fp);
            await dataStream.CopyToAsync(fileStream);
            await UpdateManifestFile();

            return metaData.Id;
        }

        public string GetBlobFilePath(Guid id)
        {
            return Path.Combine(_manifest.BlobFiles, $"{id}{BLOB_EXTENSION}");
        }

        private BlobManifest EnsureManifest()
        {
            BlobManifest? manifest;
            if (File.Exists(_manifestFilePath))
            {
                string txt = File.ReadAllText(_manifestFilePath);
                manifest = JsonSerializer.Deserialize<BlobManifest>(txt);
                if (manifest is null)
                    throw new InvalidDataException("Cannot deserialize manifest file.");
                return manifest;
            }

            manifest = new() 
            { 
                BlobFiles = _basePath 
            };
            // adds existing blobs to manifest
            DirectoryInfo dir = new(_basePath);
            foreach (var file in dir.EnumerateFiles())
            {
                if (file.Extension == BLOB_EXTENSION)
                {
                    if(Guid.TryParse(file.NameWithoutExtension, out Guid id))
                    {
                        manifest.MetaData.Add(new()
                        {
                            Id = id,
                        });
                    }
                }
            }

            string json = JsonSerializer.Serialize(manifest, _serializerOptions);
            File.WriteAllText(_manifestFilePath, json);
            return manifest;
        }

        private Task UpdateManifestFile()
        {
            string json = JsonSerializer.Serialize(_manifest, _serializerOptions);
            return File.WriteAllTextAsync(_manifestFilePath, json);
        }

        public Task<bool> ExistsAsync(Guid id) => Task.FromResult(_manifest.Exists(id));
    }
}
