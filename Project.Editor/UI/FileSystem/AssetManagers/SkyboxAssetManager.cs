using Project.Editor.UI.FileSystem.FileInspectors;
using Project.Editor.UI.Inspectors;
using Project.Editor.UI.Inspectors.Inspections;
using Runtime.Graphics;

namespace Project.Editor.UI.FileSystem.AssetManagers
{
    [AssetManager(".wav")]
    internal class WavAssetManager : AssetManager
    {
        ImageTexture icon;
        public WavAssetManager()
        {
            icon = ImageTexture.LoadFromPng(EditorUtils.GetAssetDatabase().GetAsset("assets/textures/icons/wav.png"));
        }

        public override ImageTexture GetIcon()
        {
            return icon;
        }

        public override Inspection GetInspection()
        {
            return new WavAssetInspection();
        }
    }
}
