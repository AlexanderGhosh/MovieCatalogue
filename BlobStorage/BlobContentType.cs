using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlobStorage
{
    [JsonConverter(typeof(BlobContentTypeJsonConverter))]
    public readonly struct BlobContentType
    {
        private readonly string _value;

        public BlobContentType(string contentType)
        {
            ArgumentNullException.ThrowIfNull(contentType);
            if (!BlobContentTypes.Exists(contentType))
                throw new ArgumentException($"The content type: {contentType} is not supported.", nameof(contentType));
            _value = contentType;
        }

        public static implicit operator string(BlobContentType contentType) => contentType._value;
        public static implicit operator BlobContentType(string contentType) => new(contentType);
    }
    public class BlobContentTypeJsonConverter : JsonConverter<BlobContentType>
    {
        public override BlobContentType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
                new(reader.GetString()!);

        public override void Write(Utf8JsonWriter writer, BlobContentType value, JsonSerializerOptions options) =>
                writer.WriteStringValue(value);
    }
}
