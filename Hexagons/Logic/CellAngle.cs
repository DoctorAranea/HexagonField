using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hexagons.Logic
{
    public class CellAngle
    {
        private Point coords;

        public CellAngle(Point coords)
        {
            this.coords = coords;
        }

        public int OffsetX { get; set; }
        public int OffsetY { get; set; }

        public Point Coords { get => coords; set { coords = value; } }
        public Point OffsetCoords { get => new Point(coords.X + OffsetX, coords.Y + OffsetY); }
    }
}
