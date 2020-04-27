using ProceduralBuildingsGeneration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using VisualizerLibrary;

//using VisualizerLibrary;
using WcfVisualizerLibrary;

namespace GeneratorController
{
    public class VisualizationController : IDisposable
    {
        public void InitializeService()
        {
            m_registractionHost = new VisualizersRegistry();
            var httpBinding = new BasicHttpBinding();
            try
            {
                m_visualizationControllerHost = new ServiceHost(m_registractionHost);
                var visualizationControllerServiceUri = "http://localhost:64046/visualizationControllerService";
                m_visualizationControllerHost.AddServiceEndpoint(typeof(IVisualizationControllerService), httpBinding, visualizationControllerServiceUri);
                m_visualizationControllerHost.Open();
            }
            // todo
            catch (NotFiniteNumberException e)
            {
                //m_viewModel.ApplicationStatus = $"HTTP service FAILED to start";
                //MessageBox.Show(e.Message);
            }
        }

        public void StartVisualizers(IEnumerable<string> visualizerProcesses)
        {
            foreach (var visualizerProcessName in visualizerProcesses)
            {
                ExecuteAsAdmin(visualizerProcessName);
            }
        }

        public void OpenVisualizers()
        {
            m_registractionHost.OpenClients();
        }

        public void ExecuteAsAdmin(string fileName)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = fileName;
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.Verb = "runas";
            proc.StartInfo.Arguments = "--external";
            proc.Start();
        }

        internal void Visualize(object latestEx)
        {
            throw new NotImplementedException();
        }

        public void Visualize(Stream model, ModelFormat type)
        {
            if (!model.CanSeek) throw new ArgumentException("model stream cannot be repositioned");
            var modelMeta = new ModelMetaBase();
            switch (type)
            {
                case ModelFormat.OBJ:
                    modelMeta.ModelType = ModelDataType.OBJ;
                    break;
                case ModelFormat.STL:
                    modelMeta.ModelType = ModelDataType.STL;
                    break;
                case ModelFormat.ThreeDS:
                    modelMeta.ModelType = ModelDataType.ThreeDS;
                    break;
            };
            foreach (var visualizer in m_registractionHost.Visualizers)
            {
                model.Position = 0;
                visualizer.PrepareForModel(modelMeta);
                visualizer.AcceptModel(model);
                visualizer.Visualize();
            }
        }

        public void Dispose()
        {
            m_registractionHost.Dispose();
            if (m_visualizationControllerHost.State == CommunicationState.Opened)
            {
                m_visualizationControllerHost.Close();
            }
        }

        private ServiceHost m_visualizationControllerHost;
        private VisualizersRegistry m_registractionHost;
    }
}
