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
        protected Model3D LatestModel { get; private set; }
        protected Stream LatestExportedModel { get; private set; }

        protected ModelFormat LatestExportedModelFormat { get; private set; }
        public InputController()
        {
            GenerationControler = new BuildingsGenerationController();
            ViewModel = new BuildingsViewModel();
            //ViewModel.Grammar = new ViewModelGrammar();
            VisualizationController = new VisualizationController();
            ExportController = new ExportController();
        }

        public void StartService()
        {
            VisualizationController.InitializeService();
            VisualizationController.StartVisualizers(new[] { "WpfVisualizer.exe" });
        }
        public void RequestGenerate()
        {
            VisualizationController.OpenVisualizers();
            LatestModel = GenerationControler.Generate(ViewModel);
            LatestExportedModelFormat = ModelFormat.OBJ;
            LatestExportedModel?.Dispose();
            LatestExportedModel = ExportController.ExportInStream(LatestModel, new ExportParameters { ModelFormat = LatestExportedModelFormat });
            RequestVisualize();
        }

        public void RequestExport(string filePath, ModelFormat exportFormat)
        {
            if (LatestModel != null)
            {
                LatestExportedModelFormat = exportFormat;
                LatestExportedModel?.Dispose();
                LatestExportedModel = ExportController.ExportInStream(LatestModel, new ExportParameters { ModelFormat = LatestExportedModelFormat });
            }
        }

        public void RequestVisualize()
        {
            if (LatestExportedModel != null)
            {
                try
                {
                    VisualizationController.Visualize(LatestExportedModel, LatestExportedModelFormat);
                }
                catch
                {
                    // todo
                }
            }
        }

        public void Dispose()
        {
            //GenerationControler.Dispose();
            //ExportController.Dispose();
            VisualizationController.Dispose();
            //ViewModel.Dispose();
        }

        private Stream m_latestExportedModel;
    }
}
