using ImageFiltering.Common.Models;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace ImageFiltering.Extensions
{
    public static class ConvolutionFilterExtensions
    {
        public static WriteableBitmap Convolution(this WriteableBitmap readBitmap, Kernel kernel)
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


        public static WriteableBitmap ConvolutionMedian(this WriteableBitmap readBitmap)
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

                    unsafe
                    {
                        for (int col = 0; col < width; col++)
                        {
                            for (int row = 0; row < height; row++)
                            {
                                Color pixelColor = Color.Empty;
                                List<int> redChannelValues = new();
                                List<int> blueChannelValues = new();
                                List<int> greenChannelValues = new();


                                for (int kernelCol = 0; kernelCol < 3; kernelCol++)
                                {
                                    for (int kernelRow = 0; kernelRow < 3; kernelRow++)
                                    {
                                        //TODO: If you have time after doing everything try MIRRORING for edges
                                        var pixelX = col + kernelCol - 1;
                                        var pixelY = row + kernelRow - 1;
                                        //pixelColor = writeableBitmap.GetPixel(pixelX, pixelY);

                                        //var kernelIntensity = kernel.Matrix[kernelRow, kernelCol];

                                        //Extend for out of bound ones

                                        if (pixelX < 0)
                                            pixelX = 0;

                                        if (pixelX >= readBitmap.PixelWidth)
                                            pixelX = readBitmap.PixelWidth - 1;

                                        if (pixelY < 0)
                                            pixelY = 0;

                                        if (pixelY >= readBitmap.PixelHeight)
                                            pixelY = readBitmap.PixelHeight - 1;


                                        pixelColor = readBitmap.GetPixel(pixelX, pixelY);
                                        //Console.WriteLine($"Kernel({kernelCol},{kernelRow})*Bitmap({pixelX},{pixelY})");
                                        redChannelValues.Add(pixelColor.R);
                                        blueChannelValues.Add(pixelColor.B);
                                        greenChannelValues.Add(pixelColor.G);


                                    }
                                }
                                redChannelValues.Sort();
                                int red = redChannelValues[redChannelValues.Count / 2];
                                blueChannelValues.Sort();
                                int blue = blueChannelValues[blueChannelValues.Count / 2];
                                greenChannelValues.Sort();
                                int green = greenChannelValues[greenChannelValues.Count / 2];
                                //here get median

                                writeBitmap.SetPixel(col,row,Color.FromArgb(red,green,blue));
                            }
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