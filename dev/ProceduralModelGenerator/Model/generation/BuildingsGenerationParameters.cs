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
        public double UnitsPerMeter { get; set; }
        public int Seed { get; set; }
        public IList<AssetsGroup> Assets { get; set; }
    }
}
