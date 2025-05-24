using ScottPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dkdc_test.Components.utils
{
    public class CustomAxisLimitManager : IAxisLimitManager
    {
        public CoordinateRange GetRangeX(CoordinateRange viewRangeX, CoordinateRange dataRangeX)
        {
            return viewRangeX;
        }

        public CoordinateRange GetRangeY(CoordinateRange viewRangeY, CoordinateRange dataRangeY)
        {
            return viewRangeY;
        }
    }
}
