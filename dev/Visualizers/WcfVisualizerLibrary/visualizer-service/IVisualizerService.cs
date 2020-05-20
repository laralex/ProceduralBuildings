using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using VisualizerLibrary;

namespace WcfVisualizerLibrary
{
    [DataContract(Name = "ModelMeta")]
    public class ModelMeta : ModelMetaBase
    {
        [DataMember]
        new public ModelDataType ModelType;
        [DataMember]
        new public string MaterialLibraryFilename;
        [DataMember]
        new public string[] MaterialFileIds;
    }

    [ServiceContract]
    public interface IVisualizerService
    {
        [OperationContract]
        string GetDescription();
        [OperationContract]
        void PrepareForModel(ModelMetaBase modelMetadata);
        [OperationContract]
        void AcceptMaterialLib(Stream model);
        [OperationContract]
        void PrepareForMaterialFile(string materialFileId);
        [OperationContract]
        void AcceptMaterialFile(Stream materialFile);
        [OperationContract(IsOneWay = true)]
        void VisualizeModel(Stream model);
        //[OperationContract(IsOneWay = true)]
        //void Visualize();
        [OperationContract]
        void Shutdown();
    }
}
