using System.Reflection;

namespace BlobStorage
{
    public static partial class BlobContentTypes
    {
        public const string Unknown = "application/?";
        public const string MP4 = "application/mp4";
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
                    string? value = field.GetValue(null) as string;
                    if (value is null) continue;
                    _contentTypes.Add(value);
                }

            }
        }

        public static bool Exists(string contentType) => _contentTypes.Contains(contentType);
    }
}
