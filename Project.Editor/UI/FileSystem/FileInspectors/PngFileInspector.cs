using Runtime.Graphics;
using Runtime.Logging;
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
        List<string> loadingTextures = new List<string>();
        public override void ClearCache()
        {
            textureCache = new Dictionary<string, Texture>();
        }

        public override Texture GetIcon(string filepath)
        {
            if (loadingTextures.Contains(filepath))
                return Defaults.GetLoadingTexture();

            if (textureCache.ContainsKey(filepath))
            {
                if (!textureCache[filepath].isUploaded)
                    textureCache[filepath].Upload(); // We need to upload it on the main thread, otherwise OpenGL will throw an error
                return textureCache[filepath];
            }

            // Load async
            Task.Run(() =>
            {
                lock (loadingTextures)
                {
                    loadingTextures.Add(filepath);
                }

                Texture texture = Texture.LoadFromPng(filepath, 100, 100, false);

                lock (textureCache)
                {
                    // I don't know why this is happening some times.
                    if (textureCache.ContainsKey(filepath))
                    {
                        textureCache[filepath] = texture; // Override
                        return;
                    }

                    textureCache.Add(filepath, texture);
                }

                lock (loadingTextures)
                {
                    loadingTextures.RemoveAll(x => x == filepath);
                }
            });

            return Defaults.GetFallbackTexture();
        }
    }
}
