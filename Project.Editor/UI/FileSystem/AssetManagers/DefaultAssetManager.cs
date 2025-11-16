using Project.Editor.UI.Inspectors;
using Project.Editor.UI.Inspectors.Inspections;
using Runtime.Graphics;

namespace Project.Editor.UI.FileSystem.FileInspectors
{
    /// <summary>
    /// A fallback for files we have no clue what to do with!
    /// </summary>
    internal class DefaultAssetManager : AssetManager
    {
        public override Texture GetIcon(string path)
        {
            return DefaultsTextures.GetFallbackTexture();
        }

        public override Inspection GetInspection()
        {
            return new DefaultAssetInspection();
        }
    }
}
