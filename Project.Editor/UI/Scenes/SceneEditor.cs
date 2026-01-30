using Project.Editor.Components;
using Runtime.Calc;
using Runtime.Components;
using Runtime.Components.Core;
using Runtime.Components.Physics;
using Runtime.Data;
using Runtime.Graphics;
using Runtime.Graphics.Materials;
using Runtime.Graphics.Renderers;
using Runtime.Graphics.Shaders;
using Runtime.Logging;
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
            SceneEditor.MakeAllObjectsEditorReady();
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
            if (IsPlacing())
            {
                CancelPlace();
            }

            objectCursor = gm;
            objectCursor.AddComponent(new CasualPlace());
            MakeEditorReady(objectCursor);
            Scene.main.Instantiate(gm);
        }

        public static void FinishedPlace()
        {
            if (objectCursor != null)
            {
                GameObject g = GameObject.LoadFromFile(objectCursor.GetAsset());
                objectCursor = null;

                MainThread.Run(() =>
                {
                    PlaceObject(g);
                });
            }
        }

        public static bool IsPlacing()
        {
            return objectCursor != null;
        }

        public static void CancelPlace()
        {
            Scene.main.DestroyObject(objectCursor);
            objectCursor = null;
        }

        public static void MakeAllObjectsEditorReady()
        {
            foreach (GameObject game in Scene.main.GetGameObjects())
            {
                MakeEditorReady(game);
            }
        }

        public static void MakeEditorReady(GameObject g)
        {
            AddVisibility(g);
            AddCollision(g);
            AddInteraction(g);
        }

        public static void AddVisibility(GameObject g)
        {
            if (g.GetComponent<MeshRenderer>() != null && g.GetComponent<MeshRenderer>().mesh != null)
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

            Texture texture = DefaultsTextures.GetFallbackTexture();
            if (icon != null)
            {
                Asset asset = new Asset(EditorUtils.GetAssetDatabase(), icon);
                texture = Texture.LoadFromPng(asset);
            }

            material.SetTexture("u_Texture", texture);

            Mesh mesh = Mesh.FromFileObj(EditorUtils.GetAssetDatabase().GetAsset("assets/models/gizmos.obj"));


            g.AddComponent(new MeshRenderer(material, mesh));
            //g.AddComponent(new Billboard());

            if (g.GetComponent<ICollider>() != null)
            {
                return;
            }

            g.AddComponent(new AABBoxCollider()
            {
                size = new Vector3(0.2f, 0.2f, 0.2f)
            });
        }

        public static void AddCollision(GameObject g)
        {
            if (g.GetComponent<ICollider>() != null)
            {
                return;
            }

            MeshRenderer meshRenderer = g.GetComponent<MeshRenderer>();


            try
            {
                g.AddComponent(new AABBoxCollider()
                {
                    size = meshRenderer.mesh.CalculateBounds()
                });
            }
            catch (Exception e)
            {
                Debug.Warning("Failed to calculate bounds for mesh.");
            }
        }

        public static void AddInteraction(GameObject g)
        {
            g.AddComponent(new SceneObjectInspectable());
        }
    }
}
