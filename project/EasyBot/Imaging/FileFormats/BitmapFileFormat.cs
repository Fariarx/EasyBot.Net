using System;
using System.IO;

namespace EasyBot.Imaging.FileFormats
{
    public class BitmapFileFormat
    {
        public bool IsUpsideDown => ImageHeight > 0;

        public bool IsValid { get; private set; }

        public ushort Signature;
        public uint FileSize;
        public uint Reserved;
        public uint PixelArrayOffset;

        public uint DIBHeaderSize;

        public int ImageWidth;
        public int ImageHeight;

        public ushort Planes;
        public ushort BitsPerPixel;

        public uint Compression;
        public uint ImageSize;

        public int XPixelsPerMeter;
        public int YPixelsPerMeter;

        public uint ColorsInColorTable;
        public uint ImportantColorCount;

        public byte[] ColorTable { get; private set; }

        public byte[] PixelArray { get; private set; }

        public void ReverseScanLines()
        {
            int stride = (((ImageWidth * BitsPerPixel) + 31) / 32) * 4;

            if(stride == 0) stride = BitsPerPixel < 8 ? 1 : BitsPerPixel / 8;

            byte[] buffer = new byte[PixelArray.Length];

            for(int i = (buffer.Length) - stride; i >= 0; i -= stride)
            {
                Buffer.BlockCopy(PixelArray, i, buffer, (buffer.Length - i) - stride, stride);
            }

            ImageHeight = -ImageHeight;
            PixelArray = buffer;
        }
        
        public void FixFormat()
        {
            IsValid = true;

            Signature = 0x4D42;
            Planes = 1;
            Reserved = 0;

            uint calculatedImageSize = (uint)((ImageWidth * ImageHeight) * (BitsPerPixel / 8));

            if (calculatedImageSize > ImageSize) ImageSize = calculatedImageSize;

            if(ColorTable != null)
            {
                uint calculatedInfoHeaderSize = (uint)(ColorTable.Length + 40);

                if (calculatedInfoHeaderSize > DIBHeaderSize) DIBHeaderSize = calculatedInfoHeaderSize;

                ColorsInColorTable = (uint)ColorTable.Length;
            }
            else
            {
                ColorTable = null;
                ColorsInColorTable = 0;
                ImportantColorCount = 0;
            }
            
            if(PixelArray != null)
            {
                PixelArrayOffset = (uint)(FileSize - PixelArray.Length);
                ImageSize = (uint)PixelArray.Length;
            }
            else
            {
                PixelArrayOffset = 0;
                ImageSize = 0;
                ImageWidth = 0;
                ImageHeight = 0;
            }
        }

        public void Save(string file)
        {
            if (string.IsNullOrEmpty(file)) throw new ArgumentNullException(nameof(file));

            File.WriteAllBytes(file, GetBytes());
        }

        public byte[] GetBytes()
        {
            byte[] buffer = new byte[FileSize];

            using (var writer = new BinaryWriter(new MemoryStream(buffer)))
            {
                writer.Write(Signature);
                writer.Write(FileSize);
                writer.Write(Reserved);
                writer.Write(PixelArrayOffset);
                writer.Write(DIBHeaderSize);
                writer.Write(ImageWidth);
                writer.Write(ImageHeight);
                writer.Write(Planes);
                writer.Write(BitsPerPixel);
                writer.Write(Compression);
                writer.Write(ImageSize);
                writer.Write(XPixelsPerMeter);
                writer.Write(YPixelsPerMeter);
                writer.Write(ColorsInColorTable);
                writer.Write(ImportantColorCount);

                if(ColorTable != null && ColorTable.Length != 0 && DIBHeaderSize > 40)
                {
                    writer.Write(ColorTable);
                }

                if(PixelArray != null && PixelArray.Length != 0)
                {
                    writer.Write(PixelArray);
                }
            }

            return buffer;
        }

        public static BitmapFileFormat FromMemory(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            if (bytes.Length == 0) throw new ArgumentOutOfRangeException(nameof(bytes));
            
            return FromStream(new MemoryStream(bytes));
        }
        
        public static BitmapFileFormat FromFile(string file)
        {
            if (string.IsNullOrEmpty(file)) throw new ArgumentNullException(nameof(file));

            return FromStream(File.OpenRead(file));
        }

        public static BitmapFileFormat FromStream(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            var bmp = new BitmapFileFormat();

            try
            {
                using (var reader = new BinaryReader(stream))
                {
                    // Bitmap FileHeader
                    bmp.Signature = reader.ReadUInt16();
                    
                    if(bmp.Signature == 0x4D42)
                    {
                        bmp.IsValid = true;
                    }
                    else
                    {
                        return bmp;
                    }

                    bmp.FileSize = reader.ReadUInt32();

                    bmp.Reserved = reader.ReadUInt32();

                    bmp.PixelArrayOffset = reader.ReadUInt32();

                    // BitmapInfoHeader
                    bmp.DIBHeaderSize = reader.ReadUInt32();

                    bmp.ImageWidth = reader.ReadInt32();

                    if (bmp.ImageWidth < 1)
                    {
                        bmp.IsValid = false;
                        return bmp;
                    }

                    bmp.ImageHeight = reader.ReadInt32();

                    bmp.Planes = reader.ReadUInt16();

                    bmp.BitsPerPixel = reader.ReadUInt16();
                    bmp.Compression = reader.ReadUInt32();
                    bmp.ImageSize = reader.ReadUInt32();

                    if(bmp.ImageSize == 0)
                    {
                        bmp.ImageSize = (uint)(((((bmp.ImageWidth * bmp.BitsPerPixel) + 31) & ~31) >> 3) * bmp.ImageHeight);
                    }

                    bmp.XPixelsPerMeter = reader.ReadInt32();
                    bmp.YPixelsPerMeter = reader.ReadInt32();

                    bmp.ColorsInColorTable = reader.ReadUInt32();
                    bmp.ImportantColorCount = reader.ReadUInt32();
                    
                    if (bmp.ColorsInColorTable != 0) 
                    {
                        bmp.ColorTable = reader.ReadBytes((int)(3 * bmp.ColorsInColorTable));
                    }

                    // ignores extended bitmap headers here because we dont need them currently

                    // dib section
                    stream.Position = bmp.PixelArrayOffset;
                    bmp.PixelArray = reader.ReadBytes((int)bmp.ImageSize);
                }
            }
            catch
            {
                bmp.IsValid = false;
            }

            return bmp;
        }
    }
}
