using System.ServiceModel;

namespace WcfVisualizerLibrary
{
    [ServiceContract]
    public interface IVisualizationControllerService
    {
        [OperationContract]
        bool RegisterVisualizer(string visualizerUri);
    }
}
