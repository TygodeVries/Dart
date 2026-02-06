using Project.Editor.UI.FileSystem.FileInspectors;
using Project.Editor.UI.Inspectors;
using Project.Editor.UI.Inspectors.Inspections;
using Runtime.Graphics;

namespace Project.Editor.UI.FileSystem.AssetManagers
{
    [AssetManager(".sky")]
    internal class SkyboxAssetManager : AssetManager
    {
        ImageTexture icon;
        public SkyboxAssetManager()
        {
            icon = ImageTexture.LoadFromPng(EditorUtils.GetAssetDatabase().GetAsset("assets/textures/gizmos/skybox.png"));
        }

        public override ImageTexture GetIcon()
        {
            return icon;
        }

        public override Inspection GetInspection()
        {
            return new DefaultAssetInspection();
        }
    }
}
