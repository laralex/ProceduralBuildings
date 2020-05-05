using g3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralBuildingsGeneration
{
    class FacadeUtility
    {
        public static void MakeFacadeBody(DMesh3 mesh, BuildingsGenerationParameters buildingParams)
        {
            // is given basement counter clockwise
            if (Geometry.CalcSignedPolygonArea(buildingParams.BasementPoints) < 0.0)
            {
                buildingParams.BasementPoints = buildingParams.BasementPoints.Reverse().ToList();
            }

            var properBasementPoints = ScaleCenteredPolygon(
                CenterPolygon(buildingParams.BasementPoints, out var basementCentroid),
                buildingParams.BasementLengthPerUnit
            );

            var basementTriangles = Geometry.Triangulate(properBasementPoints);

            // add vertices and triangles of basement to mesh
            var vertexToIndex = new Dictionary<Vector2d, int>();
            var addedTriangles = new int[basementTriangles.Count];
            int addedTriangleIdx = 0;
            foreach (var triangle in basementTriangles)
            {
                Vector3f upNormal = Vector3f.AxisY;

                int a0 = AddVertex3dFrom2d(mesh, vertexToIndex, triangle[0], -upNormal);
                int b0 = AddVertex3dFrom2d(mesh, vertexToIndex, triangle[1], -upNormal);
                int c0 = AddVertex3dFrom2d(mesh, vertexToIndex, triangle[2], -upNormal);

                addedTriangles[addedTriangleIdx] = mesh.AppendTriangle(c0, b0, a0);
                addedTriangleIdx++;
            }

            var heightExtruder = new MeshExtrudeMesh(mesh);
            heightExtruder.ExtrudedPositionF = (pos, normal, idx) =>
            {
                return pos + Vector3d.AxisY * buildingParams.BasementExtrudeHeight;
            };
            heightExtruder.Extrude();
        }

        private static int AddVertex3dFrom2d(DMesh3 mesh, Dictionary<Vector2d, int> vertices, Vector2d candidateV, Vector3f normal)
        {
            if (!vertices.ContainsKey(candidateV))
            {
                return vertices[candidateV] =
                        mesh.AppendVertex(new NewVertexInfo
                        {
                            v = new Vector3d(candidateV.x, 0.0, candidateV.y),
                            n = normal,
                        });
            }
            return vertices[candidateV];
        }

        private static IEnumerable<Vector2d> CenterPolygon(IList<Point2d> polygon, out Vector2d centroid)
        {
            var centroidCopy = centroid = Geometry.FindCentroid(polygon);
            return polygon.Select(point => new Vector2d
            {
                x = point.X - centroidCopy.x,
                y = point.Y - centroidCopy.y,
            });
        }

        private static IList<Vector2d> ScaleCenteredPolygon(IEnumerable<Vector2d> polygon, double scaleFactor)
        {
            return polygon.Select(p => new Vector2d
            {
                x = p.x * scaleFactor,
                y = p.y * scaleFactor,
            }).ToList();
        }
    }
}
