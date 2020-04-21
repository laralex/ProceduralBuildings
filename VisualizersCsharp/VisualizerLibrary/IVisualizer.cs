using System;
using System.IO;

namespace VisualizerLibrary
{
    
    public interface IVisualizer
    {
        string Description { get; }
        void VisualizeModel(Stream binaryModel, ModelBinaryType type = ModelBinaryType.OBJ);
        void Shutdown();
    }
    public enum ModelBinaryType
    {
        OBJ, STL, ThreeDS
    }
}
