using Project.Editor.Components;
using Project.Editor.Preview;
using Project.Editor.UI.FileSystem.FileInspectors;
using Project.Editor.UI.Inspectors;
using Project.Editor.UI.Inspectors.Inspections;
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

        Mesh previewMesh;
        Texture icon;
        public MaterialAssetManager()
        {
            // #Todo: Make this be a small preview of the model 
            previewMesh = Mesh.FromFileObj("assets/models/ModelPreview.obj")!;
            icon = Texture.LoadFromPng("assets/textures/icons/material.png");
        }

        public override Texture GetIcon()
        {
            return icon;
        }

        public override void OnOpen()
        {
            Debug.Log("Creating model preview...");

            Scene.Load(new Scene());
            MaterialData materialData = MaterialData.FromJson(filepath);

            Material material = materialData.CreateMaterial(Editor.projectPath);

            Debug.Log("Creating display object...");
            Scene.main.Instantiate(new GameObjectFactory()
                .AddComponent(new MeshRenderer(material)
                {
                    mesh = this.previewMesh
                })
                .AddComponent(new LiveMaterialPreview(filepath, Editor.projectPath))
                .AddComponent(new Transform())
                .AddComponent(new RotationPreview())
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

            Scene.main.Instantiate(new GameObjectFactory()
                .AddComponent(new SunLight())
                .Build());
        }
    }
}
