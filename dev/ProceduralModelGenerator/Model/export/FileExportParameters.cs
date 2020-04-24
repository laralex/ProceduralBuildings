using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ProceduralBuildingsGeneration
{
    public class FileExportParameters : ExportParameters
    {
        public string FileName { get; set; }
        public string Directory { get; set; }
        public string FileExtention {
            get
            {
                switch(this.ModelFormat)
                {
                    case ModelFormat.OBJ: return ".obj";
                    case ModelFormat.STL: return ".stl";
                    case ModelFormat.ThreeDS: return ".3ds";
                    default: return "";
                }
            }
        }
    }
}
