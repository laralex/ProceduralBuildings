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
            if (polygon.Count <= 2) return null;
            var triangles = new List<Triangle3d>(polygon.Count - 2);
            var polygonCopy = new List<Vector3d>(polygon);
            while(polygonCopy.Count > 3)
            {
                var ear = GetPolygonEar(polygonCopy, normal);
                if (ear == null)
                {
                    return null; throw new ArgumentException("Polygon is not simple");
                }
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

        public static IList<Vector3d> CompressPolygon(IList<Vector3d> polygon, double compressionCoef)
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
                var innerPoint1 = polygon[p] + sideDir1.UnitCross(Vector3d.AxisY) * compressionCoef;
                var innerPoint2 = polygon[p] + Vector3d.AxisY.UnitCross(sideDir2) * compressionCoef;
                
                if (!LinesIntersection3d(
                    new LineDefinition(innerPoint1, sideDir1), 
                    new LineDefinition(innerPoint2, sideDir2), 
                    out var closestPoint1, out var closestPoint2))
                {
                    result.Add(innerPoint1);
                    continue; // parallel sides
                }
                if (!closestPoint1.EpsilonEqual(closestPoint2, 0.00001)) throw new Exception("Non planar polygon");
                result.Add(closestPoint1);
            }
            return result;
        }

        public class LineDefinition
        {
            public Vector3d Point { get; set; }
            public Vector3d UnitDirection { get; set; }
            public LineDefinition(Vector3d point, Vector3d unitDirection)
            {
                Point = point;
                UnitDirection = unitDirection;
            }
        }
        public static bool LinesIntersection3d(LineDefinition lineA, LineDefinition lineB, out Vector3d closestPointA, out Vector3d closestPointB)
        {
            closestPointA = Vector3d.Zero;
            closestPointB = Vector3d.Zero;
            var dirProjection = lineA.UnitDirection.Dot(lineB.UnitDirection);
            if (Math.Abs(Math.Abs(dirProjection) - 1.0) < 0.00001)
            {
                return false; // parallel sides
            }
            var sepProjection1 = (lineB.Point - lineA.Point).Dot(lineA.UnitDirection);
            var sepProjection2 = (lineB.Point - lineA.Point).Dot(lineB.UnitDirection);
            var d1 = (sepProjection1 - dirProjection * sepProjection2) / (1 - dirProjection * dirProjection);
            var d2 = (dirProjection * sepProjection1 - sepProjection2) / (1 - dirProjection * dirProjection);
            closestPointA = lineA.Point + d1 * lineA.UnitDirection;
            closestPointB = lineB.Point + d2 * lineB.UnitDirection;
            return true;
        }
        public static IList<IList<Vector3d>> BreakPolygonSelfIntersection(IList<Vector3d> polygon)
        {
            var splitPolygon = new List<IList<Vector3d>>();
            var lines = new List<LineDefinition>(polygon.Count);
            for(int p = 0; p < polygon.Count; ++p)
            {
                var pNext = (p + 1) % polygon.Count;
                var dirP = polygon[pNext] - polygon[p];
                dirP.Normalize();
                lines.Add(new LineDefinition(polygon[p], dirP));
                splitPolygon.Add(new List<Vector3d>());
            }
            for(int line = 0; line < lines.Count; ++line)
            {
                var prevLine = (line - 1 + lines.Count) % lines.Count;
                var foundSplit = false;
                for (int otherLine = 0; otherLine < lines.Count; ++otherLine)
                {
                    if (otherLine == line || otherLine == prevLine) continue;
                    bool lineNotParallel = LinesIntersection3d(lines[line], lines[otherLine], out var closestLineP, out var closestOtherLineP1);
                    bool prevLineNotParallel = LinesIntersection3d(lines[prevLine], lines[otherLine], out var closestPrevLineP, out var closestOtherLineP2);
                    if (lineNotParallel && closestLineP.EpsilonEqual(closestOtherLineP1, 0.000001) && prevLineNotParallel && closestPrevLineP.EpsilonEqual(closestOtherLineP2, 0.000001))
                    {
                        var otherBegin = lines[otherLine].Point;
                        var otherEnd = lines[(otherLine + 1) % lines.Count].Point;
                        if (otherBegin.EpsilonEqual(closestOtherLineP1, 0.000001) ||
                            otherBegin.EpsilonEqual(closestOtherLineP2, 0.000001) ||
                            otherEnd.EpsilonEqual(closestOtherLineP1, 0.000001) ||
                            otherEnd.EpsilonEqual(closestOtherLineP2, 0.000001)) continue;
                        var firstToOtherBegin = (otherBegin - closestOtherLineP1).Normalized;
                        var firstToOtherEnd = (otherEnd - closestOtherLineP1).Normalized;
                        var secondToOtherBegin = (otherBegin - closestOtherLineP2).Normalized;
                        var secondToOtherEnd = (otherEnd - closestOtherLineP2).Normalized;

                        bool intersectsLineSegment = firstToOtherBegin.EpsilonEqual(-firstToOtherEnd, 0.000001);
                        bool intersectsPrevLineSegment = secondToOtherEnd.EpsilonEqual(-secondToOtherBegin, 0.000001);
                        if (intersectsLineSegment && intersectsPrevLineSegment)
                        {
                            foundSplit = true;
                            splitPolygon[line].Add(closestOtherLineP2);
                            splitPolygon[line].Add(closestOtherLineP1);
                            //lines.RemoveAt(other)
                            break;
                        }
                        else if (!intersectsLineSegment && !intersectsPrevLineSegment) {
                            var curSide = (lines[line].Point - otherBegin).UnitCross(lines[otherLine].UnitDirection);
                            var nextSide = (lines[(line + 1) % lines.Count].Point - otherBegin).UnitCross(lines[otherLine].UnitDirection);
                            var prevSide = (lines[prevLine].Point - otherBegin).UnitCross(lines[otherLine].UnitDirection);
                            var nextAndPrevSameSide = nextSide.EpsilonEqual(prevSide, 0.000001);
                            var curAndNextDiffSide = !nextSide.EpsilonEqual(curSide, 0.000001);
                            var intersectionPointsSameNoHit = firstToOtherBegin.EpsilonEqual(secondToOtherBegin, 0.000001) || firstToOtherEnd.EpsilonEqual(secondToOtherEnd, 0.000001);// || 
                            if (nextAndPrevSameSide && curAndNextDiffSide && !intersectionPointsSameNoHit)
                            {
                                foundSplit = true;
                                splitPolygon[line].Add(otherBegin);
                                splitPolygon[line].Add(otherEnd);
                                break;
                            }
                        }
                        
                    }
                }
                if (!foundSplit)
                {
                    splitPolygon[line].Add(polygon[line]);
                }
            }
            return splitPolygon;
        }

        public static double PointSqrDistanceToSegment(Tuple<Vector3d, Vector3d> line, Vector3d point)
        {
            var lineDir = (line.Item1 - line.Item2).Normalized;
            var pointToEnd1 = line.Item1 - point;
            var pointToEnd2 = line.Item2 - point;
            var shortestPointToLine = (pointToEnd1 - lineDir * lineDir.Dot(pointToEnd1));
            var shortestToEnd1 = (point + shortestPointToLine - line.Item1).Normalized;
            var shortestToEnd2 = (point + shortestPointToLine - line.Item2).Normalized;
            if (shortestToEnd1.EpsilonEqual(-shortestToEnd2, 0.000001))
            {
                return shortestPointToLine.LengthSquared;
            }
            return Math.Min(pointToEnd1.LengthSquared, pointToEnd2.LengthSquared);
            //var sqrDistanceToLine = .LengthSquared;
        }

    }
}
