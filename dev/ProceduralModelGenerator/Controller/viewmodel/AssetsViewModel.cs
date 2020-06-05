using ProceduralBuildingsGeneration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace GeneratorController
{
    public class AssetsViewModel : IViewModel
    {

        public AssetsLoader AssetsLoader { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public bool CorrectlyParsedManifest { get; }
        public AssetsViewModel()
        {
            AssetsLoader = new AssetsLoader();
            CorrectlyParsedManifest = false;
            try
            {
                if (AssetsLoader.TryReloadManifest())
                {
                    var missingAssets = AssetsLoader.FindMissingAssetsFiles();
                    CorrectlyParsedManifest = true;
                }
            }
            catch
            {
                // corrupt xml
            }
        }

        private string m_doorsAssetsGroup;
        public string DoorsAssetsGroupName {
            get => m_doorsAssetsGroup;
            set
            {
                if (CorrectlyParsedManifest && AssetsLoader.AssetGroups.ContainsKey(value))
                {
                    m_doorsAssetsGroup = value;
                    DoorsAssets = AssetsLoader
                        .AssetGroups[value]
                        .Assets;
                    NotifyChange("DoorsAssetsGroupName");
                    NotifyChange("DoorsAssets");
                    NotifyChange("DoorsAssetsNames");
                }
            }
        }
        public IList<Asset> DoorsAssets { get; private set; }

        public IEnumerable<string> DoorsAssetsNames {
            get => DoorsAssets?.Select(a => a.Name);
        }

        private string m_windowAssetsGroupName;
        public string WindowsAssetsGroupName
        {
            get => m_windowAssetsGroupName;
            set
            {
                if (CorrectlyParsedManifest && AssetsLoader.AssetGroups.ContainsKey(value))
                {
                    m_windowAssetsGroupName = value;
                    WindowsAssets = AssetsLoader
                        .AssetGroups[value]
                        .Assets;
                    NotifyChange("WindowsAssetsGroupName");
                    NotifyChange("WindowsAssets");
                    NotifyChange("WindowsAssetsNames");
                }
            }
        }

        public IList<Asset> WindowsAssets { get; private set; }

        public IEnumerable<string> WindowsAssetsNames
        {
            get => WindowsAssets?.Select(a => a.Name);
        }

        private int m_selectedDoorAssetIdx;
        public int SelectedDoorAssetIdx
        {
            get => m_selectedDoorAssetIdx;
            set
            {
                m_selectedDoorAssetIdx = value;
                NotifyChange("SelectedDoorAssetIdx");
            }
        }

        private int m_selectedWindowAssetIdx;
        public int SelectedWindowAssetIdx
        {
            get => m_selectedWindowAssetIdx;
            set
            {
                m_selectedWindowAssetIdx = value;
                NotifyChange("SelectedWindowAssetIdx");
            }
        }

        private int m_windowAssetTrianglesLimit;
        public int WindowAssetTrianglesLimit
        {
            get => m_windowAssetTrianglesLimit;
            set { m_windowAssetTrianglesLimit = value; NotifyChange("WindowAssetTrianglesLimit"); }
        }

        private int m_doorAssetTrianglesLimit;
        public int DoorAssetTrianglesLimit
        {
            get => m_doorAssetTrianglesLimit;
            set { m_doorAssetTrianglesLimit = value; NotifyChange("DoorAssetTrianglesLimit"); }
        }

        private void NotifyChange(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
