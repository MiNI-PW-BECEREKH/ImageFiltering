using ImageFiltering.Extensions;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace ImageFiltering.FunctionFilters.Extensions
{
    //TODO: Extend the Color class so you can chain multiple filters in one Iteration -- but should I?
    public static class FunctionFilterExtensions
    {
        public static WriteableBitmap Inversion(this WriteableBitmap readBitmap)
        {
            if (readBitmap == null)
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
                            Color colorToSet;
                            var pixelColor = readBitmap.GetPixel(col, row);

                            var red = Math.Abs(pixelColor.R - 255);
                            var blue = Math.Abs(pixelColor.B - 255);
                            var green = Math.Abs(pixelColor.G - 255);

                            colorToSet = Color.FromArgb(pixelColor.A, red, green, blue);
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


        public static WriteableBitmap BrightnessCorrection(this WriteableBitmap readBitmap, int correctionCoefficient)
        {
            if (readBitmap == null)
                throw new ArgumentNullException("Bitmap is not loaded");

            if (correctionCoefficient > 255 || correctionCoefficient < -255)
                throw new Exception("Correction Coefficient is out of bound");

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
                            Color colorToSet;
                            var pixelColor = readBitmap.GetPixel(col, row);

                            var red = Math.Clamp(pixelColor.R + correctionCoefficient, 0, 255);
                            var blue = Math.Clamp(pixelColor.B + correctionCoefficient, 0, 255);
                            var green = Math.Clamp(pixelColor.G + correctionCoefficient, 0, 255);

                            colorToSet = Color.FromArgb(pixelColor.A, red, green, blue);
                           
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


        public static WriteableBitmap ContrastEnhancement(this WriteableBitmap readBitmap, double contrastCoefficient)
        {
            if (readBitmap == null)
                throw new ArgumentNullException("Bitmap is not loaded");
            //TODO: Ask if this should be validated/checked
            //if (contrastCoefficient < 1)
            //    throw new Exception("Factor value alpha is less than 1");
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
                            Color colorToSet;
                            var pixelColor = readBitmap.GetPixel(col, row);

                            var red = (int)Math.Round(Math.Clamp((pixelColor.R - 128) * contrastCoefficient + 128, 0, 255));
                            var blue = (int)Math.Round(Math.Clamp((pixelColor.B - 128) * contrastCoefficient + 128, 0, 255));
                            var green = (int)Math.Round(Math.Clamp((pixelColor.G - 128) * contrastCoefficient + 128, 0, 255));

                            colorToSet = Color.FromArgb(pixelColor.A, red, green, blue);

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

        public static WriteableBitmap GammaCorrection(this WriteableBitmap readBitmap, double gamma)
        {
            if (readBitmap == null)
                throw new ArgumentNullException("Bitmap is not loaded");
            if (gamma < 0 )
                throw new Exception("Factor value gamma cannot be negative");

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
                            Color colorToSet;
                            var pixelColor = readBitmap.GetPixel(col, row);

                            var red = (int)Math.Round(Math.Pow(pixelColor.R / 255D, gamma) * 255);
                            var blue = (int)Math.Round(Math.Pow(pixelColor.B / 255D, gamma) * 255);
                            var green = (int)Math.Round(Math.Pow(pixelColor.G / 255D, gamma) * 255);

                            colorToSet = Color.FromArgb(pixelColor.A, red, green, blue);

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