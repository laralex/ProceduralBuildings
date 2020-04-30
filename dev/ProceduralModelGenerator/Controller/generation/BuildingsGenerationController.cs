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
        public Model3d LatestModel { get; private set; }
        public IProceduralModelsGenerator Generator { get; private set; }

        public BuildingsGenerationController()
        {
            Generator = new BuildingsModelsGenerator();
        }

        public Model3d Generate(IViewModel generationData)
        {
            var buildingsViewModel = generationData as BuildingsViewModel;
            var basementViewModel = buildingsViewModel.BasementOptions as BasementPropertiesViewModel;

            double basementLengthPerUnit;
            if (basementViewModel.SelectedSideEndpoint1 >= 0 && 
                basementViewModel.SelectedSideEndpoint2 >= 0 &&
                basementViewModel.SelectedSideEndpoint1 < basementViewModel.PolygonPoints.Count &&
                basementViewModel.SelectedSideEndpoint2 < basementViewModel.PolygonPoints.Count)
            {
                var point1 = basementViewModel.PolygonPoints[basementViewModel.SelectedSideEndpoint1];
                var point2 = basementViewModel.PolygonPoints[basementViewModel.SelectedSideEndpoint2];
                Point2D basementSelectedPoint1 = new Point2D { X = point1.X, Y = point1.Y };
                Point2D basementSelectedPoint2 = new Point2D { X = point2.X, Y = point2.Y };
                double basementSelectedSideLength = Math.Sqrt(
                    Math.Pow(basementSelectedPoint1.X - basementSelectedPoint2.X, 2) +
                    Math.Pow(basementSelectedPoint1.Y - basementSelectedPoint2.Y, 2)
                );
                basementLengthPerUnit = basementViewModel.SelectedSideLength / basementSelectedSideLength;
            }
            else
            {
                basementLengthPerUnit = 1.0;
            }
            
            var generatorParameters = new BuildingsGenerationParameters
            {
                BasementExtrudeHeight = basementViewModel.BuildingHeight,
                BasementLengthPerUnit = basementLengthPerUnit,
                BasementPoints = basementViewModel.PolygonPoints.Select(p => new Point2D { X=p.X, Y=p.Y}).ToList(),
                // to do seed
            };
            LatestModel = Generator.GenerateModel(generatorParameters);
            return LatestModel;
        }
    }
}
