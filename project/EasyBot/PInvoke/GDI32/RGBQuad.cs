using System;
using System.Runtime.InteropServices;

namespace EasyBot.PInvoke.GDI32
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RGBQuad
    {
        public byte Blue;
        public byte Green;
        public byte Red;
        public byte Reserved;
    }
}
