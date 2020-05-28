using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralBuildingsGeneration
{
    public class Geometry2d
    {
        public static IEnumerable<Point2d> CenterPolygon(IList<Point2d> polygon, out Point2d centroid)
        {
            var centroidCopy = centroid = FindCentroid(polygon);
            return polygon.Select(point => new Point2d
            {
                X = point.X - centroidCopy.X,
                Y = point.Y - centroidCopy.Y,
            });
        }

        public static IList<Point2d> ScaleCenteredPolygon(IEnumerable<Point2d> polygon, double scaleFactor)
        {
            return polygon.Select(p => new Point2d
            {
                X = p.X * scaleFactor,
                Y = p.Y * scaleFactor,
            }).ToList();
        }

        // Find the polygon's centroid.
        public static Point2d FindCentroid(IList<Point2d> polygon)
        {
            double X = 0;
            double Y = 0;
            for (int i = 0; i < polygon.Count - 1; i++)
            {
                double commonFactor =
                    polygon[i].X * polygon[i + 1].Y -
                    polygon[i + 1].X * polygon[i].Y;
                X += (polygon[i].X + polygon[i + 1].X) * commonFactor;
                Y += (polygon[i].Y + polygon[i + 1].Y) * commonFactor;
            }

            double commonFactor2 =
                    polygon.Last().X * polygon[0].Y -
                    polygon[0].X * polygon.Last().Y;
            X += (polygon.Last().X + polygon[0].X) * commonFactor2;
            Y += (polygon.Last().Y + polygon[0].Y) * commonFactor2;

            // wikipedia formula
            double polygon_area = Math.Abs(CalcSignedPolygonArea(polygon));
            X /= (6 * polygon_area);
            Y /= (6 * polygon_area);

            // if the polygon is oriented counterclockwise
            if (X < 0)
            {
                X = -X;
                Y = -Y;
            }

            return new Point2d { X = X, Y = Y };
        }

        public static double CalcSignedPolygonArea(IList<Point2d> polygon)
        {
            double area = 0.0f;
            for (int i = 0; i < polygon.Count - 1; ++i)
            {
                area += (polygon[i + 1].X - polygon[i].X) *
                        (polygon[i + 1].Y + polygon[i].Y) / 2.0;
            }
            area += (polygon[0].X - polygon.Last().X) *
                    (polygon[0].Y + polygon.Last().Y) / 2.0;
            return area;
        }
    }
}
