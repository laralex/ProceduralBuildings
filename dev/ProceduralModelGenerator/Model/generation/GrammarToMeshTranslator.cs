using g3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralBuildingsGeneration
{
    class BuildingsMeshMaker
    {
        public static DMesh3 MakeMeshFromGrammar(GrammarNode buildingWord, BuildingsGenerationParameters parameters)
        {
            var mesh = new DMesh3();
            //FacadeUtility.MakeFacadeBody(mesh, parameters);
            return mesh;
        }
    }
}
