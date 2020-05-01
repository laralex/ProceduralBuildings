using g3;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ProceduralBuildingsGeneration
{
    public class BuildingsModelsGenerator : IProceduralModelsGenerator
    {
        public BuildingsModelsGenerator()
        {
            
        }

        public Model3d GenerateModel(GenerationParameters parameters)
        {
            var buildingParams = parameters as BuildingsGenerationParameters;
            var mesh = new DMesh3(false);//DMesh3Builder.Build(

            MakeBuildingBody(mesh, buildingParams);
            AddRoof(mesh, buildingParams);
            // return result;
            if (!mesh.CheckValidity()) throw new Exception("Generated mesh is invalid"); ;
            return new Model3d { Mesh = mesh };
        }

        private void AddRoof(DMesh3 mesh, BuildingsGenerationParameters buildingParams)
        {   
            // top polygon, he
        }

        private void MakeBuildingBody(DMesh3 mesh, BuildingsGenerationParameters buildingParams)
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
                Vector3f upNormal = Vector3f.AxisZ;

                int a0 = AddVertex3dFrom2d(mesh, vertexToIndex, triangle[0], -upNormal);
                int b0 = AddVertex3dFrom2d(mesh, vertexToIndex, triangle[1], -upNormal);
                int c0 = AddVertex3dFrom2d(mesh, vertexToIndex, triangle[2], -upNormal);

                addedTriangles[addedTriangleIdx] = mesh.AppendTriangle(c0, b0, a0);
                addedTriangleIdx++;
            }

            var heightExtruder = new MeshExtrudeMesh(mesh);
            heightExtruder.ExtrudedPositionF = (pos, normal, idx) =>
            {
                return pos + Vector3d.AxisZ * buildingParams.UnitsPerMeter *
                    buildingParams.BasementExtrudeHeight;
            };
            heightExtruder.Extrude();
        }

        private int AddVertex3dFrom2d(DMesh3 mesh, Dictionary<Vector2d, int> vertices, Vector2d candidateV, Vector3f normal)
        {
            if (!vertices.ContainsKey(candidateV))
            {
                return vertices[candidateV] =
                        mesh.AppendVertex(new NewVertexInfo
                        {
                            v = new Vector3d(candidateV.x, candidateV.y, 0.0),
                            n = normal,
                        });
            }
            return vertices[candidateV];
        }

        private IEnumerable<Vector2d> CenterPolygon(IList<Point2d> polygon, out Vector2d centroid)
        {
            var centroidCopy = centroid = Geometry.FindCentroid(polygon);
            return polygon.Select(point => new Vector2d
            {
                x = point.X - centroidCopy.x,
                y = point.Y - centroidCopy.y,
            });
        }

        private IList<Vector2d> ScaleCenteredPolygon(IEnumerable<Vector2d> polygon, double scaleFactor)
        {
            return polygon.Select(p => new Vector2d
            {
                x = p.x * scaleFactor,
                y = p.y * scaleFactor,
            }).ToList();
        }
    }
}

//Vector2d e2d1 = triangle[1] - triangle[0];
//Vector2d e2d2 = triangle[2] - triangle[0];
//Vector3f e1 = new Vector3f(e2d1.x, e2d1.y, 0.0);
//Vector3f e2 = new Vector3f(e2d2.x, e2d2.y, 0.0);

//int a1 = AddVertex3dFrom2d(mesh, triangle[0], upNormal.UnitCross(e1));
//int a2 = AddVertex3dFrom2d(mesh, triangle[0], upNormal.UnitCross(e2));

//e2d1 = triangle[0] - triangle[1];
//e2d2 = triangle[2] - triangle[1];
//e1 = new Vector3f(e2d1.x, e2d1.y, 0.0);
//e2 = new Vector3f(e2d2.x, e2d2.y, 0.0);
//int b1 = AddVertex3dFrom2d(mesh, triangle[1], upNormal.UnitCross(e1));
//int b2 = AddVertex3dFrom2d(mesh, triangle[1], upNormal.UnitCross(e2));

//e2d1 = triangle[0] - triangle[2];
//e2d2 = triangle[1] - triangle[2];
//e1 = new Vector3f(e2d1.x, e2d1.y, 0.0);
//e2 = new Vector3f(e2d2.x, e2d2.y, 0.0);
//int c1 = AddVertex3dFrom2d(mesh, triangle[2], upNormal.UnitCross(e1));
//int c2 = AddVertex3dFrom2d(mesh, triangle[2], upNormal.UnitCross(e2));


// extrude basement
//var extruder = new MeshExtrudeFaces(mesh, addedTriangles);

// Vector3d.AxisZ 
/*mesh.edges
var basementEdgeLoop = new EdgeLoop(mesh, ;
var duplicateExt = new MeshExtrudeLoop(mesh, basementEdgeLoop);
duplicateExt.ExtrudedPositionF = (pos, normal, idx) =>
{
    return pos + Vector3d.AxisZ * buildingParams.UnitsPerMeter *
        buildingParams.BasementExtrudeHeight;
};*/

//var cylgen = new CappedCylinderGenerator { Height = new Random().Next(10,150), BaseRadius = 4, TopRadius = 10, WantNormals = true };
//cylgen.Generate();
//result.Mesh = cylgen.Generate().MakeDMesh();