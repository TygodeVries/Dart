using Project.Editor.Components;
using Project.Editor.Preview;
using Project.Editor.UI.FileSystem.FileInspectors;
using Project.Editor.UI.Inspectors;
using Project.Editor.UI.Inspectors.Inspections;
using Project.Editor.UI.Styles;
using Runtime.Calc;
using Runtime.Component.Core;
using Runtime.Component.Lighting;
using Runtime.Graphics;
using Runtime.Graphics.Materials;
using Runtime.Graphics.Renderers;
using Runtime.Logging;
using Runtime.Objects;
using Runtime.Scenes;

namespace Project.Editor.UI.FileSystem.AssetManagers
{
    [AssetManager(".material")]
    internal class MaterialAssetManager : AssetManager
    {

        MaterialAssetInspection inspection = new MaterialAssetInspection();
        public override Inspection GetInspection()
        {
            return inspection;
        }

        public static Mesh[] previewMeshes;
        Texture icon;
        public MaterialAssetManager()
        {
            // #Todo: Make this be a small preview of the model 

            previewMeshes = new Mesh[] {
                Mesh.FromFileObj(EditorUtils.GetAssetDatabase().GetAsset("assets/models/ModelPreview_Sphere.obj"))!,
                Mesh.FromFileObj(EditorUtils.GetAssetDatabase().GetAsset("assets/models/ModelPreview_Box.obj"))!
            };

            icon = Texture.LoadFromPng(EditorUtils.GetAssetDatabase().GetAsset("assets/textures/icons/material.png"));
        }

        public override Texture GetIcon()
        {
            return icon;
        }

        public override void OnOpen()
        {
            Debug.Log("Creating model preview...");

            Scene.Load(new Scene());
            MaterialData materialData = MaterialData.FromJson(asset!);

            Material material = materialData.CreateMaterial(asset.GetDatabase());

            Debug.Log("Creating display object...");
            Scene.main.Instantiate(new GameObjectFactory()
                .AddComponent(new MeshRenderer(material)
                {
                    mesh = previewMeshes[0]
                })
                .AddComponent(new MaterialPreview(asset!, EditorUtils.projectPath))
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
                    position = new Vector3(0, 0, -4)
                })
                .AddComponent(sceneCamera)
                .AddComponent(new CameraPreview())
                .Build());

            Scene.main.Instantiate(new GameObjectFactory()
                .AddComponent(new SunLight())
                .AddComponent(new Transform())
                .AddComponent(new RotationPreview(true))
                .Build());
        }
    }
}
