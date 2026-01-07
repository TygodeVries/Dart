using Project.Editor.Components;
using Runtime.Component;
using Runtime.Component.Core;
using Runtime.Component.Physics;
using Runtime.Graphics.Materials;
using Runtime.Graphics.Renderers;
using Runtime.Graphics.Shaders;
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
            SceneEditor.AddVisiblityInit();
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

        static GameObject? objectCursor = null;
        public static void PlaceObject(GameObject gm)
        {
            if (objectCursor != null)
            {
                CancelPlace();
            }

            objectCursor = gm;
            objectCursor.AddComponent(new CasualPlace());
            AddVisibility(objectCursor);
            Scene.main.Instantiate(gm);
        }

        public static void FinishedPlace()
        {
            objectCursor = null;
        }

        public static void CancelPlace()
        {
            Scene.main.DestroyObject(objectCursor);
            objectCursor = null;
        }

        public static void AddVisiblityInit()
        {
            foreach (GameObject game in Scene.main.GetGameObjects())
            {
                AddVisibility(game);
            }
        }

        public static void AddVisibility(GameObject g)
        {
            if (g.GetComponent<MeshRenderer>() != null)
            {
                return;
            }

            ShaderProgram shaderProgram = ShaderProgram.FromFile(
                EditorUtils.GetAssetDatabase().GetAsset("assets/shaders/gizmos/lit.vert"),
                EditorUtils.GetAssetDatabase().GetAsset("assets/shaders/gizmos/lit.frag"));

            Material material = new Material(shaderProgram);
            material.SetTexture("u_Texture", DefaultsTextures.GetFallbackTexture(), 0);

            Mesh mesh = Mesh.FromFileObj(EditorUtils.GetAssetDatabase().GetAsset("assets/models/gizmos.obj"));
            g.AddComponent(new MeshRenderer(material, mesh));
            g.AddComponent(new Billboard());
        }
    }
}
