using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using VisualizerLibrary;

namespace WpfVisualizer.Remote
{
    [ServiceContract]
    public interface IVisualizerService : IVisualizer
    {
        [OperationContract]
        new string GetDescription();
        [OperationContract]
        void PrepareForModel(ModelDataType modelType);
        [OperationContract]
        void VisualizeModel(Stream model);
        [OperationContract(IsOneWay =true)]
        new void Shutdown();
    }
}
