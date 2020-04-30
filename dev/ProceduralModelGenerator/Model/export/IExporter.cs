using System;
using System.Collections.Generic;
using System.Text;

namespace ProceduralBuildingsGeneration
{
    public interface IExporter
    {
        bool ObjExport(Model3d model, ExportParameters parameters);
        bool StlExport(Model3d model, ExportParameters parameters);
    }
}
