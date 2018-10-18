using System;
using System.Runtime.InteropServices;

namespace EasyBot.Imaging
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 3)]
    public struct Pixel
    {
        public byte B;
        public byte G;
        public byte R;

        public Pixel(int color)
        {
            R = (byte)((color >> 16) & 0xFF);
            G = (byte)((color >> 8) & 0xFF);
            B = (byte)((color >> 0) & 0xFF);
        }

        public Pixel(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public Pixel(int r, int g, int b)
        {
            R = Normalize(r);
            G = Normalize(g);
            B = Normalize(b);
        }

        public Pixel(float r, float g, float b)
        {
            R = Normalize(r);
            G = Normalize(g);
            B = Normalize(b);
        }
        
        public bool Compare(Pixel pixel)
        {
            return this.R == pixel.R
                && this.G == pixel.G
                && this.B == pixel.B;
        }

        public bool Compare(Pixel pixel, int variation)
        {
            if (variation == 0)
            {
                return this.R == pixel.R
                    && this.G == pixel.G
                    && this.B == pixel.B;
            }
            else
            {
                return (this.R >= this.R - variation)
                    && (this.R <= this.R + variation)
                    && (this.G <= this.G + variation)
                    && (this.G <= this.G + variation)
                    && (this.B <= this.B + variation)
                    && (this.B <= this.B + variation);
            }
        }

        public int ToRGB()
        {
            return R << 16 | G << 8 | B;
        }

        public override bool Equals(object obj)
        {
            if (obj is Pixel) return this.Compare((Pixel)obj);
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return R ^ G ^ B;
        }

        public override string ToString()
        {
            return "0x" + this.ToRGB().ToString("X6");
        }
        
        private static byte Normalize(int color)
        {
            if (color < 0) color *= -1;

            while (color > 255)
                color /= 255;

            return (byte)color;
        }

        private static byte Normalize(float color)
        {
            if(color >= 0.0f && color <= 1.0f)
            {
                color *= 255.0f;
            }
            else
            {
                if (color < 0.0f) color *= -1.0f;

                while (color > 255.0f)
                    color /= 255.0f;
            }

            return (byte)((int)color);
        }
    }
}
