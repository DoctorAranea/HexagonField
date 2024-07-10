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
        private CellAngle[] angles;

        public Cell(Point center, CellAngle[] angles)
        {
            this.center = center;
            this.angles = angles;
        }

        public Point Center => center;
        public IEnumerable<CellAngle> Angles => angles;
        public IEnumerable<Point> Coords => angles.Select(x => x.Coords);
        public IEnumerable<Point> OffsetCoords => angles.Select(x => x.OffsetCoords);

        public static CellAngle[] GetHexagonalAngles(Point center)
        {
            double r = HexagonsField.CELLSIZE / 2;
            double angle = -Math.PI * 0.5;
            int count = Cell.CellAnglesCount;
            CellAngle[] angles = new CellAngle[count];
            for (int i = 0; i < count; i++)
            {
                angles[i] = new CellAngle(new Point(
                    center.X + (int)Math.Round(Math.Cos(angle + Math.PI * 2.0 * i / count) * r),
                    center.Y + (int)Math.Round(Math.Sin(angle + Math.PI * 2.0 * i / count) * r)
                ));
            }

            for (int i = 0; i < count; i++)
            {
                CellAngle angleP = angles[i];
                if (angleP.Coords.Y != center.Y)
                {
                    int comrY = (int)Math.Abs(((center.Y - angleP.Coords.Y) / HexagonsField.FieldCompressureY));
                    angleP.Coords = new Point(angleP.Coords.X, angleP.Coords.Y < center.Y ? center.Y + -comrY : center.Y + comrY);
                }
            }

            return angles;
        }

        public IEnumerable<(CellAngle, CellAngle)> GetLines()
        {
            yield return (angles[5], angles[4]);
            yield return (angles[0], angles[3]);
            yield return (angles[1], angles[2]);
        }
    }
}
