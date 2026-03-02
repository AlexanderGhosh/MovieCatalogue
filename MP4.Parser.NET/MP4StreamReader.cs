namespace MP4.Parser.NET
{
    public sealed class MP4StreamReader
    {
        public FileTypeBox ReadBox(ReadOnlySpan<byte> buffer, int offset)
        {
            var boxData = buffer[offset..];
            BoxHeader header = BoxHeader.Parse(buffer);
            FileTypeBox box = default;
            box.Header = header;
            return box;
            //boxData = boxData.Slice(BoxHeader.ByteCount, header.Size);
            //box.CompatibleBrands = boxData;
        }
    }
}
