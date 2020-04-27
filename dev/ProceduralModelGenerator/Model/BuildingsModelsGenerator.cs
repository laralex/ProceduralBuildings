using g3;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProceduralBuildingsGeneration
{
    public class BuildingsModelsGenerator : IProceduralModelsGenerator
    {
        public Model3D GenerateModel(GenerationParameters parameters)
        {
            var buildingParams = parameters as BuildingsGenerationParameters;
            var h = buildingParams.HeightMeters;
            var result = new Model3D();
            var cylgen = new CappedCylinderGenerator { Height = 30, BaseRadius = 4, TopRadius = 10, WantNormals = true };
            //cylgen.Generate();
            result.Mesh = cylgen.Generate().MakeDMesh();
            result.Mesh.CheckValidity();
            return result;
        }
    }
}
