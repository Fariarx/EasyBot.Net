using System;
using System.Drawing;
using System.Drawing.Imaging;

using EasyBot.Imaging;
using EasyBot.Imaging.FileFormats;

namespace EasyBot
{
    static class Program
    {
        static void Main(string[] args)
        {
            var bmp = BitmapFileFormat.FromFile("red.bmp");

            bmp.ReverseScanLines();

            bmp.Save("other.bmp");

            //using (var bmp = new FastBitmap("red.bmp"))
            //{
            //    Console.WriteLine(bmp.GetPixel(5, 5).Compare(new Pixel(0,0,0)));
            //    Console.WriteLine(bmp.GetPixel(9, 6).ToString());
            //    Console.WriteLine(bmp.GetPixel(2, 9).ToString());
            //    Console.WriteLine(bmp.GetPixel(3, 4).ToString());
            //}

            //Console.ReadLine();
        }
    }
}
