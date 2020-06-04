using g3;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProceduralBuildingsGeneration
{
    class MeshUtility
    {
        public static int AddDistinctVertex3d(IMeshBuilder meshBuilder, Dictionary<Vector3d, int> vertices, Vector3d candidateV, Vector3f normal)
        {
            if (!vertices.ContainsKey(candidateV))
            {
                return vertices[candidateV] =
                        meshBuilder.AppendVertex(new NewVertexInfo
                        {
                            v = candidateV,
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

        public static (int[], int[]) FillPolygon(IMeshBuilder meshBuilder, IList<Vector3d> newPolygon, Vector3f normal)
        {
            if (meshBuilder == null || newPolygon == null) return (null, null);
            var triangulation = Geometry.Triangulate(newPolygon, normal);
            if (triangulation == null) return (null, null);
            //add vertices and triangles of basement to mesh
            var vertexToIndex = new Dictionary<Vector3d, int>();
            var addedTriangles = new int[triangulation.Count];
            int addedTriangleIdx = 0;
            foreach (var triangle in triangulation)
            {

                int a0 = AddDistinctVertex3d(meshBuilder, vertexToIndex, triangle[0], normal);
                int b0 = AddDistinctVertex3d(meshBuilder, vertexToIndex, triangle[1], normal);
                int c0 = AddDistinctVertex3d(meshBuilder, vertexToIndex, triangle[2], normal);

                addedTriangles[addedTriangleIdx] = meshBuilder.AppendTriangle(a0, b0, c0);
                addedTriangleIdx++;
            }
            int[] newVertices = vertexToIndex.Values.ToArray();
            return (newVertices, addedTriangles);
        }

        public struct Edge
        {
            public Vector3d BeginVertex { get; set; }
            public Vector3d EndVertex { get; set; } 
            public Edge(Vector3d beginVertex, Vector3d endVertex)
            {
                BeginVertex = beginVertex;
                EndVertex = endVertex;
            }
        }
        public static (int, int) FillBetweenEdges(DMesh3Builder meshBuilder, Edge a, Edge b, Vector3f normal)
        {
            //var sideA = a.EndVertex - a.BeginVertex;
            //var sideB = b.EndVertex - b.BeginVertex;
            //var normal = 
            var a0 = meshBuilder.AppendVertex(new NewVertexInfo(a.BeginVertex, normal));
            var a1 = meshBuilder.AppendVertex(new NewVertexInfo(a.EndVertex, normal));
            var b0 = meshBuilder.AppendVertex(new NewVertexInfo(b.BeginVertex, normal));
            var b1 = meshBuilder.AppendVertex(new NewVertexInfo(b.EndVertex, normal));

            return (meshBuilder.AppendTriangle(a0, b1, b0),
                meshBuilder.AppendTriangle(a0, a1, b1));
        }
    }
}
