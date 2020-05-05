using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorController
{
    public interface IProceduralGeneratorUi
    {
        IViewModel ViewModel { get; set; }
        bool RequestGeneration();
        bool RequestExport();
        bool RequestGenerationVisualization();
        bool RequestAssetVisualization();
    }
}
