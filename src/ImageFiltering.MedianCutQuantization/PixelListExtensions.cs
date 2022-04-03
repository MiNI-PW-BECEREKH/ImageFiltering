using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageFiltering.MedianCutQuantization
{
    public enum RGB
    {
        R, G, B
    }
    public static class PixelListExtensions
    {
        public static List<Color> Divide(this List<Color> list, int currentDepth, int maxDepth)
        {
            List<Color> result = new List<Color>();
            if (currentDepth == maxDepth)
            {
                //if we have colors as much as in the resulting image return as it is
                return list;
            }
            else if (currentDepth < maxDepth)
            {
                //if we need more divisions to have less colors divide
                var sorted = list.SortWithChannelCode();
                var leftHalf = sorted.Take(sorted.Count / 2).ToList();
                var rightHalf = sorted.Skip(sorted.Count / 2).ToList();
                result.AddRange(leftHalf.Divide(currentDepth + 1, maxDepth));
                result.AddRange(rightHalf.Divide(currentDepth + 1, maxDepth));
            }
            return result;

        }


        public static List<Color> SortWithChannelCode(this List<Color> list)
        {
            //Get the channel with greatest tone range
            var priority = list.GetPriorityChannel();


            List<Color> sorted = new();
            switch (priority)
            {
                case RGB.R:
                    sorted = list.OrderBy(pixel => pixel.R).ToList();
                    break;
                case RGB.G:
                    sorted = list.OrderBy(pixel => pixel.G).ToList();
                    break;
                case RGB.B:
                    sorted = list.OrderBy(pixel => pixel.B).ToList();
                    break;
            }
            return sorted;
        }

        public static RGB GetPriorityChannel(this List<Color> list)
        {
            //Get the channel with greatest tone range
            int minR = 255;
            int minG = 255;
            int minB = 255;
            int maxR = 0;
            int maxG = 0;
            int maxB = 0;
            foreach (var pixel in list)
            {
                if (pixel.R < minR) minR = pixel.R;
                if (pixel.R > maxR) maxR = pixel.R;
                if (pixel.G < minG) minG = pixel.G;
                if (pixel.G > maxG) maxG = pixel.G;
                if (pixel.B > maxB) maxB = pixel.B;
                if (pixel.B < minB) minB = pixel.B;
            }

            int code = 0;
            int rRange = maxR - minR;
            int gRange = maxG - minG;
            int bRange = maxB - minB;
            if (rRange > gRange && rRange > bRange) code = 0;
            else if (gRange > rRange && gRange > bRange) code = 1;
            else if (bRange > rRange && bRange > gRange) code = 2;
            return (RGB)code;
        }

    }
}
