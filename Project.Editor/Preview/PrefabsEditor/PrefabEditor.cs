using Runtime.Component.Core;
using Runtime.Objects;
using Runtime.Scenes;

namespace Project.Editor.Preview.PrefabsEditor
{
    public class PrefabEditor
    {
        string filepath;
        public PrefabEditor(string filepath)
        {
            this.filepath = filepath;
        }

        public void Open()
        {
            Scene.Load(GetScene());
        }

        public Scene GetScene()
        {
            Scene scene = new Scene();

            Camera camera = new Camera()
            {
                backgroundColor = new Runtime.Calc.Vector3(0, 0, 0.3f)
            };

            camera.SetAsMain();

            scene.Instantiate(new GameObjectFactory()
                .AddComponent(camera)
                .AddComponent(new Transform())
                .Build());

            return scene;
        }
    }
}
