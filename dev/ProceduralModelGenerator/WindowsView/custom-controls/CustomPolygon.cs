using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WindowsGeneratorView
{
    class CustomPolygon : UIElement
    {
        public static readonly int DotSize = 15;
        public static readonly int ThresholdPointSqrDistance = DotSize * DotSize;
        public Polygon Polygon { get; private set; }
        public List<Ellipse> Corners { get; private set; }
        public Line BaseSideHighlight { get; private set; }
        public int BaseSideEnd1 { get; private set; }
        public int BaseSideEnd2 { get; private set; }

        public CustomPolygon(Canvas canvas)
        {
            Polygon = new Polygon();
            Corners = new List<Ellipse>();
            BaseSideHighlight = new Line();
            BaseSideHighlight.Stroke = Brushes.Red;
            BaseSideHighlight.StrokeThickness = 4;
            canvas.Children.Add(Polygon);
            canvas.Children.Add(BaseSideHighlight);
        }
        public void MovePoint(Canvas canvas, int pointIdx, Point destination)
        {
            Polygon.Points[pointIdx] = destination;
            Corners[pointIdx].Margin = new Thickness(destination.X - DotSize / 2, destination.Y - DotSize / 2, 0, 0);
            if (pointIdx == BaseSideEnd1)
            {
                BaseSideHighlight.X1 = destination.X;
                BaseSideHighlight.Y1 = destination.Y;
            }
            else if (pointIdx == BaseSideEnd2)
            {
                BaseSideHighlight.X2 = destination.X;
                BaseSideHighlight.Y2 = destination.Y;
            }
        }

        public void AddPoint(Canvas canvas, Point location, int pointIdx = -1)
        {
            Ellipse dot = new Ellipse();
            dot.Stroke = new SolidColorBrush(Colors.Black);
            dot.StrokeThickness = 1;
            dot.Height = DotSize;
            dot.Width = DotSize;
            dot.Fill = new SolidColorBrush(Colors.Yellow);
            dot.Margin = new Thickness(location.X - DotSize / 2, location.Y - DotSize / 2, 0, 0);
            if (pointIdx == -1)
            {
                Corners.Add(dot);
                Polygon.Points.Add(location);
            }
            else
            {
                Corners.Insert(pointIdx, dot);
                Polygon.Points.Insert(pointIdx, location);
            }
            canvas.Children.Add(dot);
            DeselectEdge();
        }

        public void RemovePoint(Canvas canvas, int pointIdx)
        {
            if (pointIdx >= 0 && pointIdx < Corners.Count)
            {
                canvas.Children.Remove(Corners[pointIdx]);
                Corners.RemoveAt(pointIdx);
                Polygon.Points.RemoveAt(pointIdx);
                DeselectEdge();
            }
        }

        public void SelectEdge(int endPointIdx1, int endPointIdx2)
        {
            BaseSideEnd1 = endPointIdx1;
            BaseSideEnd2 = endPointIdx2;
            var p1 = Polygon.Points[endPointIdx1];
            var p2 = Polygon.Points[endPointIdx2];
            BaseSideHighlight.StrokeThickness = 4;
            BaseSideHighlight.X1 = p1.X;
            BaseSideHighlight.Y1 = p1.Y;
            BaseSideHighlight.X2 = p2.X;
            BaseSideHighlight.Y2 = p2.Y;
        }

        public void DeselectEdge()
        {
            BaseSideHighlight.StrokeThickness = 0;
            BaseSideEnd1 = -1;
            BaseSideEnd2 = -1;
        }
    }
}
