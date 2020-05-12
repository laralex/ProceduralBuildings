using g3;
using System;
using System.Collections.Generic;

namespace ProceduralBuildingsGeneration
{
    public class BuildingsModelsGenerator : IProceduralModelsGenerator
    {
        public Model3d GenerateModel(GenerationParameters parameters)
        {
            var buildingParams = parameters as BuildingsGenerationParameters;
            
            foreach (var doorAsset in buildingParams.DoorsAssets)
                doorAsset.OpenAssetFile();
            foreach (var windowAsset in buildingParams.WindowsAssets)
                windowAsset.OpenAssetFile();

            // make grammar
            var grammarController = new BuildingsGrammarController(buildingParams.RandomGenerator);
            var buildingWord = grammarController.TransformWordRepeatedly(parameters, 10);

            // build model
            var buildingMeshes = MakeMeshFromGrammar(buildingWord);
            if (!buildingMeshes[0].CheckValidity()) throw new Exception("Generated mesh is invalid");


            foreach (var doorAsset in buildingParams.DoorsAssets)
                doorAsset.CloseAssetFile();
            foreach (var windowAsset in buildingParams.WindowsAssets)
                windowAsset.CloseAssetFile();

            return new Model3d { Mesh = buildingMeshes };
        }

        private static IList<DMesh3> MakeMeshFromGrammar(GrammarNode buildingWord)
        {
            var mesh = new DMesh3(MeshComponents.VertexNormals | MeshComponents.FaceGroups);
            var builder = new DMesh3Builder() {
                Meshes = { mesh },
                DuplicateTriBehavior = DMesh3Builder.AddTriangleFailBehaviors.DiscardTriangle,
            };
            builder.SetActiveMesh(0);
            ApplyNodesRecursively(builder, buildingWord);
            return builder.Meshes;
        }

        private static void ApplyNodesRecursively(IMeshBuilder meshBuilder, GrammarNode currentNode)
        {
            if (currentNode == null) return;
            currentNode.BuildOnMesh(meshBuilder);
            foreach (var child in currentNode.Subnodes)
            {
                ApplyNodesRecursively(meshBuilder, child);
            }
        }
    }
}