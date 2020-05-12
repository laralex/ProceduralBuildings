using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using WcfVisualizerLibrary;

namespace GeneratorController
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class VisualizersRegistry : IVisualizationControllerService, IDisposable
    {
        public event EventHandler<VisualizerRegisteredEventArgs> VisualizerRegistered;
        public int ClientsCount => openClients.Count;
        public IEnumerable<IVisualizerService> Visualizers { get => openClients.Values.AsEnumerable(); }

        // Called by HTTP request
        public bool RegisterVisualizer(string visualizerUri)
        {
            if (!openClients.ContainsKey(visualizerUri) && !registeredClients.ContainsKey(visualizerUri))
            {
                var client = WcfServiceUtility.SpawnClient<IVisualizerService>(visualizerUri.ToString());
                registeredClients[visualizerUri] = client;
                VisualizerRegistered?.Invoke(this, new VisualizerRegisteredEventArgs
                {
                    VisualizerUri = visualizerUri,
                });
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

        public async void Dispose()
        {
            foreach (var c in openClients.Values.Concat(registeredClients.Values))
            {
                var clientChannel = (IClientChannel)c;
                if (clientChannel.State == CommunicationState.Opened)
                {
                    await Task.Run(() =>
                    {
                        c.Shutdown();
                        clientChannel.Close();
                    });
                }
                clientChannel.Dispose();
            }
            registeredClients = null;
            openClients = null;
        }

        private Dictionary<string, IVisualizerService> openClients = new Dictionary<string, IVisualizerService>();
        private Dictionary<string, IVisualizerService> registeredClients = new Dictionary<string, IVisualizerService>();
    }

    public class VisualizerRegisteredEventArgs : EventArgs
    {
        public string VisualizerUri { get; set; }
    }

}
