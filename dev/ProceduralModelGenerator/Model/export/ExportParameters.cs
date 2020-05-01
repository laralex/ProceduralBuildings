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
            if (!TryFormatFromString(Path.GetExtension(filePath).Remove(0, 1), out var modelFormat))
            {
                throw new ArgumentException("In filePath, an extension is not found/supported");
            }
            return modelFormat;
        }
        public static bool TryFormatFromString(string formatCode, out ModelFormat result)
        {
            switch (formatCode)
            {
                case "obj": result = ModelFormat.OBJ; break;
                case "stl": result = ModelFormat.STL; break;
                case "3ds": result = ModelFormat.ThreeDS; break;
                default: result = ModelFormat.OBJ; return false; 
            }
            return true;
        }
    }

    public enum ModelFormat
    {
        OBJ, STL, ThreeDS
    }
}
