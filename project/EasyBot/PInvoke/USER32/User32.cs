using System;
using System.Security;
using System.Runtime.InteropServices;

namespace EasyBot.PInvoke.USER32
{
    [SuppressUnmanagedCodeSecurity]
    public static class User32
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern int GetClientRect(IntPtr hWnd, out RECT lpRect);
    }
}
