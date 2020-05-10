using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace ProceduralBuildingsGeneration
{
    public class AssetsLoader
    {
        public static readonly string AssetsManifestPath = Path.Combine(rootDir, @"data/models/models_definitions.xml");
        public static readonly string AssetsHomeFolder = Path.Combine(rootDir, @"data/models/");
        public IDictionary<string, AssetsGroup> AssetGroups { get; private set; }
        public bool TryReloadManifest()
        {
            var manifest = new XmlDocument();
            if (!File.Exists(AssetsManifestPath)) return false;
            manifest.Load(AssetsManifestPath);
            if (manifest.DocumentElement.Name != "models") return false;
            var newAssetGroups = new Dictionary<string, AssetsGroup>(
                manifest.DocumentElement.ChildNodes.Count); 
            foreach (XmlNode assetsGroup in manifest.DocumentElement)
            {
                if (assetsGroup.Name != "group") return false;
                var groupName = assetsGroup.Attributes["name"].Value;
                newAssetGroups[groupName] = new AssetsGroup(groupName);
                foreach(XmlNode asset in assetsGroup.ChildNodes)
                {
                    string name = asset.Attributes["name"].Value;
                    string filepath = Path.Combine(AssetsHomeFolder, 
                        asset.Attributes["src"].Value);
                    bool isAppropriateFormat = ExportParameters.TryFormatFromString(
                        asset.Attributes["type"].Value, out var modelFormat);
                    if (!isAppropriateFormat ||
                        name == null || name == "" ||
                        filepath == null || filepath == "") return false;
                    newAssetGroups[groupName].Assets.Add(new Asset
                    {
                        Name = name,
                        FileFormat = modelFormat,
                        FilePath = filepath,
                    });
                }
            }
            AssetGroups = newAssetGroups;
            return true;
        }

        public IList<Asset> FindMissingAssetsFiles()
        {
            if (AssetGroups == null) throw new InvalidOperationException("Call ReloadManifest() first");
            var missingAssets = new List<Asset>();
            foreach (var assetGroup in AssetGroups)
            {
                for(int a = assetGroup.Value.Assets.Count - 1; a >= 0; --a)
                {
                    var asset = assetGroup.Value.Assets[a];
                    if (!File.Exists(asset.FilePath))
                    {
                        assetGroup.Value.Assets.RemoveAt(a);
                        missingAssets.Add(asset);
                    }
                }
            }
            return missingAssets;
        }

        private static string rootDir => Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName;
    }
}
