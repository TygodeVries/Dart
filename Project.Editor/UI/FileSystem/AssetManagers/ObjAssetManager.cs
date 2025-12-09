using Project.Editor.Preview;
using Project.Editor.UI.FileSystem.FileInspectors;
using Project.Editor.UI.Inspectors;
using Project.Editor.UI.Inspectors.Inspections;
using Runtime.Component.Core;
using Runtime.Graphics;
using Runtime.Graphics.Materials;
using Runtime.Graphics.Renderers;
using Runtime.Graphics.Shaders;
using Runtime.Logging;
using Runtime.Objects;
using Runtime.Scenes;

namespace Project.Editor.UI.FileSystem.AssetManagers
{
    [AssetManager(".obj")]
    public class ObjAssetManager : AssetManager
    {
        ObjAssetInspection inspection = new ObjAssetInspection();
        public override Inspection GetInspection()
        {
            return inspection;
        }

        Texture icon;
        public ObjAssetManager()
        {
            // #Todo: Make this be a small preview of the model 
            icon = Texture.LoadFromPng("assets/textures/icons/model.png");

            // Load a default shader
            material = new Material(ShaderProgram.FromFile("assets/shaders/previews/model_untextured.vert", "assets/shaders/previews/model_untextured.frag"));
            material.SetVector3("tintColor", new OpenTK.Mathematics.Vector3(1, 1, 1));
        }

        public override Texture GetIcon()
        {
            return icon;
        }

        Material material;

        public override void OnOpen()
        {
            Mesh? mesh = Mesh.FromFileObj(filepath!);
            if (mesh == null)
            {
                Debug.Error("Failed to load mesh, null!");
                return;
            }

            Debug.Log("Creating model preview...");

            Scene.Load(new Scene());

            Debug.Log("Creating display object...");
            Scene.main.Instantiate(new GameObjectFactory()
                .AddComponent(new MeshRenderer(material)
                {
                    mesh = mesh
                })
                .AddComponent(new Transform())
                .AddComponent(new RotationPreview(false))
                .Build());

            Debug.Log("Creating Camera...");
            Camera sceneCamera = new Camera();
            sceneCamera.SetAsMain();

            Debug.Log("Setting background...");


            sceneCamera.backgroundColor = Colors.ModelPreviewBackground;

            Scene.main.Instantiate(new GameObjectFactory()
                .AddComponent(new Transform()
                {
                    position = new OpenTK.Mathematics.Vector3(0, 0, -4)
                })
                .AddComponent(sceneCamera)
                .AddComponent(new CameraPreview())
                .Build());

        }
    }
}
