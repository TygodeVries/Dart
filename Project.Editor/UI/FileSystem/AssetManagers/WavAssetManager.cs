using Project.Editor.UI.FileSystem.FileInspectors;
using Project.Editor.UI.Inspectors;
using Project.Editor.UI.Inspectors.Inspections;
using Runtime.Graphics;

namespace Project.Editor.UI.FileSystem.AssetManagers
{
    [AssetManager(".wav")]
    internal class WavAssetManager : AssetManager
    {
        Texture icon;
        public WavAssetManager()
        {
            icon = Texture.LoadFromPng("assets/textures/icons/wav.png");
        }

        public override Texture GetIcon()
        {
            return icon;
        }

        public override Inspection GetInspection()
        {
            return new WavAssetInspection();
        }
    }
}
