using System;
using System.Runtime.InteropServices;

namespace EasyBot.PInvoke.USER32
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}
