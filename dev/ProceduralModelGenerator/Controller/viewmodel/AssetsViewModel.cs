using ProceduralBuildingsGeneration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DoorsAssetsGroupName"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DoorsAssets"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DoorsAssetsNames"));
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WindowsAssetsGroupName"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WindowsAssets"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WindowsAssetsNames"));
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedDoorAssetIdx"));
            }
        }

        private int m_selectedWindowAssetIdx;
        public int SelectedWindowAssetIdx
        {
            get => m_selectedWindowAssetIdx;
            set
            {
                m_selectedWindowAssetIdx = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedWindowAssetIdx"));
            }
        }
    }
}
