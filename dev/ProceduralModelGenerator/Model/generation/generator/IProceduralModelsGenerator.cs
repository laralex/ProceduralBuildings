using System;
using System.Collections.Generic;
using System.Text;

namespace ProceduralBuildingsGeneration
{
    public interface IProceduralModelsGenerator
    {
        Model3d GenerateModel(GenerationParameters parameters);
    }
}
