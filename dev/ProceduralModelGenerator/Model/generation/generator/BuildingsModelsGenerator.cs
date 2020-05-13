using g3;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProceduralBuildingsGeneration
{
    public class BuildingsModelsGenerator : IProceduralModelsGenerator
    {
        public Model3d GenerateModel(GenerationParameters parameters)
        {
            var buildingParams = parameters as BuildingsGenerationParameters;
            
            var assetsMeshes = new Dictionary<Asset, DMesh3>();

            LoadAssetsAsMeshes(buildingParams.DoorsAssets, 
                1.0 / buildingParams.AssetsScaleModifier, assetsMeshes);
            LoadAssetsAsMeshes(buildingParams.WindowsAssets, 
                1.0 / buildingParams.AssetsScaleModifier, assetsMeshes);
            
            // make grammar
            var grammarController = new BuildingsGrammarController(buildingParams.RandomGenerator, assetsMeshes);
            var buildingWord = grammarController.TransformWordRepeatedly(parameters, 10);

            // build model
            var buildingMeshes = MakeMeshFromGrammar(buildingWord);
            if (!buildingMeshes[0].CheckValidity()) throw new Exception("Generated mesh is invalid");


            foreach (var doorAsset in buildingParams.DoorsAssets)
                doorAsset.CloseAssetFile();
            foreach (var windowAsset in buildingParams.WindowsAssets)
                windowAsset.CloseAssetFile();
            assetsMeshes.Clear();

            //Reducer r = new Reducer(mesh);
            //r.ReduceToTriangleCount(1000);

            return new Model3d { Mesh = buildingMeshes };
        }

        private static void LoadAssetsAsMeshes(IList<Asset> assets, double scale, Dictionary<Asset, DMesh3> destination)
        {
            if (destination == null) return;
            var meshBuilder = new DMesh3Builder();
            var reader = new StandardMeshReader() { MeshBuilder = meshBuilder };
            foreach (var asset in assets)
            {
                asset.OpenAssetFile();
                var isDoorLoaded = reader.Read(asset.OpenedFile, asset.FileFormat.ToString(), null);
                if (isDoorLoaded.code == IOCode.Ok)
                {
                    var mesh = meshBuilder.Meshes.Last();
                    Reducer r = new Reducer(mesh);
                    r.ReduceToTriangleCount(500);
                    MeshTransforms.Scale(mesh, scale);
                    destination[asset] = mesh;
                }
            }   
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

        private static void ApplyNodesRecursively(DMesh3Builder meshBuilder, GrammarNode currentNode)
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