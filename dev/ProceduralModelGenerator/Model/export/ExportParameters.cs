using System;
using System.IO;

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

    public static class ModelFormatToString    
    {
        public static string ToString(this ModelFormat format)
        {
            switch (format)
            {
                case ModelFormat.OBJ: return "obj";
                case ModelFormat.STL: return "stl";
                case ModelFormat.ThreeDS: return "3ds";
                default: return null;
            }
        }
    }
}
