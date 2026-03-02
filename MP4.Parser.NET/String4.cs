using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MP4.Parser.NET
{
    [StructLayout(LayoutKind.Explicit)]
    public struct String4 : IFixedSize
    {
        public static int ByteCount => 4 * sizeof(char);
        [FieldOffset(0 * sizeof(char))]
        private readonly char _a;
        [FieldOffset(1 * sizeof(char))]
        private readonly char _b;
        [FieldOffset(2 * sizeof(char))]
        private readonly char _c;
        [FieldOffset(3 * sizeof(char))]
        private readonly char _d;

        public readonly String4 Parse(ReadOnlySpan<byte> buffer)
        {
            var data = buffer[..ByteCount];
            MemoryMarshal.Read<String4>(data);
            return this;
        }

        public override readonly string ToString() => string.Concat([_a, _b, _c, _d]);
    }
}
