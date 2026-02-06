using Project.Editor.UI.FileSystem.FileInspectors;
using Project.Editor.UI.Inspectors;
using Project.Editor.UI.Inspectors.Inspections;
using Runtime.Graphics;

namespace Project.Editor.UI.FileSystem.AssetManagers
{
    [AssetManager(".frag")]
    internal class FragmentShaderAssetManager : AssetManager
    {
        ImageTexture icon;
        public FragmentShaderAssetManager()
        {
            icon = ImageTexture.LoadFromPng(EditorUtils.GetAssetDatabase().GetAsset("assets/textures/icons/fragmentshader.png"));
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
