using g3;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ProceduralBuildingsGeneration
{
    public class BuildingsModelsGenerator : IProceduralModelsGenerator
    {
        public BuildingsModelsGenerator()
        {
            
        }

        public Model3d GenerateModel(GenerationParameters parameters)
        {
            var buildingParams = parameters as BuildingsGenerationParameters;
            var mesh = new DMesh3(false);//DMesh3Builder.Build(

            FacadeUtility.MakeFacadeBody(mesh, buildingParams);
            //AddRoof(mesh, buildingParams);
            // return result;
            if (!mesh.CheckValidity()) throw new Exception("Generated mesh is invalid"); ;
            return new Model3d { Mesh = mesh };
        }

        private void AddRoof(DMesh3 mesh, BuildingsGenerationParameters buildingParams)
        {   
            // top polygon, he
        }
    }
}