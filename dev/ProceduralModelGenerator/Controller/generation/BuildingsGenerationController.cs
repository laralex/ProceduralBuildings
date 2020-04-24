using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProceduralBuildingsGeneration;

namespace GeneratorController
{
    public class BuildingsGenerationController : IGenerationController
    {
        internal Model3D LatestModel { get; private set; }
        public IProceduralModelsGenerator Generator { get; private set; }
        public BuildingsGenerationController()
        {
            Generator = new BuildingsModelsGenerator();
        }

        public void Generate(IViewModel generationData)
        {
            var buildingsGenerationData = generationData as BuildingsViewModel;
            var generatorParameters = new BuildingsGenerationParameters();
            LatestModel = Generator.GenerateModel(generatorParameters);
        }
    }
}
