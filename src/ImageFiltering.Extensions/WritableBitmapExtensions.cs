using ImageFiltering.Common.Models;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageFiltering.Extensions
{
    public static class WritableBitmapExtensions
    {
        public static void SetPixel(this WriteableBitmap writeableBitmap, int x, int y, Color color)
        {
            if (y > writeableBitmap.PixelHeight - 1 || x > writeableBitmap.PixelWidth - 1)
                throw new Exception("Position for (x,y) is not in Bitmap");
            if (y < 0 || x < 0)
                throw new Exception("Position for (x,y) is not in Bitmap");

            IntPtr pBackBuffer = writeableBitmap.BackBuffer;
            int stride = writeableBitmap.BackBufferStride;

            unsafe
            {
                byte* pBuffer = (byte*)pBackBuffer.ToPointer();
                int location = y * stride + x * 4;

                pBuffer[location] = color.B;
                pBuffer[location + 1] = color.G;
                pBuffer[location + 2] = color.R;
                pBuffer[location + 3] = color.A;

            }

            writeableBitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
        }
        public static Color GetPixel(this WriteableBitmap writeableBitmap, int x, int y)
        {
            if (y > writeableBitmap.PixelHeight - 1 || x > writeableBitmap.PixelWidth - 1)
                return Color.Empty;
            if (y < 0 || x < 0)
                return Color.Empty;

            IntPtr pBackBuffer = writeableBitmap.BackBuffer;
            int stride = writeableBitmap.BackBufferStride;

            Color colorToReturn;

            unsafe
            {
                byte* pBuffer = (byte*)pBackBuffer.ToPointer();
                int location = y * stride + x * 4;
                colorToReturn = Color.FromArgb(pBuffer[location + 3], pBuffer[location + 2], pBuffer[location + 1], pBuffer[location]);
            }

            return colorToReturn;
        }

        public static Color ComputeKernel(this WriteableBitmap writeableBitmap, Kernel kernel, int col, int row)
        {
            int red = 0;
            int blue = 0;
            int green = 0;
            Color pixelColor = Color.Empty;
            unsafe
            {

                for (int kernelCol = 0; kernelCol < kernel.Width; kernelCol++)
                {
                    for (int kernelRow = 0; kernelRow < kernel.Height; kernelRow++)
                    {
                        //TODO: If you have time after doing everything try MIRRORING for edges
                        var pixelX = col + kernelCol - kernel.Anchor.X;
                        var pixelY = row + kernelRow - kernel.Anchor.Y;
                        //pixelColor = writeableBitmap.GetPixel(pixelX, pixelY);

                        var kernelIntensity = kernel.Matrix[kernelRow, kernelCol];

                        //Extend for out of bound ones

                        if (pixelX < 0)
                            pixelX = 0;

                        if (pixelX >= writeableBitmap.PixelWidth)
                            pixelX = writeableBitmap.PixelWidth - 1;

                        if (pixelY < 0)
                            pixelY = 0;

                        if (pixelY >= writeableBitmap.PixelHeight)
                            pixelY = writeableBitmap.PixelHeight - 1;


                        pixelColor = writeableBitmap.GetPixel(pixelX, pixelY);
                        //Console.WriteLine($"Kernel({kernelCol},{kernelRow})*Bitmap({pixelX},{pixelY})");

                        red += pixelColor.R * kernelIntensity;
                        blue += pixelColor.B * kernelIntensity;
                        green += pixelColor.G * kernelIntensity;

                    }
                }

            }
            var D = kernel.D == 0 ? 1 : kernel.D;
            red = Math.Clamp(red / D + kernel.IntensityOffset, 0, 255);
            blue = Math.Clamp(blue / D + kernel.IntensityOffset, 0, 255);
            green = Math.Clamp(green / D + kernel.IntensityOffset, 0, 255);

            return Color.FromArgb(pixelColor.A, red, green, blue);
        }

    }

}