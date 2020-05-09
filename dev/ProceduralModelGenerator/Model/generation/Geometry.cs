using g3;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProceduralBuildingsGeneration
{
    public class Geometry
    {
        public static IList<Triangle2d> Triangulate(IList<Vector2d> polygon)
        {
            var triangles = new List<Triangle2d>(polygon.Count - 2);
            var polygonCopy = new List<Vector2d>(polygon);
            while(polygonCopy.Count > 3)
            {
                var ear = GetPolygonEar(polygonCopy);
                if (ear == null) throw new ArgumentException("Polygon is not simple");
                triangles.Add(new Triangle2d(
                    polygonCopy[ear.Item1],
                    polygonCopy[ear.Item2],
                    polygonCopy[ear.Item3]));
                polygonCopy.RemoveAt(ear.Item2);
            }
            triangles.Add(new Triangle2d(polygonCopy[0],polygonCopy[1],polygonCopy[2]));
            return triangles;
        }

        public static Tuple<int, int, int> GetPolygonEar(IList<Vector2d> polygon)
        {
            int v1, vm, v2;
            for (v1 = 0; v1 < polygon.Count; v1++)
            {
                vm = (v1 + 1) % polygon.Count;
                v2 = (vm + 1) % polygon.Count;
                if (IsEarOfPolygon(polygon, v1, vm, v2)) return Tuple.Create(v1, vm, v2);
            }
            return null;
        }

        public static bool IsEarOfPolygon(IList<Vector2d> polygon, int v1, int v2, int v3)
        {
            if (CalcAngleInTriangleRad(polygon[v1], polygon[v2], polygon[v3]) > 0.0)
            {
                // concave angle
                return false;
            }
            foreach(var otherVertex in polygon.Where((v, i) => i != v1 && i != v2 && i != v3))
            {
                if (IsPointInTriangle(polygon, v1, v2, v3, otherVertex))
                {
                    return false;
                }
            }
            return true;
        }
        public static bool IsPointInTriangle(IList<Vector2d> polygon, int v1, int v2, int v3, Vector2d point)
        {
            var angleSum = 0.0;
            angleSum += CalcAngleInTriangleRad(point, polygon[v1], polygon[v2]);
            angleSum += CalcAngleInTriangleRad(point, polygon[v2], polygon[v3]);
            angleSum += CalcAngleInTriangleRad(point, polygon[v3], polygon[v1]);
            return Math.Abs(angleSum) > 1;
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

        public static float CalcAngleInTriangleRad(Vector2d samplePoint, Vector2d otherPoint1, Vector2d otherPoint2)
        {
            Vector2d sampleEdge1 = otherPoint1 - samplePoint;
            Vector2d sampleEdge2 = otherPoint2 - samplePoint;
            double cross = sampleEdge1.x * sampleEdge2.y - sampleEdge1.y * sampleEdge2.x;
            return (float)Math.Atan2(cross, sampleEdge1.Dot(sampleEdge2));
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

        public static IList<Vector3d> OffsetPolygon(IList<Vector3d> polygon, double upOffset)
        {
            var polygonCopy = new List<Vector3d>(polygon);
            for (int i = 0; i < polygonCopy.Count; ++i)
            {
                var newPoint = new Vector3d(polygonCopy[i]);
                newPoint.y += upOffset;
                polygonCopy[i] = newPoint;
            }
            return polygonCopy;
        }


    }
}
