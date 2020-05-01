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
        public AssetsLoader AssetsLoader { get; private set; }
        public BuildingsGenerationController()
        {
            Generator = new BuildingsModelsGenerator();
            AssetsLoader = new AssetsLoader();
            try
            {
                if (AssetsLoader.TryReloadManifest())
                {
                    var missingAssets = AssetsLoader.FindMissingAssetsFiles();
                }
            }
            catch
            {
                // corrupt xml
            }
        }

        public Model3d Generate(IViewModel viewModelParameters)
        {
            var generatorParameters = MakeGenerationParameters(viewModelParameters);
            LatestModel = Generator.GenerateModel(generatorParameters);
            return LatestModel;
        }

        private BuildingsGenerationParameters MakeGenerationParameters(IViewModel viewModelParameters)
        {
            var buildingsViewModel = viewModelParameters as BuildingsViewModel;
            var basementViewModel = buildingsViewModel.BasementOptions as BasementPropertiesViewModel;

            int p1, p2;
            double baseSideLength;
            if (basementViewModel.SelectedSideEndpoint1 < 0 ||
                basementViewModel.SelectedSideEndpoint2 < 0 ||
                basementViewModel.SelectedSideEndpoint1 >= basementViewModel.PolygonPoints.Count ||
                basementViewModel.SelectedSideEndpoint2 >= basementViewModel.PolygonPoints.Count)
            {
                p1 = 0;
                p2 = basementViewModel.PolygonPoints.Count - 1;
                baseSideLength = basementViewModel.PolygonPoints[p1]
                        .DistanceTo(basementViewModel.PolygonPoints[p2]);
                // no specific base side specified, so just take the longest
                for (int p = 0; p < basementViewModel.PolygonPoints.Count - 1; ++p)
                {
                    var candidateDistance = basementViewModel.PolygonPoints[p]
                        .DistanceTo(basementViewModel.PolygonPoints[p + 1]);
                    if (candidateDistance > baseSideLength)
                    {
                        p1 = p;
                        p2 = p + 1;
                        baseSideLength = candidateDistance;
                    }
                }
            }
            else
            {
                p1 = basementViewModel.SelectedSideEndpoint1;
                p2 = basementViewModel.SelectedSideEndpoint2;
                baseSideLength = basementViewModel.PolygonPoints[p1]
                    .DistanceTo(basementViewModel.PolygonPoints[p2]);
            }

            double basementLengthPerUnit = buildingsViewModel.SpaceUnitsPerMeter *
                basementViewModel.SelectedSideMeters / baseSideLength;

            return new BuildingsGenerationParameters
            {
                BasementExtrudeHeight = basementViewModel.BuildingHeight,
                BasementLengthPerUnit = basementLengthPerUnit,
                BasementPoints = basementViewModel.PolygonPoints.Select(p => new Point2d { X = p.X, Y = p.Y }).ToList(),
                UnitsPerMeter = buildingsViewModel.SpaceUnitsPerMeter,
                // to do seed
                Assets = AssetsLoader.AssetGroups,
            };
        }
        //basementViewModel.SelectedSideMeters *
        //buildingsViewModel.SpaceUnitsPerMeter
    }

    
    public static class PointsExtension
    {
        public static double DistanceTo(this System.Windows.Point t, System.Windows.Point other)
        {
            var dx = t.X - other.X;
            var dy = t.Y - other.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }

}
