using ImGuiNET;
using Runtime.Component.Core;
using Runtime.Graphics.Materials;
using Runtime.Objects;
using Runtime.Objects.Prefabs;

namespace Project.Editor.Data
{
    public class AssetCreator
    {
        public static void GUI(string folder)
        {
            if (ImGui.MenuItem("Material"))
            {
                CreateMaterial(folder);
            }

            if (ImGui.MenuItem("Prefab"))
            {
                CreatePrefab(folder);
            }
        }

        private static void CreatePrefab(string folder)
        {
            GameObjectFactory factory = new GameObjectFactory()
                .AddComponent<Transform>();

            PrefabGameObject prefab = PrefabGameObject.FromGameObject(factory.Build());
            File.WriteAllText(Path.Join(folder, "Untitled.prefab"), prefab.ToJson(true));
        }

        private static void CreateMaterial(string folder)
        {
            MaterialData materialData = new MaterialData()
            {
                FilePath = Path.Join(folder, "Untitled.material"),
                DataFields = new List<MaterialDataField>(),
                Lit = true,
                FragmentShader = "assets/shaders/lit.frag",
                VertexShader = "assets/shaders/lit.vert"
            };

            materialData.Save();
        }
    }
}
