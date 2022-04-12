using ImageFiltering.Common;

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

                if (Y > interval.Min && Y <= interval.Max)
                {
                    iR = i;
                }
            }

            for (int i = 0; i < crIntervals.Count; i++)
            {
                Interval interval = crIntervals[i];

                if (Cr > interval.Min && Cr <= interval.Max)
                {
                    iG = i;
                }
            }

            for (int i = 0; i < cbIntervals.Count; i++)
            {
                Interval interval = cbIntervals[i];

                if (Cb > interval.Min && Cb <= interval.Max)
                {
                    iB = i;
                }
            }

            return (iR, iB, iG);

        }

    }
}