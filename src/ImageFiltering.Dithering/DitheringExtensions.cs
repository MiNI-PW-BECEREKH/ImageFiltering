using ImageFiltering.Common;
using ImageFiltering.Extensions;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace ImageFiltering.Dithering
{
    public class YCbCr
    {
        public int Y { get; set; }
        public int Cb { get; set; }
        public int Cr { get; set; }


        public (int iY, int iCb, int iCr) FindPixelInterval( List<Interval> yIntervals, List<Interval> cbIntervals, List<Interval> crIntervals)
        {
            int iR = 0, iG = 0, iB = 0;
            for (int i = 0; i < yIntervals.Count; i++)
            {
                Interval interval = yIntervals[i];

                if (this.Y > interval.Min && this.Y < interval.Max)
                {
                    iR = i;
                }
            }

            for (int i = 0; i < crIntervals.Count; i++)
            {
                Interval interval = crIntervals[i];

                if (Cr > interval.Min && Cr < interval.Max)
                {
                    iG = i;
                }
            }

            for (int i = 0; i < cbIntervals.Count; i++)
            {
                Interval interval = cbIntervals[i];

                if (Cb > interval.Min && Cb < interval.Max)
                {
                    iB = i;
                }
            }

            return (iR, iB, iG);

        }

    }
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

                            var colorToSet = Color.FromArgb((int)red, (int)green, (int)blue);
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

            var rP = 1D / actualRedIntervalCount;
            var gP = 1D / actualGreenIntervalCount;
            var bP = 1D / actualBlueIntervalCount;


            //populate each interval with min and max values
            //TODO: Handle case for ODD when Ks are odd number
            for (int i = 0; i < actualRedIntervalCount; i++)
            {
                redIntervals.Add(new Interval());

                redIntervals[i].Min = (int)Math.Floor(i * rP * 255);
                redIntervals[i].Max = (int)Math.Floor((i + 1) * rP * 255);
            }

            for (int i = 0; i < actualBlueIntervalCount; i++)
            {
                blueIntervals.Add(new Interval());

                blueIntervals[i].Min = (int)Math.Floor(i * bP * 255);
                blueIntervals[i].Max = (int)Math.Floor((i + 1) * bP * 255);
            }

            for (int i = 0; i < actualGreenIntervalCount; i++)
            {
                greenIntervals.Add(new Interval());

                greenIntervals[i].Min = (int)Math.Floor(i * gP * 255);
                greenIntervals[i].Max = (int)Math.Floor((i + 1) * gP * 255);
            }



            try
            {
                bitmap.Lock();
                var bitmapColors = bitmap.ToList();
                //for each pixel find the interval
                foreach (var pixel in bitmapColors)
                {
                    var (iR, iG, iB) = pixel.FindPixelInterval(redIntervals , greenIntervals , blueIntervals );

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



        public static WriteableBitmap AverageDitheringLAB(this WriteableBitmap readBitmap, int rK, int bK, int gK, int K = 0, bool isGray = false)
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

                    var (redIntervals, greenIntervals, blueIntervals) = readBitmap.GetChannelIntervalsYCbCr(rK, bK, gK);

                    for (int col = 0; col < width; col++)
                    {
                        for (int row = 0; row < height; row++)
                        {
                            var pixelColor = readBitmap.GetPixel(col, row);

                            var tmp = new YCbCr();
                            tmp.Y = (byte)(pixelColor.R * 0.299 + 0.587 * pixelColor.G + 0.114 * pixelColor.B);
                            tmp.Cb = (byte)(128 - 0.169 * pixelColor.R - 0.331 * pixelColor.G + 0.5 * pixelColor.B);
                            tmp.Cr = (byte)(128 + 0.5 * pixelColor.R - 0.419 * pixelColor.G - 0.081 * pixelColor.B);

                            var (iR, iG, iB) = pixelColor.FindPixelInterval(redIntervals, greenIntervals, blueIntervals);
                            var y = redIntervals[iR].AssesValue(tmp.Y);
                            var cb = greenIntervals[iG].AssesValue(tmp.Cb);
                            var cr = blueIntervals[iB].AssesValue(tmp.Cr);

                            var rr = (int)Math.Clamp(y + 1.402 * (cr - 128), 0, 255);
                            var gg = (int)Math.Clamp(y - 0.344 * (cb - 128) - 0.714 * (cr - 128), 0, 255);
                            var bb = (int)Math.Clamp(y + 1.772 * (cb - 128), 0, 255);
                            var colorToSet = Color.FromArgb(rr, gg, bb);
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


        public static (List<Interval> redIntervals, List<Interval> greenIntervals, List<Interval> blueIntervals) GetChannelIntervalsYCbCr(this WriteableBitmap bitmap, int rK, int bK, int gK)
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

                var YCbCrList = new List<YCbCr>();

                foreach (var pixel in bitmapColors)
                {
                    var tmp = new YCbCr();
                    tmp.Y = (byte)Math.Clamp(pixel.R * 0.299 + 0.587 * pixel.G + 0.114 * pixel.B, 0, 255);
                    tmp.Cb = (byte)Math.Clamp(128 - 0.169 * pixel.R - 0.331 * pixel.G + 0.5 * pixel.B, 0, 255);
                    tmp.Cr = (byte)Math.Clamp(128 + 0.5 * pixel.R - 0.419 * pixel.G - 0.081 * pixel.B, 0, 255);
                    YCbCrList.Add(tmp);

                }
                foreach (var pixel in YCbCrList)
                {
                    var (iR, iG, iB) = pixel.FindPixelInterval(redIntervals, greenIntervals, blueIntervals);


                    redIntervals[iR].ColorList.Add(pixel.Y);
                    greenIntervals[iG].ColorList.Add(pixel.Cb);
                    blueIntervals[iB].ColorList.Add(pixel.Cr);

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