using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WindowsGeneratorView
{
    /// <summary>
    /// Interaction logic for BasementProperties.xaml
    /// </summary>
    public partial class BasementProperties : UserControl
    {
        public BasementProperties()
        {
            InitializeComponent();
        }

        private void OnPanelLoaded(object sender, RoutedEventArgs e)
        {
            BasementPolygon = new CustomPolygon(c_canvas);
            var widthThird = c_canvas.ActualWidth / 3;
            var heightThird = c_canvas.ActualHeight / 3;
            BasementPolygon.AddPoint(c_canvas, new Point(widthThird, heightThird));
            BasementPolygon.AddPoint(c_canvas, new Point(widthThird * 2, heightThird));
            BasementPolygon.AddPoint(c_canvas, new Point(widthThird * 2, heightThird * 2));
            BasementPolygon.AddPoint(c_canvas, new Point(widthThird, heightThird * 2));
            BasementPolygon.Polygon.Stroke = Brushes.Black;
            BasementPolygon.Polygon.StrokeThickness = 3;
            BasementPolygon.Polygon.Fill = Brushes.LightGray;
            
            c_canvas.Children.Add(BasementPolygon);
            c_canvas.MouseMove += OnCanvasMouseMove_DraggingPoint;
        }

        private double FindDistanceToPointSquared(Point pt1, Point pt2)
        {
            double dx = pt1.X - pt2.X;
            double dy = pt1.Y - pt2.Y;
            return dx * dx + dy * dy;
        }

        private double FindDistanceToSegmentSquared(Point pt, Point p1, Point p2, out Point closest)
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            if ((dx == 0) && (dy == 0))
            {
                closest = p1;
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }

            double t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) / (dx * dx + dy * dy);

            if (t < 0)
            {
                closest = new Point(p1.X, p1.Y);
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
            }
            else if (t > 1)
            {
                closest = new Point(p2.X, p2.Y);
                dx = pt.X - p2.X;
                dy = pt.Y - p2.Y;
            }
            else
            {
                closest = new Point(p1.X + t * dx, p1.Y + t * dy);
                dx = pt.X - closest.X;
                dy = pt.Y - closest.Y;
            }
            return dx * dx + dy * dy;
        }

        private bool MouseIsOverCornerPoint(Point mouse, out int hitPointIdx)
        {
            for (int i = 0; i < BasementPolygon.Polygon.Points.Count; i++)
            {
                if (FindDistanceToPointSquared(BasementPolygon.Polygon.Points[i], mouse) < CustomPolygon.ThresholdPointSqrDistance)
                {
                    hitPointIdx = i;
                    return true;
                }
            }
            hitPointIdx = -1;
            return false;
        }

        private void OnCanvasMouseMove_DraggingPoint(object sender, MouseEventArgs e)
        {
            if (IsMovingPoint && MovingPointIdx >= 0)
            {
                var mouse = e.MouseDevice.GetPosition(c_canvas);

                if (FindPointToSnap(MovingPointIdx, mouse, out var newSnapPoint) && !Keyboard.IsKeyDown(Key.LeftCtrl))
                    BasementPolygon.MovePoint(c_canvas, MovingPointIdx, newSnapPoint);
                else
                    BasementPolygon.MovePoint(c_canvas, MovingPointIdx, mouse);
            }
        }

        private void OnCanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsMovingPoint)
            {
                var mouse = e.MouseDevice.GetPosition(c_canvas);
                if (MouseIsOverCornerPoint(mouse, out var hitPoint))
                {
                    if (e.MiddleButton == MouseButtonState.Pressed)
                    {
                        BasementPolygon.RemovePoint(c_canvas, hitPoint);
                    }
                    else
                    {
                        IsMovingPoint = true;
                        MovingPointIdx = hitPoint;
                    }
                }
                else
                {
                    if (IsMouseOverEdge(mouse, out var hitPointIdx1, out var hitPointIdx2, out var closestPoint))
                    {
                        if (e.MiddleButton == MouseButtonState.Pressed)
                            BasementPolygon.AddPoint(c_canvas, mouse, hitPointIdx2);
                        else if (e.LeftButton == MouseButtonState.Pressed)
                            BasementPolygon.SelectEdge(hitPointIdx1, hitPointIdx2);
                    }
                }
            }
        }
        private void OnCanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (IsMovingPoint)
            {
                IsMovingPoint = false;
            }
        }

        private bool FindPointToSnap(int currentPointIdx, Point mouse, out Point newPointPosition)
        {
            const int SNAP_THRESHOLD = 10;
            var points = BasementPolygon.Polygon.Points;

            var p1 = mouse;
            newPointPosition = p1;
            bool foundX = false, foundY = false;
            for (int p = 0; p < points.Count; p++)
            {
                if (p == currentPointIdx) continue;
                var p2 = BasementPolygon.Polygon.Points[p];
                if (!foundX && Math.Abs(p1.X - p2.X) < SNAP_THRESHOLD)
                {
                    newPointPosition.X = p2.X;
                    foundX = true;
                }
                if (!foundY && Math.Abs(p1.Y - p2.Y) < SNAP_THRESHOLD)
                {
                    newPointPosition.Y = p2.Y;
                    foundY = true;
                }
                if (foundX && foundY) return foundX;
            }
            return foundX || foundY;
        }

        private bool IsMouseOverEdge(Point mouse, out int hitPoint1, out int hitPoint2, out Point closestPoint)
        {
            var points = BasementPolygon.Polygon.Points;
            for (int p1 = 0; p1 < points.Count; p1++)
            {
                int p2 = (p1 + 1) % points.Count;

                if (FindDistanceToSegmentSquared(mouse,
                    points[p1], points[p2], out var closest) < CustomPolygon.ThresholdPointSqrDistance)
                {
                    hitPoint1 = p1;
                    hitPoint2 = p2;
                    closestPoint = closest;
                    return true;
                }
            }

            hitPoint1 = -1;
            hitPoint2 = -1;
            closestPoint = new Point(0, 0);
            return false;
        }

        private void OnPreviewTextBoxDecimal(object sender, TextCompositionEventArgs e)
        {
            TextValidators.OnPreviewTextBoxDecimal(sender, e);
        }

        private CustomPolygon BasementPolygon;
        private bool IsMovingPoint = false;
        private int MovingPointIdx;

       
    }

    class CustomPolygon : UIElement
    {
        public static readonly int DotSize = 15;
        public static readonly int ThresholdPointSqrDistance = DotSize * DotSize;
        public Polygon Polygon { get; private set; }
        public List<Ellipse> Corners { get; private set; }
        public Line BaseSideHighlight { get; private set; }
        public float BaseSideLength { get; set; }
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
