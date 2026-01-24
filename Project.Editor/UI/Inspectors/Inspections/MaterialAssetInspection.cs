using ImGuiNET;
using Project.Editor.UI.Generic;
using Runtime;
using Runtime.Calc;
using Runtime.Data;
using Runtime.DearImGUI.Gui;
using Runtime.Graphics.Materials;
using Runtime.Graphics.Shaders;
using System.Globalization;

namespace Project.Editor.UI.Inspectors.Inspections
{
    internal class MaterialAssetInspection : AssetInspection
    {
        private MaterialData? materialData;

        public override void Open()
        {
            materialData = MaterialData.FromJson(GetAsset());
        }

        public override void Render()
        {
            if (materialData == null)
            {
                ImGui.Text("Loading Material...");
                return;
            }

            Asset[] vertexShaders = Game.GetAssetDatabase().GetAllAssetsOfType(".vert").ToArray();
            Asset[] fragmentShaders = Game.GetAssetDatabase().GetAllAssetsOfType(".frag").ToArray();

            if (ImGui.Button($"{materialData.FragmentShader}"))
            {
                AssetSelectorWindow assetSelectorWindow = new AssetSelectorWindow(".frag", Game.GetAssetDatabase());
                assetSelectorWindow.OnSelect += (AssetSelectionResult result) =>
                {
                    materialData.FragmentShader = result.asset.GetPath();
                    materialData.Save();
                };
                GuiWindow.Enable(assetSelectorWindow);
            }

            if (ImGui.Button($"{materialData.VertexShader}"))
            {
                AssetSelectorWindow assetSelectorWindow = new AssetSelectorWindow(".vert", Game.GetAssetDatabase());
                assetSelectorWindow.OnSelect += (AssetSelectionResult result) =>
                {
                    materialData.VertexShader = result.asset.GetPath();
                    materialData.Save();
                };

                GuiWindow.Enable(assetSelectorWindow);
            }

            if (string.IsNullOrEmpty(materialData.VertexShader) ||
                string.IsNullOrEmpty(materialData.FragmentShader))
                return;

            ImGUIDrawUniformOptions(materialData.VertexShader, materialData.FragmentShader, materialData);
        }

        /// <summary>
        /// Showing shader fields
        /// </summary>
        /// <param name="vertexShader"></param>
        /// <param name="fragmentShader"></param>
        /// <param name="materialData"></param>
        private void ImGUIDrawUniformOptions(string vertexShader, string fragmentShader, MaterialData materialData)
        {
            ShaderProgram shaderProgram = ShaderProgram.FromFile(Game.GetAssetDatabase().GetAsset(vertexShader), Game.GetAssetDatabase().GetAsset(fragmentShader));

            bool shouldSave = false;

            ImGui.Text("Uniforms:");
            foreach (Uniform uniform in shaderProgram.GetUniforms())
            {
                if (!uniform.showInInspector)
                    continue;

                ImGui.PushID(uniform.name);

                string displayName = uniform.name.Replace("u_", "");
                ImGui.Text(displayName);

                var field = materialData.DataFields
                    .FirstOrDefault(e => e.Name == uniform.name);

                if (field == null)
                {
                    field = new MaterialDataField
                    {
                        Name = uniform.name,
                        Type = uniform.type,
                        Value = GetDefaultValue(uniform.type)
                    };

                    materialData.DataFields.Add(field);
                    shouldSave = true;
                }

                if (uniform.type == "vec4")
                {
                    var val = Vector4.Parse(field.Value).ToNumerics();
                    if (ImGui.ColorPicker4("##vec4", ref val))
                    {
                        field.Value = new Vector4(val).ToString();
                        shouldSave = true;
                    }
                }
                else if (uniform.type == "vec3")
                {
                    var val = Vector3.Parse(field.Value).ToNumerics();
                    if (ImGui.ColorPicker3("##vec3", ref val))
                    {
                        field.Value = new Vector3(val).ToString();
                        shouldSave = true;
                    }
                }
                else if (uniform.type == "float")
                {
                    float val = float.Parse(field.Value, CultureInfo.InvariantCulture);
                    if (ImGui.InputFloat("##float", ref val))
                    {
                        field.Value = val.ToString(CultureInfo.InvariantCulture);
                        shouldSave = true;
                    }
                }
                else if (uniform.type == "sampler2D")
                {
                    if (ImGui.Button("Select"))
                    {
                        AssetSelectorWindow window = new AssetSelectorWindow(".png", Game.GetAssetDatabase());
                        window.OnSelect += (AssetSelectionResult result) =>
                        {
                            field.Value = result.asset.GetPath();
                            materialData.Save();
                            Game.GetAssetDatabase().RefreshNow();
                        };
                        GuiWindow.Enable(window);
                    }
                }

                ImGui.PopID();
            }

            if (shouldSave)
                materialData.Save();
        }

        private static string GetDefaultValue(string type)
        {
            if (type == "vec4") return "0,0,0,1";
            else if (type == "vec3") return "0,0,0";
            else if (type == "float") return "0";
            else if (type == "sampler2D") return "";
            else return "";
        }
    }
}
