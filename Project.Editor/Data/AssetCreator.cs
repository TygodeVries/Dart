using ImGuiNET;
using Project.Editor.UI.Generic;
using Runtime.Component.Core;
using Runtime.Data;
using Runtime.DearImGUI.Gui;
using Runtime.Graphics.Materials;
using Runtime.Objects;
using Runtime.Objects.Prefabs;

namespace Project.Editor.Data
{
    public class AssetCreator
    {
        public static void GUI(Asset folder)
        {
            Asset? asset = null;
            if (ImGui.MenuItem("Material"))
            {
                asset = CreateMaterial(folder);
            }

            if (ImGui.MenuItem("Prefab"))
            {
                asset = CreatePrefab(folder);
            }

            if (asset == null)
                return;

            GuiWindow.Enable(new RenameAssetWindow(asset));
        }

        private static Asset CreatePrefab(Asset folder)
        {
            GameObjectFactory factory = new GameObjectFactory()
                .AddComponent<Transform>();

            PrefabGameObject prefab = PrefabGameObject.FromGameObject(factory.Build());
            string path = Path.Join(folder.GetSystemPath(), "Untitled.prefab");
            File.WriteAllText(path, prefab.ToJson(true));
            return Asset.FromSystemPath(folder.GetDatabase(), path);
        }

        private static Asset CreateMaterial(Asset folder)
        {
            string path = Path.Join(folder.GetSystemPath(), "Untitled.material");
            MaterialData materialData = new MaterialData()
            {
                FilePath = path,
                DataFields = new List<MaterialDataField>(),
                Lit = true,
                FragmentShader = "assets/shaders/lit.frag",
                VertexShader = "assets/shaders/lit.vert"
            };

            materialData.Save();
            return Asset.FromSystemPath(folder.GetDatabase(), path);
        }
    }
}
