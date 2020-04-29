using g3;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ProceduralBuildingsGeneration
{
    public class BuildingsModelsGenerator : IProceduralModelsGenerator
    {
        public Model3D GenerateModel(GenerationParameters parameters)
        {
            var buildingParams = parameters as BuildingsGenerationParameters;
            var properBasementPoints = ScaleCenteredPolygon(
                CenterPolygon(buildingParams.BasementPoints),
                buildingParams.BasementLengthPerUnit
            );
            var basementPolygon = new g3.Polygon2d(properBasementPoints);
            var result = DMesh3Builder.Build(
                
            );
            result.Mesh.CheckValidity();
            result.Mesh.
            return result;
        }

        private IEnumerable<Vector2d> CenterPolygon(IList<Point2D> polygon)
        {
            var massCenter = new Vector2d { x=0.0, y=0.0 };
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
            return polygon.Select(point => new Vector2d
            {
                x = point.X - massCenter.x,
                y = point.Y - massCenter.y,
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