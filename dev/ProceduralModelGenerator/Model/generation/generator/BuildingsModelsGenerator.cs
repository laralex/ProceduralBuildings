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
            var mesh = new DMesh3(false);

            // make grammar
            var grammarController = new BuildingsGrammarController(buildingParams.RandomGenerator);
            var buildingWord = grammarController.TransformWordRepeatedly(parameters, 10);

            // build model
            // todo: no parameters
            var buildingMesh = MakeMeshFromGrammar(buildingWord);
            if (!buildingMesh.CheckValidity()) throw new Exception("Generated mesh is invalid"); ;
            return new Model3d { Mesh = buildingMesh };
        }

        private static DMesh3 MakeMeshFromGrammar(GrammarNode buildingWord)
        {
            var mesh = new DMesh3();
            ApplyNodesRecursively(mesh, buildingWord);
            return mesh;
        }

        private static void ApplyNodesRecursively(DMesh3 mesh, GrammarNode currentNode)
        {
            if (currentNode == null) return;
            currentNode.BuildOnMesh(mesh);
            foreach (var child in currentNode.Subnodes)
            {
                ApplyNodesRecursively(mesh, child);
            }
        }

        //private void AddRoof(DMesh3 mesh, BuildingsGenerationParameters buildingParams)
        //{   
        //    // top polygon, he
        //}

        /*
        private GrammarController MakeGrammar(BuildingsGenerationParameters parameters)
        {
            var rules = new GrammarController();
            
            // Root to floors
            {
                var floorsCollection = new FloorCollectionNode(parameters.FloorsNumber + 1);
                floorsCollection.FillFloors(parameters.FloorsNumber + 1);
                var root = new RootNode(floorsCollection.Floors);
                var consequence = new List<GrammarNode> { root };
                var antedecent = new HashSet<Type> { typeof(RootNode) };
                rules.AddRule(antedecent, consequence);
            }

            // Top floor to roof
            {
                var antedecent = new HashSet<Type> { typeof(FloorTopNode) };
                var consequence = new List<GrammarNode> { new RoofNode(parameters.RoofHeight, parameters.RoofStyle) };
                rules.AddRule(antedecent, consequence);
            }

            // ground type + normal type + parade type + none type
            var wallsUtil = new WallCollectionNode(parameters.BasementPoints.Count);
            var segmentsPerWall
            // Floor to floor with walls
            {
                var antedecent = new HashSet<Type> { typeof(FloorMiddleNode) };
                wallsUtil.FillWalls(, 
                    new List<int> { parameters.DoorWall.PointIdx1 });
                var consequence = new List<GrammarNode> { new RoofNode(parameters.RoofHeight, parameters.RoofStyle) };
                rules.AddRule(antedecent, consequence);
            }

            // Ground floor to ground floor with walls
            {
                var antedecent = new HashSet<Type> { typeof(FloorGroundNode) };
                var consequence = new List<GrammarNode> { }
            }

            // Walls on floors to walls with segments
            {

            }

            // Some segments to windows
            
            // Some segments to doors

            return rules;
        }
        */
    }
}