using Runtime.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Editor.UI
{
    public class AssetIcons
    {
        static Texture? fallback;
        public static Texture GetIconForAsset(string assetPath)
        {
            if (fallback == null)
            {
                fallback = new Texture("assets/textures/icons/Unknown.png");
            }

            return fallback;
        }
    }
}
