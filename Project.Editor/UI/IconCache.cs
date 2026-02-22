using Runtime;
using Runtime.Data;
using Runtime.Graphics;

namespace Project.Editor.UI
{
    internal class IconCache
    {
        public static Asset GetIconForAsset(Asset asset)
        {
            ImageTexture.RemoveFromCache(asset);
            Directory.CreateDirectory(Game.GetAssetDatabase().GetAsset("cache").GetSystemPath());
            Directory.CreateDirectory(Game.GetAssetDatabase().GetAsset("cache/icons").GetSystemPath());
            return Game.GetAssetDatabase().GetAsset("cache/icons/" + asset.GetPath().Replace("/", "_").Replace("\\", "_") + ".png");
        }
    }
}
