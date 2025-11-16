using Project.Editor.Data;
using Runtime.Logging;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Editor.UI.Inspectors.Inspections
{
    public abstract class AssetInspection : Inspection
    {
        string? filepath;
        MetaData? metaData;
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
