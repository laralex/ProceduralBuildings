using System;
using System.ServiceModel;

namespace WcfVisualizerLibrary
{
    [ServiceContract]
    public interface IVisualizationControllerService
    {
        [OperationContract]
        void RegisterVisualizer(string visualizerUri);
    }
}
