using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageFiltering.Common
{
    public class Interval
    {
        public List<int> ColorList = new();
        private int min;
        public int Min
        {
            get { return min; }
            set
            {
                value = Math.Clamp(value, 0, 255);
                min = value;
            }
        }

        private int max;
        public int Max
        {
            get { return max; }
            set {
                value = Math.Clamp(value,0, 255);
                max = value;
            }
        }
        public int Average { get; set; }


        public void SetAverage()
        {
            var sum = ColorList.Sum();
            var count = ColorList.Count == 0 ? 1 : ColorList.Count;
            Average = sum / count;
        }

        public int AssesValue(int value)
        {
            return value >= Average ? Max : Min;
        }
    }
}
