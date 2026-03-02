using System;
using System.Collections.Generic;
using System.Text;

namespace BlobStorage.Extensions
{
    internal static class FileInfoExtensions
    {
        extension(FileInfo fileInfo)
        {
            public string NameWithoutExtension => Path.GetFileNameWithoutExtension(fileInfo.Name);
        }
    }
}
