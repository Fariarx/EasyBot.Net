using System;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;

using EasyBot.Imaging.FileFormats;

using EasyBot.PInvoke.GDI32;
using EasyBot.PInvoke.USER32;

namespace EasyBot.Imaging
{
    public unsafe class FastBitmap : IDisposable
    {
        private IntPtr _bitmapInfoHeader;
        private IntPtr _dibSection;

        private byte* _pixels;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public int Stride { get; private set; }
        
        public FastBitmap(string filename)
        {
            if (string.IsNullOrEmpty(filename)) throw new ArgumentNullException(nameof(filename));

            var bmp = BitmapFileFormat.FromFile(filename);

            if (!bmp.IsValid) throw new ImagingException("Invalid File Format! " + filename);

            if(bmp.IsUpsideDown) bmp.ReverseScanLines();
            
            byte[] bytes = bmp.GetBytes();

            _bitmapInfoHeader = Marshal.AllocHGlobal(bytes.Length);
            _dibSection = _bitmapInfoHeader + (int)bmp.PixelArrayOffset;

            _pixels = (byte*)_dibSection.ToPointer();
            
            Marshal.Copy(bytes, 0, _bitmapInfoHeader, bytes.Length);

            Width = bmp.ImageWidth;
            Height = Math.Abs(bmp.ImageHeight);

            Stride = (((bmp.ImageWidth * bmp.BitsPerPixel) + 31) / 32) * 4;
        }

        private FastBitmap()
        {

        }

        private FastBitmap(IntPtr bitmapInfoHeader, IntPtr dibSection, int width, int height)
        {
            if (bitmapInfoHeader == IntPtr.Zero) throw new ArgumentOutOfRangeException(nameof(bitmapInfoHeader));
            if (dibSection == IntPtr.Zero) throw new ArgumentOutOfRangeException(nameof(dibSection));

            if (width < 1) throw new ArgumentOutOfRangeException(nameof(width));
            if (height < 1) throw new ArgumentOutOfRangeException(nameof(height));

            _bitmapInfoHeader = bitmapInfoHeader;
            _dibSection = dibSection;

            _pixels = (byte*)dibSection.ToPointer();
            
            Width = width;
            Height = height;

            Stride = (((Width * 24) + 31) / 32) * 4;
        }

        ~FastBitmap()
        {
            Dispose(false);
        }

        public void Save(string filename)
        {
            if (string.IsNullOrEmpty(filename)) throw new ArgumentNullException(nameof(filename));
            if (disposedValue) throw new ObjectDisposedException(nameof(FastBitmap));

            int sizeOfFileHeader = sizeof(BitmapFileHeader);
            int sizeOfInfoHeader = sizeof(BitmapInfoHeader);
            int dibSectionLength = (Width * Height) * 3;

            int realInfoHeaderSize = Marshal.ReadInt32(_bitmapInfoHeader);

            if (realInfoHeaderSize != 0 && sizeOfInfoHeader != realInfoHeaderSize) sizeOfInfoHeader = realInfoHeaderSize;

            BitmapFileHeader fileHeader = new BitmapFileHeader
            {
                OffsetBits = (uint)(sizeOfFileHeader + sizeOfInfoHeader),
                Size = (uint)(dibSectionLength + sizeOfFileHeader + sizeOfInfoHeader),
                Type = 0x4D42
            };

            byte[] bitmap = new byte[sizeOfFileHeader + sizeOfInfoHeader + dibSectionLength];
            
            Marshal.Copy((IntPtr)(void*)&fileHeader, bitmap, 0, sizeOfFileHeader);
            Marshal.Copy(_bitmapInfoHeader, bitmap, sizeOfFileHeader, sizeOfInfoHeader);
            Marshal.Copy(_dibSection, bitmap, sizeOfFileHeader + sizeOfInfoHeader, dibSectionLength);

            File.WriteAllBytes(filename, bitmap);
        }

        public Pixel GetPixel(int x, int y)
        {
            if (x < 0 || x >= Width) throw new ArgumentOutOfRangeException(nameof(x));
            if (y < 0 || y >= Height) throw new ArgumentOutOfRangeException(nameof(y));

            int index = y * Stride + x * 3;

            return *(Pixel*)(_pixels + index);
        }

        public void SetPixel(int x, int y, Pixel pixel)
        {
            if (x < 0 || x >= Width) throw new ArgumentOutOfRangeException(nameof(x));
            if (y < 0 || y >= Height) throw new ArgumentOutOfRangeException(nameof(y));

            int index = y * Stride + x * 3;

            *(Pixel*)(_pixels + index) = pixel;
        }

        public static FastBitmap FromWindow(IntPtr windowHandle)
        {
            if (User32.GetClientRect(windowHandle, out RECT clientRectangle) == 0)
            {
                throw new ImagingException("Failed to retrieves the coordinates of a window's client area! " +
                    nameof(windowHandle) +
                    ": 0x" +
                    windowHandle.ToString("X"));
            }

            int width = clientRectangle.Right;
            int height = clientRectangle.Bottom;

            if (width < 1 || height < 1)
            {
                throw new ImagingException("The retrieved client coordinates where invalid! " +
                    nameof(windowHandle) +
                    ": 0x" +
                    windowHandle.ToString("X"));
            }
            
            IntPtr windowDC = User32.GetDC(windowHandle);

            if(windowDC == IntPtr.Zero)
            {
                throw new ImagingException("Failed to retrieve a handle to a device context (DC) for the client area of the specified window! " +
                    nameof(windowHandle) +
                    ": 0x" +
                    windowHandle.ToString("X"));
            }

            IntPtr memoryDC = Gdi32.CreateCompatibleDC(windowDC);

            if (memoryDC == IntPtr.Zero)
            {
                User32.ReleaseDC(windowHandle, windowDC);

                throw new ImagingException("Failed to create a memory device context (DC) compatible with the specified device! " +
                    nameof(windowHandle) +
                    ": 0x" +
                    windowHandle.ToString("X"));
            }

            IntPtr hBitmap = Gdi32.CreateCompatibleBitmap(windowDC, width, height);

            if (hBitmap == IntPtr.Zero)
            {
                Gdi32.DeleteDC(memoryDC);
                User32.ReleaseDC(windowHandle, windowDC);

                throw new ImagingException("Failed to create a bitmap compatible with the device that is associated with the specified device context! " +
                    nameof(windowHandle) +
                    ": 0x" +
                    windowHandle.ToString("X"));
            }

            Gdi32.SelectObject(memoryDC, hBitmap);

            if (Gdi32.BitBlt(memoryDC, 0, 0, width, height, windowDC, 0, 0, TernaryRasterOperations.SRCCOPY) == 0)
            {
                Gdi32.DeleteDC(memoryDC);
                Gdi32.DeleteObject(hBitmap);
                User32.ReleaseDC(windowHandle, windowDC);

                throw new ImagingException("Failed to perform a bit-block transfer of the color data corresponding to a rectangle of pixels from the specified source device context into a destination device context! " +
                    nameof(windowHandle) +
                    ": 0x" +
                    windowHandle.ToString("X"));
            }
            else
            {
                AllocateBitmapMemory(width, height, out IntPtr bmpInfoHeader, out IntPtr dibSection);

                int result = Gdi32.GetDIBits(windowDC, hBitmap, 0, (uint)height, dibSection, bmpInfoHeader, 0);

                Gdi32.DeleteDC(memoryDC);
                Gdi32.DeleteObject(hBitmap);
                User32.ReleaseDC(windowHandle, windowDC);

                if(result == 0)
                {
                    throw new ImagingException("Failed to retrieve the bits of the specified compatible bitmap and copy them into a buffer as a DIB using the specified format! " +
                    nameof(windowHandle) +
                    ": 0x" +
                    windowHandle.ToString("X"));
                }
                else
                {
                    return new FastBitmap(bmpInfoHeader, dibSection, width, height);
                }
            }
        }

        private static void AllocateBitmapMemory(int width, int height, out IntPtr bitmapInfoHeader, out IntPtr dibSection)
        {
            int sizeOfInfoHeader = sizeof(BitmapInfoHeader);

            bitmapInfoHeader = Marshal.AllocHGlobal(sizeOfInfoHeader + (width * height) * 3);
            
            dibSection = bitmapInfoHeader + sizeOfInfoHeader;
            
            *((BitmapInfoHeader*)bitmapInfoHeader.ToPointer()) = new BitmapInfoHeader()
            {
                BitCount = 24,
                Compression = 0,
                Height = height < 0 ? height : -height,
                Planes = 1,
                Size = (uint)sizeOfInfoHeader,
                SizeImage = 0,
                Width = width
            };
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (_bitmapInfoHeader != IntPtr.Zero) Marshal.FreeHGlobal(_bitmapInfoHeader);

                _bitmapInfoHeader = IntPtr.Zero;

                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
