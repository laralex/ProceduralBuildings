using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WcfVisualizerLibrary
{
    public class WcfServiceUtility
    {
        public static ServiceInterface SpawnClient<ServiceInterface>(string visualizerUri, bool streaming = false)
        {
            Binding binding = null;
            if (streaming)
            {
                //var tbinding = new NetTcpBinding();
                var tbinding = new BasicHttpBinding();
                tbinding.TransferMode = TransferMode.Streamed;
                tbinding.MaxReceivedMessageSize = 1024 * 1024 * 500;
                binding = tbinding;
                //binding.MessageEncoding = WSMessageEncoding.Mtom;
            }
            else
            {
                var bbinding = new BasicHttpBinding();
                bbinding.TransferMode = TransferMode.Streamed;
                bbinding.MaxReceivedMessageSize = 1024 * 1024 * 500;
                binding = bbinding;
            }
            var endpoint = new EndpointAddress(visualizerUri);
            var channelFactory = new ChannelFactory<ServiceInterface>(binding, endpoint);
            return channelFactory.CreateChannel();
        }
    }
}
