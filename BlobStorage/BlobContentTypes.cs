using System.Reflection;

namespace BlobStorage
{
    public static partial class BlobContentTypes
    {
        public const string Unknown = "application/?";
        public const string MP4 = "application/mp4";
        public const string PlainText = "text/plain";
        public const string Json = "application/json";
        public const string Jpeg = "image/jpeg";
        public const string Png = "image/png";
        public const string WebP = "image/webp";

        internal static readonly Dictionary<string, BlobContentType> ExtensionMap = new(StringComparer.OrdinalIgnoreCase)
        {
            ["mp4"] = MP4,
            ["m4v"] = MP4,
            ["txt"] = PlainText,
            ["json"] = Json,
            ["jpg"] = Jpeg,
            ["jpeg"] = Jpeg,
            ["jpe"] = Jpeg,
            ["jfif"] = Jpeg,
            ["jif"] = Jpeg,
            ["jfi"] = Jpeg,
            ["pjpeg"] = Jpeg,
            ["pjp"] = Jpeg,
            ["png"] = Png,
            ["webp"] = WebP,
        };
    }

    public static partial class BlobContentTypes
    {
        private static HashSet<string> _contentTypes = [];
        static BlobContentTypes()
        {
            Type self = typeof(BlobContentTypes);
            foreach (FieldInfo field in self.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (field.FieldType == typeof(string))
                {
                    if (field.GetValue(null) is not string value)
                        continue;
                    _contentTypes.Add(value);
                }

            }
        }

        public static BlobContentType GetContentType(string extension)
        {
            extension = extension.ToLower().Trim().TrimStart('.');
            if (ExtensionMap.TryGetValue(extension, out var contentType))
                return contentType;
            return Unknown;
        }

        public static bool Exists(string contentType) => _contentTypes.Contains(contentType);
    }
}
