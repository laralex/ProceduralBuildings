using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProceduralBuildingsGeneration;

namespace GeneratorController
{
    internal class BuildingsGenerationController : IGenerationController
    {
        public Model3D LatestModel { get; private set; }
        public IProceduralModelsGenerator Generator { get; private set; }

        public BuildingsGenerationController()
        {
            Generator = new BuildingsModelsGenerator();
        }

        public Model3D Generate(IViewModel generationData)
        {
            var buildingsGenerationData = generationData as BuildingsViewModel;
            var generatorParameters = new BuildingsGenerationParameters();
            LatestModel = Generator.GenerateModel(generatorParameters);
            return LatestModel;
        }
    }
}
