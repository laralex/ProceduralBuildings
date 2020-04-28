using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;

using System.Windows;
using VisualizerLibrary;
using WcfVisualizerLibrary;

namespace HttpDummySender
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class VisualizersRegistrationService : IVisualizationControllerService, IDisposable
    {
        public int ClientsCount => openClients.Count;
        public IEnumerable<IVisualizerService> Visualizers { get => openClients.Values.AsEnumerable(); }
        
        // Called by HTTP request
        public bool RegisterVisualizer(string visualizerUri)
        {
            if (!openClients.ContainsKey(visualizerUri) && !registeredClients.ContainsKey(visualizerUri)) {
                var client = ServiceUtility.SpawnClient<IVisualizerService>(visualizerUri.ToString());
                registeredClients[visualizerUri] = client;
                return true;
            }
            return false;
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
            foreach(var c in openClients.Values.Concat(registeredClients.Values))
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

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("> launch");
            var modelFiles = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "data"));
            var rng = new Random();
            var registrationService = new VisualizersRegistrationService();
            var visualizerControllerHost = new ServiceHost(registrationService);
            var httpBinding = new BasicHttpBinding();
            var visualizationControllerServiceUri = "http://localhost:64046/visualizationControllerService";
            visualizerControllerHost.AddServiceEndpoint(typeof(IVisualizationControllerService), httpBinding, visualizationControllerServiceUri);
            visualizerControllerHost.Open();
            Console.WriteLine("> opened service");
            for (int i = 0; i < 10; ++i)
            {
                var randomModel = modelFiles[rng.Next(modelFiles.Length)];
                var model = File.OpenRead(randomModel);
                Console.WriteLine($"{i}> sending file {randomModel}");
                while(registrationService.ClientsCount == 0)
                {

                }
                registrationService.OpenClients();
                foreach(var client in registrationService.Visualizers)
                {
                    try
                    {
                        client.PrepareForModel(new ModelMetaBase { ModelType = ModelDataType.OBJ });
                        client.AcceptModel(File.OpenRead(randomModel));
                        Console.WriteLine($"{i}> sent file {randomModel}");
                        client.Visualize();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show($"{i}> e: {e.Message}");
                    }
                }
            }
            Console.WriteLine("> shutdown");
            registrationService.Dispose();
            visualizerControllerHost.Close();
        }
    }
}
