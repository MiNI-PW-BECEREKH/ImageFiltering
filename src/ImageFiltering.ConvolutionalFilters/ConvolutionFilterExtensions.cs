using ImageFiltering.Common.Models;
using System.Windows.Media.Imaging;

namespace ImageFiltering.Extensions
{
    public static class ConvolutionFilterExtensions
    {
        public static WriteableBitmap Convolution(this WriteableBitmap readBitmap, Kernel kernel)
        {
            if(readBitmap == null)
                throw new ArgumentNullException("Bitmap is not loaded");

            var writeBitmap = readBitmap.Clone();

            try
            {
                readBitmap.Lock();
                writeBitmap.Lock();

                unsafe
                {
                    var width = readBitmap.PixelWidth;
                    var height = readBitmap.PixelHeight;


                    for (int col = 0; col < width; col++)
                    {
                        for (int row = 0; row < height; row++)
                        {
                            //Store results in copy
                            //Console.WriteLine($"==Bitmap({row},{col})==");
                            var colorToSet = readBitmap.ComputeKernel(kernel, col, row);
                            writeBitmap.SetPixel(col, row, colorToSet);
                        }
                    }

                }

            }
            finally
            {
                readBitmap.Unlock();
                writeBitmap.Unlock();

            }

            return writeBitmap;
        }

    }
}