using System;
using System.Security;
using System.Runtime.InteropServices;

namespace EasyBot.PInvoke.GDI32
{
    [SuppressUnmanagedCodeSecurity]
    public static class Gdi32
    {
        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll")]
        public static extern int DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern int DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern int BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

        [DllImport("gdi32.dll")]
        public static extern int GetDIBits(IntPtr hdc, IntPtr hBitmap, uint start, uint lines, IntPtr lpvBits, IntPtr lpBitmapInfo, uint usage);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateDIBSection(IntPtr hdc, IntPtr bitmapInfo, uint usage, ref IntPtr bits, IntPtr section, uint offset);
    }
}
