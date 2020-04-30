using ProceduralBuildingsGeneration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GeneratorController
{
    public class ExportController
    {
        public bool ExportInFile(Model3d model, ExportParameters parameters)
        {
            var exporter = new FileExporter();
            switch (parameters.ModelFormat)
            {
                case ModelFormat.OBJ:
                    return exporter.ObjExport(model, parameters);
                case ModelFormat.STL:
                    return exporter.StlExport(model, parameters);
            }
            return false;
        }
        public bool ExportInStream(Model3d model, ExportParameters parameters, out Stream exportedModel)
        {
            var exporter = new MemoryExporter();
            exportedModel = null;
            switch (parameters.ModelFormat)
            {
                case ModelFormat.OBJ:
                    if (!exporter.ObjExport(model, parameters)) return false;
                    exportedModel = exporter.LatestExportedModel;
                    break;
                case ModelFormat.STL:
                    if (!exporter.StlExport(model, parameters)) return false;
                    exportedModel = exporter.LatestExportedModel;
                    break;
            }
            return false;    
        }
        private void Export(Model3d model, IExporter exporter, ExportParameters parameters)
        {
           // exporter.Export(model, parameters);
        }
    }
}
