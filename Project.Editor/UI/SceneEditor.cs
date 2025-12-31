using Runtime.Component.Core;
using Runtime.Component.Physics;
using Runtime.Graphics.Renderers;
using Runtime.Objects;
using Runtime.Scenes;

namespace Project.Editor.UI
{
    public class SceneEditor
    {
        public static void EnableInCurrentScene()
        {
            Scene scene = Scene.main;
            CreateCursor(scene);
        }

        private static void CreateCursor(Scene scene)
        {
            scene.Instantiate(new GameObjectFactory()
                .AddComponent(new Transform())
                .AddComponent(new MeshRenderer())
                .AddComponent(new AABBBoxCollider())
                .Build());

        }
    }
}
