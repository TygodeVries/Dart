using Project.Editor.Data;
using Project.Editor.UI.FileSystem.AssetManagers;
using Runtime.Graphics.Materials;
using Runtime.Graphics.Renderers;
using Runtime.Input;
using Runtime.Logging;
using Runtime.Objects;
namespace Project.Editor.Components
{
    internal class LiveMaterialPreview : IComponent
    {
        string materialPath;
        string workingDir;
        public LiveMaterialPreview(string materialPath, string workingDir)
        {
            this.materialPath = materialPath;
            this.workingDir = workingDir;


            AssetDatabase.DatabaseRefreshed += AssetDatabase_DatabaseRefreshed;
        }

        int currentPreviewModel = 0;
        public override void Update()
        {
            if (Keyboard.current.IsPressedThisFrame(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Space))
            {
                currentPreviewModel++;

                if (currentPreviewModel >= MaterialAssetManager.previewMeshes.Length)
                    currentPreviewModel = 0;

                GetComponent<MeshRenderer>().SetMesh(MaterialAssetManager.previewMeshes[currentPreviewModel]);
            }
        }

        private void AssetDatabase_DatabaseRefreshed()
        {
            try
            {


                Debug.Log("Redid material");
                MaterialData materialData = MaterialData.FromJson(Path.Combine(workingDir, materialPath));
                Material mat = materialData.CreateMaterial(workingDir);

                GetComponent<MeshRenderer>()?.SetMaterial(mat);
            }
            catch (Exception e)
            {
                Debug.Error("Failed to create material: " + e);
            }
        }
    }
}
