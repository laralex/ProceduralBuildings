using System;
using System.Collections.Generic;
using System.Text;

namespace ProceduralBuildingsGeneration
{
    public class ExportParameters
    {
        public ModelFormat ModelFormat;
    }

    public enum ModelFormat
    {
        OBJ, STL, ThreeDS
    }
}
