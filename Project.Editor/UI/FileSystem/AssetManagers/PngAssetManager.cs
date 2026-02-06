using Project.Editor.UI.Inspectors;
using Project.Editor.UI.Inspectors.Inspections;
using Runtime.Calc;
using Runtime.Graphics;
using Runtime.Logging;

namespace Project.Editor.UI.FileSystem.FileInspectors
{
    [AssetManager(".png")]
    public class PngAssetManager : AssetManager
    {
        PngAssetInspection inspection = new PngAssetInspection();
        public override Inspection GetInspection()
        {
            return inspection;
        }

        Dictionary<string, ImageTexture> textureCache = new Dictionary<string, ImageTexture>();
        HashSet<string> loadingTextures = new HashSet<string>();

        public override void ClearCache()
        {
            textureCache = new Dictionary<string, ImageTexture>();
        }

        public override ImageTexture GetIcon()
        {
            string taskPath = (string)asset.GetPath()!.Clone();
            if (textureCache.TryGetValue(taskPath, out var tex))
            {
                return tex;
            }

            lock (loadingTextures)
            {
                if (loadingTextures.Contains(taskPath))
                    return DefaultsTextures.GetLoadingTexture();

                loadingTextures.Add(taskPath);
            }

            Task.Run(() =>
            {
                Debug.Log($"Now loading: {taskPath}");

                var texture = ImageTexture.LoadFromPng(new Runtime.Data.Asset(asset.GetDatabase(), taskPath), 100, 100, false);

                lock (textureCache)
                    textureCache[taskPath] = texture;

                MainThread.Run(() =>
                {
                    if (!texture.isUploaded)
                        texture.Upload();
                });

                lock (loadingTextures)
                    loadingTextures.Remove(taskPath);
            });

            return DefaultsTextures.GetLoadingTexture();
        }

    }
}
