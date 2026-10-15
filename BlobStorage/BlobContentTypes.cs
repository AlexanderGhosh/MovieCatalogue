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

        public static bool Exists(string contentType) => _contentTypes.Contains(contentType);
    }
}
