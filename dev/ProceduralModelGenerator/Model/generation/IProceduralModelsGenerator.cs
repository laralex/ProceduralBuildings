using System;
using System.Collections.Generic;
using System.Text;

namespace ProceduralBuildingsGeneration
{
    public interface IProceduralModelsGenerator
    {
        Model3D GenerateModel(GenerationParameters parameters);
    }
}
