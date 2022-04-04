using ImageFiltering.Extensions;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace ImageFiltering.MedianCutQuantization
{
    public static class MedianCut
    {
        public static WriteableBitmap MedianCutQuantization(this WriteableBitmap bitmap, int colorsInResult)
        {
            if (bitmap == null)
                throw new ArgumentNullException("Bitmap is not loaded");
            //if (Math.Ceiling(Math.Log(colorsInResult, 2.0)) != Math.Floor(Math.Log(colorsInResult, 2.0)))
            //    throw new ArgumentException("Only powers of 2 supported");

            var writeBitmap = bitmap.Clone();

            var listOfPixels = bitmap.ToList();
            //var requestedDepth = (int)Math.Sqrt(colorsInResult);
            //Get Sections that are still in the same list sorted according to cut
            var pixels = listOfPixels.DivideAndCentroid(colorsInResult);
            


            try
            {
                bitmap.Lock();
                writeBitmap.Lock();

                unsafe
                {
                    var width = bitmap.PixelWidth;
                    var height = bitmap.PixelHeight;

                    for (int col = 0; col < width; col++)
                    {
                        for (int row = 0; row < height; row++)
                        {
                            var pixel = bitmap.GetPixel(col, row);
                            Color color = pixel.FindCuboid(pixels);
                            writeBitmap.SetPixel(col, row, color);
                        }
                    }

                }

            }
            finally
            {
                bitmap.Unlock();
                writeBitmap.Unlock();
            }
            return writeBitmap;
        }

    }
}