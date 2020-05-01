using ProceduralBuildingsGeneration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ProceduralBuildingsGeneration
{
    public class AssetsLoader
    {
        public static readonly string AssetsManifestPath = Path.Combine(rootDir, @"data/models/models_definitions.xml");
        public static readonly string AssetsHomeFolder = Path.Combine(rootDir, @"data/models/");
        public IList<AssetGroup> AssetGroups { get; private set; }
        public bool TryReloadManifest()
        {
            var manifest = new XmlDocument();
            if (!File.Exists(AssetsManifestPath)) return false;
            manifest.Load(AssetsManifestPath);
            if (manifest.DocumentElement.Name != "models") return false;
            var newAssetGroups = new List<AssetGroup>(manifest.DocumentElement.ChildNodes.Count); 
            foreach (XmlNode assetsGroup in manifest.DocumentElement)
            {
                if (assetsGroup.Name != "group") return false;
                newAssetGroups.Add(new AssetGroup(assetsGroup.Attributes["name"].Value));
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
                    newAssetGroups.Last().Assets.Add(new Asset
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
                for(int a = assetGroup.Assets.Count - 1; a >= 0; --a)
                {
                    var asset = assetGroup.Assets[a];
                    if (!File.Exists(asset.FilePath))
                    {
                        assetGroup.Assets.RemoveAt(a);
                        missingAssets.Add(asset);
                    }
                }
            }
            return missingAssets;
        }

        private static string rootDir => Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName;
    }
}
