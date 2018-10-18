using System;
using System.Runtime.InteropServices;

namespace EasyBot.PInvoke.GDI32
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct BitmapFileHeader
    {
        public ushort Type;
        public uint Size;
        public ushort Reserved1;
        public ushort Reserved2;
        public uint OffsetBits;
    }
}
