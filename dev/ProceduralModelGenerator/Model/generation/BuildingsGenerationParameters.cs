using System;
using System.Collections.Generic;
using System.Text;

namespace ProceduralBuildingsGeneration
{
    public class BuildingsGenerationParameters : GenerationParameters
    {
        public double BasementExtrudeHeight { get; set; }
        public double BasementLengthPerUnit { get; set; }
        public IList<Point2d> BasementPoints { get; set; }
        //public IList<AssetsGroup> Assets { get; set; }
        public RoofStyle RoofStyle { get; set; }
        public double RoofHeight { get; set; } 
        public int FloorsNumber { get; set; }
        public int SegmentsOnSelectedWall { get; set; }
        public int WindowsNumberOnSelectedWall { get; set; }
        public IList<Asset> WindowsAssets { get; set; }
        public IList<Asset> DoorsAssets { get; set; }
        public double AssetsScaleModifier { get; set; }
        public bool IsVerticalWindowSymmetryPreserved { get; set; }
        public WallIndices DoorWall { get; set; }
    }

    public enum RoofStyle
    {
        Flat, SlopeFlat, Slope
    }

    public struct WallIndices
    {
        public int PointIdx1;
        public int PointIdx2;
        public WallIndices(int point1 = -1, int point2 = -1)
        {
            PointIdx1 = point1;
            PointIdx2 = point2;
        }
    }
}
