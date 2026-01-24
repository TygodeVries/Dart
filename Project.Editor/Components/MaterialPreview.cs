using Project.Editor.UI.FileSystem.AssetManagers;
using Runtime;
using Runtime.Data;
using Runtime.Graphics.Materials;
using Runtime.Graphics.Renderers;
using Runtime.Input;
using Runtime.Logging;
using Runtime.Objects;
namespace Project.Editor.Components
{
    internal class MaterialPreview : Component
    {
        Asset materialAsset;
        string workingDir;
        AssetDatabase assetDatabase;
        public MaterialPreview(Asset materialAsset, string workingDir)
        {
            this.assetDatabase = Game.GetAssetDatabase();
            this.materialAsset = materialAsset;
            this.workingDir = workingDir;


            assetDatabase.DatabaseRefreshed += AssetDatabase_DatabaseRefreshed;
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


                MaterialData materialData = MaterialData.FromJson(materialAsset);
                Material mat = materialData.CreateMaterial(assetDatabase);

                GetComponent<MeshRenderer>()?.SetMaterial(mat);
            }
            catch (Exception e)
            {
                Debug.Error("Failed to create material: " + e);
            }
        }
    }
}
