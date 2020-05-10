using g3;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProceduralBuildingsGeneration
{
    class MeshUtility
    {
        public static int AddVertex3dFrom2d(DMesh3 mesh, Dictionary<Vector2d, int> vertices, Vector2d candidateV, Vector3f normal)
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

        public static Tuple<int[], int[]> AddTriangleStripBetweenPolygons(DMesh3 mesh, int[] baseVertices, IList<Vector3d> newPolygon)
        {
            if (mesh == null || baseVertices == null || newPolygon == null || baseVertices.Length != newPolygon.Count)
            {
                return null;
            }
            int[] newVertices = new int[newPolygon.Count];
            int[] newTriangles = new int[newPolygon.Count * 2];
            int v;
            for (v = 0; v < newPolygon.Count; ++v)
            {
                newVertices[v] = mesh.AppendVertex(newPolygon[v]);
            }
            int currTriangle = 0;
            for(v = 0; v < newPolygon.Count - 1; ++v)
            {
                newTriangles[currTriangle] = mesh.AppendTriangle(baseVertices[v + 1], baseVertices[v], newVertices[v]);
                newTriangles[currTriangle + 1] = mesh.AppendTriangle(baseVertices[v + 1], newVertices[v], newVertices[v + 1]);
                currTriangle += 2;
            }
            newTriangles[currTriangle] = mesh.AppendTriangle(baseVertices[0], baseVertices[v], newVertices[v]);
            newTriangles[currTriangle + 1] = mesh.AppendTriangle(baseVertices[0], newVertices[v], newVertices[0]);
            return Tuple.Create(newVertices, newTriangles);
        }

        public static Tuple<int[], int[]> FillPolygon(DMesh3 mesh, IList<Vector3d> newPolygon, Vector3f normal)
        {
            var basementTriangles = Geometry.Triangulate(newPolygon
                .Select(p => p.xz).ToList());

            //add vertices and triangles of basement to mesh
            var vertexToIndex = new Dictionary<Vector2d, int>();
            var addedTriangles = new int[basementTriangles.Count];
            int addedTriangleIdx = 0;
            foreach (var triangle in basementTriangles)
            {

                int a0 = AddVertex3dFrom2d(mesh, vertexToIndex, triangle[0], normal);
                int b0 = AddVertex3dFrom2d(mesh, vertexToIndex, triangle[1], normal);
                int c0 = AddVertex3dFrom2d(mesh, vertexToIndex, triangle[2], normal);

                addedTriangles[addedTriangleIdx] = mesh.AppendTriangle(a0, b0, c0);
                addedTriangleIdx++;
            }
            int[] newVertices = vertexToIndex.Values.ToArray();
            return Tuple.Create(newVertices, addedTriangles);
        }
    }
}
