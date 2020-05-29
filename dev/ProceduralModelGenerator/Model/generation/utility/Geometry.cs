using g3;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProceduralBuildingsGeneration
{
    public class Geometry
    {
        public static IList<Triangle3d> Triangulate(IList<Vector3d> polygon, Vector3d normal)
        {
            var triangles = new List<Triangle3d>(polygon.Count - 2);
            var polygonCopy = new List<Vector3d>(polygon);
            while(polygonCopy.Count > 3)
            {
                var ear = GetPolygonEar(polygonCopy, normal);
                if (ear == null) throw new ArgumentException("Polygon is not simple");
                triangles.Add(new Triangle3d(
                    polygonCopy[ear.Item1],
                    polygonCopy[ear.Item2],
                    polygonCopy[ear.Item3]));
                polygonCopy.RemoveAt(ear.Item2);
            }
            triangles.Add(new Triangle3d(polygonCopy[0], polygonCopy[1], polygonCopy[2]));
            return triangles;
        }

        public static Tuple<int, int, int> GetPolygonEar(IList<Vector3d> polygon, Vector3d normal)
        {
            int v1, vm, v2;
            for (v1 = 0; v1 < polygon.Count; v1++)
            {
                vm = (v1 + 1) % polygon.Count;
                v2 = (vm + 1) % polygon.Count;
                if (IsEarOfPolygon(polygon, normal, v1, vm, v2)) return Tuple.Create(v1, vm, v2);
            }
            return null;
        }

        public static bool IsEarOfPolygon(IList<Vector3d> polygon, Vector3d normal, int v1, int v2, int v3)
        {
            if (CalcAngleInTriangleRad(polygon[v2], polygon[v3], polygon[v1], normal) > 0.0)
            {
                // concave angle
                return false;
            }
            foreach(var otherVertex in polygon.Where((v, i) => i != v1 && i != v2 && i != v3))
            {
                if (IsPointInTriangle(polygon, normal, v1, v2, v3, otherVertex))
                {
                    return false;
                }
            }
            return true;
        }
        public static bool IsPointInTriangle(IList<Vector3d> polygon, Vector3d normal, int v1, int v2, int v3, Vector3d point)
        {
            var angleSum = 0.0;
            angleSum += CalcAngleInTriangleRad(point, polygon[v1], polygon[v2], normal);
            angleSum += CalcAngleInTriangleRad(point, polygon[v2], polygon[v3], normal);
            angleSum += CalcAngleInTriangleRad(point, polygon[v3], polygon[v1], normal);
            return Math.Abs(angleSum) > 1.0;
        }



        public static double CalcAngleInTriangleRad(Vector3d samplePoint, Vector3d otherPoint1, Vector3d otherPoint2, Vector3d normal)
        {
            Vector3d sampleEdge1 = otherPoint1 - samplePoint;
            Vector3d sampleEdge2 = otherPoint2 - samplePoint;
            var dot = sampleEdge1.Dot(sampleEdge2);
            var cross = sampleEdge1.Cross(sampleEdge2);
            var normalsOrientationSign = Math.Sign(cross.Dot(normal));
            var angle = normalsOrientationSign * Math.Acos(dot / (sampleEdge1.Length * sampleEdge2.Length));
            return angle;
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

        public static IEnumerable<Vector3d> CenterPolygon(IList<Vector3d> polygon, out Vector3d centroid)
        {
            var centroid2d = Geometry2d.FindCentroid(
                polygon.Select(p => new Point2d { X = p.x, Y = p.z }).ToList());
            var centroidCopy = centroid = new Vector3d { x = centroid2d.X, y = polygon[0].y, z = centroid2d.Y };
            return polygon.Select(point => point - centroidCopy);
        }

        public static IList<Vector3d> ScaleCenteredPolygon(IEnumerable<Vector3d> polygon, double scaleFactor)
        {
            return polygon.Select(point => point * scaleFactor).ToList();
        }

        public static IList<Vector3d> IntrudePolygon(IList<Vector3d> polygon, double intrude)
        {
            var result = new List<Vector3d>();
            for(int p = 0; p < polygon.Count; ++p)
            {
                var prevP = ((p - 1) + polygon.Count) % polygon.Count;
                var nextP = ((p + 1) + polygon.Count) % polygon.Count;
                var sideDir1 = polygon[nextP] - polygon[p];
                var sideDir2 = polygon[prevP] - polygon[p];
                sideDir1.Normalize();
                sideDir2.Normalize();
                var dirProjection = sideDir1.Dot(sideDir2);
                var innerPoint1 = polygon[p] + sideDir1.UnitCross(Vector3d.AxisY) * intrude;
                var innerPoint2 = polygon[p] + Vector3d.AxisY.UnitCross(sideDir2) * intrude;
                if (Math.Abs(Math.Abs(dirProjection) - 1.0) < 0.00001)
                {
                    result.Add(innerPoint1);
                    continue; // parallel sides
                }
                var sepProjection1 = (innerPoint2 - innerPoint1).Dot(sideDir1);
                var sepProjection2 = (innerPoint2 - innerPoint1).Dot(sideDir2);
                var d1 = (sepProjection1 - dirProjection * sepProjection2) / (1 - dirProjection * dirProjection);
                var d2 = (dirProjection * sepProjection1 - sepProjection2) / (1 - dirProjection * dirProjection);
                var closestPoint1 = innerPoint1 + d1 * sideDir1;
                var closestPoint2 = innerPoint2 + d2 * sideDir2;
                if (!closestPoint1.EpsilonEqual(closestPoint2, 0.00001)) throw new Exception("Non planar polygon");
                result.Add(closestPoint1);
            }
            return result;
        }

    }
}
