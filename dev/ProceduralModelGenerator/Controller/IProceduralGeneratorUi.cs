using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
