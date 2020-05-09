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

            ////var basementTriangles = Geometry.Triangulate(properBasementPoints);

            //// add vertices and triangles of basement to mesh
            //var vertexToIndex = new Dictionary<Vector2d, int>();
            //var addedTriangles = new int[basementTriangles.Count];
            //int addedTriangleIdx = 0;
            //foreach (var triangle in basementTriangles)
            //{
            //    Vector3f upNormal = Vector3f.AxisY;

            //    int a0 = AddVertex3dFrom2d(mesh, vertexToIndex, triangle[0], -upNormal);
            //    int b0 = AddVertex3dFrom2d(mesh, vertexToIndex, triangle[1], -upNormal);
            //    int c0 = AddVertex3dFrom2d(mesh, vertexToIndex, triangle[2], -upNormal);

            //    addedTriangles[addedTriangleIdx] = mesh.AppendTriangle(c0, b0, a0);
            //    addedTriangleIdx++;
            //}

            //var heightExtruder = new MeshExtrudeMesh(mesh);
            //heightExtruder.ExtrudedPositionF = (pos, normal, idx) =>
            //{
            //    return pos + Vector3d.AxisY * buildingParams.BasementExtrudeHeight;
            //};
            //heightExtruder.Extrude();
        }



    }
}
