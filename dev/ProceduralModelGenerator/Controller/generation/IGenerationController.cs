using System;
using System.Collections.Generic;
using System.Text;

using ProceduralBuildingsGeneration;

namespace GeneratorController
{
    public interface IGenerationController
    {
        IProceduralModelsGenerator Generator { get; }
        void Generate(IViewModel generationData);
    }
}
