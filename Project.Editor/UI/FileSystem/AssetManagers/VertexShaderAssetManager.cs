using Project.Editor.UI.FileSystem.FileInspectors;
using Project.Editor.UI.Inspectors;
using Project.Editor.UI.Inspectors.Inspections;
using Runtime.Graphics;

namespace Project.Editor.UI.FileSystem.AssetManagers
{
    [AssetManager(".vert")]
    internal class VertexShaderAssetManager : AssetManager
    {
        Texture icon;
        public VertexShaderAssetManager()
        {
            icon = Texture.LoadFromPng(EditorUtils.GetAssetDatabase().GetAsset("assets/textures/icons/vertexshader.png"));
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
