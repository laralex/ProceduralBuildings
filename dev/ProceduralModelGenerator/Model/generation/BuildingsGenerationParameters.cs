using System;
using System.Collections.Generic;
using System.Text;

namespace ProceduralBuildingsGeneration
{
    public class BuildingsGenerationParameters : GenerationParameters
    {
        public double BasementExtrudeHeight { get; set; }
        public double BasementLengthPerUnit { get; set; }
        public IList<Point2D> BasementPoints { get; set; }
    }
}
