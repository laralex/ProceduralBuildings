using ProceduralBuildingsGeneration;

namespace GeneratorController
{
    public interface IGenerationController
    {
        IProceduralModelsGenerator Generator { get; }
        //Model3D LatestModel { get; }
        Model3d Generate(IViewModel generationData);
    }
}
