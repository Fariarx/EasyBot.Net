using System;
using System.Runtime.InteropServices;

namespace EasyBot.PInvoke.GDI32
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BitmapInfo
    {
        public BitmapInfoHeader Header;
        public RGBQuad Colors;
    }
}
