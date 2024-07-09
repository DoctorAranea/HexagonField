using ColorMine.ColorSpaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Hexagons.Logic
{
    public class HexagonsField : Control
    {
        public const int FIELD_WIDTH = 8;
        public const int FIELD_HEIGHT = 5;
        public const int CELLSIZE = 100;

        private Cell chosenCell;
        private List<Cell> cells = new List<Cell>();

        private double fieldCompressureY = 1;

        private System.Windows.Forms.Timer timer;
        private PictureBox pBox;

        private List<Point> chosenPoints = new List<Point>();

        public HexagonsField() : base()
        {
            DoubleBuffered = true;

            //fieldRect = new Rectangle(0, 0, FIELD_WIDTH, FIELD_HEIGHT);
            GenerateMap();

            //player = new Player(Color.Red, new Point(4, 3));

            //creations = new List<Creation>();
            //creations.Add(player);

            pBox = new PictureBox();
            pBox.Parent = this;
            pBox.Dock = DockStyle.Fill;
            pBox.Paint += PBox_Paint;
            pBox.MouseClick += PBox_MouseClick;

            timer = new System.Windows.Forms.Timer();
            timer.Interval = 250;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void PBox_MouseClick(object sender, MouseEventArgs e)
        {
            ChooseCell(e.Location);
        }

        private void ChooseCell(Point point)
        {
            List<Point> points = new List<Point>();
            for (int i = 0; i < cells.Count; i++)
                points.AddRange(cells[i].Angles);

            int range = 5;

            chosenPoints.Clear();
            for (int i = 0; i < 3; i++)
            {
                var chosenPoint = FindClosestPoint(points.ToArray(), point);
                chosenPoints.Add(chosenPoint);
                points.RemoveAll(x => IsIntInRange(x.X, chosenPoints[i].X - range, chosenPoints[i].X + range) && IsIntInRange(x.Y, chosenPoints[i].Y - range, chosenPoints[i].Y + range));
            }

            chosenCell = ChooseCellsWithTriangle(chosenPoints.ToArray(), cells.ToArray());
            pBox.Invalidate();
        }

        private void PBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.White);

            Pen linePen = new Pen(Color.Black);
            var rand = new Random();
            for (int i = 0; i < cells.Count; i++)
            {
                Cell cell = cells[i];
                g.DrawPolygon(linePen, cell.Angles);
                Color clr = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
                //for (int j = 0; j < cell.Angles.Length; j++)
                //    g.FillEllipse(new SolidBrush(clr), new Rectangle(new Point(cell.Angles[j].X - 3, cell.Angles[j].Y - 3), new Size(6, 6)));
            }

            if (chosenCell != null)
                g.FillPolygon(new SolidBrush(Color.Red), chosenCell.Angles);
            if (chosenPoints.Count > 0)
            {
                for (int i = 0; i < chosenPoints.Count; i++)
                {
                    var chosenPoint = chosenPoints[i];
                    g.FillEllipse(new SolidBrush(Color.Green), new Rectangle(new Point(chosenPoint.X - 3, chosenPoint.Y - 3), new Size(6, 6)));
                }
            }
        }

        public double DistanceBetweenPoints(Point p1, Point p2) => Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow(p2.Y - p1.Y, 2));

        public Point FindClosestPoint(Point[] searchIn, Point compareTo)
        {
            return searchIn
                .Select(p => new { point = p, distance = DistanceBetweenPoints(p, compareTo) })
                .OrderBy(distances => distances.distance)
                .First().point;
        }

        private bool IsIntInRange(int x, int startRange, int endRange) => x >= startRange && x <= endRange;

        public Cell ChooseCellsWithTriangle(Point[] points, Cell[] cellsArray)
        {
            List<Cell> fromCells = cellsArray.ToList();
            int range = 3;
            for (int i = 0; i < points.Length; i++)
                fromCells.RemoveAll(x => x.Angles.Where(y => IsIntInRange(y.X, points[i].X - range, points[i].X + range) && IsIntInRange(y.Y, points[i].Y - range, points[i].Y + range)).Count() == 0);
            return fromCells.Count > 0 ? fromCells[0] : null;
        }

        public Point[] GetHexagonalPoints(Point center)
        {
            double r = CELLSIZE / 2;
            double angle = -Math.PI * 0.5;
            int count = Cell.CellAnglesCount;
            Point[] points = new Point[count];
            for (int i = 0; i < count; i++)
            {
                points[i] = new Point(
                    center.X + (int)Math.Round(Math.Cos(angle + Math.PI * 2.0 * i / count) * r),
                    center.Y + (int)Math.Round(Math.Sin(angle + Math.PI * 2.0 * i / count) * r)
                );
            }

            for (int i = 0; i < count; i++)
            {
                Point point = points[i];
                if (point.Y != center.Y)
                {
                    int comrY = (int)Math.Abs(((center.Y - point.Y) / fieldCompressureY));
                    points[i].Y = point.Y < center.Y ? center.Y + -comrY : center.Y + comrY;
                }
            }

            return points;
        }

        private void GenerateMap()
        {
            double size = CELLSIZE / 2;

            double height = size * 2;
            double width = Math.Sqrt(3) / 2 * height;

            double offsetY = height * 3 / 4 / fieldCompressureY;
            double offsetX = width;

            int startFieldY = CELLSIZE / 2;
            for (int y = 0; y < FIELD_HEIGHT; y++)
            {
                int startFieldX = y % 2 != 0 ? CELLSIZE : (int)(CELLSIZE / 1.76);
                for (int x = 0; x < FIELD_WIDTH; x++)
                {
                    int centerX = (int)(startFieldX + offsetX * x);
                    int centerY = (int)(startFieldY + offsetY * y);
                    Point center = new Point(centerX, centerY);
                    Point[] points = GetHexagonalPoints(center);
                    cells.Add(new Cell(center, points));
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            pBox.Invalidate();
            timer.Stop();
        }
    }
}
