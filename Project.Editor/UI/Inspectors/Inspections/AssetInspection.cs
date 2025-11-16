using Project.Editor.Data;
using Runtime.Logging;

namespace Project.Editor.UI.Inspectors.Inspections
{
    public abstract class AssetInspection : Inspection
    {
        string? filepath;
        MetaData? metaData;


        /// <summary>
        /// Set the file that is being inspected, called automatically in most cases.
        /// </summary>
        /// <param name="assetPath"></param>
        public void SetFilePath(string assetPath)
        {
            filepath = assetPath;
            this.metaData = MetaData.Get(assetPath);
        }

        /// <summary>
        /// Returns the active file path
        /// </summary>
        /// <returns></returns>
        public string GetActiveFilePath()
        {
            if (filepath == null)
            {
                Debug.Error("Attempting to retrieve active file path from a file inspection, that is not linked to a file!");
            }
            return filepath!;
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
