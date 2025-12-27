using Project.Editor.UI.FileSystem.FileInspectors;
using Project.Editor.UI.Inspectors;
using Project.Editor.UI.Inspectors.Inspections;
using Runtime.Component.Core;
using Runtime.Component.Test;
using Runtime.Graphics;
using Runtime.Objects;
using Runtime.Objects.Prefabs;
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
            icon = Texture.LoadFromPng(EditorUtils.GetAssetDatabase().GetAsset("assets/textures/icons/prefab.png"));
        }
        public override Texture GetIcon()
        {
            return icon;
        }


        public override void OnOpen()
        {
            Scene scene = new Scene();
            Scene.Load(scene);
            CreateSceneCamera();

            string json = File.ReadAllText(GetAsset().GetSystemPath());
            gameObject = PrefabGameObject.FromJson(json).GetGameObject();
            gameObject.EnableUpdates = false; // Avoid it from moving

            scene.Instantiate(gameObject);

            GameObjectInspection inspection = new GameObjectInspection(gameObject, asset);
            InspectorWindow.GetActive().SetInspection(inspection);
        }

        private GameObject gameObject;

        private void CreateSceneCamera()
        {
            Camera sceneCamera = new Camera();
            sceneCamera.SetAsMain();

            Scene.main.Instantiate(new GameObjectFactory()
                .AddComponent(new Transform()
                {
                    position = new Runtime.Calc.Vector3(0, 0, 0)
                })
                .AddComponent(sceneCamera)
                .AddComponent(new FlightCamera())
                .Build()
                );

            /*
            Mesh mesh = Mesh.FromFileObj("assets\\Models\\ModelPreview_Box.obj");
            Material material = new Material(ShaderProgram.FromFile("assets/shaders/lit.vert", "assets/shaders/lit.frag"));
            Scene.main.Instantiate(new GameObjectFactory()
                .AddComponent(new MeshRenderer(material, mesh))
                .AddComponent(new Transform())
                .Build()
                );
            */
        }
    }
}
