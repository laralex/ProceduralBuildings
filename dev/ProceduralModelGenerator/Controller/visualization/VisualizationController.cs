using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;

using VisualizerLibrary;
using WcfVisualizerLibrary;

namespace GeneratorController
{
    public class VisualizationController
    {
        public void Work()
        {
            var registrationService = new VisualizersRegistry();
            var httpBinding = new BasicHttpBinding();
            var visualizationControllerHost = new ServiceHost(registrationService);
            var visualizationControllerServiceUri = "http://localhost:64046/visualizationControllerService";
            visualizationControllerHost.AddServiceEndpoint(typeof(IVisualizationControllerService), httpBinding, visualizationControllerServiceUri);
            visualizationControllerHost.Open();
            Console.WriteLine("> opened service");

            Console.WriteLine("> shutdown");
            registrationService.Dispose();
            visualizationControllerHost.Close();
        }
    }
}
