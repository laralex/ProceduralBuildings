using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WcfVisualizerLibrary
{
    public class ServiceUtility
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
