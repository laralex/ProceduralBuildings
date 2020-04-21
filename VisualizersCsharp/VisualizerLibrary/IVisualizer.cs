using System;
using System.IO;

namespace VisualizerLibrary
{
    
    public interface IVisualizer
    {
        string GetDescription();
        void VisualizeModel(Stream binaryModel, ModelDataType type = ModelDataType.OBJ);
        void Shutdown();
    }
    public enum ModelDataType
    {
        OBJ, STL, ThreeDS
    }
}
