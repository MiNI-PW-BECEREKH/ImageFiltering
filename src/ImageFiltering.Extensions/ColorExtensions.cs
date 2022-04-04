using ImageFiltering.Common;
using System.Drawing;

namespace ImageFiltering.Extensions
{
    public static class ColorExtensions
    {
        public static Color FindCuboid(this Color pixel, List<Color> colors)
        {
            double smallesDist = Math.Pow(255, 3);
            Color colorToSet = Color.FromArgb(255, 255, 255);
            for (int i = 0; i < colors.Count; i++)
            {
                var distance = pixel.DistanceTo(colors[i]);
                if (distance < smallesDist)
                {
                    smallesDist = distance;
                    colorToSet = colors[i];
                }
            }
            return colorToSet;
        }

        public static double DistanceTo(this Color pixel, Color color)
        {
            return Math.Sqrt(Math.Pow(pixel.R - color.R, 2) + Math.Pow(pixel.G - color.G, 2) + Math.Pow(pixel.B - color.B, 2));
        }

        public static (int iR, int iG, int iB) FindPixelInterval(this Color pixel, int rIntervalSize, int gIntervalSize, int bIntervalSize  )
        {
            return (pixel.R / rIntervalSize,pixel.G / gIntervalSize, pixel.B / bIntervalSize );
        }

        public static (int iR, int iG, int iB) FindPixelInterval(this Color pixel, List<Interval> rIntervals, List<Interval> gIntervals, List<Interval> bIntervals)
        {
            int iR = 0 ,iG = 0, iB = 0;
            for(int i = 0; i < rIntervals.Count; i++)
            {
                Interval interval = rIntervals[i];

                if(pixel.R > interval.Min && pixel.R <= interval.Max)
                {
                    iR = i;
                }
            }

            for (int i = 0; i < gIntervals.Count; i++)
            {
                Interval interval = gIntervals[i];

                if (pixel.G > interval.Min && pixel.G <= interval.Max)
                {
                    iG = i;
                }
            }

            for (int i = 0; i < bIntervals.Count; i++)
            {
                Interval interval = bIntervals[i];

                if (pixel.B > interval.Min && pixel.B <= interval.Max)
                {
                    iB = i;
                }
            }

            return (iR, iG, iB);

        }
    }



}