using ImGuiNET;
using Runtime.Graphics.Materials;

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
