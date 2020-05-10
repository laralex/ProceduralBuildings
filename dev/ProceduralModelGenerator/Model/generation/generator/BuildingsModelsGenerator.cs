using g3;
using System;

namespace ProceduralBuildingsGeneration
{
    public class BuildingsModelsGenerator : IProceduralModelsGenerator
    {
        public Model3d GenerateModel(GenerationParameters parameters)
        {
            var buildingParams = parameters as BuildingsGenerationParameters;
            var mesh = new DMesh3(false);

            foreach (var doorAsset in buildingParams.DoorsAssets)
                doorAsset.OpenAssetFile();
            foreach (var windowAsset in buildingParams.WindowsAssets)
                windowAsset.OpenAssetFile();

            // make grammar
            var grammarController = new BuildingsGrammarController(buildingParams.RandomGenerator);
            var buildingWord = grammarController.TransformWordRepeatedly(parameters, 10);

            // build model
            var buildingMesh = MakeMeshFromGrammar(buildingWord);
            if (!buildingMesh.CheckValidity()) throw new Exception("Generated mesh is invalid");


            foreach (var doorAsset in buildingParams.DoorsAssets)
                doorAsset.CloseAssetFile();
            foreach (var windowAsset in buildingParams.WindowsAssets)
                windowAsset.CloseAssetFile();

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
    }
}