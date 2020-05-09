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
        public static DMesh3 MakeMeshFromGrammar(GrammarNode buildingWord)
        {
            var mesh = new DMesh3();
            ApplyNodesRecursively(mesh, buildingWord);
            //FacadeUtility.MakeFacadeBody(mesh, parameters);
            return mesh;
        }

        private static void ApplyNodesRecursively(DMesh3 mesh, GrammarNode currentNode)
        {
            if (currentNode == null) return;
            currentNode.BuildOnMesh(mesh);
            foreach(var child in currentNode.Subnodes)
            {
                ApplyNodesRecursively(mesh, child);
            }
        }
    }
}
