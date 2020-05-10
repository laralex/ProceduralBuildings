using System.Threading;

namespace GeneratorController
{
    public interface IProceduralGeneratorUi
    {
        string Description { get; }
        IViewModel ViewModel { set; }
        bool RequestGeneration(CancellationToken token);
        bool RequestExport(CancellationToken token);
        bool RequestGenerationVisualization(CancellationToken token);
        bool RequestAssetVisualization(CancellationToken token);
    }
}
