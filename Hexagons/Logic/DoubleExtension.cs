using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hexagons.Logic
{
    public static class DoubleExtension
    {
        public static double Map(this double value, double fromLower, double fromUpper, double toLower, double toUpper)
        {
            return toLower + (value - fromLower) / (fromUpper - fromLower) * (toUpper - toLower);
        }
    }
}
