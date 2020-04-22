using System.IO;

namespace VisualizerLibrary
{
    
    public interface IVisualizer
    {
        string GetDescription();
        void VisualizeModel(Stream model, ModelMetaBase modelMeta, Stream materialLibrary, Stream[] materialFiles);
        void Shutdown();
    }

}
