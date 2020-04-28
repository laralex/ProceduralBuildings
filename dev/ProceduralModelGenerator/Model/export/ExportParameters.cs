using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ProceduralBuildingsGeneration
{
    public class ExportParameters
    {
        public ModelFormat ModelFormat;
        public static ModelFormat FormatFromFilePath(string filePath)
        {
            switch (Path.GetExtension(filePath))
            {
                case ".obj": return ModelFormat.OBJ; 
                case ".stl": return ModelFormat.STL; 
                case ".3ds": return ModelFormat.ThreeDS;
                default: return ModelFormat.OBJ;
            }
        }
    }

    public enum ModelFormat
    {
        OBJ, STL, ThreeDS
    }
}
