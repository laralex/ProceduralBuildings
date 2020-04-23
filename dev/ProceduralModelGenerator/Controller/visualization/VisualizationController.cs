using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;

using VisualizerLibrary;
using WcfVisualizerLibrary;

namespace GeneratorController
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class VisualizersRegistrationService : IVisualizationControllerService, IDisposable
    {
        public int ClientsCount => openClients.Count;
        public IEnumerable<IVisualizerService> Visualizers { get => openClients.Values.AsEnumerable(); }

        // Called by HTTP request
        public void RegisterVisualizer(string visualizerUri)
        {
            if (!openClients.ContainsKey(visualizerUri) && !registeredClients.ContainsKey(visualizerUri))
            {
                var client = ServiceUtility.SpawnClient<IVisualizerService>(visualizerUri.ToString());
                registeredClients[visualizerUri] = client;
            }
        }
        public void OpenClients()
        {
            foreach (var kv in registeredClients)
            {
                var clientChannel = (IClientChannel)kv.Value;
                if (clientChannel.State == CommunicationState.Created)
                {
                    clientChannel.Open();
                    openClients[kv.Key] = kv.Value;
                }
            }
            registeredClients = new Dictionary<string, IVisualizerService>();
        }
        public void Clear()
        {
            Dispose();
            openClients = new Dictionary<string, IVisualizerService>();
            registeredClients = new Dictionary<string, IVisualizerService>();
        }

        public void Dispose()
        {
            foreach (var c in openClients.Values.Concat(registeredClients.Values))
            {
                var clientChannel = (IClientChannel)c;
                if (clientChannel.State == CommunicationState.Opened)
                {
                    c.Shutdown();
                    clientChannel.Close();
                }
            }
            registeredClients = null;
            openClients = null;
        }

        private Dictionary<string, IVisualizerService> openClients = new Dictionary<string, IVisualizerService>();
        private Dictionary<string, IVisualizerService> registeredClients = new Dictionary<string, IVisualizerService>();
    }

    class VisualizationController
    {
        public void Work()
        {
            var registrationService = new VisualizersRegistrationService();
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
