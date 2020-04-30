using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
