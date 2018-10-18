using System;
using System.Runtime.InteropServices;

namespace EasyBot.PInvoke.GDI32
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BitmapInfoHeader
    {
        public uint Size;
        public int Width;
        public int Height;
        public ushort Planes;
        public ushort BitCount;
        public uint Compression;
        public uint SizeImage;
        public int PixelsPerMeterX;
        public int PixelsPerMeterY;
        public uint ColorsUsed;
        public uint ColorsImportant;
    }
}
