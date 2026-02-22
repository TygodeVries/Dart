using Project.Editor.UI.FileSystem.FileInspectors;
using Project.Editor.UI.Inspectors;
using Project.Editor.UI.Inspectors.Inspections;
using Runtime.Components.Core;
using Runtime.Components.Test;
using Runtime.Data;
using Runtime.Graphics;
using Runtime.Graphics.Materials;
using Runtime.Graphics.Renderers;
using Runtime.Graphics.Shaders;
using Runtime.Objects;
using Runtime.Objects.Prefabs;
using Runtime.Scenes;

namespace Project.Editor.UI.FileSystem.AssetManagers
{
    [AssetManager(".prefab")]
    internal class PrefabAssetManager : AssetManager
    {
        private PrefabAssetInspection prefabAssetInspection = new PrefabAssetInspection();
        public override Inspection GetInspection()
        {
            return prefabAssetInspection;
        }

        private ImageTexture icon;
        public PrefabAssetManager()
        {
            icon = ImageTexture.LoadFromPng(EditorUtils.GetAssetDatabase().GetAsset("assets/textures/icons/prefab.png"));
        }
        public override ImageTexture GetIcon()
        {
            return icon;
        }


        public override void OnOpen()
        {
            Scene scene = new Scene();
            Scene.Load(scene);
            CreateSceneCamera();

            gameObject = PrefabGameObject.FromFile(GetAsset()).GetGameObject();
            gameObject.enableUpdates = false; // Avoid it from moving
            gameObject.renderGizmos = true;

            AddVisibility(gameObject);

            scene.Instantiate(gameObject);

            GameObjectInspection inspection = new GameObjectInspection(gameObject, asset);
            inspection.OnRedraw += AddVisibility;
            InspectorWindow.GetActive().SetInspection(inspection);
        }

        private GameObject? drawer;
        private void AddVisibility(GameObject g)
        {
            Scene.main.DestroyObject(drawer);
            if (g.GetComponent<MeshRenderer>() != null)
            {
                return;
            }

            ShaderProgram shaderProgram = ShaderProgram.FromFile(
                EditorUtils.GetAssetDatabase().GetAsset("assets/shaders/gizmos/lit.vert"),
                EditorUtils.GetAssetDatabase().GetAsset("assets/shaders/gizmos/lit.frag"));

            Material material = new Material(shaderProgram);

            string? icon = null;

            foreach (Component component in g.GetComponents())
            {
                string? newIcon = component.GetGizmosPath();
                if (newIcon != null)
                {
                    icon = newIcon;
                }
            }

            ImageTexture texture = DefaultsTextures.GetFallbackTexture();
            if (icon != null)
            {
                Asset asset = new Asset(EditorUtils.GetAssetDatabase(), icon);
                texture = ImageTexture.LoadFromPng(asset);
            }

            material.SetTexture("u_Texture", texture);

            Mesh mesh = Mesh.FromFileObj(EditorUtils.GetAssetDatabase().GetAsset("assets/models/gizmos.obj"));

            drawer = new GameObjectFactory()
                .AddComponent(new MeshRenderer(material, mesh))
                .Build();

            Scene.main.Instantiate(drawer);
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
        }
    }
}
