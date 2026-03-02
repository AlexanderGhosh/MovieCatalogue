using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MP4.Parser.NET
{

    [StructLayout(LayoutKind.Explicit)]
    public struct BoxHeader() : IFixedSize, IFromSpan<BoxHeader>
    {
        public static int ByteCount => 2 * sizeof(int);
        [FieldOffset(0)]
        public int _size;
        [FieldOffset(sizeof(int))]
        public String4 _type;
        public int Size
        {
            readonly get => _size;
            set => _size = value;
        }
        public String4 Type
        {
            readonly get => _type;
            set => _type = value;
        }

        public static BoxHeader Parse(ReadOnlySpan<byte> buffer)
        {
            BoxHeader res = default;
            res.Size = BinaryPrimitives.ReadInt32BigEndian(buffer);
            var typeBytes = buffer.Slice(sizeof(int), String4.ByteCount);
            var t = MemoryMarshal.Cast<byte, int>(typeBytes);
            var dst = new Span<int>();
            BinaryPrimitives.ReverseEndianness(t, dst);
            //var a = new ReadOnlySpan<int>(dst);

            //res.Type = MemoryMarshal.Read<String4>(dst);
            return res;
        }
    }
}
