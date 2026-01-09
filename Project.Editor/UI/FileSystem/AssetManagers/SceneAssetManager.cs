using Project.Editor.UI.FileSystem.FileInspectors;
using Project.Editor.UI.Inspectors;
using Project.Editor.UI.Scenes;
using Runtime.Components.Core;
using Runtime.Components.Test;
using Runtime.Data;
using Runtime.Graphics;
using Runtime.Logging;
using Runtime.Objects;
using Runtime.Scenes;

namespace Project.Editor.UI.FileSystem.AssetManagers
{
    [AssetManager(".scene")]
    internal class SceneAssetManager : AssetManager
    {
        Texture texture;
        public SceneAssetManager()
        {
            Asset asset = EditorUtils.GetAssetDatabase().GetAsset("assets/textures/icons/scene.png");
            texture = Texture.LoadFromPng(asset);
        }

        public override Texture GetIcon()
        {
            return texture;
        }

        public override Inspection GetInspection()
        {
            return null;
        }

        public override void OnOpen()
        {
            Scene? scene = Scene.LoadFromAsset(asset);

            if (scene == null)
            {
                Debug.Error("Could not load scene!!!");
                return;
            }

            foreach (GameObject gm in scene.GetGameObjects())
            {
                gm.enableUpdates = false;
            }

            Scene.Load(scene);
            SceneEditor.EnableInCurrentScene();
            CreateSceneCamera();
        }

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
