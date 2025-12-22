using Project.Editor.Preview;
using Project.Editor.Preview.PrefabsEditor;
using Project.Editor.UI.FileSystem.FileInspectors;
using Project.Editor.UI.Inspectors;
using Project.Editor.UI.Inspectors.Inspections;
using Runtime.Component.Core;
using Runtime.Graphics;
using Runtime.Graphics.Materials;
using Runtime.Graphics.Renderers;
using Runtime.Objects;
using Runtime.Scenes;

namespace Project.Editor.UI.FileSystem.AssetManagers
{
    [AssetManager(".prefab")]
    internal class PrefabAssetManager : AssetManager
    {
        PrefabAssetInspection prefabAssetInspection = new PrefabAssetInspection();
        public override Inspection GetInspection()
        {
            return prefabAssetInspection;
        }

        Texture icon;
        public PrefabAssetManager()
        {
            icon = Texture.LoadFromPng("assets/textures/icons/prefab.png");
        }
        public override Texture GetIcon()
        {
            return icon;
        }

        public override void OnOpen()
        {
            PrefabEditor prefabEditor = new PrefabEditor(filepath);
            prefabEditor.Open();

            Camera sceneCamera = new Camera();
            sceneCamera.SetAsMain();

            Material material = Material.LoadFromFile($"{Editor.projectPath}/assets/materials/untitled.material");
            Mesh mesh = Mesh.FromFileObj("assets/models/cube.obj");

            Scene.main.Instantiate(new GameObjectFactory()
                .AddComponent(new Transform()
                {
                    position = new Runtime.Calc.Vector3(0, 0, -4)
                })
                .AddComponent(sceneCamera)
                .AddComponent(new CameraPreview())
                .Build());

            Scene.main.Instantiate(new GameObjectFactory()
                .AddComponent(new Transform())
                .AddComponent(new MeshRenderer(material)
                {
                    mesh = mesh
                })
                .Build());
        }
    }
}
