namespace MP4.Parser.NET
{
    public interface IBox
    {
        public BoxHeader Header { get; }
    }

    public interface IFixedSize
    {
        public static int ByteCount => throw new NotImplementedException();
    }

    public interface IFromSpan<T>
        where T : IFromSpan<T>
    {
        public static T Parse(ReadOnlySpan<byte> buffer) => throw new NotImplementedException();
    }

}
