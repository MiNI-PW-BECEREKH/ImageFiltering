using ImageFiltering.Common;
using ImageFiltering.Extensions;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace ImageFiltering.Dithering
{
    public static class DitheringExtensions
    {
        public static WriteableBitmap AverageDithering(this WriteableBitmap readBitmap, int rK, int bK, int gK, int K = 0, bool isGray = false)
        {
            if (readBitmap == null)
                throw new ArgumentNullException("Bitmap is not loaded");
            if (rK < 2 || bK < 2 || gK < 2)
                throw new ArgumentOutOfRangeException("Range is not bounded by a second number");

            if (isGray)
            {
                if (K < 2)
                    throw new ArgumentOutOfRangeException("Range is not bounded by a second number");

                readBitmap = readBitmap.GrayScale();
                rK = bK = gK = K;

            }

            var writeBitmap = readBitmap.Clone();
            try
            {
                readBitmap.Lock();
                writeBitmap.Lock();

                unsafe
                {
                    var width = readBitmap.PixelWidth;
                    var height = readBitmap.PixelHeight;

                    var (redIntervals, greenIntervals, blueIntervals) = readBitmap.GetChannelIntervals(rK, bK, gK);

                    for (int col = 0; col < width; col++)
                    {
                        for (int row = 0; row < height; row++)
                        {
                            var pixelColor = readBitmap.GetPixel(col, row);
                            var (iR, iG, iB) = pixelColor.FindPixelInterval(redIntervals, greenIntervals, blueIntervals);
                            var red = redIntervals[iR].AssesValue(pixelColor.R);
                            var green = greenIntervals[iG].AssesValue(pixelColor.G);
                            var blue = blueIntervals[iB].AssesValue(pixelColor.B);

                            var colorToSet = Color.FromArgb(red, green, blue);
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

        public static (List<Interval> redIntervals, List<Interval> greenIntervals, List<Interval> blueIntervals) GetChannelIntervals(this WriteableBitmap bitmap, int rK, int bK, int gK)
        {
            var actualRedIntervalCount = rK - 1;
            var actualBlueIntervalCount = bK - 1;
            var actualGreenIntervalCount = gK - 1;
            var redIntervals = new List<Interval>(actualRedIntervalCount);
            var greenIntervals = new List<Interval>(actualGreenIntervalCount);
            var blueIntervals = new List<Interval>(actualBlueIntervalCount);

            var bIntervalSize = 255 / actualBlueIntervalCount;
            var gIntervalSize = 255 / actualGreenIntervalCount;
            var rIntervalSize = 255 / actualRedIntervalCount;


            //populate each interval with min and max values
            //TODO: Handle case for ODD when Ks are odd number
            for (int i = 0; i < actualRedIntervalCount; i++)
            {
                redIntervals.Add(new Interval());

                redIntervals[i].Min = i * rIntervalSize;
                redIntervals[i].Max = (i + 1) * rIntervalSize + 1;
            }

            for (int i = 0; i < actualBlueIntervalCount; i++)
            {
                blueIntervals.Add(new Interval());

                blueIntervals[i].Min = i * bIntervalSize;
                blueIntervals[i].Max = (i + 1) * bIntervalSize + 1;
            }

            for (int i = 0; i < actualGreenIntervalCount; i++)
            {
                greenIntervals.Add(new Interval());


                greenIntervals[i].Min = i * gIntervalSize;
                greenIntervals[i].Max = (i + 1) * gIntervalSize + 1;
            }



            try
            {
                bitmap.Lock();
                var bitmapColors = bitmap.ToList();
                //for each pixel find the interval
                foreach (var pixel in bitmapColors)
                {
                    var (iR, iG, iB) = pixel.FindPixelInterval(rIntervalSize + 1, gIntervalSize + 1, bIntervalSize + 1);

                    redIntervals[iR].ColorList.Add(pixel.R);
                    greenIntervals[iG].ColorList.Add(pixel.G);
                    blueIntervals[iB].ColorList.Add(pixel.B);

                }

            }
            finally
            {
                bitmap.Unlock();

                for (int i = 0; i < actualRedIntervalCount; i++)
                {
                    redIntervals[i].SetAverage();
                }

                for (int i = 0; i < actualBlueIntervalCount; i++)
                {
                    blueIntervals[i].SetAverage();
                }

                for (int i = 0; i < actualGreenIntervalCount; i++)
                {
                    greenIntervals[i].SetAverage();
                }


            }
            return (redIntervals, greenIntervals, blueIntervals);
        }


    }
}