using ImageFiltering.Extensions;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace ImageFiltering.Dithering
{
    public static class DitheringExtensions
    {
        public static WriteableBitmap AverageDithering(this WriteableBitmap readBitmap, int K)
        {
            if (readBitmap == null)
                throw new ArgumentNullException("Bitmap is not loaded");

            var writeBitmap = readBitmap.Clone();
            var (RedThresholds, GreenThresholds, BlueThresholds, redRange, greenRange, blueRange) = readBitmap.GetBitmapThresholds(K);
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
                            var pixelColor = readBitmap.GetPixel(col, row);

                            //THE WAY I USE TO FIND POSITION IN IMAGE IS WRONG
                            int redIndex = pixelColor.R / redRange;
                            int red = RedThresholds[Math.Clamp(redIndex,0,RedThresholds.Count - 1)];

                            int greenIndex = pixelColor.G / greenRange;
                            int green = GreenThresholds[Math.Clamp(greenIndex, 0, GreenThresholds.Count -1)]; ;

                            int blueIndex = pixelColor.B / blueRange;
                            int blue = BlueThresholds[Math.Clamp(blueIndex, 0, BlueThresholds.Count -1)]; ;

                            //TODO: Ask what should be returned in case of color images
                            writeBitmap.SetPixel(col, row, Color.FromArgb(pixelColor.A  ,red, green, blue));
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