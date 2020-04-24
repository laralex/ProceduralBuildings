using ProceduralBuildingsGeneration;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeneratorController
{
    public class ExportController
    {
        public void ExportInFile(Model3D model, ExportParameters parameters)
        {
           // new FileExporter().Export(model, parameters);
        }
        public void ExportInStream(Model3D model, ExportParameters parameters)
        {
           // new FileExporter().Export(model, parameters);    
        }
        private void Export(Model3D model, IExporter exporter, ExportParameters parameters)
        {
           // exporter.Export(model, parameters);
        }
    }
}
