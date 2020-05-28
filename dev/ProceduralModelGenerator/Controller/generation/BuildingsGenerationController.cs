using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
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

        public Model3d Generate(IViewModel viewModelParameters)
        {
            var generatorParameters = MakeGenerationParameters(viewModelParameters);
            LatestModel = Generator.GenerateModel(generatorParameters);
            return LatestModel;
        }

        public async Task<Model3d> GenerateAsync(IViewModel viewModelParameters, Dispatcher uiDispatcher)
        {
            BuildingsGenerationParameters generatorParameters = null;
            await Task.Run(() =>
            {
                uiDispatcher.Invoke(() =>
                    generatorParameters = MakeGenerationParameters(viewModelParameters)
                );
                LatestModel = Generator.GenerateModel(generatorParameters);
            });
            return LatestModel;
        }

        private BuildingsGenerationParameters MakeGenerationParameters(IViewModel viewModelParameters)
        {
            var vm = viewModelParameters as BuildingsViewModel;
            var seed = vm.SeedString.GetHashCode();
            var rng = new Random(seed);

            int p1, p2;
            double baseSideLength;
            if (vm.SelectedSideEndpoint1 < 0 ||
                vm.SelectedSideEndpoint2 < 0 ||
                vm.SelectedSideEndpoint1 >= vm.PolygonPoints.Count ||
                vm.SelectedSideEndpoint2 >= vm.PolygonPoints.Count)
            {
                p1 = 0;
                p2 = vm.PolygonPoints.Count - 1;
                baseSideLength = vm.PolygonPoints[p1]
                        .DistanceTo(vm.PolygonPoints[p2]);
                // no specific base side specified, so just take the longest
                for (int p = 0; p < vm.PolygonPoints.Count - 1; ++p)
                {
                    var candidateDistance = vm.PolygonPoints[p]
                        .DistanceTo(vm.PolygonPoints[p + 1]);
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
                p1 = vm.SelectedSideEndpoint1;
                p2 = vm.SelectedSideEndpoint2;
                baseSideLength = vm.PolygonPoints[p1]
                    .DistanceTo(vm.PolygonPoints[p2]);
            }

            double basementLengthPerUnit = vm.SelectedSideMeters / baseSideLength;

            var facadeHeight = Lerp(vm.BuildingMinHeight, vm.BuildingMaxHeight, rng.NextDouble());
            var floors = rng.Next(vm.MinNumberOfFloors, vm.MaxNumberOfFloors + 1);
            var segmentsOnSelectedWall = rng.Next(vm.MinSelectedWallHorizontalSegments, vm.MaxSelectedWallHorizontalSegments);
            var windowsOnSelectedWall = rng.Next(vm.MinWindowsOnSelectedWall, vm.MaxWindowsOnSelectedWall);
            windowsOnSelectedWall = (int)Math.Min(windowsOnSelectedWall, segmentsOnSelectedWall);
            var roofHeight = Lerp(vm.RoofMinHeight, vm.RoofMaxHeight, rng.NextDouble());

            var assetsViewModel = vm.AssetsViewModel as AssetsViewModel;

            IList<Asset> doorsAsset = new List<Asset> {
                assetsViewModel.DoorsAssets[vm.SelectedDoorStyleIdx]
            };
            IList<Asset> windowsAssets;
            if (vm.IsSingleStyleWindow)
                windowsAssets = new List<Asset> { assetsViewModel.WindowsAssets[vm.SelectedWindowStyleIdx] };
            else
                windowsAssets = assetsViewModel.WindowsAssets;

            var assetTriangleLimit = assetsViewModel.AssetTrianglesLimit;
            if (assetTriangleLimit <= 0)
            {
                assetTriangleLimit = int.MaxValue;
            }

            IList<Point2d> basementPoints = vm.PolygonPoints.Select(p => new Point2d { X = p.X, Y = p.Y }).ToList();
            // is given basement counter clockwise
            if (Geometry2d.CalcSignedPolygonArea(basementPoints) < 0.0)
            {
                basementPoints = basementPoints.Reverse().ToList();
                var tmp = p1;
                p1 = basementPoints.Count - 1 - p2;
                p2 = basementPoints.Count - 1 - tmp;
            }
                
            basementPoints = Geometry2d.ScaleCenteredPolygon(
                Geometry2d.CenterPolygon(basementPoints, out var basementCentroid),
                basementLengthPerUnit
            );

            if (!vm.IsDoorOnSelectedWall)
            {
                var rndPolygonPoint = rng.Next(vm.PolygonPoints.Count);
                p1 = rndPolygonPoint;
                p2 = (rndPolygonPoint + 1) % vm.PolygonPoints.Count;
            }

            // is single style window    
            return new BuildingsGenerationParameters
            {
                // assets groups
                BasementExtrudeHeight = facadeHeight,
                BasementLengthPerUnit = basementLengthPerUnit,
                BasementPoints = basementPoints,
                Seed = seed,
                RoofStyle = (ProceduralBuildingsGeneration.RoofStyle)vm.RoofStyle,
                RoofHeight = roofHeight,
                FloorsNumber = floors,
                IsVerticalWindowSymmetryPreserved = vm.IsVerticalSymmetryPreserved,
                AssetsScaleModifier = 10.0,
                SelectedWallSegments = segmentsOnSelectedWall,
                WindowsToSegmentsFraction = (float)windowsOnSelectedWall / segmentsOnSelectedWall,
                WindowsAssets = windowsAssets,
                DoorsAssets = doorsAsset,
                AssetTrianglesLimit = assetTriangleLimit,
                DoorWall = new WallIndices(p1, p2),
                RandomGenerator = rng,
            };
        }

        private static double Lerp(double a, double b, double theta)
        {
            return a + (b - a) * theta;
        }


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
