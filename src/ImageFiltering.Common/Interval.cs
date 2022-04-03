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
        public int Min { get; set; }
        public int Max { get; set; }
        public int Average { get; set; }


        public void SetAverage()
        {
            var sum = ColorList.Sum();
            Average = sum / ColorList.Count;
        }

        public int AssesValue (int value)
        {
            return value > Average ? Max : Min;
        }
    }
}
