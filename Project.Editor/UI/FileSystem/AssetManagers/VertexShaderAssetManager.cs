using Project.Editor.UI.FileSystem.FileInspectors;
using Project.Editor.UI.Inspectors;
using Project.Editor.UI.Inspectors.Inspections;
using Runtime.Graphics;

namespace Project.Editor.UI.FileSystem.AssetManagers
{
    [AssetManager(".vert")]
    internal class VertexShaderAssetManager : AssetManager
    {
        ImageTexture icon;
        public VertexShaderAssetManager()
        {
            icon = ImageTexture.LoadFromPng(EditorUtils.GetAssetDatabase().GetAsset("assets/textures/icons/vertexshader.png"));
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
