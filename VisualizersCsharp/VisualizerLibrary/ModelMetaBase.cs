namespace VisualizerLibrary
{

    public class ModelMetaBase
    {
        public ModelDataType ModelType;
        public string MaterialLibraryFilename;
        public string[] MaterialFileIds;
    }

    public enum ModelDataType
    {
        OBJ, STL, ThreeDS
    }

}
