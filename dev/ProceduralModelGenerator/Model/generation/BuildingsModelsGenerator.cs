using g3;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ProceduralBuildingsGeneration
{
    public class BuildingsModelsGenerator : IProceduralModelsGenerator
    {
        public Model3d GenerateModel(GenerationParameters parameters)
        {
            var buildingParams = parameters as BuildingsGenerationParameters;
            var mesh = new DMesh3(MeshComponents.VertexNormals);//DMesh3Builder.Build(
            var properBasementPoints = ScaleCenteredPolygon(
                CenterPolygon(buildingParams.BasementPoints, out var basementMassCenter),
                buildingParams.BasementLengthPerUnit
            );
            //var basementPolygon = new g3.Polygon2d(properBasementPoints);
            
            // is counter clockwise
            if (Geometry.CalcSignedPolygonArea(properBasementPoints) < 0.0)
            {
                properBasementPoints = properBasementPoints.Reverse().ToList();
            }
            var basementTriangles = Geometry.Triangulate(properBasementPoints);

            //int[] basementVertices = new int[properBasementPoints.Count];
            var vertexToIndex = new Dictionary<Vector2d, int>();
            foreach (var triangle in basementTriangles)
            {
                int a = AddVertex2dOrGetIdx(mesh, vertexToIndex, triangle[0]);
                int b = AddVertex2dOrGetIdx(mesh, vertexToIndex, triangle[1]);
                int c = AddVertex2dOrGetIdx(mesh, vertexToIndex, triangle[2]);
                mesh.AppendTriangle(a, b, c);
            }
            mesh.CheckValidity();
            return new Model3d { Mesh = mesh };
        }

        private int AddVertex2dOrGetIdx(DMesh3 mesh, Dictionary<Vector2d, int> vertices, Vector2d candidateV)
        {
            if (!vertices.ContainsKey(candidateV))
            {
                return vertices[candidateV] = 
                    mesh.AppendVertex(new NewVertexInfo
                    {
                        v = new Vector3d(candidateV.x, candidateV.y, 0.0),
                    });
            }
            return vertices[candidateV];
        }

        private IEnumerable<Vector2d> CenterPolygon(IList<Point2D> polygon, out Vector2d massCenter)
        {
            massCenter = new Vector2d { x=0.0, y=0.0 };
            foreach(var point in polygon)
            {
                massCenter.x += point.X;
                massCenter.y += point.Y;
            }
            if (polygon.Count > 0)
            {
                massCenter.x /= polygon.Count;
                massCenter.y /= polygon.Count;
            }
            var massCenterCopy = massCenter;
            return polygon.Select(point => new Vector2d
            {
                x = point.X - massCenterCopy.x,
                y = point.Y - massCenterCopy.y,
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

//var cylgen = new CappedCylinderGenerator { Height = new Random().Next(10,150), BaseRadius = 4, TopRadius = 10, WantNormals = true };
//cylgen.Generate();
//result.Mesh = cylgen.Generate().MakeDMesh();