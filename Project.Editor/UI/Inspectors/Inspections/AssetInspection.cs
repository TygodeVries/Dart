using Project.Editor.Data;
using Runtime.Data;
using Runtime.Logging;

namespace Project.Editor.UI.Inspectors.Inspections
{
    public abstract class AssetInspection : Inspection
    {
        Asset asset;
        MetaData? metaData;


        /// <summary>
        /// Set the file that is being inspected, called automatically in most cases.
        /// </summary>
        /// <param name="assetPath"></param>
        public void SetAsset(Asset asset)
        {
            this.asset = asset;
            this.metaData = MetaData.FromMetaFile(asset);
            Loaded();
        }

        /// <summary>
        /// Called when a filepath is set
        /// </summary>
        public virtual void Loaded() { }

        /// <summary>
        /// Returns the active file path
        /// </summary>
        /// <returns></returns>
        public Asset GetAsset()
        {
            if (asset == null)
            {
                Debug.Error("Attempting to retrieve active file path from a file inspection, that is not linked to a file!");
            }
            return asset!;
        }

        public MetaData GetActiveMetaData()
        {
            if (metaData == null)
            {
                Debug.Error("Attempting to retrieve meta data from a file inspection, that is not linked to a file!");
            }

            return metaData!;
        }
    }
}
