using System;
using System.Collections.Generic;
using System.Text;

namespace ProceduralBuildingsGeneration
{
    public interface IExporter
    {
        bool ObjExport(Model3D model, ExportParameters parameters);
        bool StlExport(Model3D model, ExportParameters parameters);
    }
}
