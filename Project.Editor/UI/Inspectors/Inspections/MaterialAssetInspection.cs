using ImGuiNET;
using Project.Editor.Data;
using Runtime.Graphics.Materials;

namespace Project.Editor.UI.Inspectors.Inspections
{
    internal class MaterialAssetInspection : AssetInspection
    {
        MaterialData materialData;
        public override void Open()
        {

        }

        public override void Render()
        {
            materialData = MaterialData.FromJson(GetActiveFilePath());

            string[] vertexShaders = AssetDatabase.GetAllAssetsOfType(".vert").ToArray();
            string[] fragmentShaders = AssetDatabase.GetAllAssetsOfType(".frag").ToArray();

            // Draw UI

            // Get the current vertex shader
            int current = vertexShaders.ToList().IndexOf(materialData.VertexShader);
            if (current == -1)
                current = 0;

            // Draw vertex UI
            if (ImGui.BeginCombo("Vertex Shader", Path.GetRelativePath(Editor.projectPath, vertexShaders[current])))
            {
                for (int i = 0; i < vertexShaders.Length; i++)
                {
                    if (ImGui.Selectable(Path.GetRelativePath(Editor.projectPath, vertexShaders[i]), i == current))
                    {
                        current = i;
                        materialData.VertexShader = vertexShaders[i];
                        materialData.Save();
                        AssetDatabase.Refresh();
                    }
                }

                ImGui.EndCombo();
            }

            // Get current fragment shader
            current = fragmentShaders.ToList().IndexOf(materialData.FragmentShader);
            if (current == -1)
                current = 0;


            if (ImGui.BeginCombo("Fragment Shader", Path.GetRelativePath(Editor.projectPath, fragmentShaders[current])))
            {
                for (int i = 0; i < fragmentShaders.Length; i++)
                {
                    if (ImGui.Selectable(Path.GetRelativePath(Editor.projectPath, fragmentShaders[i]), i == current))
                    {
                        current = i;
                        materialData.FragmentShader = fragmentShaders[i];
                        materialData.Save();
                        AssetDatabase.Refresh();
                    }
                }

                ImGui.EndCombo();
            }
        }
    }
}
