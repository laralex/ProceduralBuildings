using ProceduralBuildingsGeneration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralBuildingsGeneration
{
    public class Asset : IDisposable
    {
        public string Name { get; set; }
        public ModelFormat FileFormat { get; set; }
        public string FilePath { get; set; }
        public FileStream OpenedFile { get; private set; }
        public FileStream OpenAssetFile()
        {
            if (!File.Exists(FilePath))
                throw new FileNotFoundException("FilePath doesn't lead to an existent file");
            CloseAssetFile();
            return OpenedFile = new FileStream(FilePath, FileMode.Open);
        }

        public void CloseAssetFile()
        {
            Dispose();
        }

        public void Dispose()
        {
            OpenedFile?.Close();
            OpenedFile?.Dispose();
        }
    }

    public class AssetsGroup
    {
        public string GroupName { get; private set; }
        public IList<Asset> Assets { get; private set; }
        public AssetsGroup(string groupName, IEnumerable<Asset> assets)
        {
            GroupName = groupName;
            Assets = assets.ToList();    
        }
        public AssetsGroup(string groupName)
        {
            GroupName = groupName;
            Assets = new List<Asset>();
        }
    }
}
