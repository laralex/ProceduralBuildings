using ProceduralBuildingsGeneration;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace GeneratorController
{
    public interface IGenerationController
    {
        IProceduralModelsGenerator Generator { get; }
        Model3d Generate(IViewModel generationData);
        Task<Model3d> GenerateAsync(IViewModel generationData, Dispatcher uiDispatcher);
    }
}
