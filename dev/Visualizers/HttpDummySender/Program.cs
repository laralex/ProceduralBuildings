using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.ServiceModel;
using System.Text;

using System.Threading;
using System.Windows;
using VisualizerLibrary;
using WcfVisualizerLibrary;

namespace HttpDummySender
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class VisualizersRegistrationService : IVisualizationControllerService, IDisposable
    {
        public HashSet<string> VisualizersIds = new HashSet<string>();
        public int ClientsCount => clients.Count;
        public IEnumerable<IVisualizerService> Visualizers { get => clients.AsEnumerable(); }
        public void RegisterVisualizer(string visualizerUri)
        {
            if (VisualizersIds.Add(visualizerUri.ToString())) {
                var client = ServiceUtility.SpawnClient<IVisualizerService>(visualizerUri.ToString());
                //((IClientChannel)client).Open();
                clients.Add(client);
            }
        }
        public void OpenClients()
        {
            foreach (var c in clients)
            {
                var clientChannel = (IClientChannel)c;
                if (clientChannel.State == CommunicationState.Created)
                {
                    ((IClientChannel)c).Open();
                }
            }
        }
        public void Dispose()
        {
            foreach(var c in clients)
            {
                c.Shutdown();
                ((IClientChannel)c).Close();
            }
            clients = new List<IVisualizerService>();
            VisualizersIds = new HashSet<string>();
        }

        private List<IVisualizerService> clients = new List<IVisualizerService>();
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
