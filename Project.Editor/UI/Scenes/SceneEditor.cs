using Project.Editor.Components;
using Runtime.Component;
using Runtime.Component.Core;
using Runtime.Component.Physics;
using Runtime.Graphics.Materials;
using Runtime.Graphics.Renderers;
using Runtime.Objects;
using Runtime.Scenes;
namespace Project.Editor.UI.Scenes
{
    public class SceneEditor
    {
        public static void EnableInCurrentScene()
        {
            Scene scene = Scene.main;
            SceneEditor.scene = scene;
        }

        static Scene? scene = null;
        public static bool IsEnabledInCurrentScene()
        {
            return scene == Scene.main && Scene.main != null;
        }

        private static void CreateCursor(Scene scene)
        {
            Transform cursorTransform = new Transform();
            GameObject cursor = new GameObjectFactory()
                .AddComponent(cursorTransform)
                .Build();

            scene.Instantiate(cursor);

            Mesh? mesh_up = Mesh.FromFileObj(EditorUtils.GetAssetDatabase().GetAsset("assets/Models/Arrow/Up.obj"));
            Mesh? mesh_right = Mesh.FromFileObj(EditorUtils.GetAssetDatabase().GetAsset("assets/Models/Arrow/Right.obj"));
            Mesh? mesh_forward = Mesh.FromFileObj(EditorUtils.GetAssetDatabase().GetAsset("assets/Models/Arrow/Forward.obj"));
            Material material = Material.CreateFallback();

            scene.Instantiate(new GameObjectFactory()
                .AddComponent(new Transform())
                .AddComponent(new MeshRenderer(material, mesh_up!))
                .AddComponent(new AABBoxCollider()
                {
                    size = new Runtime.Calc.Vector3(.2f, 1, .2f),
                    offset = new Runtime.Calc.Vector3(0, 0.5f, 0)
                })
                .AddComponent(new FollowConstraint(cursorTransform))
                .AddComponent(new Draggable(new Runtime.Calc.Vector3(0, 1, 0)))
                .Build());

            scene.Instantiate(new GameObjectFactory()
                .AddComponent(new Transform())
                .AddComponent(new MeshRenderer(material, mesh_right!))
                .AddComponent(new AABBoxCollider()
                {
                    size = new Runtime.Calc.Vector3(1, .2f, .2f),
                    offset = new Runtime.Calc.Vector3(0.5f, 0, 0)
                })
                .AddComponent(new FollowConstraint(cursorTransform))
                .AddComponent(new Draggable(new Runtime.Calc.Vector3(1, 0, 0)))
                .Build());

            scene.Instantiate(new GameObjectFactory()
                .AddComponent(new Transform())
                .AddComponent(new MeshRenderer(material, mesh_forward!))
                .AddComponent(new AABBoxCollider()
                {
                    size = new Runtime.Calc.Vector3(.2f, .2f, 1),
                    offset = new Runtime.Calc.Vector3(0, 0, 0.5f)
                })
                .AddComponent(new FollowConstraint(cursorTransform))
                .AddComponent(new Draggable(new Runtime.Calc.Vector3(0, 0, 1)))
                .Build());
        }
    }
}
