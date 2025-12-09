using ImGuiNET;
using Project.Editor.Data;
using Runtime.Calc;
using Runtime.Graphics.Materials;
using Runtime.Graphics.Shaders;
using Runtime.Logging;
using System.Numerics;

namespace Project.Editor.UI.Inspectors.Inspections
{
    internal class MaterialAssetInspection : AssetInspection
    {
        MaterialData? materialData;
        public override void Open()
        {

        }

        public override void Render()
        {
            materialData = MaterialData.FromJson(GetActiveFilePath());

            string[] vertexShaders = AssetDatabase.GetAllAssetsOfType(".vert").ToArray();
            string[] fragmentShaders = AssetDatabase.GetAllAssetsOfType(".frag").ToArray();


            // Get the current vertex shader
            int current = vertexShaders.ToList().IndexOf(materialData.VertexShader);
            if (current == -1)
                current = 0;

            // Draw vertex UI
            RenderShaderSelector("Vertex Shader", vertexShaders, current, true);

            string currentVertexShader = vertexShaders[current];

            // Get current fragment shader
            current = fragmentShaders.ToList().IndexOf(materialData.FragmentShader);
            if (current == -1)
                current = 0;

            // Draw fragment UI
            RenderShaderSelector("Fragment Shader", fragmentShaders, current, false);

            string currentFragmentShader = fragmentShaders[current];

            RenderFields(currentVertexShader, currentFragmentShader, materialData);
        }

        private void RenderShaderSelector(string title, string[] shaders, int current, bool writeToVertex)
        {
            if (shaders.Length == 0)
            {
                Debug.Warning("No shaders in selector!");
                return;
            }

            if (ImGui.BeginCombo(title, Path.GetRelativePath(Editor.projectPath, shaders[current])))
            {
                for (int i = 0; i < shaders.Length; i++)
                {
                    if (ImGui.Selectable(Path.GetRelativePath(Editor.projectPath, shaders[i]), i == current))
                    {
                        current = i;

                        if (writeToVertex)
                            materialData.VertexShader = shaders[i];
                        else
                            materialData.FragmentShader = shaders[i];

                        materialData.Save();
                    }
                }

                ImGui.EndCombo();
            }
        }

        private void RenderFields(string vertexShader, string fragmentShader, MaterialData materialData)
        {
            ShaderProgram shaderProgram = ShaderProgram.FromFile(vertexShader, fragmentShader);

            bool shouldSave = false;
            int fieldindex = 0;
            foreach (Uniform uniform in shaderProgram.GetUniforms())
            {
                fieldindex++;
                if (!uniform.showInInspector)
                    continue;

                string displayName = uniform.name.Replace("u_", "");

                ImGui.Text(displayName);
                var field = materialData.DataFields
                    .FirstOrDefault(e => e.Name == uniform.name);


                if (field == null)
                {
                    field = new MaterialDataField()
                    {
                        Name = uniform.name,
                        Type = uniform.type,
                        Value = "default"
                    };

                    materialData.DataFields.Add(field);
                    shouldSave = true;
                }

                if (uniform.type == "vec4")
                {
                    Vector4 val = Encoder.NVec4(field.Value);

                    if (ImGui.ColorPicker4($"vec4##{fieldindex}", ref val))
                    {
                        field.Value = Encoder.Get(val);
                        shouldSave = true;
                    }
                }

                if (uniform.type == "sampler2D")
                {
                    List<string> textures = AssetDatabase.GetAllAssetsOfType(".png");
                    int current = textures.IndexOf(field.Value);
                    if (current == -1)
                        current = 0;
                    if (ImGui.BeginCombo($"sampler2D##{fieldindex}", Path.GetRelativePath(Editor.projectPath, textures[current])))
                    {
                        for (int i = 0; i < textures.Count; i++)
                        {
                            if (ImGui.Selectable(Path.GetRelativePath(Editor.projectPath, textures[i]), i == current))
                            {
                                field.Value = textures[i];
                                shouldSave = true;
                            }
                        }

                        ImGui.EndCombo();
                    }
                }
            }

            if (shouldSave)
            {
                materialData.Save();
            }
        }
    }
}
