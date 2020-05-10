using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GeneratorController;

namespace WindowsGeneratorView
{
    public partial class BasementProperties : UserControl
    {
        public BasementProperties(BuildingsViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = m_viewModel = viewModel;
        }

        private void OnPanelLoaded(object sender, RoutedEventArgs e)
        {
            m_polygon = new CustomPolygon(c_canvas);
            var widthThird = c_canvas.ActualWidth / 3.0;
            var heightThird = c_canvas.ActualHeight / 3.0;
            m_polygon.AddPoint(c_canvas, new Point(widthThird, heightThird));
            m_polygon.AddPoint(c_canvas, new Point(widthThird * 2.0, heightThird));
            m_polygon.AddPoint(c_canvas, new Point(widthThird * 2.0, heightThird * 2.0));
            m_polygon.AddPoint(c_canvas, new Point(widthThird, heightThird * 2.0));
            m_polygon.Polygon.Stroke = Brushes.Black;
            m_polygon.Polygon.StrokeThickness = 3;
            m_polygon.Polygon.Fill = Brushes.LightGray;
            
            c_canvas.Children.Add(m_polygon);
            c_canvas.MouseMove += OnCanvasMouseMove_DraggingPoint;

            OnCanvasLostFocus(sender, e);
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
            for (int i = 0; i < m_polygon.Polygon.Points.Count; i++)
            {
                if (FindDistanceToPointSquared(m_polygon.Polygon.Points[i], mouse) < CustomPolygon.ThresholdPointSqrDistance)
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
            if (m_isMovingPoint && m_movingPointIdx >= 0)
            {
                var mouse = e.MouseDevice.GetPosition(c_canvas);

                if (FindPointToSnap(m_movingPointIdx, mouse, out var newSnapPoint) && !Keyboard.IsKeyDown(Key.LeftCtrl))
                    m_polygon.MovePoint(c_canvas, m_movingPointIdx, newSnapPoint);
                else
                    m_polygon.MovePoint(c_canvas, m_movingPointIdx, mouse);
            }
        }

        private void OnCanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!m_isMovingPoint)
            {
                var mouse = e.MouseDevice.GetPosition(c_canvas);
                if (MouseIsOverCornerPoint(mouse, out var hitPoint))
                {
                    if (e.MiddleButton == MouseButtonState.Pressed)
                    {
                        if (m_polygon.Polygon.Points.Count > 3)
                        {
                            m_polygon.RemovePoint(c_canvas, hitPoint);
                        }
                        
                    }
                    else
                    {
                        m_isMovingPoint = true;
                        m_movingPointIdx = hitPoint;
                    }
                }
                else
                {
                    if (IsMouseOverEdge(mouse, out var hitPointIdx1, out var hitPointIdx2, out var closestPoint))
                    {
                        if (e.MiddleButton == MouseButtonState.Pressed)
                            m_polygon.AddPoint(c_canvas, mouse, hitPointIdx2);
                        else if (e.LeftButton == MouseButtonState.Pressed)
                            m_polygon.SelectEdge(hitPointIdx1, hitPointIdx2);
                    }
                }
            }
        }
        private void OnCanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (m_isMovingPoint)
            {
                m_isMovingPoint = false;
            }
        }

        private bool FindPointToSnap(int currentPointIdx, Point mouse, out Point newPointPosition)
        {
            const int SNAP_THRESHOLD = 10;
            var points = m_polygon.Polygon.Points;

            var p1 = mouse;
            newPointPosition = p1;
            bool foundX = false, foundY = false;
            for (int p = 0; p < points.Count; p++)
            {
                if (p == currentPointIdx) continue;
                var p2 = m_polygon.Polygon.Points[p];
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
            var points = m_polygon.Polygon.Points;
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

        private CustomPolygon m_polygon;
        private bool m_isMovingPoint = false;
        private int m_movingPointIdx;
        private BuildingsViewModel m_viewModel;

        private void OnCanvasLostFocus(object sender, RoutedEventArgs e)
        {
            m_viewModel.SelectedSideEndpoint1 = m_polygon.BaseSideEnd1;
            m_viewModel.SelectedSideEndpoint2 = m_polygon.BaseSideEnd2;
            m_viewModel.PolygonPoints = m_polygon.Polygon.Points;
        }
    }
}
