using Project.Editor.UI.FileSystem.FileInspectors;
using Project.Editor.UI.Inspectors;
using Runtime.Component.Core;
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
            Scene.Load(scene);
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
