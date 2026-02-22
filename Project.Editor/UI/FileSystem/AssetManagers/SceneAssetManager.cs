using Project.Editor.Data;
using Project.Editor.EditorModes;
using Project.Editor.UI.FileSystem.FileInspectors;
using Project.Editor.UI.Inspectors;
using Runtime.Data;
using Runtime.Graphics;

namespace Project.Editor.UI.FileSystem.AssetManagers
{
    [AssetManager(".scene")]
    internal class SceneAssetManager : AssetManager
    {
        private ImageTexture texture;
        public SceneAssetManager()
        {
            Asset asset = EditorUtils.GetAssetDatabase().GetAsset("assets/textures/icons/scene.png");
            texture = ImageTexture.LoadFromPng(asset);
        }

        public override ImageTexture GetIcon()
        {
            return texture;
        }

        public override Inspection GetInspection()
        {
            return null;
        }

        public override void OnOpen()
        {
            EditorPrefs.SetValue("last_open_scene", asset.GetPath());
            EditorMode.SetMode(Mode.Build);
        }

    }
}
