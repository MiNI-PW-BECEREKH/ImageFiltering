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
        public static List<Color> DivideAndCentroid(this List<Color> list, int aimedNumberOfColors)
        {
            var result = new List<Color>();
            Queue<List<Color>> cuboidQ = new();
            cuboidQ.Enqueue(list);
            while(cuboidQ.Count < aimedNumberOfColors)
            {
                var cuboid = cuboidQ.Dequeue();
                var sorted = cuboid.SortWithChannelCode();
                var leftCuboid = sorted.Take(sorted.Count / 2).ToList();
                var rightCuboid = sorted.Skip(sorted.Count / 2).ToList();
                cuboidQ.Enqueue(leftCuboid);
                cuboidQ.Enqueue(rightCuboid);
            }
            
            //Find Centroids
            foreach(var cuboid in cuboidQ)
            {
                int meanR = 0, meanG = 0, meanB = 0;
                foreach(var pixel in cuboid)
                {
                    meanR += pixel.R;
                    meanG += pixel.G;
                    meanB += pixel.B;
                }
                result.Add(Color.FromArgb(255, meanR/ cuboid.Count, meanG/cuboid.Count, meanB/cuboid.Count));
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
