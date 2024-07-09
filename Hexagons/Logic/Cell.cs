using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hexagons.Logic
{
    public class Cell
    {
        public static int CellAnglesCount = 6;

        private Point center;
        private Point[] angles;

        public Cell(Point center, Point[] angles)
        {
            this.center = center;
            this.angles = angles;
        }

        public Point Center => center;
        public Point[] Angles => angles;
    }
}
