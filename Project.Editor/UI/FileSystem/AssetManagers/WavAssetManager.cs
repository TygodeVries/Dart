using Project.Editor.UI.FileSystem.FileInspectors;
using Project.Editor.UI.Inspectors;
using Project.Editor.UI.Inspectors.Inspections;
using Runtime.Graphics;

namespace Project.Editor.UI.FileSystem.AssetManagers
{
    [AssetManager(".sky")]
    internal class SkyboxAssetManager : AssetManager
    {
        Texture icon;
        public SkyboxAssetManager()
        {
            icon = Texture.LoadFromPng(EditorUtils.GetAssetDatabase().GetAsset("assets/textures/gizmos/skybox.png"));
        }

        public override Texture GetIcon()
        {
            return icon;
        }

        public override Inspection GetInspection()
        {
            return new DefaultAssetInspection();
        }
    }
}
