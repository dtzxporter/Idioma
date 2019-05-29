using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Idioma
{
    public static class TgaWriter
    {
        public static void Write(System.Windows.Media.Imaging.WriteableBitmap bitmap, Stream output)
        {
            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;
            var bytesPerPixel = (bitmap.Format.BitsPerPixel + 7) / 8;
            var stride = bitmap.PixelWidth * bytesPerPixel;
            var bufferSize = bitmap.PixelHeight * stride;
            var pixels = new int[bufferSize];
            bitmap.CopyPixels(pixels, stride, 0);
            byte[] pixelsArr = new byte[pixels.Length * 4];

            int offsetSource = 0;
            int width4 = width * 4;
            int width8 = width * 8;
            int offsetDest = (height - 1) * width4;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int value = pixels[offsetSource];
                    pixelsArr[offsetDest] = (byte)(value & 255); // b
                    pixelsArr[offsetDest + 1] = (byte)((value >> 8) & 255); // g
                    pixelsArr[offsetDest + 2] = (byte)((value >> 16) & 255); // r
                    pixelsArr[offsetDest + 3] = (byte)(value >> 24); // a

                    offsetSource++;
                    offsetDest += 4;
                }
                offsetDest -= width8;
            }

            byte[] header = new byte[] {
            0, // ID length
            0, // no color map
            2, // uncompressed, true color
            0, 0, 0, 0,
            0,
            0, 0, 0, 0, // x and y origin
            (byte)(width & 0x00FF),
            (byte)((width & 0xFF00) >> 8),
            (byte)(height & 0x00FF),
            (byte)((height & 0xFF00) >> 8),
            32, // 32 bit bitmap
            0 };

            using (BinaryWriter writer = new BinaryWriter(output))
            {
                writer.Write(header);
                writer.Write(pixelsArr);
            }
        }
    }


}
