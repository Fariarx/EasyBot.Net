using System;

using EasyBot.Imaging;

namespace EasyBotTest
{
    class Program
    {
        static void Main(string[] args)
        {
            int clr = 0x05CC0A;

            Pixel pixel = new Pixel(clr);

            Console.WriteLine("0x" + clr.ToString("X8"));
            Console.WriteLine(pixel.ToString());

            Console.ReadLine();
        }
    }
}
