using Project.Editor.UI.FileSystem.FileInspectors;
using Project.Editor.UI.Inspectors;
using Project.Editor.UI.Scenes;
using Runtime.Calc;
using Runtime.Component.Core;
using Runtime.Component.Lighting;
using Runtime.Component.Test;
using Runtime.Logging;
using Runtime.Objects;
using Runtime.Scenes;

namespace Project.Editor.UI.FileSystem.AssetManagers
{
    [AssetManager(".scene")]
    internal class SceneAssetManager : AssetManager
    {
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
                gm.EnableUpdates = false;
            }

            Scene.Load(scene);
            CreateSceneCamera();
            SceneEditor.EnableInCurrentScene();
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
                .AddComponent(new PointLight()
                {
                    color = new Vector3(1, 1, 1),
                    intensity = 10
                })
                .Build()
                );
        }

    }
}
