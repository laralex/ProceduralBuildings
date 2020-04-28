using ProceduralBuildingsGeneration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorController
{
    public class InputController : IDisposable
    {
        public IGenerationController GenerationControler { get; private set; }
        public ExportController ExportController { get; private set; }
        public VisualizationController VisualizationController { get; private set; }
        public BuildingsViewModel ViewModel { get; private set; }

        protected ModelFormat LatestModelTemporaryFileFormat { get; private set; }
        public InputController()
        {
            GenerationControler = new BuildingsGenerationController();
            ViewModel = new BuildingsViewModel();
            //ViewModel.Grammar = new ViewModelGrammar();
            VisualizationController = new VisualizationController();
            ExportController = new ExportController();
        }

        public bool StartService()
        {
            return VisualizationController.InitializeService();
            //VisualizationController.StartVisualizers(new[] { "WpfVisualizer.exe" });
        }
        public void RequestGenerate()
        {
            VisualizationController.OpenVisualizers();
            m_latestModel = GenerationControler.Generate(ViewModel);
            LatestModelTemporaryFileFormat = ModelFormat.OBJ;
            m_latestModelTemporaryfile?.Dispose();
            ExportController.ExportInStream(m_latestModel, 
                new ExportParameters { ModelFormat = LatestModelTemporaryFileFormat }, 
                out m_latestModelTemporaryfile);
            RequestVisualize();
        }

        public bool RequestExport(string filepath)
        {
            if (m_latestModel != null)
            {
                var exportResult = ExportController.ExportInFile(m_latestModel, new FileExportParameters
                {
                    FilePath = filepath,
                    ModelFormat = ExportParameters.FormatFromFilePath(filepath)
                });
                return exportResult;
            }
            return false;
        }

        public bool RequestVisualize()
        {
            if (m_latestModelTemporaryfile == null) return false;
            {
                try
                {
                    VisualizationController.Visualize(m_latestModelTemporaryfile, LatestModelTemporaryFileFormat);
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public void Dispose()
        {
            //GenerationControler.Dispose();
            //ExportController.Dispose();
            VisualizationController.Dispose();
            //ViewModel.Dispose();
        }

        private Model3D m_latestModel;
        private Stream m_latestModelTemporaryfile;
    }
}
