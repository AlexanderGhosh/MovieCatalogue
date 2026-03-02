using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MP4.Parser.NET
{

    [StructLayout(LayoutKind.Explicit)]
    public struct FileTypeBox : IBox
    {
        [FieldOffset(0)]
        private BoxHeader _header;
        [FieldOffset(8)]
        private uint _majorBrand;
        [FieldOffset(12)]
        private uint _minorVersion;
        [FieldOffset(16)]
        private String4[] _compatibleBrands;

        public BoxHeader Header
        {
            readonly get => _header;
            set => _header = value;
        }
        public uint MajorBrand
        {
            readonly get => _majorBrand;
            set => _majorBrand = value;
        }
        public uint MinorVersion
        {
            readonly get => _minorVersion;
            set => _minorVersion = value;
        }
        public String4[] CompatibleBrands
        {
            readonly get => _compatibleBrands;
            set => _compatibleBrands = value;
        }
    }
}
