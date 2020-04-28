using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProceduralBuildingsGeneration;

namespace GeneratorController
{
    internal class BuildingsGenerationController : IGenerationController
    {
        public Model3D LatestModel { get; private set; }
        public IProceduralModelsGenerator Generator { get; private set; }

        public BuildingsGenerationController()
        {
            Generator = new BuildingsModelsGenerator();
        }

        public Model3D Generate(IViewModel generationData)
        {
            var buildingsGenerationData = generationData as BuildingsViewModel;

            var point1 = buildingsGenerationData.BasementOptions.PolygonPoints[buildingsGenerationData.BasementOptions.SelectedSideEndpoint1];
            var point2 = buildingsGenerationData.BasementOptions.PolygonPoints[buildingsGenerationData.BasementOptions.SelectedSideEndpoint2];
            Point2D basementSelectedPoint1 = new Point2D { X = point1.X, Y = point1.Y };
            Point2D basementSelectedPoint2 = new Point2D { X = point2.X, Y = point2.Y };
            double basementSelectedSideLength = Math.Sqrt(
                Math.Pow(basementSelectedPoint1.X - basementSelectedPoint2.X, 2) +
                Math.Pow(basementSelectedPoint1.Y - basementSelectedPoint2.Y, 2)
            );
            var generatorParameters = new BuildingsGenerationParameters
            {
                BasementExtrudeHeight = buildingsGenerationData.BasementOptions.BuildingHeight,
                BasementLengthPerUnit = buildingsGenerationData.BasementOptions.SelectedSideLength / basementSelectedSideLength,
                BasementPoints = buildingsGenerationData.BasementOptions.PolygonPoints.Select(p => new Point2D { X=p.X, Y=p.Y}).ToList(),
                // to do seed
            };
            LatestModel = Generator.GenerateModel(generatorParameters);
            return LatestModel;
        }
    }
}
