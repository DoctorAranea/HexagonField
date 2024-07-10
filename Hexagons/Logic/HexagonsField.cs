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
        public const int FIELD_WIDTH = 25;
        public const int FIELD_HEIGHT = 20;
        public const int CELLSIZE = 50;

        private Cell chosenCell;
        private Cell[,] cells = new Cell[FIELD_WIDTH, FIELD_HEIGHT];

        private System.Windows.Forms.Timer timer;
        private PictureBox pBox;

        private List<Point> chosenPoints = new List<Point>();

        public HexagonsField() : base()
        {
            DoubleBuffered = true;

            GenerateMap();

            pBox = new PictureBox();
            pBox.Parent = this;
            pBox.Dock = DockStyle.Fill;
            pBox.Paint += PBox_Paint;
            pBox.MouseClick += PBox_MouseClick;

            timer = new System.Windows.Forms.Timer();
            timer.Interval = updatePeriod;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        public static double FieldCompressureY { get; private set; } = 1;

        private List<Cell> Cells
        {
            get
            {
                var returnedList = new List<Cell>();
                for (int y = 0; y < cells.GetLength(1); y++)
                {
                    for (int x = 0; x < cells.GetLength(0); x++)
                    {
                        returnedList.Add(cells[x, y]);
                    }
                }
                return returnedList;
            }
        }

        private void PBox_MouseClick(object sender, MouseEventArgs e)
        {
            ChooseCell(e.Location);
        }

        private void PBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.White);

            Pen linePen = new Pen(Color.Black);
            var rand = new Random();
            bool even = true;

            for (int i = 0; i < Cells.Count; i++)
            {
                if (i % FIELD_WIDTH == 0)
                    even = !even;

                Cell cell = Cells[i];
                Point[] angles = cell.Coords.ToArray();
                double offsetSum = 256 / 2;
                offsetSum += cell.Angles.Sum(x => x.OffsetY);
                if (offsetSum < 0) offsetSum = 0;
                if (offsetSum > 255) offsetSum = 255;
                //g.DrawPolygon(linePen, cell.OffsetCoords.ToArray());
                Rgb rgb = new Rgb() { R = 0, G = 0, B = 255 };
                Hsb hsb = rgb.To<Hsb>();
                hsb.B = 1 - offsetSum.Map(0, 255, 0, 1)/*rand.Next(75, 100) / (double)100*/;
                hsb.S = offsetSum.Map(0, 255, 0, 1)/*rand.Next(75, 100) / (double)100*/;
                Color clr = hsb.ToSystemColor();
                Color totalClr = Color.FromArgb(clr.R, clr.G, rand.Next(180, 220));
                g.FillPolygon(new SolidBrush(totalClr), cell.OffsetCoords.ToArray());

                //Color clr = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
                //for (int j = 0; j < cell.Angles.Count(); j++)
                //    g.FillEllipse(new SolidBrush(clr), new Rectangle(new Point(cell.Angles.ElementAt(j).X - 3, cell.Angles.ElementAt(j).Y - 3), new Size(6, 6)));

            }

            if (chosenCell != null)
                g.FillPolygon(new SolidBrush(Color.Red), chosenCell.OffsetCoords.ToArray());

            //if (chosenPoints.Count > 0)
            //{
            //    for (int i = 0; i < chosenPoints.Count; i++)
            //    {
            //        var chosenPoint = chosenPoints[i];
            //        g.FillEllipse(new SolidBrush(Color.Green), new Rectangle(new Point(chosenPoint.X - 3, chosenPoint.Y - 3), new Size(6, 6)));
            //    }
            //}

            //for (int i = 0; i < Cells.Count; i++)
            //{
            //    var cell = Cells[i];
            //    Point needAngle = cell.Angles.ToArray()[1];
            //    g.FillEllipse(new SolidBrush(Color.Orange), new Rectangle(new Point(needAngle.X - 3, needAngle.Y - 3), new Size(6, 6)));
            //}
        }

        private void ChooseCell(Point point)
        {
            List<Point> points = new List<Point>();
            for (int i = 0; i < Cells.Count; i++)
                points.AddRange(Cells[i].Coords);

            chosenPoints.Clear();
            for (int i = 0; i < 3; i++)
            {
                var chosenPoint = FindClosestPoint(points.ToArray(), point);
                chosenPoints.Add(chosenPoint);
                points.RemoveAll(x => x == chosenPoints[i]);
            }

            chosenCell = ChooseCellsWithTriangle(chosenPoints.ToArray(), Cells.ToArray());
            pBox.Invalidate();
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
            for (int i = 0; i < points.Length; i++)
                fromCells.RemoveAll(x => x.Coords.Where(y => y == points[i]).Count() == 0);
            return fromCells.Count > 0 ? fromCells[0] : null;
        }

        private void GenerateMap()
        {
            double size = CELLSIZE / 2;

            double height = size * 2;
            double width = Math.Sqrt(3) / 2 * height;

            double offsetY = height * 3 / 4 / FieldCompressureY;
            double offsetX = width;

            List<CellAngle> bufAngles = new List<CellAngle>();
            int startFieldY = CELLSIZE + 10;
            for (int y = 0; y < FIELD_HEIGHT; y++)
            {
                int startFieldX = y % 2 != 0 ? CELLSIZE : (int)(CELLSIZE / 1.76);
                for (int x = 0; x < FIELD_WIDTH; x++)
                {
                    int centerX = (int)(startFieldX + offsetX * x);
                    int centerY = (int)(startFieldY + offsetY * y);
                    Point center = new Point(centerX, centerY);
                    CellAngle[] points = Cell.GetHexagonalAngles(center);
                    for (int i = 0; i < points.Length; i++)
                    {
                        var existAngle = bufAngles.FirstOrDefault(z => IsIntInRange(points[i].Coords.X, z.Coords.X - 5, z.Coords.X + 5) && IsIntInRange(points[i].Coords.Y, z.Coords.Y - 5, z.Coords.Y + 5));
                        if (existAngle == null)
                            bufAngles.Add(points[i]);
                        else
                            points[i] = existAngle;
                    }
                    cells[x, y] = new Cell(center, points);
                }
            }
        }

        int animOffsetY = 5;
        int updatePeriod = 300;
        bool up;
        private void Timer_Tick(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                while (true)
                {
                    for (int y = 0; y < cells.GetLength(1); y++)
                    {
                        var line = cells[0, y].GetLines().ElementAt(0);
                        line.Item1.OffsetY = animOffsetY;
                        line.Item2.OffsetY = animOffsetY;
                    }

                    int upSpeed = 15;
                    for (int x = 0; x < cells.GetLength(0); x++)
                    {
                        for (int y = 0; y < cells.GetLength(1); y++)
                        {
                            var line0 = cells[x, y].GetLines().ElementAt(0);
                            var line1 = cells[x, y].GetLines().ElementAt(1);
                            var line2 = cells[x, y].GetLines().ElementAt(2);

                            line0.Item1.OffsetY -= upSpeed;
                            line0.Item2.OffsetY -= upSpeed;

                            line1.Item1.OffsetY -= upSpeed;
                            line1.Item2.OffsetY -= upSpeed;

                            line2.Item1.OffsetY -= upSpeed;
                            line2.Item2.OffsetY -= upSpeed;
                        }

                        Invoke(new Action(() =>
                        {
                            pBox.Invalidate();
                        }));
                        Thread.Sleep(50);
                    }
                }
            }).Start();

            new Thread(() =>
            {
                while (true)
                {
                    int downSpeed = 2;
                    var offsetCells = Cells.Select(z => z.Angles).ToList();
                    for (int i = 0; i < offsetCells.Count; i++)
                    {
                        for (int j = 0; j < offsetCells[i].Count(); j++)
                        {
                            if (offsetCells[i].ElementAt(j).OffsetY < 15)
                            {
                                offsetCells[i].ElementAt(j).OffsetY += downSpeed;
                            }
                        }
                    }
                    Thread.Sleep(50);
                }
            }).Start();

            timer.Stop();
        }

        int stopCount = 0;
        int stopCountMax = 3;
        private void MoveOffset()
        {
            //if (stopCount > 0)
            //{
            //    stopCount--;
            //    return;
            //}

            if (animOffsetY >= 6)
            {
                stopCount = stopCountMax;
                up = true;
            }

            if (animOffsetY <= -12)
            {

                stopCount = stopCountMax;
                up = false;
            }

            animOffsetY += up ? -1 : 1;
        }
    }
}
