using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ProceduralBuildingsGeneration
{
    public class MemoryExporter : IExporter
    {
        public Stream LatestExportedModel { get; private set; }
        public MemoryExporter()
        {
            m_fileExporter = new FileExporter();
            m_fileExportParameters = new FileExportParameters
            {
                FilePath = Path.Combine(Path.GetTempPath(), "temp_model.obj"),
                ModelFormat = ModelFormat.OBJ,
            };
        }
        public bool ObjExport(Model3D model, ExportParameters parameters)
        {
            m_fileExportParameters.ModelFormat = ModelFormat.OBJ;
            return Export(model);
        }

        public bool StlExport(Model3D model, ExportParameters parameters)
        {
            m_fileExportParameters.ModelFormat = ModelFormat.STL;
            return Export(model);
        }

        private bool Export(Model3D model)
        {
            var fileExportResult = m_fileExporter.ObjExport(model, m_fileExportParameters);
            if (!fileExportResult) return false;
            try
            {
                var fs = new FileStream(m_fileExportParameters.FilePath, FileMode.Open);
                LatestExportedModel = fs;
            }
            catch
            {
                return false;
            }
            return true;
        }

        private IExporter m_fileExporter;
        private FileExportParameters m_fileExportParameters;
    }
}
