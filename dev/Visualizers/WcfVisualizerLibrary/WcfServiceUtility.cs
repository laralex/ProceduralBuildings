using System.ServiceModel;

namespace WcfVisualizerLibrary
{
    public class WcfServiceUtility
    {
        public static ServiceInterface SpawnClient<ServiceInterface>(string visualizerUri)
        {
            var binding = new BasicHttpBinding();
            var endpoint = new EndpointAddress(visualizerUri);
            var channelFactory = new ChannelFactory<ServiceInterface>(binding, endpoint);
            return channelFactory.CreateChannel();
        }
    }
}
