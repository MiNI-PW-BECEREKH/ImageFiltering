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
            if (Math.Ceiling(Math.Log(colorsInResult, 2.0)) != Math.Floor(Math.Log(colorsInResult, 2.0)))
                throw new ArgumentException("Only powers of 2 supported");

            var writeBitmap = bitmap.Clone();

            var listOfPixels = bitmap.ToList();
            var requestedDepth = (int)Math.Sqrt(colorsInResult);
            var pixels = listOfPixels.Divide(0,requestedDepth);
            var sizeOfCuboid = pixels.Count / colorsInResult;
            List<Color> colorsForGivenCuboid = new List<Color>();
            int counter = 0;
            int meanR = 0, meanG = 0, meanB = 0;
            foreach (var pixel in pixels)
            {
                if (counter < sizeOfCuboid - 1)
                {
                    var temp = pixel;
                    meanR += temp.R;
                    meanG += temp.G;
                    meanB += temp.B;
                    counter++;
                }
                else
                {
                    colorsForGivenCuboid.Add(Color.FromArgb(255, meanR / sizeOfCuboid, meanG / sizeOfCuboid, meanB / sizeOfCuboid));
                    meanR = 0; meanG = 0; meanB = 0; counter = 0;
                }
            }



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
                            Color color = pixel.FindCuboid(colorsForGivenCuboid);
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