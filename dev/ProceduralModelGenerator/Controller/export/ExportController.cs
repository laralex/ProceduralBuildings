using ProceduralBuildingsGeneration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GeneratorController
{
    public class ExportController
    {
        public void ExportInFile(Model3D model, ExportParameters parameters)
        {
            var exporter = new FileExporter();
            switch (parameters.ModelFormat)
            {
                case ModelFormat.OBJ:
                    exporter.ObjExport(model, parameters);
                    break;
                case ModelFormat.STL:
                    exporter.StlExport(model, parameters);
                    break;
            }
        }
        public Stream ExportInStream(Model3D model, ExportParameters parameters)
        {
            var exporter = new MemoryExporter();
            switch (parameters.ModelFormat)
            {
                case ModelFormat.OBJ:
                    if (!exporter.ObjExport(model, parameters)) return null;
                    return exporter.LatestExportedModel;
                case ModelFormat.STL:
                    if (!exporter.StlExport(model, parameters)) return null;
                    return exporter.LatestExportedModel;
            }
            return null;    
        }
        private void Export(Model3D model, IExporter exporter, ExportParameters parameters)
        {
           // exporter.Export(model, parameters);
        }
    }
}
