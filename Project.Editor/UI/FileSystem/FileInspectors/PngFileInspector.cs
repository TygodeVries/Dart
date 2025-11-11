using Runtime.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Editor.UI.FileSystem.FileInspectors
{
    [FileInspector(".png")]
    public class PngFileInspector : FileInspector
    {
        Dictionary<string, Texture> textureCache = new Dictionary<string, Texture>();

        public override void ClearCache()
        {
            textureCache = new Dictionary<string, Texture>();
        }

        public override Texture GetIcon(string filepath)
        {
            if (textureCache.ContainsKey(filepath))
                return textureCache[filepath];

            textureCache.Add(filepath, Texture.LoadFromPng(filepath, 100, 100));
            return textureCache[filepath];
        }
    }
}
