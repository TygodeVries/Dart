using Project.Editor.UI.Inspectors;
using Project.Editor.UI.Inspectors.Inspections;
using Runtime.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Editor.UI.FileSystem.FileInspectors
{
    internal class DefaultAssetManager : AssetManager
    {
        public override Texture GetIcon(string path)
        {
            return Defaults.GetFallbackTexture();
        }

        public override Inspection GetInspection()
        {
            return new DefaultAssetInspection();
        }
    }
}
